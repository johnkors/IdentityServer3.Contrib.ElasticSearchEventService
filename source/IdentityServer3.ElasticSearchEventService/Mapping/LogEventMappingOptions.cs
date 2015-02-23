using System.Collections.Generic;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class LogEventMappingOptions
    {
        public ObjectMapCollection DetailMaps { get; set; }
        public IDictionary<string, object> AlwaysAdd { get; set; }

        public LogEventMappingOptions()
        {
            DetailMaps = new ObjectMapCollection();
            AlwaysAdd = new Dictionary<string, object>();
        }
    }
}