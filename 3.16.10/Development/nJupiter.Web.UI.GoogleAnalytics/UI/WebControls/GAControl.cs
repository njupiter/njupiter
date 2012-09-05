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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
[assembly: System.Web.UI.WebResource("nJupiter.Web.UI.GoogleAnalytics.GALinks.js", "text/js", PerformSubstitution = true)]
namespace nJupiter.Web.UI.GoogleAnalytics.UI.WebControls {
	[ToolboxData("<{0}:GAControl runat=server></{0}:GAControl>")]
	public class GAControl : WebControl {

		#region Fields

		private string accountNumber;
		private Transaction transaction;
		private string campaignSource = string.Empty;
		private string campaignMedium = string.Empty;
		private string campaignTerm = string.Empty;
		private string campaignContent = string.Empty;
		private string campaignName = string.Empty;
		private string campaignSourceQs = string.Empty;
		private string campaignMediumQs = string.Empty;
		private string campaignTermQs = string.Empty;
		private string campaignContentQs = string.Empty;
		private string campaignNameQs = string.Empty;
		private string urchinSource = "//www.google-analytics.com/ga.js";

		#endregion

		#region Public Properties

		/// <summary>
		/// Account ID from the tracking code, in the format UA-xxxx-x
		/// </summary>
		/// <value>The account number.</value>
		[Bindable(true)]
		public string AccountNumber {
			get { return this.accountNumber; }
			set { this.accountNumber = value; }
		}

		/// <summary>
		/// Location of JavaScript file for Google Analytics, can be left default. Omitting the protocol (HTTP) means the browser will default to the host page protocol.
		/// </summary>
		/// <value>The urchin source.</value>
		[DefaultValue("//www.google-analytics.com/ga.js")]
		public string UrchinSource {
			get { return this.urchinSource; }
			set { this.urchinSource = value; }
		}

		#endregion

