using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IdentityServer3.ElasticSearchEventService.Extensions;
using IdentityServer3.ElasticSearchEventService.Mapping;
using IdentityServer3.ElasticSearchEventService.Mapping.Configuration;
using Serilog.Events;
using Thinktecture.IdentityServer.Core.Events;
using Unittests.Extensions;
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

            AssertContains(logEvent.Properties, Expected("Details.String", Quote("Polse")));
        }
        
        [Fact]
        public void AlwaysAddedValues_AreAlwaysAdded()
        {
            var mapper = CreateMapper(b => b.AlwaysAdd("some", "value"));

            var logEvent = mapper.Map(CreateEvent(new object()));

            AssertContains(logEvent.Properties, Expected("some", Quote("value")));
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

            AssertContains(logEvent.Properties, Expected("Details.specificName", Quote(Some.String)));
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

            AssertContains(logEvent.Properties, Expected("Details.ThrowsException", s => s.StartsWith("\"athrew")));
        }

        [Fact]
        public void DefaultMapAllMembers_MapsAllPropertiesAndFieldsForDetails()
        {
            var mapper = CreateMapper(b => b
                .DetailMaps(m => m
                    .DefaultMapAllMembers()
                ));
            var logEvent = mapper.Map(CreateEvent(new TestDetails()));

            var members = typeof (TestDetails).GetMembers().Where(m => m is PropertyInfo || m is FieldInfo);

            foreach (var member in members)
            {
                var m = member;
                AssertContains(logEvent.Properties, p => p.Key == "Details."+ m.Name);
            }
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

            Assert.Equal(Quote(details.Inner.ToJsonSuppressErrors()), logEvent.Properties["Details.Inner"].ToString());
        }

        private static string Quote(string value)
        {
            return string.Format("\"{0}\"", value);
        }

        private static void AssertContains<T>(IEnumerable<T> collection, Expression<Func<T, bool>> predicate)
        {
            var func = predicate.Compile();
            if (collection.Any(func))
            {
                return;
            }
            throw new Exception(string.Format("Expected {0} with {1}, but found none.", typeof(T).Name, predicate.ToFriendlyString()));
        }

        private static Expression<Func<KeyValuePair<string, LogEventPropertyValue>, bool>> Expected(string key, Func<string,bool> satisfies)
        {
            return p => p.Key == key && satisfies(p.Value.ToString());
        }

        private static Expression<Func<KeyValuePair<string, LogEventPropertyValue>, bool>> Expected(string key, string value)
        {
            return p => p.Key == key && p.Value.ToString() == value;
        }
        

        private static void Print(object value)
        {
            throw new Exception(string.Format("{0}>{1}<",Environment.NewLine, value));
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