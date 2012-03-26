#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Globalization;
using System.Web.UI.HtmlControls;

using nJupiter.Web.UI.ControlAdapters;
using nJupiter.Web.UI.Controls;

namespace nJupiter.Web.UI {

	public static class ControlHandler {
		#region Enums
		public enum RegisterTargetPreference {
			Auto,
			Head,
			ScriptHolder,
			Page
		}
		#endregion

		#region Constructors
		#endregion

		#region Properties
		public static bool IsIE { get { return HttpContext.Current.Request.UserAgent != null && HttpContext.Current.Request.UserAgent.IndexOf("MSIE") > 0; } }

		public static bool IsPreIE7 {
			get {
				if(HttpContext.Current.Request.UserAgent != null) {
					int i = HttpContext.Current.Request.UserAgent.IndexOf("MSIE");
					if(i > 0) {
						i = i + 5;
						if(HttpContext.Current.Request.UserAgent.Length > i) {
							string versionString = HttpContext.Current.Request.UserAgent.Substring(i, 1);
							try {
								int version = int.Parse(versionString, NumberFormatInfo.InvariantInfo);
								if(version < 7) {
									return true;
								}
							} catch(FormatException) { }
						}
					}
				}
				return false;
			}
		}
		#endregion

		#region Methods
		public static Control FindControl(Control rootControl, string uniqueId) {
			return FindControl(rootControl, uniqueId, false);
		}
		public static Control FindControl(Control rootControl, string uniqueId, bool ensureChildControls) {
			return FindControlsRecursive(rootControl, uniqueId, ensureChildControls);
		}
		public static Control[] FindControlsOnType(Control rootControl, Type type) {
			return FindControlsOnType(rootControl, type, false);
		}
		public static Control[] FindControlsOnType(Control rootControl, Type type, bool ensureChildControls) {
			ArrayList container = FindControlsOnTypeRecursive(rootControl, type, new ArrayList(), false, ensureChildControls);
			return (Control[])container.ToArray(type);
		}
		public static Control FindFirstControlOnType(Control rootControl, Type type) {
			return FindFirstControlOnType(rootControl, type, false);
		}
		public static Control FindFirstControlOnType(Control rootControl, Type type, bool ensureChildControls) {
			ArrayList container = FindControlsOnTypeRecursive(rootControl, type, new ArrayList(), true, ensureChildControls);
			if(container.Count > 0) {
				return (Control)container[0];
			}
			return null;
		}

		public static void RegisterClientScriptBlock(Type type, string key, string script, RegisterTargetPreference targetPreference) {
			if(HttpContext.Current != null) {
				switch(targetPreference) {
					case RegisterTargetPreference.Auto:
					case RegisterTargetPreference.ScriptHolder:
					WebScriptHolder webScriptHolder = HttpContext.Current.Items[typeof(WebScriptHolder)] as WebScriptHolder;
					if(webScriptHolder != null) {
						webScriptHolder.RegisterClientScriptBlock(type, key, script);
						break;
					}
					goto case RegisterTargetPreference.Head;
					case RegisterTargetPreference.Head:
					HtmlHeadAdapter htmlHeadAdapter = HttpContext.Current.Items[typeof(HtmlHeadAdapter)] as HtmlHeadAdapter;
					if(htmlHeadAdapter != null) {
						htmlHeadAdapter.RegisterClientScriptBlock(type, key, script);
						break;
					}
					WebHead webHead = HttpContext.Current.Items[typeof(WebHead)] as WebHead;
					if(webHead != null) {
						webHead.RegisterClientScriptBlock(type, key, script);
						break;
					}
					goto case RegisterTargetPreference.Page;
					case RegisterTargetPreference.Page:
					Page page = HttpContext.Current.CurrentHandler as Page;
					if(page != null)
						page.ClientScript.RegisterClientScriptBlock(type, key, script);
					break;
				}
			}
		}
		public static void RegisterClientScriptBlock(Type type, string key, string script) {
			RegisterClientScriptBlock(type, key, script, RegisterTargetPreference.Auto);
		}
		public static void RegisterClientScriptInclude(Type type, string key, string url, RegisterTargetPreference targetPreference) {
			RegisterClientScriptBlock(type, key, string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\" src=\"{0}\"></script>", HttpUtility.HtmlAttributeEncode(url)), targetPreference);
		}
		public static void RegisterClientScriptInclude(Type type, string key, string url) {
			RegisterClientScriptInclude(type, key, url, RegisterTargetPreference.Auto);
		}
		public static void RegisterClientScriptResource(Type type, string resourceName, RegisterTargetPreference targetPreference) {
			Page page = HttpContext.Current.CurrentHandler as Page;
			if(page != null) {
				string url = page.ClientScript.GetWebResourceUrl(type, resourceName);
				RegisterClientScriptInclude(type, resourceName, url, targetPreference);
			}
		}
		public static void RegisterClientScriptResource(Type type, string resourceName) {
			RegisterClientScriptResource(type, resourceName, RegisterTargetPreference.Auto);
		}
		#endregion

