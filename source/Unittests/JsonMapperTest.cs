using IdentityServer3.ElasticSearchEventService.Extensions;
using IdentityServer3.ElasticSearchEventService.Mapping;
using Unittests.Proofs;
using Unittests.TestData;
using Xunit;

namespace Unittests
{
    public class JsonMapperTest
    {
        [Fact]
        public void GetFields_SerializesObjectToJson()
        {
            var mapper = new JsonMapper();

            var details = new TestDetails();
            var fields = mapper.GetFields(details);

            fields["Json"].IsEqualTo(details.ToJsonSuppressErrors());
        }

        [Fact]
        public void GetFields_UsesSpecifiedKey()
        {
            var mapper = new JsonMapper(Some.Key);

            var details = new TestDetails();
            var fields = mapper.GetFields(details);

            fields.DoesContainKey(Some.Key);
        }
    }
}