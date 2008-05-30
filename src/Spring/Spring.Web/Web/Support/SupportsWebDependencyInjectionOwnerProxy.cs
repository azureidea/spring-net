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

using System.Web.UI;
using Spring.Context;
using Spring.Context.Support;
using Spring.Util;
using Spring.Web.Support;

#endregion

namespace Spring.Web.Support
{
    /// <summary>
    /// Wraps a Control to make it DI aware
    /// </summary>
    /// <author>Erich Eichinger</author>
    /// <version>$Id: SupportsWebDependencyInjectionOwnerProxy.cs,v 1.2 2008/05/13 14:22:47 oakinger Exp $</version>
    internal class SupportsWebDependencyInjectionOwnerProxy : Control, ISupportsWebDependencyInjection
    {
        // Control Fields and Methods	    
        private readonly ControlAccessor _targetControl;
        private IApplicationContext _defaultApplicationContext;

        /// <summary>
        /// Wraps a control to make it DI aware
        /// </summary>
        /// <param name="defaultApplicationContext"></param>
        /// <param name="targetControl"></param>
        public SupportsWebDependencyInjectionOwnerProxy(IApplicationContext defaultApplicationContext, Control targetControl)
        {
            _defaultApplicationContext = defaultApplicationContext;
            _targetControl = new ControlAccessor(targetControl);
        }

        public IApplicationContext DefaultApplicationContext
        {
            get { return this._defaultApplicationContext; }
            set { this._defaultApplicationContext = value; }
        }

        /// <summary>
        /// Performs DI before adding the control to it's parent
        /// </summary>
        /// <param name="control"></param>
        /// <param name="index"></param>
        protected override void AddedControl(Control control, int index)
        {
            // do DI
            WebDependencyInjectionUtils.InjectDependenciesRecursive(_defaultApplicationContext, control);

            _targetControl.AddedControl(control, index);
        }

        /// <summary>
        /// Delegates call to decorated control
        /// </summary>
        /// <param name="control"></param>
        protected override void RemovedControl(Control control)
        {
            _targetControl.RemovedControl(control);
        }
    }
}