		#region Helper Methods
		private static ArrayList FindControlsOnTypeRecursive(Control rootControl, Type type, ArrayList container, bool breakOnHit, bool ensureChildControls) {
			if(ensureChildControls) {
				rootControl.FindControl(string.Empty);
				// The idiots at microsoft have made some vital metods protected so we have to do this miserable hack to ensure that child controls are built
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
		private static Control FindControlsRecursive(Control rootControl, string uniqueId, bool ensureChildControls) {
			if(ensureChildControls) {
				rootControl.FindControl(string.Empty);
				// The idiots at microsoft have made some vital metods protected so we have to do this miserable hack to ensure that child controls are built
			}
			foreach(Control control in rootControl.Controls) {
				if(control.UniqueID == uniqueId) {
					return control;
				}
				if(control.Controls.Count > 0) {
					Control innerControl = FindControlsRecursive(control, uniqueId, ensureChildControls);
					if(innerControl != null) {
						return innerControl;
					}
				}
			}
			return null;
		}

		public static void WriteOnClickAttribute(HtmlTextWriter writer, HtmlControl control, bool submitsAutomatically, bool submitsProgramatically, bool causesValidation, string validationGroup) {
			System.Web.UI.AttributeCollection attributes = control.Attributes;
			string clientValidateEvent = null;
			if(submitsAutomatically) {
				if(causesValidation) {
					clientValidateEvent = GetClientValidateEvent(validationGroup);
				}
				control.Page.ClientScript.RegisterForEventValidation(control.UniqueID);
			} else if(submitsProgramatically) {
				if(causesValidation) {
					clientValidateEvent = GetClientValidatedPostback(control, validationGroup);
				} else {
					clientValidateEvent = control.Page.ClientScript.GetPostBackEventReference(control, string.Empty, true);
				}
			} else {
				control.Page.ClientScript.RegisterForEventValidation(control.UniqueID);
			}
			if(clientValidateEvent != null) {
				if(attributes["language"] != null) {
					attributes.Remove("language");
				}
				writer.WriteAttribute("language", "javascript");
				string onClick = attributes["onclick"];
				if(onClick != null) {
					attributes.Remove("onclick");
					writer.WriteAttribute("onclick", onClick + " " + clientValidateEvent);
				} else {
					writer.WriteAttribute("onclick", clientValidateEvent);
				}
			}
		}

		public static string GetClientValidateEvent(string validationGroup) {
			if(validationGroup == null) {
				validationGroup = string.Empty;
			}
			return ("if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate('" + validationGroup + "'); ");
		}

		public static string GetClientValidatedPostback(Control control, string validationGroup) {
			return GetClientValidatedPostback(control, validationGroup, string.Empty);
		}

		public static string GetClientValidatedPostback(Control control, string validationGroup, string argument) {
			string postBackEventReference = control.Page.ClientScript.GetPostBackEventReference(control, argument, true);
			return (GetClientValidateEvent(validationGroup) + postBackEventReference);
		}
		#endregion
	}

}
