using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer3.ElasticSearchEventService.Mapping.Configuration;
using Serilog.Events;
using Serilog.Parsing;
using Thinktecture.IdentityServer.Core.Events;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class DefaultLogEventMapper : ILogEventMapper
    {
        private const string None = "None";

        private readonly MappingConfiguration _configuration;

        public DefaultLogEventMapper() : this(new MappingConfiguration())
        {
        }

        public DefaultLogEventMapper(MappingConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LogEvent Map<T>(Event<T> evt)
        {
            var properties = GetProperties(evt).Concat(GetContextProps(evt)).ToList();

            var ts = DateTimeOffset.UtcNow;
            if (evt.Context != null)
            {
                ts = evt.Context.TimeStamp;
            }

            var errorMessage = !string.IsNullOrEmpty(evt.Message) ? evt.Message : None;

            var messageTemplateTokens = new List<MessageTemplateToken>
            {
                new PropertyToken("message", errorMessage)
            };

            var messageTemplate = new MessageTemplate(errorMessage, messageTemplateTokens);

            return new LogEvent(ts, LogEventLevel.Information, null, messageTemplate, properties);
        }

        private IEnumerable<LogEventProperty> GetProperties<T>(Event<T> evt)
        {
            yield return LogEventProp("Category", evt.Category);
            yield return LogEventProp("EventType", evt.EventType);
            yield return LogEventProp("Id", evt.Id);
            yield return LogEventProp("Name", evt.Name);

            var detailFields = _configuration.DetailMaps.GetFields(evt.Details) ?? new Dictionary<string, object>();
            foreach (var field in detailFields)
            {
                yield return LogEventProp(string.Format("Details.{0}", field.Key), field.Value);
            }

            foreach (var alwaysAddedValue in _configuration.AlwaysAddedValues)
            {
                yield return LogEventProp(alwaysAddedValue.Key, alwaysAddedValue.Value);
            }
        }

        private static IEnumerable<LogEventProperty> GetContextProps<T>(Event<T> evt)
        {
            if (evt.Context != null)
            {
                yield return new LogEventProperty("HasContext", new ScalarValue(true));
                yield return LogEventProp("ActivityId", evt.Context.ActivityId);
                yield return LogEventProp("MachineName", evt.Context.MachineName);
                yield return LogEventProp("ProcessId", evt.Context.ProcessId);
                yield return LogEventProp("RemoteIpAddress", evt.Context.RemoteIpAddress);
                yield return LogEventProp("SubjectId", evt.Context.SubjectId);
            }
            else
            {
                yield return new LogEventProperty("HasContext", new ScalarValue(false));
                yield return LogEventProp("ActivityId");
                yield return LogEventProp("MachineName");
                yield return LogEventProp("ProcessId");
                yield return LogEventProp("RemoteIpAddress");
                yield return LogEventProp("SubjectId");
            }
        }

        private static LogEventProperty LogEventProp(string name, object eventProp = null)
        {
            var prop = eventProp ?? None;
            return new LogEventProperty(name, new ScalarValue(prop));
        }
    }
}