using System;
using IdentityServer3.ElasticSearchEventService.Mapping;
using Serilog.Core;
using Thinktecture.IdentityServer.Core.Events;

namespace Thinktecture.IdentityServer.Services.Contrib
{
    public class Emitter
    {
        private readonly ILogEventSink _sink;
        private readonly ILogEventMapper _mapper;

        public Emitter(ILogEventSink sink, ILogEventMapper mapper = null)
        {
            if (sink == null)
            {
                throw new ArgumentNullException();
            }
            _sink = sink;
            _mapper = mapper ?? new DefaultLogEventMapper();
        }

        public void Emit<T>(Event<T> evt)
        {
            var nativeEvent = _mapper.Map(evt);
            _sink.Emit(nativeEvent);
        }
    }
}