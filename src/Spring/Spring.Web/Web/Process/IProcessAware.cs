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

using System.Web;

namespace Spring.Web.Process
{
    /// <summary>
    /// Interface that should be implemented by all <see cref="IHttpHandler"/>s
    /// that want to be aware of the <see cref="IProcess"/> they belong to. 
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: IProcessAware.cs,v 1.2 2006/05/18 21:37:52 markpollack Exp $</version>
    public interface IProcessAware
    {
        /// <summary>
        /// Gets or sets a process instance.
        /// </summary>
        IProcess Process { get; set; }
    }
}