using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper.ExpressionGeneration
{
    /// <summary>
    /// Implements <see cref="IExpressionToSqlGenerator"/>
    /// </summary>
    public class ExpressionToSqlGenerator : IExpressionToSqlGenerator
    {
        private LambdaExpression _lambda;
        private readonly List<KeyValuePair<string, object>> _sqlParameters;
        private int _paramCount;

        /// <summary>
        /// Instantiates the <see cref="ExpressionToSqlGenerator"/> class
        /// </summary>
        public ExpressionToSqlGenerator()
        {
            _sqlParameters = new List<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.BreakDownBinaryExpression(BinaryExpression)"/>
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the expression type is not supported and thrown 
        /// by <see cref="DetermineAndDoNextExecution(Expression, bool)"/></exception>
        /// <exception cref="InvalidOperationException">Thrown by <see cref="DetermineAndDoNextExecution(Expression, bool)"/></exception>
        public StringBuilder BreakDownBinaryExpression(BinaryExpression expression)
        {
            var sb = new StringBuilder();

            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(" != ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                default:
                    throw new NotSupportedException("The library does not currently support " +
                        $"the condition '{expression.NodeType.ToString()}'");
            }

            var left = new StringBuilder(DetermineAndDoNextExecution(expression.Left, 
                expression.NodeType != ExpressionType.AndAlso && 
                expression.NodeType != ExpressionType.OrElse));
            var right = new StringBuilder(DetermineAndDoNextExecution(expression.Right, true));

            return left.Insert(0, "(").Append(sb).Append(right).Append(")");
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.DetermineAndDoNextExecution(Expression, bool)"/>
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown by the handler methods</exception>
        /// <exception cref="InvalidOperationException">Thrown if the expression type is not recognized</exception>
        public string DetermineAndDoNextExecution(Expression expression, bool hasRightHandCompare)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                return BreakDownBinaryExpression(binaryExpression).ToString();
            }

            if (expression is MemberExpression memberExpression)
            {
                return HandleMemberExpression(memberExpression, hasRightHandCompare);
            }

            if (expression is UnaryExpression unaryExpression)
            {
                return HandleUnaryExpression(unaryExpression);
            }

            if (expression is ConstantExpression constantExpression)
            {
                return HandleConstantExpression(constantExpression);
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                return HandleMethodCallExpression(methodCallExpression);
            }
            throw new InvalidOperationException("Unrecognized expression in execution sequence");
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.ExecuteMethodOrMemberExpression(Expression)"/>
        /// </summary>
        public object ExecuteMethodOrMemberExpression(Expression expression)
        {
            var objMember = Expression.Convert(expression, typeof(object));
            var getterExpression = Expression.Lambda<Func<object>>(objMember);
            var getter = getterExpression.Compile();
            return getter();
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.GetBuiltCondition(out List{KeyValuePair{string, object}}, out int)"/>
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown by <see cref="DetermineAndDoNextExecution(Expression, bool)"/></exception>
        /// <exception cref="InvalidOperationException">Thrown by <see cref="DetermineAndDoNextExecution(Expression, bool)"/></exception>
        public string GetBuiltCondition(out List<KeyValuePair<string, object>> sqlParameters, out int paramCount)
        {
            var condition = DetermineAndDoNextExecution(_lambda.Body, _lambda.Body is BinaryExpression);
            sqlParameters = _sqlParameters;
            paramCount = _paramCount;
            return condition;
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.HandleConstantExpression(ConstantExpression)"/>
        /// </summary>
        public string HandleConstantExpression(ConstantExpression expression)
        {
            var paramName = $"PARAM{_paramCount++}";
            _sqlParameters.Add(new KeyValuePair<string, object>(paramName, expression.Value));
            return $"@{paramName}";
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.HandleMemberExpression(MemberExpression, bool)"/>
        /// </summary>
        public string HandleMemberExpression(MemberExpression expression, bool hasRightHandCompare)
        {
            if (expression.Expression is ParameterExpression parameterExpression)
            {
                if (expression.Type == typeof(bool) && !hasRightHandCompare)
                {
                    return $"{parameterExpression.Name}.{expression.Member.Name} = 1";
                }
                return $"{parameterExpression.Name}.{expression.Member.Name}";
            }

            var paramName = $"PARAM{_paramCount++}";
            _sqlParameters.Add(new KeyValuePair<string, object>(paramName, ExecuteMethodOrMemberExpression(expression)));
            return $"@{paramName}";
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.HandleMethodCallExpression(MethodCallExpression)"/>
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the method name is not supported or 
        /// if an inner expression is not a constant, method call, or member expression</exception>
        public string HandleMethodCallExpression(MethodCallExpression expression)
        {
            var memberExpression = (MemberExpression)expression.Object;
            if (memberExpression.Expression is ParameterExpression parameterExpression)
            {
                switch (expression.Method.Name)
                {
                    case nameof(object.ToString):
                        return $"CAST({parameterExpression.Name}.{memberExpression.Member.Name} AS VARCHAR(MAX))";
                    case nameof(string.ToUpper):
                        return $"UPPER({parameterExpression.Name}.{memberExpression.Member.Name})";
                    case nameof(string.ToLower):
                        return $"LOWER({parameterExpression.Name}.{memberExpression.Member.Name})";
                    case nameof(string.Contains):
                    case nameof(string.StartsWith):
                    case nameof(string.EndsWith):
                        var argExpression = expression.Arguments[0];
                        object argValue;

                        if (argExpression is ConstantExpression argConstantExpression)
                        {
                            argValue = argConstantExpression.Value;
                        }
                        else if (argExpression is MemberExpression argMemberExpression)
                        {
                            if (argMemberExpression.Expression is ParameterExpression argParameterExpression)
                            {
                                switch (expression.Method.Name)
                                {
                                    case nameof(string.Contains):
                                        return $"{parameterExpression.Name}.{memberExpression.Member.Name} LIKE " +
                                            $"'%' + {argParameterExpression.Name}.{argMemberExpression.Member.Name} + '%'";
                                    case nameof(string.StartsWith):
                                        return $"{parameterExpression.Name}.{memberExpression.Member.Name} LIKE " +
                                            $"{argParameterExpression.Name}.{argMemberExpression.Member.Name} + '%'";
                                    case nameof(string.EndsWith):
                                        return $"{parameterExpression.Name}.{memberExpression.Member.Name} LIKE " +
                                            $"'%' + {argParameterExpression.Name}.{argMemberExpression.Member.Name}";
                                }
                            }

                            argValue = ExecuteMethodOrMemberExpression(argMemberExpression);
                        }
                        else if (argExpression is MethodCallExpression argMethodCallExpression)
                        {
                            var methodMemberExpression = (MemberExpression)argMethodCallExpression.Object;
                            if (methodMemberExpression.Expression is ParameterExpression argParameterExpression)
                            {
                                switch (expression.Method.Name)
                                {
                                    case nameof(string.Contains):
                                        return $"{parameterExpression.Name}.{memberExpression.Member.Name} LIKE " +
                                            $"'%' + {HandleMethodCallExpression(argMethodCallExpression)} + '%'";
                                    case nameof(string.StartsWith):
                                        return $"{parameterExpression.Name}.{memberExpression.Member.Name} LIKE " +
                                            $"{HandleMethodCallExpression(argMethodCallExpression)} + '%'";
                                    case nameof(string.EndsWith):
                                        return $"{parameterExpression.Name}.{memberExpression.Member.Name} LIKE " +
                                            $"'%' + {HandleMethodCallExpression(argMethodCallExpression)}";
                                }
                            }

                            argValue = ExecuteMethodOrMemberExpression(methodMemberExpression);
                        }
                        else
                        {
                            throw new NotSupportedException("Only member, constant, and method call expressions are supported within method expressions");
                        }

                        var argParamName = $"PARAM{_paramCount++}";

                        switch (expression.Method.Name)
                        {
                            case nameof(string.Contains):
                                _sqlParameters.Add(new KeyValuePair<string, object>(argParamName, $"%{argValue}%"));
                                break;
                            case nameof(string.StartsWith):
                                _sqlParameters.Add(new KeyValuePair<string, object>(argParamName, $"{argValue}%"));
                                break;
                            case nameof(string.EndsWith):
                                _sqlParameters.Add(new KeyValuePair<string, object>(argParamName, $"%{argValue}"));
                                break;
                        }
                        return $"{parameterExpression.Name}.{memberExpression.Member.Name} LIKE @{argParamName}";
                    case nameof(string.Trim):
                        return $"LTRIM(RTRIM({parameterExpression.Name}.{memberExpression.Member.Name}))";
                    case nameof(string.TrimStart):
                        return $"LTRIM({parameterExpression.Name}.{memberExpression.Member.Name})";
                    case nameof(string.TrimEnd):
                        return $"RTRIM({parameterExpression.Name}.{memberExpression.Member.Name})";
                    default:
                        throw new NotSupportedException($"The library does not support the call '{expression.Method.Name}'");
                }
            }

            var paramName = $"PARAM{_paramCount++}";
            _sqlParameters.Add(new KeyValuePair<string, object>(paramName, ExecuteMethodOrMemberExpression(expression)));
            return $"@{paramName}";            
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.HandleUnaryExpression(UnaryExpression)"/>
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the expression type is not a bool</exception>
        public string HandleUnaryExpression(UnaryExpression expression)
        {
            var operand = (MemberExpression)expression.Operand;
            var parameterExpression = (ParameterExpression)operand.Expression;
            if (expression.NodeType == ExpressionType.Convert)
            {
                return $"{parameterExpression.Name}.{operand.Member.Name}";
            }

            if (expression.Type != typeof(bool))
            {
                throw new NotSupportedException("Only bool types are supported for unary expressions at this time");
            }
                        
            return $"{parameterExpression.Name}.{operand.Member.Name} = 0";
        }

        /// <summary>
        /// Implements <see cref="IExpressionToSqlGenerator.SetLambdaAndParameterCount(LambdaExpression, int)"/>
        /// </summary>
        public void SetLambdaAndParameterCount(LambdaExpression lambda, int count)
        {
            _lambda = lambda;
            _paramCount = count;
        }
    }
}
