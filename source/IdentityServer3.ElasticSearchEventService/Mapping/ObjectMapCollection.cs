using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class ObjectMapCollection
    {
        private readonly IDictionary<Type, IObjectMapper> _detailFieldsMappers = new Dictionary<Type, IObjectMapper>();

        public void For<T>(Func<T, IDictionary<string, object>> getFields)
        {
            _detailFieldsMappers[typeof(T)] = new ObjectMapper<T>(getFields);
        }

        public IDictionary<string, object> GetFields<T>(T details)
        {
            var mapper = GetMapper<T>();
            return mapper == null
                ? new Dictionary<string, object> { { "Raw", JsonConvert.SerializeObject(details) } }
                : mapper.GetFields(details);
        }

        private ObjectMapper<T> GetMapper<T>()
        {
            if (_detailFieldsMappers.ContainsKey(typeof(T)))
            {
                return (ObjectMapper<T>)_detailFieldsMappers[typeof(T)];
            }
            return null;
        }
    }
}