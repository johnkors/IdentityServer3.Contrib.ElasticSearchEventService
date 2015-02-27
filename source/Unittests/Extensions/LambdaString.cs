using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IdentityServer3.ElasticSearchEventService.Extensions;

namespace Unittests.Extensions
{
    public class LambdaString
    {
        private readonly Stack<object> _expressions = new Stack<object>();

        public string FriendlyString { get; private set; }

        public LambdaString(LambdaExpression lambda)
        {
            Visit(lambda.Body);
            FriendlyString = string.Join(" ", _expressions.Reverse());
        }

        private void Visit(Expression expression)
        {
            if (expression == null)
            {
                return;
            }
            DoVisit((dynamic)expression);
        }

        private void DoVisit(BinaryExpression binary)
        {
            Visit(binary.Left);
            _expressions.Push(Friendly(binary.NodeType));
            Visit(binary.Right);
        }

        private static string Friendly(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return "AND";
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return "OR";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return "==";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.Negate:
                    return "-";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                default:
                    return nodeType.ToString();
            }
        }

        private void DoVisit(InvocationExpression invocation)
        {
            foreach (var arg in invocation.Arguments)
            {
                Visit(arg);
            }
            Visit(invocation.Expression);
        }

        private void DoVisit(MemberExpression memberExpression)
        {
            if (memberExpression.Expression is ParameterExpression)
            {
                _expressions.Push(memberExpression.Member.Name);
            }
            else if (memberExpression.Expression is ConstantExpression && memberExpression.Member is FieldInfo)
            {
                var constant = (ConstantExpression) memberExpression.Expression;
                var field = (FieldInfo)memberExpression.Member;
                if (typeof (Delegate).IsAssignableFrom(memberExpression.Type))
                {
                    var del = (Delegate) field.GetValue(constant.Value);
                    var name = string.Format("{0}({1})", field.Name,
                        string.Join(",", del.Method.GetParameters().Select(p => string.Format("{0} {1}", p.ParameterType.Name, p.Name))));
                    _expressions.Push(name);
                    

                }
                else
                {
                    var target = constant.Value;
                    var value = field.GetValue(target);
                    _expressions.Push(Format(value));
                }
            }
            else
            {
                Visit(memberExpression.Expression);
            }
        }

        private void DoVisit(ParameterExpression parameter)
        {
            if (parameter.IsByRef)
            {
                _expressions.Push(GetValue(parameter));
            }
        }

        private void DoVisit(MethodCallExpression methodCall)
        {
            var isExtension = methodCall.Method.IsExtensionMethod();
            if (isExtension)
            {
                Visit(methodCall.Arguments[0]);
            }
            Visit(methodCall.Object);
            _expressions.Push(methodCall.Method.Name);
            var arguments = isExtension ? methodCall.Arguments.Skip(1) : methodCall.Arguments;
            foreach (var expression in arguments)
            {
                Visit(expression);
            }
        }

        private static object GetValue(Expression expression)
        {
            var lambda = Expression.Lambda(expression);
            var value = lambda.Compile().DynamicInvoke(new object[0]);
            return value;
        }

        private static object Format(object value)
        {
            if (value is string)
            {
                return string.Format("\"{0}\"", value);
            }
            if (value is char)
            {
                return string.Format("'{0}'", value);
            }
            return value.ToString();
        }

        private void DoVisit(ConstantExpression constant)
        {
            _expressions.Push(Format(constant.Value));
        }

        private void DoVisit(UnaryExpression unary)
        {
            Visit(unary.Operand);
        }

        private void DoVisit(LambdaExpression lambda)
        {
            Visit(lambda.Body);
        }

        private void DoVisit(object unknown)
        {
            _expressions.Push(unknown);
        }

        public override string ToString()
        {
            return FriendlyString;
        }
    }
}