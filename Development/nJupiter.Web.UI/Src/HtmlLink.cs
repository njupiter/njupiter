using System;

namespace nJupiter.Web.UI {
	public class HtmlLink {
		#region Members
		private readonly string url;
		private readonly string text;
		#endregion

		#region Constructors
		public HtmlLink(string url, string text) {
			if(url == null) {
				throw new ArgumentNullException("url");
			}
			if(text == null) {
				throw new ArgumentNullException("text");
			}
			this.url = url;
			this.text = text;
		}
		#endregion

		#region Properties
		public string Url { get { return this.url; } }
		public string Text { get { return this.text; } }
		#endregion
	}
}
