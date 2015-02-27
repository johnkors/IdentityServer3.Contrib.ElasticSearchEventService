using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        public static IEnumerable<MemberInfo> GetPublicPropertiesAndFields(this Type type)
        {
            return type.GetMembers().Where(m => m is PropertyInfo || m is FieldInfo);
        }

        public static bool IsExtensionMethod(this MethodInfo method)
        {
            return method.IsDefined(typeof(ExtensionAttribute), false);
        }

        private static readonly IDictionary<Type, string> ValueNames = new Dictionary<Type, string>
        {
            {typeof(int), "int"},
            {typeof(short), "short"},
            {typeof(byte), "byte"},
            {typeof(bool), "bool"},
            {typeof(long), "long"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
            {typeof(string), "string"}
        };

        public static string GetFriendlyName(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (ValueNames.ContainsKey(type))
            {
                return ValueNames[type];
            }
            if (type.IsGenericType)
            {
                return string.Format("{0}<{1}>", type.Name.Split('`')[0],
                    string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName)));
            }
            return type.Name;
        }
    }
}