		/// <summary>
		/// Encode strings for JavaScript
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns></returns>
		private static string Enc(string s) {
			if(string.IsNullOrEmpty(s)) {
				return string.Empty;
			}
			int i;
			int len = s.Length;
			StringBuilder sb = new StringBuilder(len + 4);
			string t;

			for(i = 0; i < len; i += 1) {
				char c = s[i];
				switch(c) {
					case '>':
					case '\'':
					case '"':
					case '\\':
					sb.Append('\\');
					sb.Append(c);
					break;
					case '\b':
					sb.Append("\\b");
					break;
					case '\t':
					sb.Append("\\t");
					break;
					case '\n':
					sb.Append("\\n");
					break;
					case '\f':
					sb.Append("\\f");
					break;
					case '\r':
					sb.Append("\\r");
					break;
					default:
					if(c < ' ') {
						//t = "000" + Integer.toHexString(c); 
						string tmp = new string(c, 1);
						t = "000" + int.Parse(tmp, System.Globalization.NumberStyles.HexNumber);
						sb.Append("\\u" + t.Substring(t.Length - 4));
					} else {
						sb.Append(c);
					}
					break;
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the control content.</param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer) {
			// load GA JS
			writer.Write(string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>\r\n", HttpUtility.HtmlEncode(this.urchinSource)));
			// load Control JS
			writer.Write(string.Format("<script src=\"{0}\" type=\"text/javascript\"></script>\r\n", Page.ClientScript.GetWebResourceUrl(typeof(GAControl), "nJupiter.Web.UI.GoogleAnalytics.GALinks.js")));

			writer.Write("<script type=\"text/javascript\">\r\n//<!--\r\n");

			// set account ID
			writer.Write(string.Format("_uacct=\"{0}\";_udn='none';\r\n", Enc(this.accountNumber)));

			// set campaign source properties
			if(this.campaignSource != string.Empty && this.campaignMedium != string.Empty && this.campaignName != string.Empty) {
				string js = string.Format("GAControl_SetCampaign({0},{1},{2},{3},{4});\r\n",
					Enc(this.campaignSource), Enc(this.campaignMedium), Enc(this.campaignTerm), Enc(this.campaignContent), Enc(this.campaignName));
				writer.Write(js);
			}

			// initialise the tracker
			writer.Write("urchinTracker();\r\n");

			// trigger extra page hits
			foreach(string loc in this.extraLocations) {
				writer.Write(string.Format("urchinTracker('{0}');\r\n", Enc(loc)));
			}

			// pass cookie on links to other domains
			if(this.extraDomains.Count > 0) {
				writer.Write(string.Format("_udn='none';_ulink=1;GAControl_AltSites(Array('{0}'));\r\n", String.Join("','", this.extraDomains.ToArray())));
			}

			if(this.transaction != null) {
				writer.Write(string.Format("GAControl_SetTrans('{0}');\r\n", Enc(this.transaction.GetLines())));
			}

			writer.Write("//-->\r\n</script>");
		}


		/// <summary>
		/// E-commerce transaction details, GA must be in e-commerce tracking mode.
		/// </summary>
		/// <value>The transaction.</value>
		public Transaction Transaction {
			get { return this.transaction; }
			set { this.transaction = value; }
		}


		#region Alternative Domains

		private readonly List<String> extraDomains = new List<string>();
		/// <summary>
		/// Add other URLs that are part of this site. Users cookies will be passed across in links and forms so the tracking session can carry across domains. The other site must be using the same GA account.
		/// </summary>
		/// <param name="url">The URL.</param>
		public void AddAlternativeUrl(string url) {
			this.extraDomains.Add(Enc(url));
		}

		#endregion

		#region Arbitary Locations

		private readonly List<String> extraLocations = new List<string>();
		/// <summary>
		/// Add locations to be saved as page hits, can be used to track events for conversion goals
		/// </summary>
		/// <param name="location">Absolute path of location, eg. /event/download/</param>
		public void AddLocation(string location) {
			this.extraLocations.Add(Enc(location));
		}

		#endregion

		#region Arbitary Campaign Sources

		/// <summary>
		/// Set campaign source values.
		/// </summary>
		/// <param name="campaignName">To identify a specific product promotion or strategic campaign.</param>
		/// <param name="campaignSource">To identify a search engine, newsletter name, or other source.</param>
		/// <param name="campaignMedium">To identify a medium such as email or cost-per-click.</param>
		/// <param name="campaignTerm">To note the keywords for this ad.</param>
		/// <param name="campaignContent">To differentiate ads or links that point to the same URL.</param>
		public void SetSource(string campaignName, string campaignSource, string campaignMedium, string campaignTerm, string campaignContent) {
			this.campaignSource = campaignSource;
			this.campaignMedium = campaignMedium;
			this.campaignTerm = campaignTerm;
			this.campaignContent = campaignContent;
			this.campaignName = campaignName;
		}

		/// <summary>
		/// Sets the source.
		/// </summary>
		/// <param name="campaignName">To identify a specific product promotion or strategic campaign.</param>
		/// <param name="campaignSource">To identify a search engine, newsletter name, or other source.</param>
		/// <param name="campaignMedium">To identify a medium such as email or cost-per- click.</param>
		/// <param name="campaignTerm">To note the keywords for this ad.</param>
		public void SetSource(string campaignName, string campaignSource, string campaignMedium, string campaignTerm) {
			SetSource(campaignName, campaignSource, campaignMedium, campaignTerm, "");
		}

		/// <summary>
		/// Sets the source.
		/// </summary>
		/// <param name="campaignName">To identify a specific product promotion or strategic campaign.</param>
		/// <param name="campaignSource">To identify a search engine, newsletter name, or other source.</param>
		/// <param name="campaignMedium">To identify a medium such as email or cost-per- click.</param>
		public void SetSource(string campaignName, string campaignSource, string campaignMedium) {
			SetSource(campaignName, campaignSource, campaignMedium, "", "");
		}

		/// <summary>
		/// To identify a specific product promotion or strategic campaign.
		/// </summary>
		/// <value>The name of the campaign.</value>
		public string CampaignName {
			get { return this.campaignName; }
			set { this.campaignName = value; }
		}

		/// <summary>
		/// To differentiate ads or links that point to the same URL.
		/// </summary>
		/// <value>The content of the campaign.</value>
		public string CampaignContent {
			get { return this.campaignContent; }
			set { this.campaignContent = value; }
		}

		/// <summary>
		/// To note the keywords for this ad.
		/// </summary>
		/// <value>The campaign term.</value>
		public string CampaignTerm {
			get { return this.campaignTerm; }
			set { this.campaignTerm = value; }
		}

		/// <summary>
		/// To identify a medium such as email or cost-per-click.
		/// </summary>
		/// <value>The campaign medium.</value>
		public string CampaignMedium {
			get { return this.campaignMedium; }
			set { this.campaignMedium = value; }
		}

		/// <summary>
		/// To identify a search engine, newsletter name, or other source.
		/// </summary>
		/// <value>The campaign source.</value>
		public string CampaignSource {
			get { return this.campaignSource; }
			set { this.campaignSource = value; }
		}

		#endregion

		#region Campaign Source Mapping

		/// <summary>
		/// Querystring item name to identify a specific product promotion or strategic campaign.
		/// </summary>
		/// <value>The name of the map campaign.</value>
		public string MapCampaignName {
			get { return this.campaignNameQs; }
			set { this.campaignNameQs = value; }
		}

		/// <summary>
		/// Querystring item name to differentiate ads or links that point to the same URL.
		/// </summary>
		/// <value>The content of the map campaign.</value>
		public string MapCampaignContent {
			get { return this.campaignContentQs; }
			set { this.campaignContentQs = value; }
		}

		/// <summary>
		/// Querystring item name to note the keywords for this ad.
		/// </summary>
		/// <value>The map campaign term.</value>
		public string MapCampaignTerm {
			get { return this.campaignTermQs; }
			set { this.campaignTermQs = value; }
		}

		/// <summary>
		/// Querystring item name to identify a medium such as email or cost-per-click.
		/// </summary>
		/// <value>The map campaign medium.</value>
		public string MapCampaignMedium {
			get { return this.campaignMediumQs; }
			set { this.campaignMediumQs = value; }
		}

		/// <summary>
		/// Querystring item name to identify a search engine, newsletter name, or other source.
		/// </summary>
		/// <value>The map campaign source.</value>
		public string MapCampaignSource {
			get { return this.campaignSourceQs; }
			set { this.campaignSourceQs = value; }
		}


		#endregion
	}
}
