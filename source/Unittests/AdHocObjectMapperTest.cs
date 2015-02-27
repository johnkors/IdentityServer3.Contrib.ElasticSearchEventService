using System.Linq;
using IdentityServer3.ElasticSearchEventService.Extensions;
using IdentityServer3.ElasticSearchEventService.Mapping;
using Unittests.Proofs;
using Unittests.TestData;
using Xunit;

namespace Unittests
{
    public class AdHocObjectMapperTest
    {
        [Fact]
        public void GetFields_MapsAllPublicPropertiesAndFields()
        {
            var mapper = new AdHocObjectMapper();

            var details = new TestDetails();
            var fields = mapper.GetFields(details);

            var memberNames = typeof (TestDetails).GetPublicPropertiesAndFields().Select(m => m.Name);

            fields.DoesContainKeys(memberNames);
        }
    }
}