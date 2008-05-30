#region License

/*
 * Copyright 2002-2004 the original author or authors.
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

#region Imports

using System;
using System.Runtime.Serialization;

#endregion

namespace Spring.Dao
{
	/// <summary> 
    /// Exception thrown when the underlyingresource denied a permission to 
    /// access a specific element, such as a specific database table.
	/// </summary>
	/// <author>Juergen Hoeller</author>
	/// <author>Mark Pollack (.NET)</author>
	/// <version>$Id: PermissionDeniedDataAccessException.cs,v 1.1 2006/06/12 10:39:19 markpollack Exp $</version>
	[Serializable]
	public class PermissionDeniedDataAccessException : InvalidDataAccessResourceUsageException
	{
		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Dao.PermissionDeniedDataAccessException"/> class.
		/// </summary>
		public PermissionDeniedDataAccessException() {}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Dao.PermissionDeniedDataAccessException"/> class.
		/// </summary>
		/// <param name="message">
		/// A message about the exception.
		/// </param>
		public PermissionDeniedDataAccessException( string message ) : base( message ) {}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Dao.PermissionDeniedDataAccessException"/> class.
		/// </summary>
		/// <param name="message">
		/// A message about the exception.
		/// </param>
		/// <param name="rootCause">
		/// The root exception (from the underlying data access API, such as ADO.NET).
		/// </param>
		public PermissionDeniedDataAccessException( string message, Exception rootCause)
			: base( message , rootCause ) {}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Dao.PermissionDeniedDataAccessException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
		/// that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="System.Runtime.Serialization.StreamingContext"/>
		/// that contains contextual information about the source or destination.
		/// </param>
		protected PermissionDeniedDataAccessException(
			SerializationInfo info, StreamingContext context ) : base( info, context ) {}
	}
}