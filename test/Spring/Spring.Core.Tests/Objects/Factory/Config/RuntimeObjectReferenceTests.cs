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

using NUnit.Framework;

#endregion

namespace Spring.Objects.Factory.Config
{
	/// <summary>
	/// Unit tests for the RuntimeObjectReference class.
    /// </summary>
    /// <author>Rick Evans</author>
    /// <version>$Id: RuntimeObjectReferenceTests.cs,v 1.2 2006/04/09 07:24:50 markpollack Exp $</version>
	[TestFixture]
    public sealed class RuntimeObjectReferenceTests
    {
        [Test]
        public void InstantiationIsImplictlyNotToParent()
		{
			RuntimeObjectReference ror = new RuntimeObjectReference("foo");
			Assert.IsFalse(ror.IsToParent,
				"IsToParent property must default to false if not " +
				"using the explicit variant of the ctor.");
        }
	}
}
