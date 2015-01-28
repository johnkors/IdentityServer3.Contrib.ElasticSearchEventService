using System.Collections.Generic;

namespace Thinktecture.IdentityServer.Services.Contrib
{
    public interface IAddExtraPropertiesToEvents
    {
        IDictionary<string, string> GetNonIdServerFields();
    }
}