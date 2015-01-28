using System.Collections.Generic;


namespace Thinktecture.IdentityServer.Services.Contrib
{
    public class NoOpAdder : IAddExtraPropertiesToEvents
    {
        public IDictionary<string, string> GetNonIdServerFields()
        {
            return new Dictionary<string, string>();
        }
    }
}