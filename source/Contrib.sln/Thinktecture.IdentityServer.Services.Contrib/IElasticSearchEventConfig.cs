using System;

namespace Thinktecture.IdentityServer.Services.Contrib
{
    public interface IElasticSearchEventConfig
    {
        Uri ElasticSearchUri { get; }
        string TypeName { get; }
    }
}