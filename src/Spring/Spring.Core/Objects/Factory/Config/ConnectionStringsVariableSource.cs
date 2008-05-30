#if NET_2_0
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

using System;
using System.Collections.Specialized;
using System.Configuration;
using Spring.Util;

namespace Spring.Objects.Factory.Config
{
    /// <summary>
    /// Implementation of <see cref="IVariableSource"/> that
    /// resolves variable name connection strings defined in
    /// the standard .NET configuration file.
    /// </summary>
    /// <remarks>
    /// <p>
    /// When the &lt;connectionStrings&gt; configuration section is processed by this class,
    /// two variables are defined for each connection string: one for connection string and
    /// the second one for the provider name.</p>
    /// <p>
    /// Variable names are generated by appending '.connectionString' and '.providerName'
    /// literals to the value of the <c>name</c> attribute of the connection string element.
    /// For example:</p>
    /// <pre>
    /// <connectionStrings>
    ///    <add name="myConn" connectionString="..." providerName="..." />
    /// </connectionStrings>
    /// </pre>
    /// <p>
    /// will result in two variables being created: <b>myConn.connectionString</b> and <b>myConn.providerName</b>.
    /// You can reference these variables within your object definitions, just like any other variable.</p>
    /// </remarks>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: ConnectionStringsVariableSource.cs,v 1.4 2007/08/02 22:18:32 markpollack Exp $</version>
    [Serializable]
    public class ConnectionStringsVariableSource : IVariableSource
    {
        private NameValueCollection variables;

        /// <summary>
        /// Resolves variable value for the specified variable name.
        /// </summary>
        /// <param name="name">
        /// The name of the variable to resolve.
        /// </param>
        /// <returns>
        /// The variable value if able to resolve, <c>null</c> otherwise.
        /// </returns>
        public string ResolveVariable(string name)
        {
            if (variables == null)
            {
                InitVariables();
            }
            return variables.Get(name);
        }

        /// <summary>
        /// Initializes properties based on the specified 
        /// property file locations.
        /// </summary>
        private void InitVariables()
        {
            variables = new NameValueCollection();
            ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;
            foreach (ConnectionStringSettings setting in settings)
            {
                string providerName = setting.ProviderName;

                variables.Add(setting.Name + ".connectionString", setting.ConnectionString);
                variables.Add(setting.Name + ".providerName",
                              StringUtils.HasText(providerName) ? providerName : "System.Data.SqlClient");
            }
        }
    }
}
#endif