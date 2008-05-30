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

#endregion

namespace Spring.Objects.Factory.Support
{
    /// <summary>
    /// Central interface for factories that can create
    /// <see cref="IConfigurableObjectDefinition"/>
    /// instances.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Allows for replaceable object definition factories using the Strategy
    /// pattern.
    /// </p>
    /// </remarks>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: IObjectDefinitionFactory.cs,v 1.10 2007/07/30 18:00:01 markpollack Exp $</version>
    public interface IObjectDefinitionFactory
    {

        /// <summary>
        /// Factory style method for getting concrete
        /// <see cref="IConfigurableObjectDefinition"/>
        /// instances.
        /// </summary>
        /// <param name="typeName">
        /// The FullName of the <see cref="System.Type"/> of the defined object.
        /// </param>
        /// <param name="parent">The name of the parent object definition (if any).</param>
        /// <param name="domain">
        /// The <see cref="System.AppDomain"/> against which any class names
        /// will be resolved into <see cref="System.Type"/> instances.  It can be null to register the
        /// object class just by name.
        /// </param>
        /// <returns>
        /// An
        /// <see cref="Spring.Objects.Factory.Support.AbstractObjectDefinition"/>
        /// instance.
        /// </returns>
        AbstractObjectDefinition CreateObjectDefinition(string typeName, string parent, AppDomain domain);


    }
}
