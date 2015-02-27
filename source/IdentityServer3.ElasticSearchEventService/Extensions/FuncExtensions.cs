using System;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public static class FuncExtensions
    {
        public static object TryInvoke<T>(this Func<T, object> func, T item)
        {
            return TryInvoke(() => func(item));
        }

        public static object TryInvoke(this Func<object> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                var inner = ex.GetMostInner();
                return string.Format("threw {0}: {1}", inner.GetType().Name, inner.Message);
            }
        }
    }
}