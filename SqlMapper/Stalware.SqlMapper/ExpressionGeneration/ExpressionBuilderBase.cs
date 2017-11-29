using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper.ExpressionGeneration
{
    /// <summary>
    /// Base class for builders that require expressions
    /// </summary>
    public class ExpressionBuilderBase
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
        /// Instantiates the <see cref="ExpressionBuilderBase"/> class
        /// </summary>
        /// <param name="generatorType">The expression generator type that implements <see cref="IExpressionToSqlGenerator"/></param>
        protected ExpressionBuilderBase(Type generatorType)
        {
            GeneratorType = generatorType ?? typeof(ExpressionToSqlGenerator);
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
    }
}
