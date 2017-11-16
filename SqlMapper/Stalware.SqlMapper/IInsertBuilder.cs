﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Interface that contains methods for creating an INSERT statement
    /// </summary>
    /// <typeparam name="T">The default table type for the insert statement</typeparam>
    public interface IInsertBuilder<T> where T : new()
    {
        /// <summary>
        /// Include the server call for a new Guid on the id column
        /// </summary>
        /// <returns>Self</returns>
        IInsertBuilder<T> AddServerGuidIdStatement();

        /// <summary>
        /// Inserts all properties of an object to the table
        /// </summary>
        /// <param name="record">The record to insert</param>
        /// <param name="includeId">Should the id column be explicitly inserted</param>
        /// <returns>Self</returns>
        IInsertBuilder<T> InsertAll(T record, bool includeId = false);

        /// <summary>
        /// Inserts all properties of an object except the specified properties
        /// </summary>
        /// <param name="record">The record to insert</param>
        /// <param name="exceptColumns">The properties to exclude in the insert</param>
        /// <param name="includeId">Should the id column be explicitly inserted</param>
        /// <returns>Self</returns>
        IInsertBuilder<T> InsertAllExcept(T record, Func<T, object> exceptColumns, bool includeId = false);

        /// <summary>
        /// Inserts only the indicated properties in the insert
        /// </summary>
        /// <param name="record">The record to insert</param>
        /// <param name="onlyColumns">The properties to include in the insert</param>
        /// <param name="includeId">Should the id column be explicitly inserted</param>
        /// <returns>Self</returns>
        IInsertBuilder<T> InsertOnly(T record, Func<T, object> onlyColumns, bool includeId = false);

        /// <summary>
        /// Buils the insert statement
        /// </summary>
        /// <returns>A <see cref="SqlMapperResult"/> that contains the query and parameters</returns>
        SqlMapperResult Build();
    }
}
