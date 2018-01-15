using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper.ExpressionGeneration
{
    /// <summary>
    /// Base class for builders that require expressions
    /// </summary>
    public class BuilderBase
    {
        /// <summary>
        /// The expression generator type that implements <see cref="IExpressionToSqlGenerator"/>
        /// </summary>
        protected readonly Type GeneratorType;

        /// <summary>
        /// The parameter count
        /// </summary>
        protected int ParamCount;

        /// <summary>
        /// The <see cref="SqlMapperResult"/> result that contains the query and parameters
        /// </summary>
        protected readonly SqlMapperResult Result;

        /// <summary>
        /// The builder that builds the WHERE clause
        /// </summary>
        protected readonly StringBuilder WhereBuilder;

        /// <summary>
        /// Instantiates the <see cref="BuilderBase"/> class
        /// </summary>
        /// <param name="generatorType">The expression generator type that implements <see cref="IExpressionToSqlGenerator"/></param>
        protected BuilderBase(Type generatorType)
        {
            GeneratorType = generatorType ?? typeof(ExpressionToSqlGenerator);
            WhereBuilder = new StringBuilder();
            Result = new SqlMapperResult();
        }

        /// <summary>
        /// Resets the class properties to their default values
        /// </summary>
        protected void ResetProps()
        {
            ParamCount = 0;
            Result.Query = null;
            Result.Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Traverses a lambda expression, builds a SQL statement, and adds to the parameters
        /// </summary>
        /// <param name="lambda">The lambda expression</param>
        /// <returns>The condition built from traversing the expression tree</returns>
        protected virtual string GetConditionAndSetParameters(LambdaExpression lambda)
        {
            var generator = (IExpressionToSqlGenerator)Activator.CreateInstance(GeneratorType);
            generator.SetLambdaAndParameterCount(lambda, ParamCount);

            var condition = generator.GetBuiltCondition(out var parameters, out ParamCount);
            foreach (var param in parameters)
            {
                Result.Parameters.Add(param.Key, param.Value);
            }
            return condition;
        }

        /// <summary>
        /// Traverses the predicate expression and obtains the column name and the comma delimited parameters that are required to be used in the WHERE clause
        /// </summary>
        /// <typeparam name="T">The table type</typeparam>
        /// <typeparam name="TValue">The type of values</typeparam>
        /// <param name="predicate">A predicate that returns the column to use for the IN clause</param>
        /// <param name="values">The values to include in the IN clause</param>
        /// <returns>A tuple that contains the column name and the comma delimited parameters for the WHERE clause</returns>
        protected (string columnName, string inClause) DoIn<T, TValue>(Expression<Func<T, object>> predicate, IEnumerable<TValue> values) where T : new()
        {
            var method = predicate.Compile();
            var obj = method(new T());

            var alias = predicate.Parameters[0].Name;
            var props = obj?.GetType().GetProperties();
            string columnName;
            if (props != null && props.Any())
            {
                if (props.Count() > 1)
                {
                    throw new ArgumentException("The predicate can only contain one property");
                }

                columnName = $"{alias}.{props.ElementAt(0).Name}";
            }
            else
            {
                if (!(predicate.Body is MemberExpression memberExpression))
                {
                    if (predicate.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
                    {
                        memberExpression = unaryMemberExpression;
                    }
                    else
                    {
                        throw new ArgumentException("The predicate can only be a single member expression");
                    }
                }

                columnName = $"{alias}.{memberExpression.Member.Name}";
            }

            var inClause = new StringBuilder();
            foreach (var value in values)
            {
                var propName = $"PARAM{ParamCount++}";
                inClause.Append($"@{propName}, ");
                Result.Parameters.Add(propName, value);
            }
            inClause.Remove(inClause.Length - 2, 2);
            return (columnName, inClause.ToString());
        }

        /// <summary>
        /// Add the custom where clause to the where builder
        /// </summary>
        /// <param name="sql">The custom where clause</param>
        /// <param name="parameters">The parameters required for the where clause</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="sql"/> argument does not start with 'WHERE '</exception>
        protected void DoWhereCustomSql(string sql, Dictionary<string, object> parameters)
        {
            var trimmedSql = sql.Trim();
            if (!trimmedSql.ToLower().StartsWith("where ")) throw new ArgumentException("The sql parameter does not begin with 'WHERE '");
                        
            WhereBuilder.Clear().Append($" {trimmedSql}");
            foreach (var pair in parameters)
            {
                Result.Parameters.Add(pair.Key, pair.Value);
            }
        }
    }
}
