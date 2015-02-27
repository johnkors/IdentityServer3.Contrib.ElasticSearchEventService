using System;
using Serilog.Core;
using Serilog.Sinks.ElasticSearch;
using Thinktecture.IdentityServer.Core.Events;
using Thinktecture.IdentityServer.Services.Contrib;
using Unittests.TestData;
using Xunit;

namespace Unittests
{
    public class EmitterTests
    {
        private static readonly DateTimeOffset MockTimeStamp = new DateTimeOffset(2000,1,1,0,0,0, new TimeSpan(0,0,0,0));

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
                TimeStamp = ts.HasValue ? ts.Value : MockTimeStamp
            };
            evt.Details = useNullDetails ? null : new TestObject { SomeString = "This is some custom string" };
            return evt;
        }

        /// <summary>
        /// Use this instead of mocks to test against your instance.
        /// </summary>
        /// <returns></returns>
        private ILogEventSink GetRealSink()
        {
            var options = new ElasticsearchSinkOptions(new Uri("http://your.elasticsearch.instance"));
            options.TypeName = "idsrvevent";
            return new ElasticsearchSink(options);
        }
    }
}
