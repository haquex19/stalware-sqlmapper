using Stalware.SqlMapper.ExpressionGeneration;
using Stalware.SqlMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Implements <see cref="IUpdateBuilder{T}"/> and inherits <see cref="ExpressionBuilderBase"/>
    /// </summary>
    public class UpdateBuilder<T> : ExpressionBuilderBase, IUpdateBuilder<T> where T : new()
    {
        private readonly StringBuilder _updateBuilder;
        private readonly StringBuilder _setBuilder;
        private readonly StringBuilder _whereBuilder;
        private readonly T _record;
        private readonly string _idColumn;
        private bool _preventWhereOnIdAutoAdd = false;

        /// <summary>
        /// Instantiates the <see cref="UpdateBuilder{T}"/> class.
        /// </summary>
        /// <param name="record">The record to update</param>
        /// <param name="generatorType">The expression type that implements <see cref="IExpressionToSqlGenerator"/>. Pass null to use the 
        /// default type</param>
        public UpdateBuilder(T record, Type generatorType = null) : base(generatorType)
        {
            _updateBuilder = new StringBuilder();
            _setBuilder = new StringBuilder();
            _whereBuilder = new StringBuilder();
            _record = record;

            _idColumn = typeof(T).GetProperties().GetIdColumnAttributeName();
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Build"/>
        /// </summary>
        /// <returns></returns>
        public SqlMapperResult Build()
        {
            if (_whereBuilder.Length == 0 && !_preventWhereOnIdAutoAdd)
            {
                var paramName = $"PARAM{ParamCount++}";
                _whereBuilder.Append($" WHERE {_idColumn} = @{paramName}");

                var idValue = _record.GetIdColumnAttributeValue();
                Result.Parameters.Add(paramName, idValue);
            }

            _setBuilder.Insert(0, "SET ");
            _setBuilder.Remove(_setBuilder.Length - 2, 2);
            _updateBuilder.Append($"UPDATE {typeof(T).Name} {_setBuilder}{_whereBuilder}");
            Result.Query = _updateBuilder.ToString();
            return Result;
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Clear"/>
        /// </summary>
        /// <returns></returns>
        public IUpdateBuilder<T> Clear()
        {
            _updateBuilder.Clear();
            _setBuilder.Clear();
            _whereBuilder.Clear();
            _preventWhereOnIdAutoAdd = false;
            ResetProps();
            return this;
        }

        /// <summary>
        /// Implements <see cref="IInable{T, TBuilder}.In(Expression{Func{T, object}}, object[])"/>
        /// </summary>
        public IUpdateBuilder<T> In(Expression<Func<T, object>> predicate, params object[] values)
        {
            var tuple = DoIn(predicate, values);
            var alias = predicate.Parameters[0].Name;
            var column = tuple.columnName.Replace($"{alias}.", string.Empty);
            _whereBuilder.Append(_whereBuilder.Length == 0 ? $" WHERE {column} IN ({tuple.inClause})" : $" AND {column} IN ({tuple.inClause})");
            return this;
        }

        /// <summary>
        /// Implements <see cref="IWhereOnIdPreventable{TBuilder}.PreventWhereOnIdAutoAdd"/>
        /// </summary>
        /// <returns></returns>
        public IUpdateBuilder<T> PreventWhereOnIdAutoAdd()
        {
            _preventWhereOnIdAutoAdd = true;
            return this;
        }

        /// <summary>
        /// Implements <see cref="IUpdateBuilder{T}.UpdateAll(bool)"/>
        /// </summary>
        public IUpdateBuilder<T> UpdateAll(bool skipIdColumn = true)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                if (skipIdColumn && prop.Name == _idColumn)
                {
                    continue;
                }

                AddPropToUpdateAndValue(prop);
            }
            return this;
        }

        /// <summary>
        /// Implements <see cref="IUpdateBuilder{T}.UpdateAllExcept(Func{T, object}, bool)"/>
        /// </summary>
        public IUpdateBuilder<T> UpdateAllExcept(Func<T, object> exceptColumns, bool skipIdColumn = true)
        {
            var properties = typeof(T).GetProperties();
            var obj = exceptColumns(new T());
            var exceptProps = obj.GetColumnProperties();
            
            foreach (var prop in properties)
            {
                if ((skipIdColumn && prop.Name == _idColumn) || (exceptProps.Any(x => x.Name == prop.Name)))
                {
                    continue;
                }

                AddPropToUpdateAndValue(prop);
            }
            return this;
        }

        /// <summary>
        /// Implements <see cref="IUpdateBuilder{T}.UpdateOnly(Func{T, object})"/>
        /// </summary>
        public IUpdateBuilder<T> UpdateOnly(Func<T, object> onlyColumns)
        {
            var properties = typeof(T).GetProperties();
            var obj = onlyColumns(new T());
            var onlyProps = obj.GetColumnProperties();

            foreach (var prop in properties)
            {
                if (onlyProps.Any(x => x.Name == prop.Name))
                {
                    AddPropToUpdateAndValue(prop);
                }
            }
            return this;
        }

        /// <summary>
        /// Implements <see cref="IWhereable{T, TBuilder}.Where(Expression{Func{T, bool}})"/>
        /// </summary>
        public IUpdateBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            _whereBuilder.Append(_whereBuilder.Length == 0 ? " WHERE " : " AND ");

            var condition = GetConditionAndSetParameters(predicate);
            var parameter = predicate.Parameters[0].Name;
            _whereBuilder.Append(condition.Replace($"{parameter}.", string.Empty));
            return this;
        }

        private void AddPropToUpdateAndValue(PropertyInfo property)
        {
            var value = property.GetValue(_record);
            _setBuilder.Append($"{property.Name} = @{property.Name}, ");
            Result.Parameters.Add(property.Name, value);
        }
    }
}
