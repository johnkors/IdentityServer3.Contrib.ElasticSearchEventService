using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IdentityServer3.Core.Events;
using IdentityServer3.ElasticSearchEventService.Extensions;
using IdentityServer3.ElasticSearchEventService.Mapping;
using IdentityServer3.ElasticSearchEventService.Mapping.Configuration;
using Serilog.Events;
using Unittests.Extensions;
using Unittests.Proofs;
using Unittests.TestData;
using Xunit;

namespace Unittests
{
    public class DefaultLogEventMapperTests
    {
        [Fact]
        public void SpecifiedProperties_AreMapped()
        {
            var mapper = CreateMapper(b => b
                .DetailMaps(c => c
                    .For<TestDetails>(m => m
                        .Map(d => d.String)
                    )
                ));

            var details = new TestDetails
            {
                String = "Polse"
            };

            var logEvent = mapper.Map(CreateEvent(details));

            logEvent.Properties.DoesContain(LogEventValueWith("Details.String", Quote("Polse")));
        }
        
        [Fact]
        public void AlwaysAddedValues_AreAlwaysAdded()
        {
            var mapper = CreateMapper(b => b.AlwaysAdd("some", "value"));

            var logEvent = mapper.Map(CreateEvent(new object()));

            logEvent.Properties.DoesContain(LogEventValueWith("some", Quote("value")));
        }

        [Fact]
        public void MapToSpecifiedName_MapsToSpecifiedName()
        {
            var mapper = CreateMapper(b => b
                .DetailMaps(m => m
                    .For<TestDetails>(o => o
                        .Map("specificName", d => d.String)
                    )
                ));

            var logEvent = mapper.Map(CreateEvent(new TestDetails {String = Some.String}));

            logEvent.Properties.DoesContain(LogEventValueWith("Details.specificName", Quote(Some.String)));
        }

        [Fact]
        public void PropertiesThatThrowsException_AreMappedAsException()
        {
            var mapper = CreateMapper(b => b
                .DetailMaps(m => m
                    .For<TestDetails>(o => o
                        .Map(d => d.ThrowsException)
                    )
                ));

            var logEvent = mapper.Map(CreateEvent(new TestDetails()));

            logEvent.Properties.DoesContain(LogEventValueWith("Details.ThrowsException", s => s.StartsWith("\"threw")));
        }

        [Fact]
        public void DefaultMapAllMembers_MapsAllPropertiesAndFieldsForDetails()
        {
            var mapper = CreateMapper(b => b
                .DetailMaps(m => m
                    .DefaultMapAllMembers()
                ));
            var logEvent = mapper.Map(CreateEvent(new TestDetails()));

            var members = typeof (TestDetails).GetPublicPropertiesAndFields().Select(m => string.Format("Details.{0}", m.Name));

            logEvent.Properties.DoesContainKeys(members);
        }

        [Fact]
        public void ComplexMembers_AreMappedToJson()
        {
            var mapper = CreateMapper(b => b
                .DetailMaps(m => m
                    .DefaultMapAllMembers()
                ));
            var details = new TestDetails();
            var logEvent = mapper.Map(CreateEvent(details));

            var logProperty = logEvent.GetProperty<ScalarValue>("Details.Inner");

            logProperty.Value.IsEqualTo(details.Inner.ToJsonSuppressErrors());
        }

        private static string Quote(string value)
        {
            return string.Format("\"{0}\"", value);
        }

        private static Expression<Func<KeyValuePair<string, LogEventPropertyValue>, bool>> LogEventValueWith(string key, Func<string,bool> satisfies)
        {
            return p => p.Key == key && satisfies(p.Value.ToString());
        }

        private static Expression<Func<KeyValuePair<string, LogEventPropertyValue>, bool>> LogEventValueWith(string key, string value)
        {
            return p => p.Key == key && p.Value.ToString() == value;
        }

        private static Event<T> CreateEvent<T>(T details)
        {
            return new Event<T>("category", "name", EventTypes.Information, 42, details);
        }

        private static DefaultLogEventMapper CreateMapper(Action<MappingConfigurationBuilder> setup)
        {
            var builder = new MappingConfigurationBuilder();
            setup(builder);
            return new DefaultLogEventMapper(builder.GetConfiguration());
        }
    }
}