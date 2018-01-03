using Stalware.SqlMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Interface that contains SELECT related mappings
    /// </summary>
    /// <typeparam name="T">The initial table name type</typeparam>
    public interface ISelectBuilder<T> : IBuilder<ISelectBuilder<T>>, IWhereable<T, ISelectBuilder<T>>, IInable<T, ISelectBuilder<T>> where T : new()
    {
        /// <summary>
        /// Begins the select command by beginning the SELECT clause and by setting the 
        /// FROM clause
        /// </summary>
        /// <param name="predicate">A predicate that returns an object which contains 
        /// the columns that needs to be retrieved</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> Select(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Adds the DISTINCT keyword to the SELECT clause
        /// </summary>
        /// <returns>Self</returns>
        ISelectBuilder<T> Distinct();

        /// <summary>
        /// Adds a JOIN clause to join a table
        /// </summary>
        /// <typeparam name="TJoin">The table type to join</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ON clause. The object created by the type parameter in <see cref="ISelectBuilder{T}"/>
        /// and the current <typeparamref name="TJoin"/> are passed to the predicate as parameters.</param>
        /// <param name="columnPredicate">A predicate that returns an object which contains 
        /// the columns that needs to be retrieved for <typeparamref name="TJoin"/></param>
        /// <returns>Self</returns>
        ISelectBuilder<T> Join<TJoin>(Expression<Func<T, TJoin, bool>> predicate, Expression<Func<TJoin, object>> columnPredicate) where TJoin : new();

        /// <summary>
        /// Adds a JOIN clause to join a table
        /// </summary>
        /// <typeparam name="TJoin1">Additional table type that is required for the conditional</typeparam>
        /// <typeparam name="TJoin2">The table type to join</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ON clause. The object created by the type parameter in <see cref="ISelectBuilder{T}"/>
        /// and <typeparamref name="TJoin2"/> are passed to the predicate as parameters.</param>
        /// <param name="columnPredicate">A predicate that returns an object which contains 
        /// the columns that needs to be retrieved for <typeparamref name="TJoin2"/></param>
        /// <returns>Self</returns>
        ISelectBuilder<T> Join<TJoin1, TJoin2>(Expression<Func<TJoin1, TJoin2, bool>> predicate, Expression<Func<TJoin2, object>> columnPredicate) 
            where TJoin1 : new() 
            where TJoin2 : new();

        /// <summary>
        /// Adds a LEFT JOIN clause to join a table
        /// </summary>
        /// <typeparam name="TJoin">The table type to join</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ON clause. The object created by the type parameter in <see cref="ISelectBuilder{T}"/>
        /// and the current <typeparamref name="TJoin"/> are passed to the predicate as parameters.</param>
        /// <param name="columnPredicate">A predicate that returns an object which contains 
        /// the columns that needs to be retrieved for <typeparamref name="TJoin"/></param>
        /// <returns>Self</returns>
        ISelectBuilder<T> LeftJoin<TJoin>(Expression<Func<T, TJoin, bool>> predicate, Expression<Func<TJoin, object>> columnPredicate) where TJoin : new();

        /// <summary>
        /// Adds a LEFT JOIN clause to join a table
        /// </summary>
        /// <typeparam name="TJoin1">Additional table type that is required for the conditional</typeparam>
        /// <typeparam name="TJoin2">The table type to join</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ON clause. The objects created by <typeparamref name="TJoin1"/> 
        /// and <typeparamref name="TJoin2"/> are passed to the predicate as parameters.</param>
        /// <param name="columnPredicate">A predicate that returns an object which contains 
        /// the columns that needs to be retrieved for <typeparamref name="TJoin2"/></param>
        /// <returns>Self</returns>
        ISelectBuilder<T> LeftJoin<TJoin1, TJoin2>(Expression<Func<TJoin1, TJoin2, bool>> predicate, Expression<Func<TJoin2, object>> columnPredicate)
            where TJoin1 : new() 
            where TJoin2 : new();

        /// <summary>
        /// Adds a RIGHT JOIN clause to join a table
        /// </summary>
        /// <typeparam name="TJoin">The table type to join</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ON clause. The object created by the type parameter in <see cref="ISelectBuilder{T}"/>
        /// and the current <typeparamref name="TJoin"/> are passed to the predicate as parameters.</param>
        /// <param name="columnPredicate">A predicate that returns an object which contains 
        /// the columns that needs to be retrieved for <typeparamref name="TJoin"/></param>
        /// <returns>Self</returns>
        ISelectBuilder<T> RightJoin<TJoin>(Expression<Func<T, TJoin, bool>> predicate, Expression<Func<TJoin, object>> columnPredicate) where TJoin : new();

        /// <summary>
        /// Adds a RIGHT JOIN clause to join a table
        /// </summary>
        /// <typeparam name="TJoin1">Additional table type that is required for the conditional</typeparam>
        /// <typeparam name="TJoin2">The table type to join</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ON clause. The objects created by <typeparamref name="TJoin1"/> 
        /// and <typeparamref name="TJoin2"/> are passed to the predicate as parameters.</param>
        /// <param name="columnPredicate">A predicate that returns an object which contains 
        /// the columns that needs to be retrieved for <typeparamref name="TJoin2"/></param>
        /// <returns>Self</returns>
        ISelectBuilder<T> RightJoin<TJoin1, TJoin2>(Expression<Func<TJoin1, TJoin2, bool>> predicate, Expression<Func<TJoin2, object>> columnPredicate)
            where TJoin1 : new() 
            where TJoin2 : new();

        /// <summary>
        /// Adds a WHERE clause for a specified table
        /// </summary>
        /// <typeparam name="TOther">The table to use to populate the WHERE clause</typeparam>
        /// <param name="predicate">A conditional predicate to populate the WHERE clause. The object created by 
        /// <typeparamref name="TOther"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> Where<TOther>(Expression<Func<TOther, bool>> predicate) where TOther : new();

        /// <summary>
        /// Adds an OR statement to the original WHERE clause
        /// </summary>
        /// <param name="predicate">A conditional predicate to populate the WHERE clause. The object created by the type parameter in <see cref="ISelectBuilder{T}"/>
        /// is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> WhereOr(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds an OR statement for a specified table to the original WHERE clause
        /// </summary>
        /// <typeparam name="TOther">The table to use to populate the WHERE clause</typeparam>
        /// <param name="predicate">A conditional predicate to populate the WHERE clause. The object created by 
        /// <typeparamref name="TOther"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> WhereOr<TOther>(Expression<Func<TOther, bool>> predicate) where TOther : new();

        /// <summary>
        /// Adds to an ORDER BY clause
        /// </summary>
        /// <param name="predicate">A conditional predicate to populate the ORDER BY clause. The object created by the type parameter in <see cref="ISelectBuilder{T}"/>
        /// is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderBy(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Adds to an ORDER BY clause for a specified table
        /// </summary>
        /// <typeparam name="TOther">The table to use to populate the ORDER BY clause</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ORDER BY clause. The object created by 
        /// <typeparamref name="TOther"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderBy<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new();

        /// <summary>
        /// Adds to an ORDER BY clause using multiple columns
        /// </summary>
        /// <param name="predicate">A predicate that returns an object that contains the columns required to populate the ORDER BY clause. 
        /// The object created by the type parameter in <see cref="ISelectBuilder{T}"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderByMultiple(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Adds to an ORDER BY clause for a specified table uisng multiple columns
        /// </summary>
        /// <typeparam name="TOther">The table to use to populate the ORDER BY clause</typeparam>
        /// <param name="predicate">A predicate that returns an object that contains the columns required to populate the ORDER BY clause. 
        /// The object created by <typeparamref name="TOther"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderByMultiple<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new();

        /// <summary>
        /// Adds to an ORDER BY clause with the DESC keyword
        /// </summary>
        /// <param name="predicate">A conditional predicate to populate the ORDER BY clause. The object created by the type parameter in <see cref="ISelectBuilder{T}"/>
        /// is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderByDesc(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Adds to an ORDER BY clause for a specified table with the DESC keyword
        /// </summary>
        /// <typeparam name="TOther">The table to use to populate the ORDER BY clause</typeparam>
        /// <param name="predicate">A conditional predicate to populate the ORDER BY clause. The object created by 
        /// <typeparamref name="TOther"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderByDesc<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new();

        /// <summary>
        /// Adds to an ORDER BY clause using multiple columns with the DESC keyword
        /// </summary>
        /// <param name="predicate">A predicate that returns an object that contains the columns required to populate the ORDER BY clause. 
        /// The object created by the type parameter in <see cref="ISelectBuilder{T}"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderByMultipleDesc(Expression<Func<T, object>> predicate);

        /// <summary>
        /// Adds to an ORDER BY clause for a specified table uisng multiple columns with the DESC keyword
        /// </summary>
        /// <typeparam name="TOther">The table to use to populate the ORDER BY clause</typeparam>
        /// <param name="predicate">A predicate that returns an object that contains the columns required to populate the ORDER BY clause. 
        /// The object created by <typeparamref name="TOther"/> is passed to the predicate</param>
        /// <returns>Self</returns>
        ISelectBuilder<T> OrderByMultipleDesc<TOther>(Expression<Func<TOther, object>> predicate) where TOther : new();
    }
}
