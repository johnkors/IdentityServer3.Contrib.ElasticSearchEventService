using System;
using System.Linq;
using IdentityServer3.ElasticSearchEventService.Mapping;
using IdentityServer3.ElasticSearchEventService.Mapping.Configuration;
using Thinktecture.IdentityServer.Core.Events;
using Unittests.TestData;
using Xunit;

namespace Unittests
{
    public class DefaultLogEventMapperTests
    {
        [Fact]
        public void Wohoo()
        {
            var configuration = new MappingConfigurationBuilder()
                .DetailMaps(c => c
                    .For<TestObject>(o => o
                        .Map(t => t.SomeString)
                        .Map(t => t.GetType().Name)
                        .Map(t => t.Inner.Numbers.FirstOrDefault())
                    )
                )
                .AlwaysAdd("pølse", "maker")
                .GetConfiguration();

            var mapper = new DefaultLogEventMapper(configuration);

            var logEvent = mapper.Map(new Event<TestObject>("category", "name", EventTypes.Information, 42, new TestObject{SomeString = "pølse", Inner = new InnerObject{Value="maker"}}));
            
            Print(string.Join(Environment.NewLine, logEvent.Properties.Select(p => string.Join("=", p.Key, p.Value))));
        }

        private static void Print(string value)
        {
            throw new Exception(value);
        }
    }
}