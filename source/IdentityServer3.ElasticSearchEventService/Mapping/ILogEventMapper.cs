using IdentityServer3.Core.Events;
using Serilog.Events;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public interface ILogEventMapper
    {
        LogEvent Map<T>(Event<T> evt);
    }
}