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
    }
}
