using System.Linq.Expressions;

namespace Unittests.Extensions
{
    public static class ExpressionExtensions
    {
        public static string ToFriendlyString(this LambdaExpression lambda)
        {
            return new LambdaString(lambda).FriendlyString;
        }
    }
}