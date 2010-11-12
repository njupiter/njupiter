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
using System.Globalization;
using System.IO;

namespace nJupiter.Web.Syndication {

	internal class AtomWriter : FeedWriter {

		public AtomWriter(Stream stream, FeedType feedType) : base(stream, feedType) { }

		#region Methods
		private void WriteHeader(IFeed feed) {
			this.XmlWriter.WriteStartDocument();
			this.XmlWriter.WriteStartElement("feed", "http://www.w3.org/2005/Atom");
			if(!string.IsNullOrEmpty(feed.Language)) {
				this.XmlWriter.WriteAttributeString("xml", "lang", null, feed.Language);
			}
			this.XmlWriter.WriteElementString("id", feed.Id);
			this.XmlWriter.WriteElementString("title", feed.Title);

			this.XmlWriter.WriteStartElement("link");
			this.XmlWriter.WriteAttributeString("href", feed.Uri.AbsoluteUri);
			this.XmlWriter.WriteEndElement();

			this.XmlWriter.WriteStartElement("link");
			this.XmlWriter.WriteAttributeString("href", feed.FeedUri.AbsoluteUri);
			this.XmlWriter.WriteAttributeString("rel", "self");
			this.XmlWriter.WriteEndElement();

			if(feed.Author != null) {
				this.XmlWriter.WriteStartElement("author");
				this.XmlWriter.WriteElementString("name", feed.Author.Name);
				if(feed.Author.Email != null) {
					this.XmlWriter.WriteElementString("email", feed.Author.Email);
				}
				this.XmlWriter.WriteEndElement();
			}

			this.XmlWriter.WriteElementString("subtitle", feed.Description);

			if(feed.Image != null) {
				this.XmlWriter.WriteElementString("logo", feed.Image.Uri.ToString());
			}
		}

		private void WriteFooter() {
			this.XmlWriter.WriteEndDocument();
		}


		private void WriteFeedItem(IFeedItem item) {
			this.XmlWriter.WriteStartElement("entry");
			this.XmlWriter.WriteElementString("id", item.Id);
			if(!string.IsNullOrEmpty(item.Title)) {
				this.XmlWriter.WriteElementString("title", item.Title);
			}
			if(item.Uri != null) {
				this.XmlWriter.WriteStartElement("link");
				this.XmlWriter.WriteAttributeString("href", item.Uri.AbsoluteUri);
				this.XmlWriter.WriteEndElement();
			}
			if(item.Author != null) {
				this.XmlWriter.WriteStartElement("author");
				this.XmlWriter.WriteElementString("name", item.Author.Name);
				if(item.Author.Email != null) {
					this.XmlWriter.WriteElementString("email", item.Author.Email);
				}
				this.XmlWriter.WriteEndElement();
			}
			string summary = !string.IsNullOrEmpty(item.Description) ? item.Description : item.Title;
			if(ContainsHtml(summary)) {
				this.XmlWriter.WriteStartElement("summary");
				this.XmlWriter.WriteAttributeString("type", "html");
				this.XmlWriter.WriteCData(summary);
				this.XmlWriter.WriteEndElement();
			} else {
				this.XmlWriter.WriteElementString("summary", summary);
			}
			if(!item.PublishDate.Equals(DateTime.MinValue)) {
				DateTime universalPublishDate = item.PublishDate.ToUniversalTime();
				this.XmlWriter.WriteElementString("updated", universalPublishDate.ToString("o", DateTimeFormatInfo.InvariantInfo));
			}
			this.XmlWriter.WriteEndElement();
		}

		private void WriteLastUpdated(DateTime lastUpdated) {
			if(!lastUpdated.Equals(DateTime.MinValue)) {
				this.XmlWriter.WriteElementString("updated", lastUpdated.ToString("o", DateTimeFormatInfo.InvariantInfo));
			}
		}

		private static bool ContainsHtml(string text) {
			if(string.IsNullOrEmpty(text)) {
				return false;
			}
			//HACK: This can probably be done in a much better way
			text = text.Trim();
			return text.Contains("<") && (text.Contains("/>") || text.Contains("</"));
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
