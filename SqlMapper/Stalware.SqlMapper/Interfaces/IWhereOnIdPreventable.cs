using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Interfaces
{
    /// <summary>
    /// An interface that prevents the WHERE Id = [id column] from being auto generated
    /// </summary>
    /// <typeparam name="TBuilder">The builder type parameter</typeparam>
    public interface IWhereOnIdPreventable<TBuilder>
    {
        /// <summary>
        /// Prevent the clause WHERE Id = [id column] from being auto inserted if a where clause has not already been provided
        /// </summary>
        /// <returns>The builder</returns>
        TBuilder PreventWhereOnIdAutoAdd();
    }
}
