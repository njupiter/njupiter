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

using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace nJupiter.Web.UI {
	public class ControlHandlerImpl : IControlHandler {

		public void WriteOnClickAttribute(HtmlTextWriter writer, HtmlControl control, bool submitsAutomatically, bool submitsProgramatically, bool causesValidation, string validationGroup) {
			var attributes = control.Attributes;
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
				var onClick = attributes["onclick"];
				if(onClick != null) {
					attributes.Remove("onclick");
					writer.WriteAttribute("onclick", string.Format("{0} {1}", onClick, clientValidateEvent));
				} else {
					writer.WriteAttribute("onclick", clientValidateEvent);
				}
			}
		}

		public string GetClientValidateEvent(string validationGroup) {
			return string.Format("if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate('{0}'); ", validationGroup);
		}

		public string GetClientValidatedPostback(Control control, string validationGroup) {
			return GetClientValidatedPostback(control, validationGroup, string.Empty);
		}

		public string GetClientValidatedPostback(Control control, string validationGroup, string argument) {
			var postBackEventReference = control.Page.ClientScript.GetPostBackEventReference(control, argument, true);
			return (GetClientValidateEvent(validationGroup) + postBackEventReference);
		}
	}

}
