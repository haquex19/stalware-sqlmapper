using Stalware.SqlMapper.Attributes;
using Stalware.SqlMapper.Interfaces;
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
        /// A builder that inserts text before the INSERT clause
        /// </summary>
        protected readonly StringBuilder BeforeBuilder;

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

            BeforeBuilder = new StringBuilder();
            InsertBuilder = new StringBuilder();
            ValuesBuilder = new StringBuilder();
            BetweenBuilder = new StringBuilder();
            EndBuilder = new StringBuilder();
            Result = new SqlMapperResult();

            IdColumnName = typeof(T).GetProperties().GetIdColumnAttributeName();
        }
        
        /// <summary>
        /// Creats an abstraction for <see cref="IInsertBuilder{T}.AddServerGuidIdStatement"/>
        /// </summary>
        public abstract IInsertBuilder<T> AddServerGuidIdStatement();

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Build"/>
        /// </summary>
        public SqlMapperResult Build()
        {
            InsertBuilder.Insert(0, $"INSERT INTO {typeof(T).Name} (");
            InsertBuilder.Remove(InsertBuilder.Length - 2, 2);

            ValuesBuilder.Insert(0, "VALUES (");
            ValuesBuilder.Remove(ValuesBuilder.Length - 2, 2);

            BeforeBuilder.Append($"{InsertBuilder}) {BetweenBuilder}{ValuesBuilder}){EndBuilder}");
            Result.Query = BeforeBuilder.ToString();
            return Result;
        }

        /// <summary>
        /// Implements <see cref="IBuilder{TBuilder}.Clear"/>
        /// </summary>
        public virtual IInsertBuilder<T> Clear()
        {
            BeforeBuilder.Clear();
            InsertBuilder.Clear();
            ValuesBuilder.Clear();
            BetweenBuilder.Clear();
            EndBuilder.Clear();
            Result.Query = null;
            Result.Parameters = new Dictionary<string, object>();
            IdColumnAdded = false;
            return this;
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
            var exceptProperties = exceptObj.GetColumnProperties();

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
            var onlyColumnsProperties = onlyColumnsObj.GetColumnProperties();

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

            Result.Parameters.Add(property.Name, value);
        }       
    }
}
