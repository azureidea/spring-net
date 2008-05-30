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

using System;
using System.Runtime.Serialization;

namespace Spring.Transaction
{
	/// <summary> 
	/// Exception thrown when a transaction can't be created using an
	/// underlying transaction API such as COM+.
	/// </summary>
	/// <author>Rod Johnson</author>
	/// <author>Griffin Caprio (.NET)</author>
	/// <version>$Id: CannotCreateTransactionException.cs,v 1.5 2006/05/18 21:37:51 markpollack Exp $</version>
	[Serializable]
	public class CannotCreateTransactionException : TransactionException
	{
		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Transaction.CannotCreateTransactionException"/> class.
		/// </summary>
		public CannotCreateTransactionException() {}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Transaction.CannotCreateTransactionException"/> class.
		/// </summary>
		/// <param name="message">
		/// A message about the exception.
		/// </param>
		public CannotCreateTransactionException( string message ) : base( message ) {}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Transaction.CannotCreateTransactionException"/> class.
		/// </summary>
		/// <param name="message">
		/// A message about the exception.
		/// </param>
		/// <param name="rootCause">
		/// The root exception that is being wrapped.
		/// </param>
		public CannotCreateTransactionException( string message, Exception rootCause)
			: base( message , rootCause ) {}

		/// <summary>
		/// Creates a new instance of the
		/// <see cref="Spring.Transaction.CannotCreateTransactionException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
		/// that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="System.Runtime.Serialization.StreamingContext"/>
		/// that contains contextual information about the source or destination.
		/// </param>
		protected CannotCreateTransactionException(
			SerializationInfo info, StreamingContext context ) : base( info, context ) {}
	}
}