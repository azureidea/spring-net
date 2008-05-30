#region Licence

/*
 * Copyright � 2002-2005 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#if NET_2_0

#region Imports



#endregion

using Spring.Data.Support;

namespace Spring.Data.Generic
{
	/// <summary>
	/// Provides a name to a ResultSetProcessor for use with AdoOperation subclasses
	/// such as StoredProcedure.
	/// </summary>
	/// <author>Mark Pollack (.NET)</author>
	/// <version>$Id: NamedResultSetProcessor.cs,v 1.2 2007/07/25 13:32:31 oakinger Exp $</version>
	public class NamedResultSetProcessor<T>
	{
		#region Fields

        private IRowCallback rowCallback;
	    private IRowMapper<T> rowMapper;
	    private IResultSetExtractor<T> resultSetExtractor;
	    private string name;
		
	    #endregion

		#region Constructor (s)

        /// <summary>
        /// Initializes a new instance of the class with a
        /// IRowCallback instance
        /// </summary>
        /// <param name="name">The name of the associated row callback for use with output 
        /// result set mappers.</param>
        /// <param name="rowcallback">A row callback instance.</param>
		public NamedResultSetProcessor(string name, IRowCallback rowcallback)
		{
            this.name = name;
		    this.rowCallback = rowcallback;
		}
	    
	    /// <summary>
        /// Initializes a new instance of the class with a
        /// IRowMapper instance
	    /// </summary>
	    /// <param name="name">The name of the associated row mapper.</param>
	    /// <param name="rowMapper">A IRowMapper instance.</param>
	    public NamedResultSetProcessor(string name, IRowMapper<T> rowMapper)
	    {
	        this.name = name;
	        this.rowMapper = rowMapper;
	    }
	    
        /// <summary>
        /// Initializes a new instance of the class with a
        /// IResultSetExtractor instance
        /// </summary>
        /// <param name="name">The name of the associated result set extractor.</param>
        /// <param name="resultSetExtractor">A IResultSetExtractor instance.</param>
        public NamedResultSetProcessor(string name, IResultSetExtractor<T> resultSetExtractor)
        {
            this.name = name;
            this.resultSetExtractor = resultSetExtractor;
        }


		#endregion

		#region Properties
	    public string Name
	    {
	        get
	        {
	            return name;
	        }
	    }


        public IRowCallback RowCallback
	    {
	        get { return rowCallback; }
	    }

	    public IRowMapper<T> RowMapper
	    {
	        get { return rowMapper; }
	    }

	    public IResultSetExtractor<T> ResultSetExtractor
	    {
	        get { return resultSetExtractor; }
	    }

	    #endregion


	}
}

#endif
