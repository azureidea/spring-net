#region License

/*
 * Copyright 2002-2007 the original author or authors.
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

using System.Collections;

namespace Spring.Aspects.Exceptions
{
    /// <summary>
    /// Returns a token to indicate that this exception should be swallowed.
    /// </summary>
    /// <author>Mark Pollack</author>
    /// <version>$Id: SwallowExceptionHandler.cs,v 1.2 2007/10/02 21:56:53 markpollack Exp $</version>
    public class SwallowExceptionHandler : AbstractExceptionHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwallowExceptionHandler"/> class.
        /// </summary>
        public SwallowExceptionHandler()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwallowExceptionHandler"/> class.
        /// </summary>
        /// <param name="exceptionNames">The exception names.</param>
        public SwallowExceptionHandler(string[] exceptionNames) : base(exceptionNames)
        {
        }



        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <returns>The return value from handling the exception, if not rethrown or a new exception is thrown.</returns>
        public override object HandleException(IDictionary callContextDictionary)
        {
            return "swallow";
        }
    }
}