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
using System.IO;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;
using System.ComponentModel;

namespace nJupiter.Web.UI.Controls {

	[ToolboxItem(true)]
	[Obsolete("Try using System.Web.UI.HtmlControls.HtmlForm together with nJupiter.Web.UI.ControlAdapters.HtmlFormAdapter (or ASP.NET 4)")]
	public class WebForm : HtmlForm {

		private const string XhtmlStrictRendering = "v_XHTMLStrictRendering";

		public bool XHTMLStrictRendering {
			get {
				if(this.ViewState[XhtmlStrictRendering] == null)
					return true;
				return (bool)this.ViewState[XhtmlStrictRendering];
			}
			set {
				this.ViewState[XhtmlStrictRendering] = value;
			}
		}
		public string CssClass {
			get {
				var result = this.ViewState[HtmlAttribute.Class] as string;
				if(result != null)
					return result;
				return string.Empty;
			}
			set {
				this.ViewState[HtmlAttribute.Class] = value;
			}
		}

		protected override void RenderAttributes(HtmlTextWriter writer) {
			if(this.CssClass.Length > 0)
				this.Attributes.Add(HtmlAttribute.Class, this.CssClass);

			base.RenderAttributes(writer);
		}
		protected override void Render(HtmlTextWriter writer) {
			if(this.XHTMLStrictRendering && writer.GetType() == typeof(HtmlTextWriter)) {
				var w = new StrictHtmlTextWriter(writer);
				base.Render(w);
			} else {
				base.Render(writer);
			}
		}
		protected override ControlAdapter ResolveAdapter() {
			if(this.XHTMLStrictRendering) {
				return null;
			}
			return base.ResolveAdapter();
		}
	}

	public class StrictHtmlTextWriter : HtmlTextWriter {
		public StrictHtmlTextWriter(TextWriter writer)
			: this(writer, DefaultTabString) {
		}

		public StrictHtmlTextWriter(TextWriter writer, string tabString)
			: base(writer, tabString) {
		}

		private const string InputBeginning = "<input type=\"hidden\" name=\"";

		private bool openInput;
		private bool removeNext;
		private IdnMapping idnMapping;

		private IdnMapping IdnMapping {
			get { return this.idnMapping ?? (this.idnMapping = new IdnMapping()); }
		}

		public override bool IsValidFormAttribute(string attribute) {
			if(attribute == null) {
				throw new ArgumentNullException("attribute");
			}
			if(attribute.Equals("name"))
				return false;
			return base.IsValidFormAttribute(attribute);
		}

		public override void AddAttribute(HtmlTextWriterAttribute key, string value) {
			switch(key) {
				case HtmlTextWriterAttribute.Href:
				case HtmlTextWriterAttribute.Src:
					try {
					Uri uri;
					if(Uri.TryCreate(value, UriKind.Absolute, out uri)) {
						//if an absolute valid uri, then convert to ascii (renders navigatable international domain names)
						if(!string.IsNullOrEmpty(uri.Host)) {
							var newUri = new UriBuilder(uri);
							newUri.Host = this.IdnMapping.GetAscii(uri.Host);
							value = newUri.Uri.AbsoluteUri;
						}
					}
				} catch(UriFormatException) { }
				break;
			}
			base.AddAttribute(key, value);
		}


		public override void Write(string s) {
			s = ProcessString(s);
			if(s.Length > 0) {
				base.Write(s);
			}
		}

		public override void WriteLine(string s) {
			s = ProcessString(s);
			if(s.Length > 0) {
				base.Write(s);
			}
		}

		// Removes id-attribute from hidden input elements.
		// Ugly as hell but when Microsoft close down every god damn
		// method and make them internal or private this is the only
		// choice. The best would have been if Microsoft did it the
		// right way from the beginning. But what shall we expect from
		// a company that close every damn thing down and think security
		// is equivalent with obscurity... Viva open source!
		private string ProcessString(string s) {
			const string correctScriptStart = "\r\n<script type=\"text/javascript\">\r\n<!--//--><![CDATA[//><!--\r\n";
			const string correctScriptEnd = "//--><!]]>\r\n</script>\r\n";
			// Before .NET 2.50727.832
			const string wrongScriptStartOld = "\r\n<script type=\"text/javascript\">\r\n<!--\r\n";
			const string wrongScriptEndOld = "// -->\r\n</script>\r\n";
			// After .NET 2.50727.832
			const string wrongScriptStart = "\r\n<script type=\"text/javascript\">\r\n//<![CDATA[\r\n";
			const string wrongScriptEnd = "//]]>\r\n</script>\r\n";


			if(s == null)
				return string.Empty;
			if(s.Equals(wrongScriptStart) || s.Equals(wrongScriptStartOld)) {
				return correctScriptStart;
			}

			if(s.Equals(wrongScriptEnd) || s.Equals(wrongScriptEndOld)) {
				return correctScriptEnd;
			}

			if(this.removeNext) {
				if(s.StartsWith("\"")) {
					s = s.Substring("\"".Length);
					this.removeNext = false;
				} else {
					return string.Empty;
				}
			}
			if(this.openInput && s.Equals("\" id=\"")) {
				this.removeNext = true;
				this.openInput = false;
				return "\"";
			}
			if(s.EndsWith(InputBeginning)) {
				this.openInput = true;
			}
			return s;
		}
	}
}