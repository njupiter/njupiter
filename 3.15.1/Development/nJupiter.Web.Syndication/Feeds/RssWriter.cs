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
using System.IO;
using System.Globalization;

namespace nJupiter.Web.Syndication {

	internal class RssWriter : FeedWriter {

		public RssWriter(Stream stream, FeedType type) : base(stream, type) {}

		#region Methods
		private void WriteHeader(IFeed feed) {
			this.XmlWriter.WriteStartDocument();
			this.XmlWriter.WriteStartElement("rss");
			this.XmlWriter.WriteAttributeString("version", "2.0");
			this.XmlWriter.WriteAttributeString("xmlns", "atom", null, "http://www.w3.org/2005/Atom");
		
			this.XmlWriter.WriteStartElement("channel");

			this.XmlWriter.WriteElementString("title", feed.Title);
			this.XmlWriter.WriteElementString("link", feed.Uri.ToString());

			this.XmlWriter.WriteStartElement("atom","link", null);
			this.XmlWriter.WriteAttributeString("href", feed.FeedUri.ToString());
			this.XmlWriter.WriteAttributeString("rel", "self");
			this.XmlWriter.WriteAttributeString("type", "application/rss+xml");
			this.XmlWriter.WriteEndElement();

			this.XmlWriter.WriteElementString("description", feed.Description);
			if(!feed.PublishDate.Equals(DateTime.MinValue)) {
				this.XmlWriter.WriteElementString("pubDate", feed.PublishDate.ToUniversalTime().ToString("r", DateTimeFormatInfo.InvariantInfo));
			}
			if(!string.IsNullOrEmpty(feed.Language)) {
				this.XmlWriter.WriteElementString("language", feed.Language);
			}
			if(feed.Image != null) {
				this.XmlWriter.WriteStartElement("image");
				this.XmlWriter.WriteElementString("url", feed.Image.Uri.ToString());
				this.XmlWriter.WriteElementString("title", feed.Title);
				this.XmlWriter.WriteElementString("link", feed.Uri.ToString());
				if(!string.IsNullOrEmpty(feed.Image.Description)) {
					this.XmlWriter.WriteElementString("description", feed.Image.Description);
				}
				if(!feed.Image.Size.IsEmpty) {
					int width = feed.Image.Size.Width <= 144 ? feed.Image.Size.Width : 144;
					int height =feed.Image.Size.Height <= 400 ? feed.Image.Size.Height : 400;
					this.XmlWriter.WriteElementString("width", width.ToString(NumberFormatInfo.InvariantInfo));
					this.XmlWriter.WriteElementString("height", height.ToString(NumberFormatInfo.InvariantInfo));
				}
				this.XmlWriter.WriteEndElement();
			}
		}

		private void WriteFooter() {
			this.XmlWriter.WriteEndDocument();
		}


		private void WriteFeedItem(IFeedItem item) {
			this.XmlWriter.WriteStartElement("item");
			if(!string.IsNullOrEmpty(item.Title)) {
				this.XmlWriter.WriteElementString("title", item.Title);
			}
			if(item.Uri != null) {
				this.XmlWriter.WriteElementString("link", item.Uri.ToString());
				this.XmlWriter.WriteStartElement("guid");
				this.XmlWriter.WriteAttributeString("isPermaLink", "true");
				this.XmlWriter.WriteString(item.Uri.ToString());
				this.XmlWriter.WriteEndElement();
			}
			if(!string.IsNullOrEmpty(item.Description)) {
				this.XmlWriter.WriteElementString("description", item.Description);
			}
			if(!item.PublishDate.Equals(DateTime.MinValue)) {
				DateTime universalPublishDate = item.PublishDate.ToUniversalTime();
				this.XmlWriter.WriteElementString("pubDate", universalPublishDate.ToString("r", DateTimeFormatInfo.InvariantInfo));
			}
			if(item.Author != null) {
				string author = !string.IsNullOrEmpty(item.Author.Email) ? string.Format("{0} ({1})", item.Author.Email, item.Author.Name) : item.Author.Name;
				this.XmlWriter.WriteElementString("author", author);
			}
			this.XmlWriter.WriteEndElement();
		}

		private void WriteLastUpdated(DateTime lastUpdated) {
			if(!lastUpdated.Equals(DateTime.MinValue)) {
				this.XmlWriter.WriteElementString("lastBuildDate", lastUpdated.ToString("r", DateTimeFormatInfo.InvariantInfo));
			}
		}

		public override void Write(IFeed feed) {
			if(this.XmlWriter != null) {
				this.WriteHeader(feed);
				DateTime lastBuildDate = feed.PublishDate.ToUniversalTime();
				if(feed.Items != null) {
					foreach(IFeedItem item in feed.Items) {
						DateTime universalPublishDate = item.PublishDate.ToUniversalTime();
						if(universalPublishDate > lastBuildDate) {
							lastBuildDate = universalPublishDate;
						}
					}
				}
				this.WriteLastUpdated(lastBuildDate);
				if(feed.Items != null) {
					foreach(IFeedItem item in feed.Items) {
						WriteFeedItem(item);
					}
				}
				this.WriteFooter();
			}
		}
		#endregion

	}

}