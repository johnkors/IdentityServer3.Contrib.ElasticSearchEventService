using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class JsonMapper : IObjectMapper
    {
        public IDictionary<string, object> GetFields(object item)
        {
            var dictionary = new Dictionary<string, object>();
            if (item == null)
            {
                return dictionary;
            }
            dictionary["Json"] = JsonConvert.SerializeObject(item);
            return dictionary;
        }
    }
}