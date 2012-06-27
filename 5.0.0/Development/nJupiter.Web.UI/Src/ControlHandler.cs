#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace nJupiter.Web.UI {
	public class ControlHandler {

		public static IControlHandler Instance = new ControlHandlerImpl();

		#region Legacy
		// This enum is nestled just because of legacy reasons to not break interface compatibility
		[Obsolete("Use RegisterTargetPreference in nJupiter.Web.UI instead")]
		public enum RegisterTargetPreference {
			Auto,
			Head,
			ScriptHolder,
			Page
		}

		[Obsolete("Use the same property on the UserAgent.Instance instead")]
		public static bool IsIE { get { return UserAgent.Instance.IsIE; } }

		[Obsolete("Use the same property on the UserAgent.Instance instead")]
		public static bool IsPreIE7 { get { return UserAgent.Instance.IsPreIE7; } }

		[Obsolete("Use the same method on the ControlFinder.Instance instead")]
		public static Control FindControl(Control rootControl, string uniqueId) {
			return ControlFinder.Instance.FindControl(rootControl, uniqueId);
		}

		[Obsolete("Use the same method on the ControlFinder.Instance instead")]
		public static Control FindControl(Control rootControl, string uniqueId, bool ensureChildControls) {
			return ControlFinder.Instance.FindControl(rootControl, uniqueId, ensureChildControls);
		}
		
		[Obsolete("Use the same method on the ControlFinder.Instance instead")]
		public static Control[] FindControlsOnType(Control rootControl, Type type) {
			return ControlFinder.Instance.FindControlsOnType(rootControl, type);
		}
	
		[Obsolete("Use the same method on the ControlFinder.Instance instead")]
		public static Control[] FindControlsOnType(Control rootControl, Type type, bool ensureChildControls) {
			return ControlFinder.Instance.FindControlsOnType(rootControl, type, ensureChildControls);
		}

		[Obsolete("Use the same method on the ControlFinder.Instance instead")]
		public static Control FindFirstControlOnType(Control rootControl, Type type) {
			return ControlFinder.Instance.FindFirstControlOnType(rootControl, type);
		}

		[Obsolete("Use the same method on the ControlFinder.Instance instead")]
		public static Control FindFirstControlOnType(Control rootControl, Type type, bool ensureChildControls) {
			return ControlFinder.Instance.FindFirstControlOnType(rootControl, type, ensureChildControls);
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptBlock(Type type, string key, string script, RegisterTargetPreference targetPreference) {
			ClientScriptRegistrator.Instance.RegisterClientScriptBlock(type, key, script, GetNewTargetPreference(targetPreference));
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptBlock(Type type, string key, string script) {
			ClientScriptRegistrator.Instance.RegisterClientScriptBlock(type, key, script);
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptInclude(Type type, string key, string url, RegisterTargetPreference targetPreference) {
			ClientScriptRegistrator.Instance.RegisterClientScriptBlock(type, key, url, GetNewTargetPreference(targetPreference));
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptInclude(Type type, string key, string url) {
			ClientScriptRegistrator.Instance.RegisterClientScriptInclude(type, key, url);
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptResource(Type type, string resourceName, RegisterTargetPreference targetPreference) {
			ClientScriptRegistrator.Instance.RegisterClientScriptResource(type, resourceName, GetNewTargetPreference(targetPreference));
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptResource(Type type, string resourceName) {
			ClientScriptRegistrator.Instance.RegisterClientScriptResource(type, resourceName);
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptBlock(Type type, string key, string script, UI.RegisterTargetPreference targetPreference) {
			ClientScriptRegistrator.Instance.RegisterClientScriptBlock(type, key, script, targetPreference);
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptInclude(Type type, string key, string url, UI.RegisterTargetPreference targetPreference) {
			ClientScriptRegistrator.Instance.RegisterClientScriptInclude(type, key, url, targetPreference);
		}

		[Obsolete("Use the same method on the ClientScriptRegistrator.Instance instead")]
		public static void RegisterClientScriptResource(Type type, string resourceName, UI.RegisterTargetPreference targetPreference) {
			ClientScriptRegistrator.Instance.RegisterClientScriptResource(type, resourceName, targetPreference);
		}

		[Obsolete("Use the same method on the ControlHandler.Instance instead")]
		public static void WriteOnClickAttribute(HtmlTextWriter writer, HtmlControl control, bool submitsAutomatically, bool submitsProgramatically, bool causesValidation, string validationGroup) {
			Instance.WriteOnClickAttribute(writer, control, submitsAutomatically, submitsProgramatically, causesValidation, validationGroup);
		}

		[Obsolete("Use the same method on the ControlHandler.Instance instead")]
		public static string GetClientValidateEvent(string validationGroup) {
			return Instance.GetClientValidateEvent(validationGroup);
		}

		[Obsolete("Use the same method on the ControlHandler.Instance instead")]
		public static string GetClientValidatedPostback(Control control, string validationGroup) {
			return Instance.GetClientValidatedPostback(control, validationGroup);
		}

		internal static UI.RegisterTargetPreference GetNewTargetPreference(RegisterTargetPreference targetPreference) {
			if(targetPreference == RegisterTargetPreference.Head) {
				return UI.RegisterTargetPreference.Head;
			}
			if(targetPreference == RegisterTargetPreference.Page) {
				return UI.RegisterTargetPreference.Page;
			}
			if(targetPreference == RegisterTargetPreference.ScriptHolder) {
				return UI.RegisterTargetPreference.ScriptHolder;
			}
			return UI.RegisterTargetPreference.Auto;
		}

		internal static RegisterTargetPreference GetOldTargetPreference(UI.RegisterTargetPreference targetPreference) {
			if(targetPreference == UI.RegisterTargetPreference.Head) {
				return RegisterTargetPreference.Head;
			}
			if(targetPreference == UI.RegisterTargetPreference.Page) {
				return RegisterTargetPreference.Page;
			}
			if(targetPreference == UI.RegisterTargetPreference.ScriptHolder) {
				return RegisterTargetPreference.ScriptHolder;
			}
			return RegisterTargetPreference.Auto;
		}
		#endregion
	
	}
}
