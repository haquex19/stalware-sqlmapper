using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// A class for containing property names for an expando object. Meant for internal use only and to follow and support
    /// the LINQ anonymous object column selection pattern for dynamic <see cref="ExpandoObject"/> types.
    /// </summary>
    internal class ExpandoPropertyInfo : PropertyInfo
    {
        private readonly string _name;

        /// <summary>
        /// Instantiates the <see cref="ExpandoPropertyInfo"/> class
        /// </summary>
        /// <param name="name">The property name</param>
        public ExpandoPropertyInfo(string name)
        {
            _name = name;
        }

        public override PropertyAttributes Attributes => throw new NotImplementedException();

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override Type PropertyType => throw new NotImplementedException();

        public override Type DeclaringType => throw new NotImplementedException();

        /// <summary>
        /// The property name
        /// </summary>
        public override string Name { get { return _name; } }

        public override Type ReflectedType => throw new NotImplementedException();

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
