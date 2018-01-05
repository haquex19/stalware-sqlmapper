using Stalware.SqlMapper.ExpressionGeneration;
using Stalware.SqlMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// A class for generating a DELETE statement
    /// </summary>
    /// <typeparam name="T">The table type to delete</typeparam>
    public class DeleteBuilder<T> : BuilderBase, IBuilder<DeleteBuilder<T>>, IWhereOnIdPreventable<DeleteBuilder<T>>, IWhereable<T, DeleteBuilder<T>> where T : new()
    {
        private readonly StringBuilder _deleteBuilder;
        private readonly IEnumerable<T> _records;
        private bool _preventWhereOnIdAutoAdd;

        /// <summary>
        /// Instantiates the <see cref="DeleteBuilder{T}"/> class
        /// </summary>
        /// <param name="record">The record to delete</param>
        /// <param name="generatorType">The expression type that implements <see cref="IExpressionToSqlGenerator"/>. Pass null to use the 
        /// default type</param>
        public DeleteBuilder(T record, Type generatorType = null) : this(generatorType)
        {
            _records = new List<T> { record };
        }

        /// <summary>
        /// Instantiates the <see cref="DeleteBuilder{T}"/> class
        /// </summary>
        /// <param name="records">The list of records to delete</param>
        /// <param name="generatorType">The expression type that implements <see cref="IExpressionToSqlGenerator"/>. Pass null to use the 
        /// default type</param>
        public DeleteBuilder(IEnumerable<T> records, Type generatorType = null) : this(generatorType)
        {
            _records = records;
        }

        private DeleteBuilder(Type generatorType) : base(generatorType)
        {
            _deleteBuilder = new StringBuilder();
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Build"/>
        /// </summary>
        public SqlMapperResult Build()
        {
            if (WhereBuilder.Length == 0 && !_preventWhereOnIdAutoAdd)
            {
                var type = typeof(T);
                var properties = type.GetProperties();
                var idProp = properties.GetIdColumnAttribute();

                if (_records.Count() == 1)
                {
                    var paramName = $"PARAM{ParamCount++}";
                    WhereBuilder.Append($" WHERE {idProp.Name} = @{paramName}");
                    Result.Parameters.Add(paramName, idProp.GetValue(_records.ElementAt(0)));
                }
                else
                {
                    WhereBuilder.Append($" WHERE {idProp.Name} IN (");
                    foreach (var record in _records)
                    {
                        var paramName = $"PARAM{ParamCount++}";
                        WhereBuilder.Append($"@{paramName}, ");
                        Result.Parameters.Add(paramName, idProp.GetValue(record));
                    }
                    WhereBuilder.Remove(WhereBuilder.Length - 2, 2);
                    WhereBuilder.Append(")");
                }
            }

            _deleteBuilder.Append($"DELETE FROM {typeof(T).Name}").Append(WhereBuilder);
            Result.Query = _deleteBuilder.ToString();
            return Result;
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Clear"/>
        /// </summary>
        /// <returns></returns>
        public DeleteBuilder<T> Clear()
        {
            _deleteBuilder.Clear();
            WhereBuilder.Clear();
            ResetProps();
            return this;
        }

        /// <summary>
        /// Implements <see cref="IWhereOnIdPreventable{TBuilder}.PreventWhereOnIdAutoAdd"/>
        /// </summary>
        public DeleteBuilder<T> PreventWhereOnIdAutoAdd()
        {
            _preventWhereOnIdAutoAdd = true;
            return this;
        }

        /// <summary>
        /// Implements <see cref="IWhereable{T, TBuilder}.Where(Expression{Func{T, bool}})"/>
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the delete builder was instantiated with a list of records to delete and 
        /// that list contains more than one record</exception>
        public DeleteBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (_records.Count() > 1)
            {
                throw new NotSupportedException("The where clause cannot be changed when a list of records that contain more than one record are used");
            }

            WhereBuilder.Append(WhereBuilder.Length == 0 ? " WHERE " : " AND ");

            var condition = GetConditionAndSetParameters(predicate);
            var parameter = predicate.Parameters[0].Name;
            WhereBuilder.Append(condition.Replace($"{parameter}.", string.Empty));
            return this;
        }

        /// <summary>
        /// Implements <see cref="IWhereable{T, TBuilder}.WhereCustomSql(string, Dictionary{string, object})"/>
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="sql"/> argument does not start with 'WHERE '</exception>
        public DeleteBuilder<T> WhereCustomSql(string sql, Dictionary<string, object> parameters)
        {
            DoWhereCustomSql(sql, parameters);
            return this;
        }
    }
}
