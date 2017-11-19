using Stalware.SqlMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Insertions
{
    /// <summary>
    /// Implements <see cref="InsertBuilderBase{T}"/>.
    /// </summary>
    /// <remarks>Provides MySQL specific insert statements</remarks>
    public class MySQLInsertBuilder<T> : InsertBuilderBase<T> where T : new()
    {
        private bool _serverGuidIdStatementAdded;

        /// <summary>
        /// Instantiates the <see cref="MySQLInsertBuilder{T}"/> class by setting the record to insert
        /// </summary>
        /// <param name="record">The record object to insert</param>
        public MySQLInsertBuilder(T record) : base(record) { }

        /// <summary>
        /// Overrides <see cref="InsertBuilderBase{T}.AddServerGuidIdStatement"/> and implements <see cref="IInsertBuilder{T}.AddServerGuidIdStatement"/>
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the <see cref="IdColumn"/> attribute is not found</exception>
        /// <exception cref="InvalidOperationException">Thrown if the id column has already been added</exception>
        public override IInsertBuilder<T> AddServerGuidIdStatement()
        {
            if (string.IsNullOrWhiteSpace(IdColumnName))
            {
                throw new KeyNotFoundException("A property with the IdColumn attribute was not found. It is required to " +
                    "execute this method");
            }

            if (IdColumnAdded)
            {
                throw new InvalidOperationException("The id column has already been added");
            }

            InsertBuilder.Append($"{IdColumnName}, ");
            ValuesBuilder.Append("UUID(), ");
            _serverGuidIdStatementAdded = true;
            return this;
        }

        /// <summary>
        /// Modifies the insert statement so that a new UUID can be retrieved after executing the statement
        /// </summary>
        /// <returns>Self</returns>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="AddServerGuidIdStatement"/> method was not called</exception>
        public virtual IInsertBuilder<T> GetInsertedUUID()
        {
            if (!_serverGuidIdStatementAdded)
            {
                throw new InvalidOperationException("The server guid id statement has not beed added. This is required");
            }

            InsertBuilder.Insert(0, "SET @temp = SELECT UUID(); ");
            ValuesBuilder.Replace("UUID(), ", "@temp, ");
            EndBuilder.Append("; SELECT @temp;");
            return this;
        }

        /// <summary>
        /// Adds to the insert statement so that the last auto incremented id can be retrieved
        /// </summary>
        /// <returns>Self</returns>
        public virtual IInsertBuilder<T> GetLastAutoIncrementId()
        {
            EndBuilder.Append("; SELECT LAST_INSERT_ID();");
            return this;
        }
    }
}
