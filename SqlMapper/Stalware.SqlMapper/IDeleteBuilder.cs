using Stalware.SqlMapper.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Interface that contains DELETE related mappings
    /// </summary>
    /// <typeparam name="T">The table name type</typeparam>
    public interface IDeleteBuilder<T> : IBuilder<IDeleteBuilder<T>>, IWhereOnIdPreventable<IDeleteBuilder<T>>, IWhereable<T, IDeleteBuilder<T>> where T : new() { }
}
