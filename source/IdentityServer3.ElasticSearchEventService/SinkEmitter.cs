using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using Thinktecture.IdentityServer.Core.Events;

namespace Thinktecture.IdentityServer.Services.Contrib
{
    public class Emitter
    {
        private readonly ILogEventSink _sink;

        public Emitter(ILogEventSink sink)
        {
            _sink = sink;
        }

        public void Emit<T>(Event<T> evt, Action<List<LogEventProperty>> addAdditionalProperties = null)
        {
            var jsonDetails = JsonConvert.SerializeObject(evt.Details);
            var properties = new List<LogEventProperty>
            {
                new LogEventProperty("Type", new ScalarValue("IdServerEvent")),
                new LogEventProperty("Category", new ScalarValue(evt.Category)),
                new LogEventProperty("ActivityId", new ScalarValue(evt.Context.ActivityId)),
                new LogEventProperty("MachineName", new ScalarValue(evt.Context.MachineName)),
                new LogEventProperty("ProcessId", new ScalarValue(evt.Context.ProcessId)),
                new LogEventProperty("RemoteIpAddress", new ScalarValue(evt.Context.RemoteIpAddress)),
                new LogEventProperty("SubjectId", new ScalarValue(evt.Context.SubjectId)),
                new LogEventProperty("Details", new ScalarValue(jsonDetails)),
                new LogEventProperty("EventType", new ScalarValue(evt.EventType)),
                new LogEventProperty("Id", new ScalarValue(evt.Id)),
                new LogEventProperty("Message", new ScalarValue(evt.Message)),
                new LogEventProperty("Name", new ScalarValue(evt.Name))
            };

            if (addAdditionalProperties != null)
            {
                addAdditionalProperties(properties);
            }

            var messageTemplateTokens = new List<MessageTemplateToken>
            {
                new PropertyToken("message", evt.Message)
            };
            var messageTemplate = new MessageTemplate(evt.Message, messageTemplateTokens);
            var nativeEvent = new LogEvent(evt.Context.TimeStamp, LogEventLevel.Information, null, messageTemplate, properties);
            _sink.Emit(nativeEvent);
        }
    }
}