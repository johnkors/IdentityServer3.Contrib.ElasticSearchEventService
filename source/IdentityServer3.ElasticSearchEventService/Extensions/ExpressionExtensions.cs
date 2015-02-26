using System;
using System.Linq.Expressions;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetMemberPath<T>(this Expression<Func<T, object>> expression)
        {
            return new MemberPathVisitor(expression).MemberPath;
        }

        public static MemberExpression GetMemberExpressionOrNull<T>(this Expression<Func<T, object>> expression)
        {
            return GetMemberExpression(expression.Body);
        }

        public static Expression StripQuotes(this Expression expression)
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
            return null;
        }
    }
}