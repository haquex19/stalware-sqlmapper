using Stalware.SqlMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Insertions
{
    /// <summary>
    /// Implements <see cref="InsertBuilderBase{T}"/>
    /// </summary>
    /// <remarks>Provides SQL Server specific insert statements</remarks>
    public class SQLServerInsertBuilder<T> : InsertBuilderBase<T> where T : new()
    {
        /// <summary>
        /// Instantiates the <see cref="SQLServerInsertBuilder{T}"/> class by setting the record to insert
        /// </summary>
        /// <param name="record">The record object to insert</param>
        public SQLServerInsertBuilder(T record) : base(record) { }

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
            ValuesBuilder.Append("NEWID(), ");
            return this;
        }

        /// <summary>
        /// Attach SQL Server statements to the INSERT builder such that the inserted id can be retrieved
        /// </summary>
        /// <typeparam name="TType">The id type</typeparam>
        /// <returns>Self</returns>
        /// <exception cref="NotSupportedException">Thrown if the type is unrecognized and cannot be mapped to a 
        /// SQL Server type</exception>
        public virtual IInsertBuilder<T> GetInsertedId<TType>()
        {
            string sqlColumnType;
            var type = typeof(TType);

            if (type == typeof(long))
            {
                sqlColumnType = "BIGINT";
            }
            else if (type == typeof(byte[]))
            {
                sqlColumnType = "VARBINARY(MAX)";
            }
            else if (type == typeof(bool))
            {
                sqlColumnType = "BIT";
            }
            else if (type == typeof(string))
            {
                sqlColumnType = "VARCHAR(MAX)";
            }
            else if (type == typeof(DateTime))
            {
                sqlColumnType = "DATETIME";
            }
            else if (type == typeof(DateTimeOffset))
            {
                sqlColumnType = "DATETIMEOFFSET(7)";
            }
            else if (type == typeof(decimal))
            {
                sqlColumnType = "DECIMAL(18, 0)";
            }
            else if (type == typeof(float))
            {
                sqlColumnType = "FLOAT";
            }
            else if (type == typeof(int))
            {
                sqlColumnType = "INT";
            }
            else if (type == typeof(double))
            {
                sqlColumnType = "REAL";
            }
            else if (type == typeof(short))
            {
                sqlColumnType = "SMALLINT";
            }
            else if (type == typeof(TimeSpan))
            {
                sqlColumnType = "TIME(7)";
            }
            else if (type == typeof(byte))
            {
                sqlColumnType = "TINYINT";
            }
            else if (type == typeof(Guid))
            {
                sqlColumnType = "UNIQUEIDENTIFIER";
            }
            else
            {
                throw new NotSupportedException($"The type {type.Name} is currently not supported for conversion with a SQL Server column");
            }

            InsertBuilder.Insert(0, $"CREATE TABLE #temp ({IdColumnName} {sqlColumnType}); ");
            BetweenBuilder.Append($"OUTPUT INSERTED.{IdColumnName} INTO #temp ");
            EndBuilder.Append($"; SELECT {IdColumnName} FROM #temp;");
            return this;
        }

        /// <summary>
        /// Attach SQL Server statements to the INSERT builder such that the inserted id can be retrieved
        /// </summary>
        /// <param name="columnType">An explicity column type to use</param>
        /// <returns>Self</returns>
        public virtual IInsertBuilder<T> GetInsertedId(string columnType = null)
        {
            InsertBuilder.Insert(0, $"CREATE TABLE #temp ({IdColumnName} {columnType}); ");
            BetweenBuilder.Append($"OUTPUT INSERTED.{IdColumnName} INTO #temp ");
            EndBuilder.Append($"; SELECT {IdColumnName} FROM #temp;");
            return this;
        }
    }
}
