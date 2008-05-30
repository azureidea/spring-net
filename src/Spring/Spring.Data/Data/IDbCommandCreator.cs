#region License

/*
 * Copyright � 2002-2006 the original author or authors.
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

using System.Data;
using Spring.Data.Common;

#endregion

namespace Spring.Data
{
	/// <summary>
	/// One of the central callback interfaces used by the AdoTemplate class
	/// This interface creates a IDbCommand.  Implementations are responsible
	/// for configuring the created command with command type, command text and
	/// any necessary parameters. 
	/// </summary>
	/// <author>Mark Pollack (.NET)</author>
	/// <version>$Id: IDbCommandCreator.cs,v 1.6 2007/08/06 20:13:39 markpollack Exp $</version>
	public interface IDbCommandCreator 
	{
        /// <summary>
        /// Creates a IDbCommand.
        /// </summary>
        /// <param name="dbProvider">The IDbProvider reference that can be used to create the command in a 
        /// provider independent manner.  The provider supplied is the same as used in configuration of AdoTemplate.</param>
        /// <returns></returns>
	    IDbCommand CreateDbCommand(IDbProvider dbProvider);
	}
}
