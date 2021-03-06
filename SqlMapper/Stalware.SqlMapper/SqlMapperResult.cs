﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Stalware.SqlMapper
{
    /// <summary>
    /// Class that contains a query and parameters for that query
    /// </summary>
    public class SqlMapperResult
    {
        /// <summary>
        /// The query
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// The paramaters for the query string in <see cref="Query"/>
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Instantiates the <see cref="SqlMapperResult"/> class
        /// </summary>
        public SqlMapperResult()
        {
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Instantiates the <see cref="SqlMapperResult"/> class by setting the query and its parameters
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="parameters">The parameters for the query string in <paramref name="query"/></param>
        public SqlMapperResult(string query, Dictionary<string, object> parameters)
        {
            Query = query;
            Parameters = parameters;
        }
    }
}
