using Stalware.SqlMapper.ExpressionGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Stalware.SqlMapper.Enums;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Implements <see cref="ISelectBuilder{T}"/>
    /// </summary>
    public class SelectBuilder<T> : ISelectBuilder<T> where T : new()
    {
        private readonly List<string> _selectList;
        private readonly StringBuilder _selectBuilder;
        private readonly StringBuilder _fromBuilder;
        private readonly StringBuilder _whereBuilder;
        private readonly StringBuilder _orderByBuilder;
        private bool _firstSelectCalled;
        private readonly SqlMapperResult _mapperResult;
        private readonly Type _generatorType;
        private int _paramCount = 0;

        /// <summary>
        /// Instantiates the <see cref="SelectBuilder{T}"/> class
        /// </summary>
        public SelectBuilder()
        {
            _selectList = new List<string>();
            _selectBuilder = new StringBuilder();
            _fromBuilder = new StringBuilder();
            _whereBuilder = new StringBuilder();
            _orderByBuilder = new StringBuilder();
            _mapperResult = new SqlMapperResult();
            _generatorType = typeof(ExpressionToSqlGenerator);
        }

        /// <summary>
        /// Instantiates the <see cref="SelectBuilder{T}"/> class with a type that represents a 
        /// custom implementation of <see cref="IExpressionToSqlGenerator"/>
        /// </summary>
        /// <param name="expressionToSqlGeneratorImplementationType">The type that implements the 
        /// <see cref="IExpressionToSqlGenerator"/> interface</param>
        public SelectBuilder(Type expressionToSqlGeneratorImplementationType) : this()
        {
            _generatorType = expressionToSqlGeneratorImplementationType;
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
        /// Implements <see cref="ISelectBuilder{T}.Where(Expression{Func{T, bool}})"/>
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
        /// Implements <see cref="ISelectBuilder{T}.Build"/>
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

            _mapperResult.Query = _selectBuilder.ToString();
            return _mapperResult;
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
            var properties = conditionObj.GetType().GetProperties();

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
            var properties = columnObj.GetType().GetProperties();

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

        private string GetConditionAndSetParameters(LambdaExpression lambda)
        {
            var generator = (IExpressionToSqlGenerator)Activator.CreateInstance(_generatorType);
            generator.SetLambdaAndParameterCount(lambda, _paramCount);

            var condition = generator.GetBuiltCondition(out var parameters, out _paramCount);
            _mapperResult.Parameters.AddRange(parameters);
            return condition;
        }
    }
}
