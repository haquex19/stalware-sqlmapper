using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper.Attributes
{
    /// <summary>
    /// An attribute class that specifies the primary key column for a table. Can only be used on properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IdColumn : Attribute { }
}
