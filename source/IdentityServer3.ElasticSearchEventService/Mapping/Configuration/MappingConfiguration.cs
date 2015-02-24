using System.Collections.Generic;

namespace IdentityServer3.ElasticSearchEventService.Mapping.Configuration
{
    public class MappingConfiguration
    {
        public IDictionary<string, object> AlwaysAddedValues { get; set; }
        public DetailMappingConfiguration DetailMaps { get; set; }

        public MappingConfiguration()
        {
            DetailMaps = new DetailMappingConfiguration();
            AlwaysAddedValues = new Dictionary<string, object>();
        }

        public MappingConfiguration AlwaysAdd(IEnumerable<KeyValuePair<string, object>> values)
        {
            AlwaysAddedValues = new Dictionary<string, object>();
            foreach (var pair in values)
            {
                AlwaysAddedValues[pair.Key] = pair.Value;
            }
            return this;
        }
    }
}