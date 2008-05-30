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

namespace Spring.Context
{
	/// <summary>
	/// The callback for application events.
	/// </summary>
	public delegate void ApplicationEventHandler(object sender, ApplicationEventArgs e);

	/// <summary>
	/// A listener for application events.
	/// </summary>
	/// <author>Rod Johnson</author>
	/// <author>Griffin Caprio (.NET)</author>
	/// <version>$Id: IApplicationEventListener.cs,v 1.2 2006/04/09 07:18:38 markpollack Exp $</version>
	[EventListener]
	public interface IApplicationEventListener
	{
		/// <summary>
		/// Handle an application event.
		/// </summary>
		/// <param name="sender">
		/// The source of the event.
		/// </param>
		/// <param name="e">
		/// The event that is to be handled.
		/// </param>
		void HandleApplicationEvent(object sender, ApplicationEventArgs e);
	}
}