using System;
using System.Linq.Expressions;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetMemberName<T>(this Expression<Func<T, object>> expression)
        {
            return new ExpressionVisitor().GetPropertyPath(expression);
        }

        private static Expression StripQuotes(Expression expression)
        {
            var exp = expression;
            while (exp is UnaryExpression)
            {
                exp = ((UnaryExpression) exp).Operand;
            }
            return exp;
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            return DoGetMemberExpression((dynamic)expression);
        }

        private static MemberExpression DoGetMemberExpression(MemberExpression expression)
        {
            return expression;
        }

        private static MemberExpression DoGetMemberExpression(UnaryExpression expression)
        {
            return GetMemberExpression(expression.Operand);
        }

        private static MemberExpression DoGetMemberExpression(object invalid)
        {
            throw new InvalidOperationException(string.Format("Don't know how to get MemberExpression from {0}", invalid.GetType()));
        }
    }
}