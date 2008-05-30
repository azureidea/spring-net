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

using System.Reflection;
using Common.Logging;
using Spring.Aop;
using Spring.Caching;
using Spring.Util;

#endregion

namespace Spring.Aspects.Cache
{
    /// <summary>
    /// Implementation of a parameter caching advice.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This advice can be used to cache the parameter of the method. 
    /// </p>
    /// <p>
    /// Information that determines where, how and for how long the return value 
    /// will be cached are retrieved from the <see cref="CacheParameterAttribute"/>s 
    /// that are defined on the pointcut.
    /// </p>
    /// <p>
    /// Parameter values are cached *after* the target method is invoked in order to
    /// capture any parameter state changes it might make (for example, it is common
    /// to set an object identifier within the save method for the persistent entity).
    /// </p>
    /// </remarks>
    /// <seealso cref="CacheParameterAttribute"/>
    /// <author>Aleksandar Seovic</author>
    /// <version>$Id: CacheParameterAdvice.cs,v 1.7 2008/05/05 15:00:55 markpollack Exp $</version>
    public class CacheParameterAdvice : BaseCacheAdvice, IAfterReturningAdvice
    {
        // shared logger instance
        private static readonly ILog logger = LogManager.GetLogger(typeof(CacheParameterAdvice));

        /// <summary>
        /// Executes after target <paramref name="method"/>
        /// returns <b>successfully</b>.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Note that the supplied <paramref name="returnValue"/> <b>cannot</b>
        /// be changed by this type of advice... use the around advice type
        /// (<see cref="AopAlliance.Intercept.IMethodInterceptor"/>) if you
        /// need to change the return value of an advised method invocation.
        /// The data encapsulated by the supplied <paramref name="returnValue"/>
        /// can of course be modified though.
        /// </p>
        /// </remarks>
        /// <param name="returnValue">
        /// The value returned by the <paramref name="target"/>.
        /// </param>
        /// <param name="method">The intecepted method.</param>
        /// <param name="arguments">The intercepted method's arguments.</param>
        /// <param name="target">The target object.</param>
        /// <seealso cref="AopAlliance.Intercept.IMethodInterceptor.Invoke"/>
        public void AfterReturning(object returnValue, MethodInfo method, object[] arguments, object target)
        {
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo p = parameters[i];
                CacheParameterAttribute[] paramInfoArray =
                    (CacheParameterAttribute[])p.GetCustomAttributes(typeof(CacheParameterAttribute), false);

                bool isLogDebugEnabled = logger.IsDebugEnabled;
                foreach (CacheParameterAttribute paramInfo in paramInfoArray)
                {
                    if (EvalCondition(paramInfo.Condition, paramInfo.ConditionExpression, arguments[i], null))
                    {
                        ICache cache = GetCache(paramInfo.CacheName);
                        AssertUtils.ArgumentNotNull(cache, "CacheName",
                                                    "Parameter cache with the specified name [" + paramInfo.CacheName +
                                                    "] does not exist.");
                        object key = paramInfo.KeyExpression.GetValue(arguments[i]);
                        #region Instrumentation

                        if (isLogDebugEnabled)
                        {
                            logger.Debug("Caching parameter for key [" + key + "].");
                        }

                        #endregion
                        cache.Insert(key, arguments[i], paramInfo.TimeToLiveTimeSpan);
                    }
                }
            }
        }
    }
}