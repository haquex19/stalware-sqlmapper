using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Insertions
{
    /// <summary>
    /// Interface for providing SQL Server specific insert statements
    /// </summary>
    /// <typeparam name="T">The table type parameter</typeparam>
    public interface ISqlServerInsertBuilder<T> : IInsertBuilder<T> where T : new()
    {
        /// <summary>
        /// Attach SQL Server statements to the INSERT builder such that the inserted id can be retrieved
        /// </summary>
        /// <typeparam name="TType">The id type</typeparam>
        /// <returns>Self</returns>
        ISqlServerInsertBuilder<T> GetInsertedId<TType>();

        /// <summary>
        /// Attach SQL Server statements to the INSERT builder such that the inserted id can be retrieved
        /// </summary>
        /// <param name="columnType">An explicity column type to use</param>
        /// <returns>Self</returns>
        ISqlServerInsertBuilder<T> GetInsertedId(string columnType);
    }
}
