using Stalware.SqlMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Implements <see cref="InsertBuilderBase{T}"/>. Provides SQL Server specific insert statements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SQLServerInsertBuilder<T> : InsertBuilderBase<T> where T : new()
    {
        /// <summary>
        /// Overrides <see cref="InsertBuilderBase{T}.AddServerGuidIdStatement"/> and implements <see cref="IInsertBuilder{T}.AddServerGuidIdStatement"/>
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the <see cref="IdColumn"/> attribute is not found</exception>
        public override IInsertBuilder<T> AddServerGuidIdStatement()
        {
            if (string.IsNullOrWhiteSpace(IdColumnName))
            {
                throw new KeyNotFoundException("A property with the IdColumn attribute was not found. It is required to " +
                    "execute this method");
            }

            InsertBuilder.Append($"{IdColumnName}, ");
            ValuesBuilder.Append("NEWID(), ");
            return this;
        }

        /// <summary>
        /// Attach SQL Server statements to the INSERT builder such that the inserted id can be retrieved
        /// </summary>
        /// <typeparam name="TType">The id type</typeparam>
        /// <param name="columnType">Optional type that can be passed if none of the default mapped types 
        /// conform to the <typeparamref name="TType"/> type</param>
        /// <returns>Self</returns>
        /// <exception cref="NotSupportedException">Thrown if the type is unrecognized and cannot be mapped to a 
        /// SQL Server type</exception>
        public IInsertBuilder<T> GetInsertedId<TType>(string columnType = null)
        {
            string sqlColumnType;
            var type = typeof(TType);

            if (string.IsNullOrWhiteSpace(columnType))
            {
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
            }
            else
            {
                sqlColumnType = columnType;
            }

            InsertBuilder.Insert(0, $"CREATE TABLE #temp ({IdColumnName} {columnType}); ");
            BetweenBuilder.Append($"OUTPUT INSERTED.{IdColumnName} INTO #temp ");
            EndBuilder.Append($" SELECT {IdColumnName} FROM #temp;");
            return this;
        }
    }
}
