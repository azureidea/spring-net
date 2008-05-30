#region License

/*
 * Copyright � 2002-2007 the original author or authors.
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

using Spring.Objects.Factory;

#endregion

namespace Spring.Objects.Factory.Config
{
    /// <summary>
    /// Extension of the <see cref="Spring.Objects.Factory.IFactoryObject"/> interface 
    /// that injects dependencies into the object managed by the factory.
    /// </summary>
    /// <author>Bruno Baia</author>
    /// <version>$Id: IConfigurableFactoryObject.cs,v 1.1 2007/07/29 19:39:27 markpollack Exp $</version>
    public interface IConfigurableFactoryObject : IFactoryObject
    {
        /// <summary>
        /// Gets the template object definition that should be used 
        /// to configure the instance of the object managed by this factory.
        /// </summary>
        IObjectDefinition ProductTemplate { get; }
    }
}