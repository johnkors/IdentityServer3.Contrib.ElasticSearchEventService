using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IdentityServer3.ElasticSearchEventService.Extensions;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class TypedObjectMapper<T> : IObjectMapper
    {
        public Type Type { get; private set; }

        private readonly IDictionary<string, Func<T, object>> _maps = new Dictionary<string, Func<T, object>>();

        public IEnumerable<MemberInfo> HandledMembers { get { return MappedMembers.Concat(IgnoredMembers); } }
        public ISet<MemberInfo> IgnoredMembers { get; private set; }
        public ISet<MemberInfo> MappedMembers { get; private set; }
        public IEnumerable<MemberInfo> UnmappedMembers { get { return Type.GetMembers().Where(m => HandledMembers.All(o => o.Name != m.Name)); } }

        public TypedObjectMapper()
        {
            Type = typeof (T);
            MappedMembers = new HashSet<MemberInfo>();
            IgnoredMembers = new HashSet<MemberInfo>();
        }

        public TypedObjectMapper<T> Ignore(Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.GetMemberExpressionOrNull();
            if (memberExpression == null)
            {
                return this;
            }
            IgnoredMembers.Add(memberExpression.Member);
            return this;
        }

        public TypedObjectMapper<T> Map(Expression<Func<T, object>> expression)
        {
            return Map(expression.GetMemberPath(), expression);
        }

        public TypedObjectMapper<T> Map(string name, Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.GetMemberExpressionOrNull();
            if (memberExpression != null && memberExpression.Member.BelongsTo(Type))
            {
                MappedMembers.Add(memberExpression.Member);
            }
            var func = expression.Compile();
            _maps[name] = expression.Body.Type.IsSimpleType()
                ? func
                : t => func(t).ToJsonSuppressErrors();
            return this;
        }

        public TypedObjectMapper<T> MapRemainingMembers()
        {
            foreach (var property in UnmappedMembers.OfType<PropertyInfo>())
            {
                Map(property.Name, property.ToLambda<T>());
            }
            foreach (var field in UnmappedMembers.OfType<FieldInfo>())
            {
                Map(field.Name, field.ToLambda<T>());
            }
            return this;
        }

        public IDictionary<string, object> GetFields(object item)
        {
            if (item is T)
            {
                return GetFields((T)item);
            }
            // Err...?
            return new Dictionary<string, object>();
        }

        public IDictionary<string, object> GetFields(T item)
        {
            return IsNull(item)
                ? new Dictionary<string, object>()
                : DoGetFields(item);
        }

        private IDictionary<string, object> DoGetFields(T item)
        {
            var fields = new Dictionary<string, object>();
            foreach (var pair in _maps)
            {
                fields[pair.Key] = pair.Value.TryInvoke(item);
            }
            return fields;
        }

        private static bool IsNull(object o)
        {
            return o == null;
        }

        public TypedObjectMapper<T> MapRemainingMembersAsJson(string name = "Json")
        {
            _maps[name] = RemainingMembersAsJson;
            return this;
        }

        private object RemainingMembersAsJson(T item)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var property in UnmappedMembers.OfType<PropertyInfo>())
            {
                dictionary[property.Name] = property.TryGetValue(item);
            }
            foreach (var field in UnmappedMembers.OfType<FieldInfo>())
            {
                dictionary[field.Name] = field.TryGetValue(item);
            }
            return dictionary.ToJsonSuppressErrors();
        }
    }
}