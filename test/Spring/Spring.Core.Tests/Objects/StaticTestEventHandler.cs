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

namespace Spring.Objects {

	/// <summary>
	/// Exposes a static method that has a signature compatible with the
	/// EventHandler delegate.
    /// </summary>
    /// <version>$Id: StaticTestEventHandler.cs,v 1.3 2006/04/09 07:24:50 markpollack Exp $</version>
    public class StaticTestEventHandler
    {
        public static void HandleArbitraryEvent (object sender, EventArgs e) 
        {
            _eventWasHandled = true;
        }

        public static bool EventWasHandled 
        {
            get 
            {
                return _eventWasHandled;
            }
        }

        protected static bool _eventWasHandled;
	}
}
