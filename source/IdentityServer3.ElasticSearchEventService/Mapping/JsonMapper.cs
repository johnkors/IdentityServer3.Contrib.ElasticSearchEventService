using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class JsonMapper : IObjectMapper
    {
        private readonly string _fieldName;

        public JsonMapper() : this("Json")
        {
        }

        public JsonMapper(string fieldName)
        {
            _fieldName = fieldName;
        }

        public IDictionary<string, object> GetFields(object item)
        {
            var dictionary = new Dictionary<string, object>();
            if (item == null)
            {
                return dictionary;
            }
            dictionary[_fieldName] = JsonConvert.SerializeObject(item);
            return dictionary;
        }
    }
}