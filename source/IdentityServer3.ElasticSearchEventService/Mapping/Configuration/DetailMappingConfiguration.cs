using System;
using System.Collections.Generic;

namespace IdentityServer3.ElasticSearchEventService.Mapping.Configuration
{
    public class DetailMappingConfiguration
    {
        public IDictionary<Type, IObjectMapper> ObjectMappers { get; set; }
        public IObjectMapper DefaultMapper { get; set; }

        public DetailMappingConfiguration()
        {
            ObjectMappers = new Dictionary<Type, IObjectMapper>();
            DefaultMapper = new AdHocObjectMapper();
        }

        public DetailMappingConfiguration Add<T>(Action<TypedObjectMapper<T>> action)
        {
            var mapper = new TypedObjectMapper<T>();
            action(mapper);
            ObjectMappers[typeof(T)] = mapper;
            return this;
        }

        public IDictionary<string, object> GetFields<T>(T details)
        {
            var mapper = GetDetailMapper<T>();
            return mapper.GetFields(details);
        }

        private IObjectMapper GetDetailMapper<T>()
        {
            var type = typeof(T);
            if (ObjectMappers.ContainsKey(type))
            {
                return ObjectMappers[type];
            }
            return DefaultMapper;
        }
    }
}