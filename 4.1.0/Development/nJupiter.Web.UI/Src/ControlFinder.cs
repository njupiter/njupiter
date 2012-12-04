#region Copyright & License
/*
	Copyright (c) 2005-2012 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace nJupiter.Web.UI {
	public class ControlFinder : IControlFinder {
		
		public Control FindControl(Control rootControl, string uniqueId) {
			return FindControl(rootControl, uniqueId, false);
		}
		
		public Control FindControl(Control rootControl, string uniqueId, bool ensureChildControls) {
			return FindControlsRecursive(rootControl, uniqueId, ensureChildControls);
		}

		public T[] FindControlsOnType<T>(Control rootControl) {
			return FindControlsOnType<T>(rootControl, false);
		}

		public T[] FindControlsOnType<T>(Control rootControl, bool ensureChildControls) {
			var container = FindControlsOnTypeRecursive(rootControl, typeof(T), new List<Control>(), false, ensureChildControls);
			return container.Cast<T>().ToArray();
		}

		public T FindFirstControlOnType<T>(Control rootControl) {
			return FindFirstControlOnType<T>(rootControl, false);
		}

		public T FindFirstControlOnType<T>(Control rootControl, bool ensureChildControls) {
			var container = FindControlsOnTypeRecursive(rootControl, typeof(T), new List<Control>(), true, ensureChildControls);
			return container.Cast<T>().FirstOrDefault();
		}

		public Control[] FindControlsOnType(Control rootControl, Type type) {
			return FindControlsOnType(rootControl, type, false);
		}
		public Control[] FindControlsOnType(Control rootControl, Type type, bool ensureChildControls) {
			var container = FindControlsOnTypeRecursive(rootControl, type, new List<Control>(), false, ensureChildControls);
			return container.ToArray();
		}
		public Control FindFirstControlOnType(Control rootControl, Type type) {
			return FindFirstControlOnType(rootControl, type, false);
		}
		public Control FindFirstControlOnType(Control rootControl, Type type, bool ensureChildControls) {
			var container = FindControlsOnTypeRecursive(rootControl, type, new List<Control>(), true, ensureChildControls);
			return container.FirstOrDefault();
		}
		
		private Control FindControlsRecursive(Control rootControl, string uniqueId, bool ensureChildControls) {
			if(ensureChildControls) {
				rootControl.FindControl(string.Empty);
				// Microsoft have made some vital metods protected so we have to do this hack to ensure that child controls are built
			}
			foreach(Control control in rootControl.Controls) {
				if(control.UniqueID == uniqueId) {
					return control;
				}
				if(control.Controls.Count > 0) {
					var innerControl = FindControlsRecursive(control, uniqueId, ensureChildControls);
					if(innerControl != null) {
						return innerControl;
					}
				}
			}
			return null;
		}
	
		private List<Control> FindControlsOnTypeRecursive(Control rootControl, Type type, List<Control> container, bool breakOnHit, bool ensureChildControls) {
			if(ensureChildControls) {
				rootControl.FindControl(string.Empty);
				// Microsoft have made some vital metods protected so we have to do this hack to ensure that child controls are built
			}
			foreach(Control control in rootControl.Controls) {
				if(type.IsInstanceOfType(control)) {
					container.Add(control);
					if(breakOnHit) {
						break;
					}
				}
				if(control.Controls.Count > 0) {
					container = FindControlsOnTypeRecursive(control, type, container, breakOnHit, ensureChildControls);
				}
			}
			return container;
		}

		public static IControlFinder Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IControlFinder instance = new ControlFinder();
		}
	}
}