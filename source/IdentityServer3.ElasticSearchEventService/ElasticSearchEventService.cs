using System.Threading.Tasks;
using IdentityServer3.Core.Events;
using IdentityServer3.Core.Services;
using IdentityServer3.ElasticSearchEventService.Mapping;
using Serilog.Sinks.Elasticsearch;

namespace Thinktecture.IdentityServer.Services.Contrib
{
    public class ElasticSearchEventService : IEventService
    {
        private readonly Emitter _emitter;

        public ElasticSearchEventService(ElasticsearchSinkOptions options, ILogEventMapper mapper = null)
        {
            var sink = new ElasticsearchSink(options);
            _emitter = new Emitter(sink, mapper);
        }
        
        public Task RaiseAsync<T>(Event<T> evt)
        {
            _emitter.Emit(evt);
            return Task.FromResult(0);
        }
    }
}
