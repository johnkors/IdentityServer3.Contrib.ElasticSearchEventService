using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IdentityServer3.ElasticSearchEventService.Extensions;
using Newtonsoft.Json;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class ObjectMapper<T> : IObjectMapper
    {
        public Type Type { get; private set; }

        private readonly IDictionary<string, Func<T, object>> _maps = new Dictionary<string, Func<T, object>>();

        public ISet<MemberInfo> MappedMembers { get; private set; }
        public IEnumerable<MemberInfo> UnmappedMembers { get { return Type.GetMembers().Where(m => MappedMembers.All(o => o.Name != m.Name)); } }

        public ObjectMapper()
        {
            Type = typeof (T);
            MappedMembers = new HashSet<MemberInfo>();
        }

        public ObjectMapper<T> Map(Expression<Func<T, object>> expression)
        {
            return Map(expression.GetMemberPath(), expression);
        }

        public ObjectMapper<T> Map(string name, Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.GetMemberExpressionOrNull();
            if (memberExpression != null && memberExpression.Member.BelongsTo(Type))
            {
                MappedMembers.Add(memberExpression.Member);
            }
            _maps[name] = expression.Compile();
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
                fields[pair.Key] = pair.Value(item);
            }
            return fields;
        }

        private static bool IsNull(object o)
        {
            return o == null;
        }

        public ObjectMapper<T> MapRemainingMembersAsJson(string name = "Json")
        {
            _maps[name] = RemainingMembersAsJson;
            return this;
        }

        private object RemainingMembersAsJson(T item)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var property in UnmappedMembers.OfType<PropertyInfo>())
            {
                dictionary[property.Name] = property.GetValue(item);
            }
            foreach (var field in UnmappedMembers.OfType<FieldInfo>())
            {
                dictionary[field.Name] = field.GetValue(item);
            }
            return JsonConvert.SerializeObject(dictionary);
        }
    }
}