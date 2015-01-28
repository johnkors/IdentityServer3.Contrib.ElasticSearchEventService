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
        private string _NONE = "None";

        public Emitter(ILogEventSink sink)
        {
            _sink = sink;
        }

        public void Emit<T>(Event<T> evt, Action<List<LogEventProperty>> addAdditionalProperties = null)
        {
            var details = _NONE;
            if (evt.Details != null)
            {
                try
                {
                    var jsonDetails = JsonConvert.SerializeObject(evt.Details);
                    details = jsonDetails;
                }
                catch
                {
                    details = _NONE + ". Unserializable " + details.GetType();
                }
            }

            var properties = new List<LogEventProperty>
            {   
                LogEventProp("Category", evt.Category),
                LogEventProp("Details", details),
                LogEventProp("EventType", evt.EventType),
                LogEventProp("Id", evt.Id),                                
                LogEventProp("Name", evt.Name)
            };

            var ts = DateTimeOffset.UtcNow;
            if (evt.Context != null)
            {
                ts = evt.Context.TimeStamp;
            }
            
            AddContextProps(properties, evt);

            if (addAdditionalProperties != null)
            {
                addAdditionalProperties(properties);
            }

            var errorMessage = !string.IsNullOrEmpty(evt.Message) ? evt.Message : _NONE;

            var messageTemplateTokens = new List<MessageTemplateToken>
            {
                new PropertyToken("message", errorMessage)
            };

            var messageTemplate = new MessageTemplate(errorMessage, messageTemplateTokens);
            var nativeEvent = new LogEvent(ts, LogEventLevel.Information, null, messageTemplate, properties);
            _sink.Emit(nativeEvent);
        }

        private void AddContextProps<T>(List<LogEventProperty> properties, Event<T> evt)
        {
            if (evt.Context != null)
            {
                var contextProps = new List<LogEventProperty>
                {
                    new LogEventProperty("HasContext", new ScalarValue(true)),
                    LogEventProp("ActivityId", evt.Context.ActivityId),
                    LogEventProp("MachineName", evt.Context.MachineName),
                    LogEventProp("ProcessId", evt.Context.ProcessId),
                    LogEventProp("RemoteIpAddress", evt.Context.RemoteIpAddress),
                    LogEventProp("SubjectId", evt.Context.SubjectId)
                };
                properties.AddRange(contextProps);
            }
            else
            {
                var emptyCtxProps = new List<LogEventProperty>
                {
                    new LogEventProperty("HasContext", new ScalarValue(false)),
                    LogEventProp("ActivityId"),
                    LogEventProp("MachineName"),
                    LogEventProp("ProcessId"),
                    LogEventProp("RemoteIpAddress"),
                    LogEventProp("SubjectId")
                };
                properties.AddRange(emptyCtxProps);
            }

        }

        private LogEventProperty LogEventProp(string name, object eventProp = null)
        {
            var prop = eventProp ?? _NONE;
            return new LogEventProperty(name, new ScalarValue(prop));
        }
    }
}