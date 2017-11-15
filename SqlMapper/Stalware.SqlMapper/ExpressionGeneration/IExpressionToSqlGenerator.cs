using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper.ExpressionGeneration
{
    /// <summary>
    /// Interface that contains methods for mapping an expression to SQL statements
    /// </summary>
    public interface IExpressionToSqlGenerator
    {
        /// <summary>
        /// Sets the <see cref="LambdaExpression"/> to use throughout the mapping and set the 
        /// initial number for the parameters
        /// </summary>
        /// <param name="lambda">The <see cref="LambdaExpression"/> expression</param>
        /// <param name="count">The initial parameter count</param>
        void SetLambdaAndParameterCount(LambdaExpression lambda, int count);

        /// <summary>
        /// Get the expression in a SQL statement
        /// </summary>
        /// <param name="sqlParameters">The mapped parameters</param>
        /// <param name="paramCount">The final parameter count</param>
        /// <returns>The SQL statement mapped from the provided lambda expression</returns>
        string GetBuiltCondition(out List<KeyValuePair<string, object>> sqlParameters, out int paramCount);

        /// <summary>
        /// Travel a <see cref="BinaryExpression"/> expression
        /// </summary>
        /// <param name="expression">The <see cref="BinaryExpression"/> expression</param>
        /// <returns>A <see cref="StringBuilder"/> that contains the mapped SQL statements from the <paramref name="expression"/></returns>
        StringBuilder BreakDownBinaryExpression(BinaryExpression expression);

        /// <summary>
        /// Executes a <see cref="MethodCallExpression"/> or <see cref="MemberExpression"/>
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>The value of the executed expression</returns>
        object ExecuteMethodOrMemberExpression(Expression expression);

        /// <summary>
        /// Handles a <see cref="MemberExpression"/>
        /// </summary>
        /// <param name="expression">The <see cref="MemberExpression"/> expression</param>
        /// <param name="hasRightHandCompare">Value that determines if a condition for comparing exists 
        /// in the right hand side of an expression</param>
        /// <returns>The mapped SQL statement</returns>
        string HandleMemberExpression(MemberExpression expression, bool hasRightHandCompare);

        /// <summary>
        /// Handles a <see cref="UnaryExpression"/>
        /// </summary>
        /// <param name="expression">The <see cref="UnaryExpression"/> expression</param>
        /// <returns>The mapped SQL statement</returns>
        string HandleUnaryExpression(UnaryExpression expression);

        /// <summary>
        /// Handles a <see cref="ConstantExpression"/>
        /// </summary>
        /// <param name="expression">The <see cref="ConstantExpression"/></param>
        /// <returns>The mapped SQL statement</returns>
        string HandleConstantExpression(ConstantExpression expression);

        /// <summary>
        /// Handles a <see cref="MethodCallExpression"/>
        /// </summary>
        /// <param name="expression">The <see cref="MethodCallExpression"/></param>
        /// <returns>The mapped SQL statement</returns>
        string HandleMethodCallExpression(MethodCallExpression expression);

        /// <summary>
        /// Determine the type of expression and pass to one of the handler methods
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <param name="hasRightHandCompare">Value that determines if a condition for comparing exists 
        /// in the right hand side of an expression</param>
        /// <returns>The mapped SQL statement</returns>
        string DetermineAndDoNextExecution(Expression expression, bool hasRightHandCompare);
    }
}
