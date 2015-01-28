using System;
using System.Collections.Generic;
using FakeItEasy;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.ElasticSearch;
using Thinktecture.IdentityServer.Core.Events;
using Thinktecture.IdentityServer.Services.Contrib;
using Xunit;

namespace Unittests
{
    public class SinkEmitterTests
    {
        private static readonly DateTimeOffset _mockTimeStamp = new DateTimeOffset(2000,1,1,0,0,0, new TimeSpan(0,0,0,0));

        [Fact]
        public void WhenEmitting_EmitsAllIdServerProperties()
        {
            var mockSink = new Fake<ILogEventSink>();
            ILogEventSink logEventSink = mockSink.FakedObject;
            var emitter = new Emitter(logEventSink);

            var evt = CreateIdSrvEvent();
            
            // Fetch object that was sent into elastic sink 
            LogEvent objectSentIn = null;
            A.CallTo(() => logEventSink.Emit(A<LogEvent>._)).Invokes(call => objectSentIn = call.GetArgument<LogEvent>(0));

            emitter.Emit(evt);

            
            var seriLogEvent = CreateExpectedLoggedSerilogEvent(evt);

            Assert.Equal(objectSentIn.Properties, seriLogEvent.Properties);
        }


        [Fact]
        public void WhenEmittingAdditionals_EmitsAllIdServerPropertiesAndAdditonals()
        {
            var mockSink = new Fake<ILogEventSink>();
            ILogEventSink logEventSink = mockSink.FakedObject;
            var emitter = new Emitter(logEventSink);

            var evt = CreateIdSrvEvent();

            // Fetch object that was sent into elastic sink 
            LogEvent objectSentIn = null;
            A.CallTo(() => logEventSink.Emit(A<LogEvent>._)).Invokes(call => objectSentIn = call.GetArgument<LogEvent>(0));

            emitter.Emit(evt, p => p.Add(new LogEventProperty("LogThis", new ScalarValue("SomeAdditionalValue"))));


            var seriLogEvent = CreateExpectedLoggedSerilogEvent(evt, AdditionalProp:new LogEventProperty("LogThis", new ScalarValue("SomeAdditionalValue")));
            
            Assert.Equal(objectSentIn.Properties, seriLogEvent.Properties);
        }

        [Fact]
        public void WhenReceivingNoDetails_AddsNoneStringToParameters()
        {
            var mockSink = new Fake<ILogEventSink>();
            ILogEventSink logEventSink = mockSink.FakedObject;
            var emitter = new Emitter(logEventSink);

            var evt = CreateIdSrvEvent(null, true);
            

            // Fetch object that was sent into elastic sink 
            LogEvent objectSentIn = null;
            A.CallTo(() => logEventSink.Emit(A<LogEvent>._)).Invokes(call => objectSentIn = call.GetArgument<LogEvent>(0));

            emitter.Emit(evt);

            
            var details = objectSentIn.Properties["Details"];
            string detailStr = details.ToString();
            Assert.True(detailStr.Contains("None"));
        }

        [Fact]
        public void WhenReceivingNullContext_AddsNoneStringToContext()
        {
            var mockSink = new Fake<ILogEventSink>();
            ILogEventSink logEventSink = mockSink.FakedObject;
            var emitter = new Emitter(logEventSink);

            var evt = CreateIdSrvEvent();
            evt.Context = null;

            // Fetch object that was sent into elastic sink 
            LogEvent objectSentIn = null;
            A.CallTo(() => logEventSink.Emit(A<LogEvent>._)).Invokes(call => objectSentIn = call.GetArgument<LogEvent>(0));

            emitter.Emit(evt);


            

            Assert.Equal(objectSentIn.Properties["HasContext"].ToString(), "False");
        }

        [Fact(Skip = "Only use if you want to test against a real elastic node")]
        public void TestHostedInstance()
        {
            var r = GetRealSink();
            var emitter = new Emitter(r);
            var evt = CreateIdSrvEvent(DateTimeOffset.UtcNow);
            emitter.Emit(evt);
        }

        private static Event<TestObject> CreateIdSrvEvent(DateTimeOffset? ts = null, bool useNullDetails = false)
        {
            var evt = new Event<TestObject>("SomeCategory", "SomeName", EventTypes.Success, 1, "SomeMessage");
          
            evt.Context = new EventContext
            {
                ActivityId = "SomeActivityId",
                MachineName = "SomeMachineName",
                ProcessId = 2,
                RemoteIpAddress = "SomeRemoteIpAdress",
                SubjectId = "SomeSubjectId",
                TimeStamp = ts.HasValue ? ts.Value : _mockTimeStamp
            };
            evt.Details = useNullDetails ? null : new TestObject { SomeString = "This is some custom string" };
            return evt;
        }

        private static LogEvent CreateExpectedLoggedSerilogEvent(Event<TestObject> evt, LogEventProperty AdditionalProp = null, string expecteddetails = null)
        {
            var details = "";
            if (expecteddetails != null)
            {
                details = "None";
            }
            else
            {
                details = JsonConvert.SerializeObject(evt.Details);    
            }
            
            var properties = new List<LogEventProperty>
            {
                new LogEventProperty("HasContext", new ScalarValue(true)),
                new LogEventProperty("Type", new ScalarValue("IdServerEvent")),
                new LogEventProperty("Category", new ScalarValue(evt.Category)),
                new LogEventProperty("Details", new ScalarValue(details)),
                new LogEventProperty("EventType", new ScalarValue(evt.EventType)),
                new LogEventProperty("Id", new ScalarValue(evt.Id)),
                new LogEventProperty("Name", new ScalarValue(evt.Name)),
                new LogEventProperty("ActivityId", new ScalarValue(evt.Context.ActivityId)),
                new LogEventProperty("MachineName", new ScalarValue(evt.Context.MachineName)),
                new LogEventProperty("ProcessId", new ScalarValue(evt.Context.ProcessId)),
                new LogEventProperty("RemoteIpAddress", new ScalarValue(evt.Context.RemoteIpAddress)),
                new LogEventProperty("SubjectId", new ScalarValue(evt.Context.SubjectId)),
            };

            if (AdditionalProp != null)
            {
                properties.Add(AdditionalProp);
            }

            var messageTemplateTokens = new List<MessageTemplateToken>
            {
                new PropertyToken("message", "SomeMessage")
            };
            var messageTemplate = new MessageTemplate("SomeMessage", messageTemplateTokens);
            var seriLogEvent = new LogEvent(evt.Context.TimeStamp, LogEventLevel.Information, null, messageTemplate, properties);
            return seriLogEvent;
        }

        /// <summary>
        /// Use this instead of mocks to test against your instance.
        /// </summary>
        /// <returns></returns>
        private ILogEventSink GetRealSink()
        {
            var options = new ElasticsearchSinkOptions(new Uri("http://your.logstashserver.com"));
            options.TypeName = "idsrvevent";
            return new ElasticsearchSink(options);
        }
    }

    public class TestObject
    {
        public string SomeString { get; set; }
    }
}
