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
using System.Xml;

namespace nJupiter.Web.Syndication {

	public sealed class Feed : IFeed {
		#region Members
		private readonly string title;
		private readonly string id;
		private readonly Uri uri;
		private readonly Uri feedUri;
		private readonly string description;
		private readonly DateTime publishDate;
		private readonly IFeedImage image;
		private readonly string language;
		private readonly IFeedItem[] items;
		private readonly IAuthor author;
		private readonly Dictionary<XmlQualifiedName, string> customElements;
		#endregion

		#region Constructors
		public Feed(string title, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items) : this(title, string.Empty, uri, feedUri, description, publishDate, language, image, items, null, null) { }
		public Feed(string title, Guid id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items) : this(title, "urn:uuid:" + id, uri, feedUri, description, publishDate, language, image, items, null, null) { }
		public Feed(string title, string id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items) : this(title, id, uri, feedUri, description, publishDate, language, image, items, null, null) { }

		public Feed(string title, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, IAuthor author) : this(title, string.Empty, uri, feedUri, description, publishDate, language, image, items, author, null) { }
		public Feed(string title, Guid id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, IAuthor author) : this(title, "urn:uuid:" + id, uri, feedUri, description, publishDate, language, image, items, author, null) { }
		public Feed(string title, string id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, IAuthor author) : this(title, id, uri, feedUri, description, publishDate, language, image, items, author, null) { }

		public Feed(string title, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, Dictionary<XmlQualifiedName, string> customElements) : this(title, string.Empty, uri, feedUri, description, publishDate, language, image, items, null, customElements) { }
		public Feed(string title, Guid id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, Dictionary<XmlQualifiedName, string> customElements) : this(title, "urn:uuid:" + id, uri, feedUri, description, publishDate, language, image, items, null, customElements) { }
		public Feed(string title, string id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, Dictionary<XmlQualifiedName, string> customElements) : this(title, id, uri, feedUri, description, publishDate, language, image, items, null, customElements) { }

		public Feed(string title, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, IAuthor author, Dictionary<XmlQualifiedName, string> customElements) : this(title, string.Empty, uri, feedUri, description, publishDate, language, image, items, author, customElements) { }
		public Feed(string title, Guid id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, IAuthor author, Dictionary<XmlQualifiedName, string> customElements) : this(title, "urn:uuid:" + id, uri, feedUri, description, publishDate, language, image, items, author, customElements) { }
		public Feed(string title, string id, Uri uri, Uri feedUri, string description, DateTime publishDate, string language, IFeedImage image, IFeedItem[] items, IAuthor author, Dictionary<XmlQualifiedName, string> customElements) {
			if(title == null) {
				throw new ArgumentNullException("title");
			}
			if(uri == null) {
				throw new ArgumentNullException("uri");
			}
			this.title = title;
			this.uri = uri;
			this.feedUri = feedUri ?? uri; //Feed Uri is mandatory if the feed shall be 100% standard compliant. But some older implementations does not have such value and therefor we accept null at the moment, shall probably be removed and cast an exeption. Maybe changed later.
			this.description = description;
			this.publishDate = publishDate;
			this.language = language;
			this.image = image;
			this.items = items;
			this.author = author;
			this.id = !string.IsNullOrEmpty(id) ? id : uri.AbsoluteUri;
			this.customElements = customElements ?? new Dictionary<XmlQualifiedName, string>();
		}
		#endregion

		#region Properties
		public string Title { get { return this.title; } }
		public string Id { get { return this.id; } }
		public Uri Uri { get { return this.uri; } }
		public Uri FeedUri { get { return this.feedUri; } }
		public string Description { get { return this.description; } }
		public DateTime PublishDate { get { return this.publishDate; } }
		public string Language { get { return this.language; } }
		public IFeedImage Image { get { return this.image; } }
		public IFeedItem[] Items { get { return this.items; } }
		public IAuthor Author { get { return this.author; } }
		public Dictionary<XmlQualifiedName, string> CustomElements { get { return this.customElements; } }
		#endregion
	}

}
