using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public class ExpressionVisitor
    {
        private readonly IList<string> _names = new List<string>();

        public string GetPropertyPath(LambdaExpression lambda)
        {
            Visit(lambda.Body);
            return string.Join(".", _names);
        }

        public void Visit(Expression expression)
        {
            DoVisit((dynamic)expression);
        }

        private void DoVisit(UnaryExpression expression)
        {
            Visit(expression.Operand);
        }

        private void DoVisit(LambdaExpression expression)
        {
            Visit(expression.Body);
        }

        private void DoVisit(MemberExpression member)
        {
            if (member.Expression != null)
            {
                Visit(member.Expression);
            }
            _names.Add(member.Member.Name);
        }

        private void DoVisit(ParameterExpression parameter)
        {
        }

        private void DoVisit(MethodCallExpression method)
        {
            if (method.Method.IsDefined(typeof (ExtensionAttribute), false))
            {
                Visit(method.Arguments.First());
            }
            _names.Add(method.Method.Name);
        }

        private static void DoVisit(object invalid)
        {
            var type = invalid.GetType();
            return;
            //throw new InvalidOperationException(string.Format("Don't know how to visit {0}", invalid.GetType()));
        }
    }
}