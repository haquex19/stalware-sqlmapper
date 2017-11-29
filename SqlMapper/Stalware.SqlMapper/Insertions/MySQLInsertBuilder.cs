using Stalware.SqlMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Insertions
{
    /// <summary>
    /// Implements <see cref="InsertBuilderBase{T}"/> and <see cref="IMySqlInsertBuilder{T}"/>
    /// </summary>
    public class MySqlInsertBuilder<T> : InsertBuilderBase<T>, IMySqlInsertBuilder<T> where T : new()
    {
        private bool _serverGuidIdStatementAdded;

        /// <summary>
        /// Instantiates the <see cref="MySqlInsertBuilder{T}"/> class by setting the record to insert
        /// </summary>
        /// <param name="record">The record object to insert</param>
        public MySqlInsertBuilder(T record) : base(record) { }

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
            IdColumnAdded = true;
            _serverGuidIdStatementAdded = true;
            return this;
        }

        /// <summary>
        /// Overrides <see cref="InsertBuilderBase{T}.Clear"/>
        /// </summary>
        public override IInsertBuilder<T> Clear()
        {
            _serverGuidIdStatementAdded = false;
            return base.Clear();
        }

        /// <summary>
        /// Implements <see cref="IMySqlInsertBuilder{T}.GetInsertedUUID"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="AddServerGuidIdStatement"/> method was not called</exception>
        public IMySqlInsertBuilder<T> GetInsertedUUID()
        {
            if (!_serverGuidIdStatementAdded)
            {
                throw new InvalidOperationException("The server guid id statement has not beed added. This is required");
            }

            BeforeBuilder.Append("SET @temp = SELECT UUID(); ");
            ValuesBuilder.Replace("UUID(), ", "@temp, ");
            EndBuilder.Append("; SELECT @temp;");
            return this;
        }

        /// <summary>
        /// Implements <see cref="IMySqlInsertBuilder{T}.GetLastAutoIncrementId"/>
        /// </summary>
        public IMySqlInsertBuilder<T> GetLastAutoIncrementId()
        {
            EndBuilder.Append("; SELECT LAST_INSERT_ID();");
            return this;
        }
    }
}
