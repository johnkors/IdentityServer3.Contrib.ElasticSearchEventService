using System;
using System.Collections.Generic;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class ObjectMapper<T> : IObjectMapper
    {
        private readonly Func<T, IDictionary<string, object>> _getFields;

        public ObjectMapper(Func<T, IDictionary<string, object>> getFields)
        {
            _getFields = getFields;
        }

        public IDictionary<string, object> GetFields(T details)
        {
            return _getFields(details);
        }
    }
}