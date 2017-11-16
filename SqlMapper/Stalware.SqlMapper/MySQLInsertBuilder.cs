using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper
{
    public class MySQLInsertBuilder<T> : InsertBuilderBase<T> where T : new()
    {
        public override IInsertBuilder<T> AddServerGuidIdStatement()
        {
            if (string.IsNullOrWhiteSpace(IdColumnName))
            {
                throw new KeyNotFoundException("A property with the IdColumn attribute was not found. It is required to " +
                    "execute this method");
            }

            InsertBuilder.Append($"{IdColumnName}, ");
            ValuesBuilder.Append("UUID(), ");
            return this;
        }        
    }
}
