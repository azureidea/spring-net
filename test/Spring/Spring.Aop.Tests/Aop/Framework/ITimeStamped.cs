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
#endregion

namespace Spring.Aop.Framework
{
	/// <summary>
	/// This interface can be implemented by cacheable objects
	/// or cache entries, to enable the freshness of objects
	/// to be checked.
	/// </summary>
	/// <author>Rod Johnson</author>
	/// <author>Choy Rim (.NET)</author>
	/// <version>$Id: ITimeStamped.cs,v 1.1 2004/08/01 10:20:20 choyrim Exp $</version>
	public interface ITimeStamped
	{
		/// <summary>
		/// Return the timestamp for this object.
		/// </summary>
		DateTime TimeStamp { get; }
	}
}
