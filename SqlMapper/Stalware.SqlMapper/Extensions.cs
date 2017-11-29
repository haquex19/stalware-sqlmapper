using Stalware.SqlMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Extension methods that are used by the library
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets the property that contains the <see cref="IdColumn"/> attribute
        /// </summary>
        /// <param name="self">The list of properties</param>
        /// <returns>The property with the <see cref="IdColumn"/> attribute</returns>
        internal static PropertyInfo GetIdColumnAttribute(this PropertyInfo[] self)
        {
            var idProperty = self.FirstOrDefault(x => Attribute.IsDefined(x, typeof(IdColumn)));
            return idProperty;
        }

        /// <summary>
        /// Gets the property name of the property that contains the <see cref="IdColumn"/> attribute
        /// </summary>
        /// <param name="self">The list of properties</param>
        /// <returns>The name of the property with the <see cref="IdColumn"/> attribute</returns>
        internal static string GetIdColumnAttributeName(this PropertyInfo[] self)
        {
            var idProperty = self.FirstOrDefault(x => Attribute.IsDefined(x, typeof(IdColumn)));
            return idProperty?.Name;
        }

        /// <summary>
        /// Gets the property value of the property that contains the <see cref="IdColumn"/> attribute
        /// </summary>
        /// <param name="self">The object that contains the property</param>
        /// <returns>The value of the property with the <see cref="IdColumn"/> attribute</returns>
        internal static object GetIdColumnAttributeValue(this object self)
        {
            var properties = self.GetType().GetProperties();
            var idProperty = properties.FirstOrDefault(x => Attribute.IsDefined(x, typeof(IdColumn)));
            return idProperty?.GetValue(self);
        }
    }
}
