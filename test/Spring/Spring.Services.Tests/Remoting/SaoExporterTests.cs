#region License

/*
 * Copyright 2004 the original author or authors.
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

namespace Spring.Remoting
{
	/// <summary>
	/// Unit tests for the SaoExporter class.
	/// </summary>
	/// <author>Bruno Baia</author>
	/// <author>Mark Pollack</author>
	/// <version>$Id: SaoExporterTests.cs,v 1.4 2007/02/20 19:40:12 aseovic Exp $</version>
	[TestFixture]
	public class SaoExporterTests : BaseRemotingTestFixture
	{
        [Test]
		[ExpectedException(typeof(ArgumentException))]
		public void BailsWhenNotConfigured ()
		{
			SaoExporter exp = new SaoExporter();
			exp.AfterPropertiesSet();
		}
	}
}
