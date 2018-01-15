using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper.Interfaces
{
    /// <summary>
    /// An interface for implementing an IN clause for the WHERE clause
    /// </summary>
    /// <typeparam name="T">The table type</typeparam>
    /// <typeparam name="TBuilder">The builder type</typeparam>
    public interface IInable<T, TBuilder>
    {
        /// <summary>
        /// Adds to the WHERE clause by adding an IN clause. (EX: ID IN (1, 2, 3, 4, 5))
        /// </summary>
        /// <typeparam name="TValue">The type of values</typeparam>
        /// <param name="predicate">A predicate that returns the column to use for the IN clause</param>
        /// <param name="values">The values to include in the IN clause</param>
        /// <returns>Self</returns>
        TBuilder In<TValue>(Expression<Func<T, object>> predicate, IEnumerable<TValue> values);
    }
}
