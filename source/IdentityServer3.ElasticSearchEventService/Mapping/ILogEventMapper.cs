using Serilog.Events;
using Thinktecture.IdentityServer.Core.Events;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public interface ILogEventMapper
    {
        LogEvent Map<T>(Event<T> evt);
    }
}