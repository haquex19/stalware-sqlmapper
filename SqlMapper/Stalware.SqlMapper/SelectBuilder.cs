﻿using Stalware.SqlMapper.ExpressionGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Stalware.SqlMapper.Enums;
using Stalware.SqlMapper.Interfaces;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Implements <see cref="ISelectBuilder{T}"/>
    /// </summary>
    public class SelectBuilder<T> : ExpressionBuilderBase, ISelectBuilder<T> where T : new()
    {
        private readonly List<string> _selectList;
        private readonly StringBuilder _selectBuilder;
        private readonly StringBuilder _fromBuilder;
        private readonly StringBuilder _whereBuilder;
        private readonly StringBuilder _orderByBuilder;
        private bool _firstSelectCalled;

        /// <summary>
        /// Instantiates the <see cref="SelectBuilder{T}"/> class
        /// </summary>
        /// <param name="generatorType">The expression type that implements <see cref="IExpressionToSqlGenerator"/>. Pass null to use the 
        /// default type</param>
        public SelectBuilder(Type generatorType = null) : base(generatorType)
        {
            _selectList = new List<string>();
            _selectBuilder = new StringBuilder();
            _fromBuilder = new StringBuilder();
            _whereBuilder = new StringBuilder();
            _orderByBuilder = new StringBuilder();
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.Distinct"/>
        /// </summary>
        public ISelectBuilder<T> Distinct()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.Join{TJoin}(Expression{Func{T, TJoin, bool}}, Expression{Func{TJoin, object}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if predicate alias and column alias do not match</exception>
        public ISelectBuilder<T> Join<TJoin>(Expression<Func<T, TJoin, bool>> predicate, Expression<Func<TJoin, object>> columnPredicate) where TJoin : new()
        {
            return DoJoin(predicate, columnPredicate, JoinTypes.Inner);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.LeftJoin{TJoin}(Expression{Func{T, TJoin, bool}}, Expression{Func{TJoin, object}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if predicate alias and column alias do not match</exception>
        public ISelectBuilder<T> LeftJoin<TJoin>(Expression<Func<T, TJoin, bool>> predicate, Expression<Func<TJoin, object>> columnPredicate) where TJoin : new()
        {
            return DoJoin(predicate, columnPredicate, JoinTypes.Left);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.RightJoin{TJoin}(Expression{Func{T, TJoin, bool}}, Expression{Func{TJoin, object}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if predicate alias and column alias do not match</exception>
        public ISelectBuilder<T> RightJoin<TJoin>(Expression<Func<T, TJoin, bool>> predicate, Expression<Func<TJoin, object>> columnPredicate) where TJoin : new()
        {
            return DoJoin(predicate, columnPredicate, JoinTypes.Right);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.Join{TJoin1, TJoin2}(Expression{Func{TJoin1, TJoin2, bool}}, Expression{Func{TJoin2, object}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if predicate alias and column alias do not match</exception>
        public ISelectBuilder<T> Join<TJoin1, TJoin2>(Expression<Func<TJoin1, TJoin2, bool>> predicate, Expression<Func<TJoin2, object>> columnPredicate)
            where TJoin1 : new()
            where TJoin2 : new()
        {
            return DoJoin(predicate, columnPredicate, JoinTypes.Inner);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.LeftJoin{TJoin1, TJoin2}(Expression{Func{TJoin1, TJoin2, bool}}, Expression{Func{TJoin2, object}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if predicate alias and column alias do not match</exception>
        public ISelectBuilder<T> LeftJoin<TJoin1, TJoin2>(Expression<Func<TJoin1, TJoin2, bool>> predicate, Expression<Func<TJoin2, object>> columnPredicate)
            where TJoin1 : new()
            where TJoin2 : new()
        {
            return DoJoin(predicate, columnPredicate, JoinTypes.Left);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.RightJoin{TJoin1, TJoin2}(Expression{Func{TJoin1, TJoin2, bool}}, Expression{Func{TJoin2, object}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if predicate alias and column alias do not match</exception>
        public ISelectBuilder<T> RightJoin<TJoin1, TJoin2>(Expression<Func<TJoin1, TJoin2, bool>> predicate, Expression<Func<TJoin2, object>> columnPredicate)
            where TJoin1 : new()
            where TJoin2 : new()
        {
            return DoJoin(predicate, columnPredicate, JoinTypes.Right);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderBy(Expression{Func{T, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderBy(Expression<Func<T, object>> predicate)
        {
            return DoOrderBy(predicate, false);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderBy{TOther}(Expression{Func{TOther, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderBy<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new()
        {
            return DoOrderBy(predicate, false);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderByMultiple(Expression{Func{T, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderByMultiple(Expression<Func<T, object>> predicate)
        {
            return DoOrderByMultiple(predicate, false);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderByMultiple{TOther}(Expression{Func{TOther, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderByMultiple<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new()
        {
            return DoOrderByMultiple(predicate, false);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderByDesc(Expression{Func{T, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderByDesc(Expression<Func<T, object>> predicate)
        {
            return DoOrderBy(predicate, true);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderByDesc{TOther}(Expression{Func{TOther, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderByDesc<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new()
        {
            return DoOrderBy(predicate, true);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderByMultipleDesc(Expression{Func{T, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderByMultipleDesc(Expression<Func<T, object>> predicate)
        {
            return DoOrderByMultiple(predicate, true);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.OrderByMultipleDesc{TOther}(Expression{Func{TOther, object}})"/>
        /// </summary>
        public ISelectBuilder<T> OrderByMultipleDesc<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new()
        {
            return DoOrderByMultiple(predicate, true);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.Select(Expression{Func{T, object}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if this method is called more than once</exception>
        public ISelectBuilder<T> Select(Expression<Func<T, object>> predicate)
        {
            if (_firstSelectCalled)
            {
                throw new InvalidOperationException("This method can only be called once");
            }            

            var type = typeof(T);
            var tableName = type.Name;
            var alias = predicate.Parameters[0].Name;
            AddToSelectList(predicate);

            _selectBuilder.Append($"SELECT ");
            _fromBuilder.Append($"FROM {tableName} AS {alias}");
            _firstSelectCalled = true;
            return this;
        }

        /// <summary>
        /// Implements <see cref="IWhereable{T, TBuilder}.Where(Expression{Func{T, bool}})"/>
        /// </summary>
        public ISelectBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            return DoWhere(predicate);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.Where{TOther}(Expression{Func{TOther, bool}})"/>
        /// </summary>
        public ISelectBuilder<T> Where<TOther>(Expression<Func<TOther, bool>> predicate) where TOther : new()
        {
            return DoWhere(predicate);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.WhereOr(Expression{Func{T, bool}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the WHERE clause has not yet been populated</exception>
        public ISelectBuilder<T> WhereOr(Expression<Func<T, bool>> predicate)
        {
            return DoWhereOr(predicate);
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.WhereOr{TOther}(Expression{Func{TOther, bool}})"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the WHERE clause has not yet been populated</exception>
        public ISelectBuilder<T> WhereOr<TOther>(Expression<Func<TOther, bool>> predicate) where TOther : new()
        {
            return DoWhereOr(predicate);
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Build"/>
        /// </summary>
        public SqlMapperResult Build()
        {
            _selectBuilder
                .Append($"{string.Join(", ", _selectList)} ")
                .Append(_fromBuilder);

            if (_whereBuilder.Length > 0)
            {
                _selectBuilder.Append($" {_whereBuilder}");
            }

            if (_orderByBuilder.Length > 0)
            {
                _selectBuilder.Append($" {_orderByBuilder}");
            }   

            Result.Query = _selectBuilder.ToString();
            return Result;
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Clear"/>
        /// </summary>
        public ISelectBuilder<T> Clear()
        {
            _selectList.Clear();
            _selectBuilder.Clear();
            _fromBuilder.Clear();
            _whereBuilder.Clear();
            _orderByBuilder.Clear();
            _firstSelectCalled = false;
            ResetProps();
            return this;
        }

        /// <summary>
        /// Implements <see cref="ISelectBuilder{T}.In(Expression{Func{T, object}}, object[])"/>
        /// </summary>
        public ISelectBuilder<T> In(Expression<Func<T, object>> predicate, params object[] values)
        {
            var method = predicate.Compile();
            var obj = method(new T());

            var alias = predicate.Parameters[0].Name;
            var props = obj?.GetType().GetProperties();
            string columnName;
            if (props != null && props.Any())
            {
                if (props.Count() > 1)
                {
                    throw new ArgumentException("The predicate can only contain one property");
                }

                columnName = $"{alias}.{props.ElementAt(0).Name}";
            }
            else
            {
                if (!(predicate.Body is MemberExpression memberExpression))
                {
                    throw new ArgumentException("The predicate can only be a single member expression");
                }

                columnName = $"{alias}.{memberExpression.Member.Name}";
            }            

            var inClause = new StringBuilder();
            foreach (var value in values)
            {
                var propName = $"PARAM{ParamCount++}";
                inClause.Append($"@{propName}, ");
                Result.Parameters.Add(propName, value);
            }
            inClause.Remove(inClause.Length - 2, 2);

            _whereBuilder.Append(_whereBuilder.Length == 0 ? $"WHERE {columnName} IN ({inClause})" : $" AND {columnName} IN ({inClause})");
            return this;
        }

        private ISelectBuilder<T> DoOrderBy<TType>(Expression<Func<TType, object>> predicate, bool desc)
        {
            var memberExpression = predicate.Body is UnaryExpression unaryExpression ? 
                (MemberExpression)unaryExpression.Operand : 
                (MemberExpression)predicate.Body;            
            var alias = predicate.Parameters[0].Name;
            var descStr = desc ? " DESC" : string.Empty;
            var tableColumn = memberExpression.Member.Name;

            _orderByBuilder.Append(_orderByBuilder.Length == 0 ? $"ORDER BY {alias}.{tableColumn}{descStr}" : $", {alias}.{tableColumn}{descStr}");
            return this;
        }

        private ISelectBuilder<T> DoOrderByMultiple<TType>(Expression<Func<TType, object>> predicate, bool desc) where TType : new()
        {
            var method = predicate.Compile();
            var conditionObj = method(new TType());
            var properties = conditionObj.GetColumnProperties();

            _orderByBuilder.Append(_orderByBuilder.Length == 0 ? "ORDER BY " : ", ");
            var alias = predicate.Parameters[0].Name;
            var descStr = desc ? " DESC" : string.Empty;

            foreach (var prop in properties)
            {
                _orderByBuilder.Append($"{alias}.{prop.Name}{descStr}, ");
            }
            _orderByBuilder.Remove(_orderByBuilder.Length - 2, 2);

            return this;
        }                

        private ISelectBuilder<T> DoJoin<TJoin1, TJoin2>(Expression<Func<TJoin1, TJoin2, bool>> predicate, 
            Expression<Func<TJoin2, object>> columnPredicate, JoinTypes joinType)
            where TJoin1 : new()
            where TJoin2 : new()
        {
            var joiningTableName = typeof(TJoin2).Name;
            var joiningAlias = predicate.Parameters[1].Name;
            var columnAlias = columnPredicate.Parameters[0].Name;

            string joinTypeStr;
            switch (joinType)
            {
                case JoinTypes.Left:
                    joinTypeStr = "LEFT ";
                    break;
                case JoinTypes.Right:
                    joinTypeStr = "RIGHT ";
                    break;
                default:
                    joinTypeStr = string.Empty;
                    break;
            }

            if (columnAlias != joiningAlias)
            {
                throw new InvalidOperationException($"The alias on the join type is '{joiningAlias}' and the alias on the column predicate is '{columnAlias}'. " +
                    $"These alias's cannot differ and must match");
            }

            var condition = GetConditionAndSetParameters(predicate);
            _fromBuilder
                .Append($" {joinTypeStr}JOIN {joiningTableName} AS {joiningAlias} ON {condition}");

            AddToSelectList(columnPredicate);
            return this;
        }

        private ISelectBuilder<T> DoWhere<TType>(Expression<Func<TType, bool>> predicate)
        {
            var condition = GetConditionAndSetParameters(predicate);
            _whereBuilder.Append(_whereBuilder.Length == 0 ? $"WHERE {condition}" : $" AND {condition}");
            return this;
        }

        private ISelectBuilder<T> DoWhereOr<TType>(Expression<Func<TType, bool>> predicate)
        {
            if (_whereBuilder.Length == 0)
            {
                throw new InvalidOperationException("The method 'Where' needs to be called before this method can be called");
            }

            var condition = GetConditionAndSetParameters(predicate);
            _whereBuilder.Append($" OR {condition}");
            return this;
        }

        private void AddToSelectList<TColumn>(Expression<Func<TColumn, object>> predicate) where TColumn : new()
        {
            var method = predicate.Compile();
            var alias = predicate.Parameters[0].Name;
            var columnObj = method(new TColumn());
            var properties = columnObj.GetColumnProperties();

            if (properties.Any())
            {
                foreach (var prop in properties)
                {
                    _selectList.Add($"{alias}.{prop.Name}");
                }
            }
            else
            {
                _selectList.Add($"{alias}.*");
            }
        }
    }
}
