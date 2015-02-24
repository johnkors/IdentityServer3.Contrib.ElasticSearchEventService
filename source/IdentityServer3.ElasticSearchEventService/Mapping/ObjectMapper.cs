using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IdentityServer3.ElasticSearchEventService.Extensions;

namespace IdentityServer3.ElasticSearchEventService.Mapping
{
    public class ObjectMapper<T> : IObjectMapper
    {
        private readonly IDictionary<string, Func<T, object>> _maps = new Dictionary<string, Func<T, object>>();

        public ObjectMapper<T> Map(Expression<Func<T, object>> expression)
        {
            return Map(expression.GetMemberName(), expression.Compile());
        }

        public ObjectMapper<T> Map(string name, Func<T, object> func)
        {
            _maps[name] = func;
            return this;
        }

        public IDictionary<string, object> GetFields(object item)
        {
            return GetFields((T) item);
        }

        public IDictionary<string, object> GetFields(T item)
        {
            var fields = new Dictionary<string, object>();
            foreach (var pair in _maps)
            {
                fields[pair.Key] = pair.Value(item);
            }
            return fields;
        }
    }
}