using System.Collections.Generic;
using IdentityServer3.ElasticSearchEventService.Mapping;
using Thinktecture.IdentityServer.Services.Contrib;
using Xunit;

namespace Unittests
{
    public class DefaultLogEventMapperTests
    {
        [Fact]
        public void Wohoo()
        {
            var options = new LogEventMappingOptions
            {
                DetailMaps = new ObjectMapCollection()
            };
            
            var mapper = new DefaultLogEventMapper(options);
        }
    }
}