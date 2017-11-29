using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stalware.SqlMapper.Interfaces
{
    /// <summary>
    /// Common functionality for all builders
    /// </summary>
    /// <typeparam name="TBuilder">The builder table type</typeparam>
    public interface IBuilder<TBuilder>
    {
        /// <summary>
        /// Clears all states and the query generation of the builder and starts wiht a new state
        /// </summary>
        /// <returns>The current builder</returns>
        TBuilder Clear();

        /// <summary>
        /// Builds the SQL statement
        /// </summary>
        /// <returns>A <see cref="SqlMapperResult"/> that contains the query and parameters</returns>
        SqlMapperResult Build();
    }
}
