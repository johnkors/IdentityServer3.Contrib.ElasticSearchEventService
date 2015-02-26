using System;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception GetMostInner(this Exception exception)
        {
            if (exception == null)
            {
                return null;
            }
            var inner = exception;
            while (inner.InnerException != null)
            {
                inner = inner.InnerException;
            }
            return inner;
        }
    }
}