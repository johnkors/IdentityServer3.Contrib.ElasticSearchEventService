using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using IdentityServer3.ElasticSearchEventService.Extensions;
using Unittests.Extensions;

namespace Unittests.Proofs
{
    public static class ProofExtensions
    {
        public static void DoesContain<T>(this IEnumerable<T> collection, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();
            if (collection.Any(func))
            {
                return;
            }
            var message = new StringBuilder()
                .AppendFormat("Expected {0} with {1}", typeof (T).GetFriendlyName(), condition.ToFriendlyString())
                .AppendLine(" but found only:").AppendLine()
                .Append(string.Join(Environment.NewLine, collection))
                .ToString();

            throw new ProofException(message);
        }

        public static void DoesOnlyContain<T>(this IEnumerable<T> actualValues, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();
            var items = actualValues.ToList();
            if (!items.Any())
            {
                throw new ProofException("There are no items in the collection");
            }

            var failingItems = items.Where(item => !func(item)).ToList();

            if (failingItems.Any())
            {
                throw new ProofException(string.Format("These do not satisfy {0}:{1}{2}", condition.ToFriendlyString(), Environment.NewLine, string.Join(Environment.NewLine, failingItems)));
            }
        }

        public static void IsEqualTo<T>(this T actual, T expected)
        {
            if (!actual.Equals(expected))
            {
                throw new ProofException(string.Format("Expected {0}, actual: {1}", expected, actual));
            }
        }

        public static void DoesContainKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> actual, TKey expectedKey)
        {
            actual.DoesContainKeys(new []{expectedKey});
        }

        public static void DoesContainKeys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> actual, IEnumerable<TKey> expectedKeys)
        {
            var actualKeys = actual.Select(v => v.Key).ToList();

            var missingKeys = expectedKeys.Where(k => !actualKeys.Contains(k)).ToList();

            if (missingKeys.Any())
            {
                throw new ProofException(string.Format("Following keys were not found:{0}{1}", Environment.NewLine, string.Join(Environment.NewLine, missingKeys)));
            }
        }

        public static void Is<T>(this T item, Expression<Func<T, bool>> condition)
        {
            var func = condition.Compile();
            if (!func(item))
            {
                throw new ProofException(string.Format("{0} does not satisfy {1}", item, condition.ToFriendlyString()));
            }
        }
    }
}