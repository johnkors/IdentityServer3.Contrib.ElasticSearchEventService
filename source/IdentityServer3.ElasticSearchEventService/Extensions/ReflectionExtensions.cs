using System;
using System.Reflection;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool BelongsTo(this MemberInfo member, Type type)
        {
            return member.DeclaringType != null &&
                (member.DeclaringType == type || member.DeclaringType.IsAssignableFrom(type));
        }
    }
}