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

using System.Web;

using nJupiter.Web.UI;

namespace nJupiter.Services.Forum.UI {

	public class WebTextFormatter : TextFormatter {
		#region Constants
		private const bool DefaultAutoHyperlink = true;
		private const bool DefaultHtmlEncode = true;
		private const bool DefaultConvertNewLinesToBr = true;
		#endregion

		#region Variables
		private bool autoHyperlink = DefaultAutoHyperlink;
		private bool htmlEncode = DefaultHtmlEncode;
		private bool convertNewLinesToBr = DefaultConvertNewLinesToBr;

		#endregion

		#region Properties
		public bool AutoHyperlink { get { return this.autoHyperlink; } set { this.autoHyperlink = value; } }
		public bool HtmlEncode { get { return this.htmlEncode; } set { this.htmlEncode = value; } }
		public bool ConvertNewLinesToBr { get { return this.convertNewLinesToBr; } set { this.convertNewLinesToBr = value; } }
		#endregion

		#region Constructors
		public WebTextFormatter() { }
		public WebTextFormatter(bool autoHyperlink, bool convertNewLinesToBr, bool htmlEncode) {
			this.autoHyperlink = autoHyperlink;
			this.convertNewLinesToBr = convertNewLinesToBr;
			this.htmlEncode = htmlEncode;
		}
		#endregion

		#region TextFormatter Implementation
		public string FormatText(string text) {
			string output = text;
			if(this.HtmlEncode) {
				output = HttpUtility.HtmlEncode(output);
			}
			if(this.ConvertNewLinesToBr) {
				output = HtmlHandler.ConvertNewLinesToBr(output);
			}
			if(this.AutoHyperlink) {
				output = HtmlHandler.AutoHyperlinkText(output, true);
			}
			return output;
		}
		#endregion
	}

}
