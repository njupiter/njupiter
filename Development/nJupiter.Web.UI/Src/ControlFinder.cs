using System;
using System.Collections;
using System.Web.UI;

namespace nJupiter.Web.UI {
	public class ControlFinder : IControlFinder {
		public Control FindControl(Control rootControl, string uniqueId) {
			return FindControl(rootControl, uniqueId, false);
		}
		public Control FindControl(Control rootControl, string uniqueId, bool ensureChildControls) {
			return FindControlsRecursive(rootControl, uniqueId, ensureChildControls);
		}
		public Control[] FindControlsOnType(Control rootControl, Type type) {
			return FindControlsOnType(rootControl, type, false);
		}
		public Control[] FindControlsOnType(Control rootControl, Type type, bool ensureChildControls) {
			var container = FindControlsOnTypeRecursive(rootControl, type, new ArrayList(), false, ensureChildControls);
			return (Control[])container.ToArray(type);
		}
		public Control FindFirstControlOnType(Control rootControl, Type type) {
			return FindFirstControlOnType(rootControl, type, false);
		}
		public Control FindFirstControlOnType(Control rootControl, Type type, bool ensureChildControls) {
			var container = FindControlsOnTypeRecursive(rootControl, type, new ArrayList(), true, ensureChildControls);
			if(container.Count > 0) {
				return (Control)container[0];
			}
			return null;
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
	
		private ArrayList FindControlsOnTypeRecursive(Control rootControl, Type type, ArrayList container, bool breakOnHit, bool ensureChildControls) {
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