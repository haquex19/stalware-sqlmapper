using Stalware.SqlMapper.ExpressionGeneration;
using Stalware.SqlMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// A class for generating a DELETE statement
    /// </summary>
    /// <typeparam name="T">The table type to delete</typeparam>
    public class DeleteBuilder<T> : ExpressionBuilderBase, IBuilder<DeleteBuilder<T>>, IWhereOnIdPreventable<DeleteBuilder<T>>, IWhereable<T, DeleteBuilder<T>> where T : new()
    {
        private readonly StringBuilder _deleteBuilder;
        private readonly StringBuilder _whereBuilder;        
        private readonly T _record;
        private bool _preventWhereOnIdAutoAdd;

        /// <summary>
        /// Instantiates the <see cref="DeleteBuilder{T}"/> class
        /// </summary>
        /// <param name="record">The record to delete</param>
        /// <param name="generatorType">The expression type that implements <see cref="IExpressionToSqlGenerator"/>. Pass null to use the 
        /// default type</param>
        public DeleteBuilder(T record, Type generatorType = null) : base(generatorType)
        {
            _record = record;
            _deleteBuilder = new StringBuilder();
            _whereBuilder = new StringBuilder();
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Build"/>
        /// </summary>
        public SqlMapperResult Build()
        {
            if (_whereBuilder.Length == 0 && !_preventWhereOnIdAutoAdd)
            {
                var type = typeof(T);
                var properties = type.GetProperties();
                var paramName = $"PARAM{ParamCount++}";
                var idProp = properties.GetIdColumnAttribute();

                _whereBuilder.Append($" WHERE {idProp.Name} = @{paramName}");
                Result.Parameters.Add(paramName, idProp.GetValue(_record));
            }

            _deleteBuilder.Append($"DELETE FROM {typeof(T).Name}").Append(_whereBuilder);
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
            _whereBuilder.Clear();
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
        public DeleteBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            _whereBuilder.Append(_whereBuilder.Length == 0 ? " WHERE " : " AND ");

            var condition = GetConditionAndSetParameters(predicate);
            var parameter = predicate.Parameters[0].Name;
            _whereBuilder.Append(condition.Replace($"{parameter}.", string.Empty));
            return this;
        }
    }
}
