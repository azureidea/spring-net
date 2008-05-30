#region License

/*
 * Copyright 2002-2005 the original author or authors.
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
using DotNetMock.Dynamic;
using NUnit.Framework;
using Spring.Core.IO;
using Spring.Util;

#endregion

namespace Spring.Objects.Factory
{
	/// <summary>
	/// Unit tests for the ObjectDefinitionStoreException class.
	/// </summary>
	/// <author>Rick Evans</author>
	/// <version>$Id: ObjectDefinitionStoreExceptionTests.cs,v 1.5 2007/08/08 17:49:04 bbaia Exp $</version>
	[TestFixture]
	public sealed class ObjectDefinitionStoreExceptionTests
	{
		[Test]
		public void FromResource()
		{
			string expectedName = "bing";
			string expectedResourceDescription = "mock.resource";
			DynamicMock mock = new DynamicMock(typeof(IResource));
			mock.ExpectAndReturn("Description", expectedResourceDescription);
			IResource resource = (IResource) mock.Object;
			ObjectDefinitionStoreException inex
				= new ObjectDefinitionStoreException(resource, expectedName, "mmm...");
			mock.Verify();
			CheckSerialization(inex, expectedName, expectedResourceDescription);
		}

		[Test]
		public void FromNullResource()
		{
			string expectedName = "bing";
			string expectedResourceDescription = string.Empty;
			ObjectDefinitionStoreException inex
				= new ObjectDefinitionStoreException(
				(IResource) null, expectedName, "mmm...");
			CheckSerialization(inex, expectedName,
				expectedResourceDescription);
		}

		[Test]
		public void SerializesWithNoState()
		{
			ObjectDefinitionStoreException inex
				= new ObjectDefinitionStoreException();
			CheckSerialization(inex, string.Empty, string.Empty);
		}

		[Test]
		public void SerializesWithJustExceptionMessage()
		{
			string expectedName = string.Empty;
			string expectedResourceDescription = string.Empty;
			string expectedMessage = "Woppadoosa";
			ObjectDefinitionStoreException inex
				= new ObjectDefinitionStoreException(expectedMessage);
			CheckSerialization(inex, expectedName,
				expectedResourceDescription);
		}

		[Test]
		public void SerializesAllState()
		{
			string expectedName = "bing";
			string expectedResourceDescription = "foo.txt";
			ObjectDefinitionStoreException inex
				= new ObjectDefinitionStoreException(
				expectedResourceDescription, expectedName, "mmm...");
			CheckSerialization(inex, expectedName,
				expectedResourceDescription);
		}

		private static void CheckSerialization(
			ObjectDefinitionStoreException inex, string expectedName,
			string expectedResourceDescription)
		{
			ObjectDefinitionStoreException outex = (ObjectDefinitionStoreException)
				SerializationTestUtils.SerializeAndDeserialize(inex);
			Assert.AreEqual(expectedName, outex.ObjectName,
			                "The 'ObjectName' property was not serialized / deserialized correctly.");
			Assert.AreEqual(expectedResourceDescription, outex.ResourceDescription,
				"The 'ResourceDescription' property was not serialized / deserialized correctly.");
		}
	}
}