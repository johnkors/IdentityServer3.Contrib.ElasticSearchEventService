using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class AdHocObjectMapper : IObjectMapper
    {
        private readonly IDictionary<Type, IObjectMapper> _maps = new Dictionary<Type, IObjectMapper>();

        public IDictionary<string, object> GetFields(object item)
        {
            if (item == null)
            {
                return new Dictionary<string, object>();
            }
            var mapper = GetMapperForType(item.GetType());
            return mapper.GetFields(item);
        }

        private IObjectMapper GetMapperForType(Type type)
        {
            if (!_maps.ContainsKey(type))
            {
                var mapper = (IObjectMapper) GetType().GetMethod("CreateMapper", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(type).Invoke(this, new object[0]);
                _maps[type] = mapper;
            }
            return _maps[type];
        }

        // Do not delete. It is used by reflection
        private IObjectMapper CreateMapper<T>()
        {
            return new TypedObjectMapper<T>().MapRemainingMembers();
        }
    }
}