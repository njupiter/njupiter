using System;
using System.Collections.Specialized;

namespace nJupiter.Web.UI {
	public class HtmlLink {
		#region Members
		private readonly string url;
		private readonly string text;
		private NameValueCollection attributes;
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
		public NameValueCollection Attributes { 
			get {
				if(this.attributes == null) {
					this.attributes = new NameValueCollection();
				}
				return this.attributes; 
			} 
		}
		#endregion
	}
}
