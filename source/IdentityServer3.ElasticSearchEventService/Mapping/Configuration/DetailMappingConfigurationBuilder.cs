using System;

namespace IdentityServer3.ElasticSearchEventService.Mapping.Configuration
{
    public class DetailMappingConfigurationBuilder
    {
        private readonly DetailMappingConfiguration _configuration;

        public DetailMappingConfigurationBuilder()
        {
            _configuration = new DetailMappingConfiguration();
        }

        public DetailMappingConfigurationBuilder For<T>(Action<TypedObjectMapper<T>> map)
        {
            var mapper = new TypedObjectMapper<T>();
            map(mapper);
            _configuration.ObjectMappers[typeof (T)] = mapper;
            return this;
        }

        public DetailMappingConfigurationBuilder DefaultMapAllMembers()
        {
            return DefaultMapper(new AdHocObjectMapper());
        }

        public DetailMappingConfigurationBuilder DefaultToJson(string fieldName = "Json")
        {
            return DefaultMapper(new JsonMapper(fieldName));
        }

        public DetailMappingConfigurationBuilder DefaultMapper(IObjectMapper defaultMapper)
        {
            _configuration.DefaultMapper = defaultMapper;
            return this;
        }

        public DetailMappingConfiguration GetConfiguration()
        {
            return _configuration;
        }
    }
}