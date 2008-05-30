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

using System;
using System.Reflection;

using Spring.Caching;
using Spring.Context;
using Spring.Expressions;
using System.Collections;

#endregion

namespace Spring.Aspects.Cache
{
    /// <summary>
    /// Base class for different cache advice implementations that provide
    /// access to common functionality, such as obtaining a cache instance.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: BaseCacheAdvice.cs,v 1.3 2007/04/01 15:04:04 bbaia Exp $</version>
    public class BaseCacheAdvice : IApplicationContextAware
    {
        private IApplicationContext applicationContext;
        
        /// <summary>
        /// Set the <see cref="Spring.Context.IApplicationContext"/> that this
        /// object runs in.
        /// </summary>
        public IApplicationContext ApplicationContext
        {
            get { return applicationContext; }
            set { applicationContext = value; }
        }

        /// <summary>
        /// Returns an <see cref="ICache"/> instance based on the cache name.
        /// </summary>
        /// <param name="name">The name of the cache.</param>
        /// <returns>
        /// Cache instance for the specified <paramref name="name"/> if one
        /// is registered in the application context, or <c>null</c> if it isn't.
        /// </returns>
        public ICache GetCache(string name)
        {
            return applicationContext.GetObject(name) as ICache;
        }

        /// <summary>
        /// Prepares variables for expression evaluation by packaging all 
        /// method arguments into a dictionary, keyed by argument name.
        /// </summary>
        /// <param name="method">
        /// Method to get parameters info from.
        /// </param>
        /// <param name="arguments">
        /// Argument values to package.
        /// </param>
        /// <returns>
        /// A dictionary containing all method arguments, keyed by method name.
        /// </returns>
        protected static IDictionary PrepareVariables(MethodInfo method, object[] arguments)
        {
            ParameterInfo[] parameters = method.GetParameters();

            IDictionary vars = new Hashtable();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo p = parameters[i];
                vars[p.Name] = arguments[i];
            }
            return vars;
        }

        /// <summary>
        /// Evaluates a SpEL expression as a boolean value.
        /// </summary>
        /// <param name="condition">
        /// The expression string that should be evaluated.
        /// </param>
        /// <param name="conditionExpression">
        /// The SpEL expression instance that should be evaluated.
        /// </param>
        /// <param name="context">
        /// The object to evaluate expression against.
        /// </param>
        /// <param name="variables">
        /// The expression variables dictionary.
        /// </param>
        /// <returns>
        /// The evaluated boolean.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// If the SpEL expression could not be successfuly resolved to a boolean.
        /// </exception>
        protected static bool EvalCondition(string condition, 
            IExpression conditionExpression, object context, IDictionary variables)
        {
            if (conditionExpression == null)
            {
                return true;
            }
            else
            {
                object value = conditionExpression.GetValue(context, variables);
                if (value is bool)
                {
                    return (bool)value;
                }
                else
                {
                    throw new InvalidOperationException(String.Format(
                        "The SpEL expression '{0}' could not be successfuly resolved to a boolean.", 
                        condition));
                }
            }
        }

        /// <summary>
        /// Retrieves custom attribute for the specified attribute type.
        /// </summary>
        /// <param name="method">
        /// Method to get attribute from.
        /// </param>
        /// <param name="attributeType">
        /// Attribute type.
        /// </param>
        /// <returns>
        /// Attribute instance if one is found, <c>null</c> otherwise.
        /// </returns>
        protected static object GetCustomAttribute(MethodInfo method, Type attributeType)
        {
            object[] attributes = method.GetCustomAttributes(attributeType, false);
            if (attributes.Length > 0)
            {
                return attributes[0];
            }
            return null;
        }
    }
}