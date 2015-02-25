using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class UniversalObjectMapper : IObjectMapper
    {
        private readonly IDictionary<Type, IList<MemberInfo>> _maps = new Dictionary<Type, IList<MemberInfo>>();

        public IDictionary<string, object> GetFields(object item)
        {
            if (item == null)
            {
                return new Dictionary<string, object>();
            }
            var members = GetMembersForType(item.GetType());
            var dictionary = new Dictionary<string, object>();
            foreach (var property in members.OfType<PropertyInfo>())
            {
                dictionary[property.Name] = property.GetValue(item);
            }
            foreach (var field in members.OfType<FieldInfo>())
            {
                dictionary[field.Name] = field.GetValue(item);
            }
            return dictionary;
        }

        private IList<MemberInfo> GetMembersForType(Type type)
        {
            if (!_maps.ContainsKey(type))
            {
                _maps[type] = type.GetMembers();
            }
            return _maps[type];
        }
    }
}