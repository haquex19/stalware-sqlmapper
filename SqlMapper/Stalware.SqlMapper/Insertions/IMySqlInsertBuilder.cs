using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Insertions
{
    /// <summary>
    /// Interface for providing MySQL specific insert statements
    /// </summary>
    /// <typeparam name="T">The table type parameter</typeparam>
    public interface IMySqlInsertBuilder<T> : IInsertBuilder<T> where T : new()
    {
        /// <summary>
        /// Modifies the insert statement so that a new UUID can be retrieved after executing the statement
        /// </summary>
        /// <returns>Self</returns>
        IMySqlInsertBuilder<T> GetInsertedUUID();

        /// <summary>
        /// Adds to the insert statement so that the last auto incremented id can be retrieved
        /// </summary>
        /// <returns>Self</returns>
        IMySqlInsertBuilder<T> GetLastAutoIncrementId();
    }
}
