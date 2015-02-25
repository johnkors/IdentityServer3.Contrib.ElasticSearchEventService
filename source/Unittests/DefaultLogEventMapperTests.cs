using System;
using System.Linq;
using IdentityServer3.ElasticSearchEventService.Mapping;
using IdentityServer3.ElasticSearchEventService.Mapping.Configuration;
using Thinktecture.IdentityServer.Core.Events;
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
                    .For<AccessTokenIssuedDetails>(m => m
                        .Map(d => d.ClientId)
                        .Map(d => d.ToString())
                        .MapRemainingMembersAsJson()
                        .MapRemainingMembers()
                    )
                    .DefaultMapAllMembers()
                )
                .AlwaysAdd("agurk", () => DateTime.Now)
                .GetConfiguration();

            var mapper = new DefaultLogEventMapper(new MappingConfiguration());

            var logEvent = mapper.Map(new Event<AccessTokenIssuedDetails>("category", "name", EventTypes.Information, 42, new AccessTokenIssuedDetails{ClientId = "klienten"}));
            
            Print(string.Join(Environment.NewLine, logEvent.Properties.Select(p => string.Join("=", p.Key, p.Value))));
        }

        private static void Print(object value)
        {
            throw new Exception(string.Format(">{0}<", value));
        }
    }
}