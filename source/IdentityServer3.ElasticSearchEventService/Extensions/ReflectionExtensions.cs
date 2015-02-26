using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public static class ReflectionExtensions
    {
        public static Expression<Func<T, object>> ToLambda<T>(this PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof (T), "item");
            var call = Expression.Call(parameter, property.GetGetMethod());
            return Expression.Lambda<Func<T, object>>(call, parameter);
        }

        public static Expression<Func<T, object>> ToLambda<T>(this FieldInfo field)
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            var call = Expression.Field(parameter, field);
            return Expression.Lambda<Func<T, object>>(call, parameter);
        }

        public static bool BelongsTo(this MemberInfo member, Type type)
        {
            return member.DeclaringType != null &&
                (member.DeclaringType == type || member.DeclaringType.IsAssignableFrom(type));
        }

        public static object TryGetValue(this PropertyInfo property, object item)
        {
            return TryGet(() => property.GetValue(item));
        }

        public static object TryGetValue(this FieldInfo field, object item)
        {
            return TryGet(() => field.GetValue(item));
        }

        private static object TryGet(Func<object> func)
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

        public static bool IsSimpleType(this Type type)
        {
            return type.IsValueType || type == typeof (string);
        }
    }
}