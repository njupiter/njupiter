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
using System.IO;
using System.Reflection;

[assembly: System.Web.UI.WebResource("GAScript.js", "text/js")]
namespace nJupiter.Web.UI.GoogleAnalytics {
	public class GoogleAnalyticsHandler {
		#region Fields

		private bool registerGaScript;
		private string uaCode = string.Empty;
		private const string GoogleAnalyticsScriptResource = "nJupiter.Web.UI.GoogleAnalytics.GAScript.js";
		private Page currentPage;

		#endregion

		#region Private Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleAnalyticsHandler"/> class.
		/// </summary>
		public GoogleAnalyticsHandler() {
			if(CurrentPage != null)
				CurrentPage.PreRender += this.CurrentPagePreRender;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles the PreRender event of the currentPage control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void CurrentPagePreRender(object sender, EventArgs e) {
			if(!this.registerGaScript)
				return;

			string script = string.Format(Script, UACode);

			CurrentPage.ClientScript.RegisterStartupScript(typeof(GoogleAnalyticsHandler), typeof(GoogleAnalyticsHandler).Name, script);

			// TODO: Implement support for transactions
		}



		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the UA code.
		/// </summary>
		/// <value>The UA code.</value>
		public string UACode {
			get { return this.uaCode; }
			set { this.uaCode = value; }
		}

		/// <summary>
		/// Gets or sets the current page.
		/// </summary>
		/// <value>The current page.</value>
		public Page CurrentPage {
			get {
				if(this.currentPage == null && System.Web.HttpContext.Current.Handler is Page)
					this.currentPage = System.Web.HttpContext.Current.Handler as Page;
				return this.currentPage;
			}
		}

		/// <summary>
		/// Gets or sets the script.
		/// </summary>
		/// <value>The script.</value>
		private static string Script {
			get {
				if(System.Web.HttpContext.Current.Cache[GoogleAnalyticsScriptResource] == null)
					System.Web.HttpContext.Current.Cache[GoogleAnalyticsScriptResource] = GetScriptResource();

				return System.Web.HttpContext.Current.Cache[GoogleAnalyticsScriptResource] as string;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Registers the client script.
		/// </summary>
		public void RegisterClientScript() {
			this.registerGaScript = true;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the script resource.
		/// </summary>
		/// <returns></returns>
		private static string GetScriptResource() {
			Assembly assembly = Assembly.GetExecutingAssembly();

			Stream stream = assembly != null ? assembly.GetManifestResourceStream(GoogleAnalyticsScriptResource) : null;
			if(stream != null) {
				StreamReader streamReader = new StreamReader(stream);
				return streamReader.ReadToEnd();
			}
			return string.Empty;
		}

		#endregion
	}
}
