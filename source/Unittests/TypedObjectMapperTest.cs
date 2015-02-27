using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IdentityServer3.ElasticSearchEventService.Extensions;
using IdentityServer3.ElasticSearchEventService.Mapping;
using Unittests.Proofs;
using Unittests.TestData;
using Xunit;

namespace Unittests
{
    public class TypedObjectMapperTest
    {
        [Fact]
        public void Map_MapsOnlySpecificMembers()
        {
            var mapper = new TypedObjectMapper<TestDetails>()
                .Map(d => d.String);

            var details = new TestDetails { String = Some.String };
            var fields = mapper.GetFields(details);

            fields.DoesOnlyContain(Field("String", Some.String));
        }

        [Fact]
        public void MapRemainingMembers_MapsRemainingMembers()
        {
            var mapper = new TypedObjectMapper<TestDetails>()
                .MapRemainingMembers();

            var details = new TestDetails { String = Some.String };
            var fields = mapper.GetFields(details);

            var allMembers = typeof (TestDetails).GetPublicPropertiesAndFields().Select(m => m.Name);

            fields.DoesContainKeys(allMembers);
        }

        [Fact]
        public void MapNonMember_MapsThemToo()
        {
            var mapper = new TypedObjectMapper<TestDetails>()
                .Map(d => d.GetType().Name);
            
            var details = new TestDetails();
            var fields = mapper.GetFields(details);

            fields.DoesContain(Field("GetType().Name", details.GetType().Name));
        }

        private static Expression<Func<KeyValuePair<string, object>, bool>> Field(string key, object value)
        {
            return p => p.Key == key && p.Value == value;
        }
    }
}