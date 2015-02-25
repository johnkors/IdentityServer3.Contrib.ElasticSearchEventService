using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IdentityServer3.ElasticSearchEventService.Extensions
{
    public class MemberPathVisitor
    {
        private readonly IList<string> _names = new List<string>();

        public string MemberPath { get { return string.Join(".", _names); } }

        public MemberPathVisitor(LambdaExpression lambda)
        {
            Visit(lambda.Body);
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

        private void DoVisit(MethodCallExpression method)
        {
            if (IsExtensionMethod(method.Method))
            {
                Visit(method.Arguments.First());
            }
            _names.Add(string.Format("{0}()", method.Method.Name));
        }

        private static bool IsExtensionMethod(MethodInfo method)
        {
            return method.IsDefined(typeof (ExtensionAttribute), false);
        }

        private static void DoVisit(object invalid)
        {
            
        }

        public override string ToString()
        {
            return MemberPath;
        }
    }
}