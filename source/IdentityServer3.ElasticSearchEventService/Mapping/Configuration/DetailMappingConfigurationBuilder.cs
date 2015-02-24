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

        public DetailMappingConfigurationBuilder For<T>(Action<ObjectMapper<T>> map)
        {
            var mapper = new ObjectMapper<T>();
            map(mapper);
            _configuration.ObjectMappers[typeof (T)] = mapper;
            return this;
        }

        public DetailMappingConfiguration GetConfiguration()
        {
            return _configuration;
        }
    }
}