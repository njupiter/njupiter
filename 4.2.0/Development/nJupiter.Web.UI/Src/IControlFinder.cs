using System;
using System.Web.UI;

namespace nJupiter.Web.UI {
	public interface IControlFinder {
		Control FindControl(Control rootControl, string uniqueId);
		Control FindControl(Control rootControl, string uniqueId, bool ensureChildControls);
		Control[] FindControlsOnType(Control rootControl, Type type);
		Control[] FindControlsOnType(Control rootControl, Type type, bool ensureChildControls);
		Control FindFirstControlOnType(Control rootControl, Type type);
		Control FindFirstControlOnType(Control rootControl, Type type, bool ensureChildControls);
	}
}