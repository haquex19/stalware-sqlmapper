using Stalware.SqlMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Interface that contains UPDATE related mappings
    /// </summary>
    /// <typeparam name="T">The table name type</typeparam>
    public interface IUpdateBuilder<T> : IBuilder<IUpdateBuilder<T>>, IWhereOnIdPreventable<IUpdateBuilder<T>>, IWhereable<T, IUpdateBuilder<T>> where T : new()
    {
        /// <summary>
        /// Updates all properties of an object to the table
        /// </summary>
        /// <param name="skipIdColumn">Indicate if the id column should be skipped in the update statement. Default is true</param>
        /// <returns>Self</returns>
        IUpdateBuilder<T> UpdateAll(bool skipIdColumn = true);

        /// <summary>
        /// Updates all properties of an object except the specified properties
        /// </summary>
        /// <param name="exceptColumns">The properties to exclude in the update</param>
        /// <param name="skipIdColumn">Indicate if the id column should be skipped in the update statement. Default is true</param>
        /// <returns>Self</returns>
        IUpdateBuilder<T> UpdateAllExcept(Func<T, object> exceptColumns, bool skipIdColumn = true);

        /// <summary>
        /// Updates only the indicated properties in the insert
        /// </summary>
        /// <param name="onlyColumns">The properties to include in the insert</param>
        /// <returns>Self</returns>
        IUpdateBuilder<T> UpdateOnly(Func<T, object> onlyColumns);        
    }
}
