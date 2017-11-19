using Stalware.SqlMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Stalware.SqlMapper.Insertions
{
    /// <summary>
    /// Implements <see cref="IInsertBuilder{T}"/>
    /// </summary>
    public abstract class InsertBuilderBase<T> : IInsertBuilder<T> where T : new()
    {
        /// <summary>
        /// The insert builder
        /// </summary>
        protected readonly StringBuilder InsertBuilder;

        /// <summary>
        /// The values builder for the VALUES clause
        /// </summary>
        protected readonly StringBuilder ValuesBuilder;

        /// <summary>
        /// A builder that inserts text between the INSERT INTO and VALUES clause
        /// </summary>
        protected readonly StringBuilder BetweenBuilder;

        /// <summary>
        /// A builder that inserts text after the VALUES text
        /// </summary>
        protected readonly StringBuilder EndBuilder;

        /// <summary>
        /// The name of the id colum property designated by the <see cref="IdColumn"/> attribute
        /// </summary>
        protected readonly string IdColumnName;

        /// <summary>
        /// The <see cref="SqlMapperResult"/> result object
        /// </summary>
        protected readonly SqlMapperResult Result;

        /// <summary>
        /// The record object to insert
        /// </summary>
        protected readonly T Record;

        /// <summary>
        /// Indicates if the id column has already been added to the insert statement
        /// </summary>
        protected bool IdColumnAdded;

        /// <summary>
        /// Instantiates the <see cref="InsertBuilderBase{T}"/> abstract class
        /// </summary>
        protected InsertBuilderBase(T record)
        {
            Record = record;
            var type = typeof(T);

            InsertBuilder = new StringBuilder($"INSERT INTO {type.Name} (");
            ValuesBuilder = new StringBuilder("VALUES (");
            BetweenBuilder = new StringBuilder();
            EndBuilder = new StringBuilder();
            Result = new SqlMapperResult();

            var idProperty = type.GetProperties().FirstOrDefault(x => Attribute.IsDefined(x, typeof(IdColumn)));
            IdColumnName = idProperty?.Name;
        }
        
        /// <summary>
        /// Creats an abstraction for <see cref="IInsertBuilder{T}.AddServerGuidIdStatement"/>
        /// </summary>
        public abstract IInsertBuilder<T> AddServerGuidIdStatement();

        /// <summary>
        /// Implements <see cref="IInsertBuilder{T}.Build"/>
        /// </summary>
        public SqlMapperResult Build()
        {
            RemoveInsertAndValuesExtraComma();

            InsertBuilder.Append($") {BetweenBuilder}{ValuesBuilder}){EndBuilder}");
            Result.Query = InsertBuilder.ToString();
            return Result;
        }

        /// <summary>
        /// Implements <see cref="IInsertBuilder{T}.InsertAll(bool)"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="includeId"/> is set to true but the 
        /// id column has already been added</exception>
        public IInsertBuilder<T> InsertAll(bool includeId = false)
        {
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                if (!includeId && prop.Name == IdColumnName)
                {
                    continue;
                }

                AddPropToInsertAndValues(prop, includeId);
            }

            return this;
        }

        /// <summary>
        /// Implements <see cref="IInsertBuilder{T}.InsertAllExcept(Func{T, object}, bool)"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="includeId"/> is set to true but the 
        /// id column has already been added</exception>
        public IInsertBuilder<T> InsertAllExcept(Func<T, object> exceptColumns, bool includeId = false)
        {
            var properties = typeof(T).GetProperties();
            var exceptObj = exceptColumns(new T());
            var exceptProperties = exceptObj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (exceptProperties.Any(x => x.Name == prop.Name) || (!includeId && prop.Name == IdColumnName))
                {
                    continue;
                }

                AddPropToInsertAndValues(prop, includeId);
            }

            return this;
        }

        /// <summary>
        /// Implements <see cref="IInsertBuilder{T}.InsertOnly(Func{T, object}, bool)"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="includeId"/> is set to true but the 
        /// id column has already been added</exception>
        public IInsertBuilder<T> InsertOnly(Func<T, object> onlyColumns, bool includeId = false)
        {
            var properties = typeof(T).GetProperties();
            var onlyColumnsObj = onlyColumns(new T());
            var onlyColumnsProperties = onlyColumnsObj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (onlyColumnsProperties.Any(x => x.Name == prop.Name))
                {
                    if (!includeId && prop.Name == IdColumnName)
                    {
                        continue;
                    }

                    AddPropToInsertAndValues(prop, includeId);
                }
            }            

            return this;
        }

        private void AddPropToInsertAndValues(PropertyInfo property, bool includeId)
        {
            if (property.Name == IdColumnName && IdColumnAdded && includeId)
            {
                throw new InvalidOperationException("Requested to add the id column, but the id column has already been added");
            }

            if (property.Name == IdColumnName && includeId)
            {
                IdColumnAdded = true;
            }

            InsertBuilder.Append($"{property.Name}, ");
            ValuesBuilder.Append($"@{property.Name}, ");
            var value = property.GetValue(Record);

            Result.Parameters.Add(new KeyValuePair<string, object>(property.Name, value));
        }

        private void RemoveInsertAndValuesExtraComma()
        {
            InsertBuilder.Remove(InsertBuilder.Length - 2, 2);
            ValuesBuilder.Remove(ValuesBuilder.Length - 2, 2);
        }        
    }
}
