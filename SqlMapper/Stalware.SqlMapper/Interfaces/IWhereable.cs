using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper.Interfaces
{
    /// <summary>
    /// An interface for implementing a WHERE clause statement
    /// </summary>
    /// <typeparam name="T">The table type parameter</typeparam>
    /// <typeparam name="TBuilder">The builder type parameter</typeparam>
    public interface IWhereable<T, TBuilder>
    {
        /// <summary>
        /// Adds a WHERE clause
        /// </summary>
        /// <param name="predicate">A conditional predicate to populate the WHERE clause. The object created by the first type parameter in <see cref="IWhereable{T, TBuilder}"/> 
        /// is passed to the predicate</param>
        /// <returns>The builder</returns>
        TBuilder Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Specify your own WHERE clause with this method.
        /// </summary>
        /// <remarks>
        /// This will reset the WHERE clause. The <paramref name="sql"/> argument must begin with 'WHERE '. 
        /// The <paramref name="parameters"/> argument will not replace the current list of paramters. It will only be added on.
        /// </remarks>
        /// <param name="sql">The WHERE clause</param>
        /// <param name="parameters">The parameters needed for the WHERE clause</param>
        /// <returns>The builder</returns>
        TBuilder WhereCustomSql(string sql, Dictionary<string, object> parameters);
    }
}
