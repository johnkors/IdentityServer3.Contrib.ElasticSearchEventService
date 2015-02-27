using System.Collections.Generic;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public interface IObjectMapper
    {
        IDictionary<string, object> GetFields(object item);
    }
}