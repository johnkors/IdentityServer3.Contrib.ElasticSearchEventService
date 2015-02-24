using System;
using System.Collections.Generic;

namespace IdentityServer3.ElasticSearchEventService.Mapping.Configuration
{
    public class MappingConfigurationBuilder
    {
        private readonly DetailMappingConfigurationBuilder _detailMappingConfigurationBuilder;

        private readonly IDictionary<string, object> _alwaysAddedValues = new Dictionary<string, object>();

        public MappingConfigurationBuilder()
        {
            _detailMappingConfigurationBuilder = new DetailMappingConfigurationBuilder();
        }

        public MappingConfigurationBuilder DetailMaps(Action<DetailMappingConfigurationBuilder> configure)
        {
            configure(_detailMappingConfigurationBuilder);
            return this;
        }

        public MappingConfigurationBuilder AlwaysAdd(string key, object value)
        {
            _alwaysAddedValues[key] = value;
            return this;
        }

        public MappingConfiguration GetConfiguration()
        {
            return new MappingConfiguration
            {
                AlwaysAddedValues = _alwaysAddedValues,
                DetailMaps = _detailMappingConfigurationBuilder.GetConfiguration()
            };
        }
    }
}