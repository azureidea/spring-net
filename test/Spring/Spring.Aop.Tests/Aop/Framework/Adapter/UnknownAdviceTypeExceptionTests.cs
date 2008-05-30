#region License

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

#region Imports

using System;

using NUnit.Framework;

#endregion

namespace Spring.Aop.Framework.Adapter
{
	/// <summary>
	/// Unit tests for the UnknownAdviceTypeException class.
    /// </summary>
    /// <author>Rick Evans</author>
    /// <version>$Id: UnknownAdviceTypeExceptionTests.cs,v 1.2 2006/04/09 07:19:05 markpollack Exp $</version>
	[TestFixture]
    public sealed class UnknownAdviceTypeExceptionTests
    {
        [Test]
        public void InstantiationWithNullAdviceDoesNotThrowAnotherException()
        {
            new UnknownAdviceTypeException(null);
        }
	}
}
