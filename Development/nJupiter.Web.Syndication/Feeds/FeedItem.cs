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
using System.Collections.Generic;
using System.Xml;

namespace nJupiter.Web.Syndication {

	public sealed class FeedItem : IFeedItem {
		#region Members
		private readonly string title;
		private readonly string id;
		private readonly Uri uri;
		private readonly DateTime publishDate;
		private readonly string description;
		private readonly IAuthor author;
		private readonly Dictionary<XmlQualifiedName, string> customElements;
		#endregion

		#region Constructors
		public FeedItem(string title, string description, Uri uri, DateTime publishDate) : this(title, string.Empty, description, uri, publishDate, null, null) { }
		public FeedItem(string title, Guid id, string description, Uri uri, DateTime publishDate) : this(title, "urn:uuid:" + id, description, uri, publishDate, null, null) { }
		public FeedItem(string title, string id, string description, Uri uri, DateTime publishDate) : this(title, id, description, uri, publishDate, null, null) { }
		public FeedItem(string title, string description, Uri uri, DateTime publishDate, IAuthor author) : this(title, string.Empty, description, uri, publishDate, author, null) { }
		public FeedItem(string title, Guid id, string description, Uri uri, DateTime publishDate, IAuthor author) : this(title, "urn:uuid:" + id, description, uri, publishDate, author, null) { }
		public FeedItem(string title, string id, string description, Uri uri, DateTime publishDate, IAuthor author) : this(title, id, description, uri, publishDate, author, null) { }


		public FeedItem(string title, string description, Uri uri, DateTime publishDate, Dictionary<XmlQualifiedName, string> customElements) : this(title, string.Empty, description, uri, publishDate, null, customElements) { }
		public FeedItem(string title, Guid id, string description, Uri uri, DateTime publishDate, Dictionary<XmlQualifiedName, string> customElements) : this(title, "urn:uuid:" + id, description, uri, publishDate, null, customElements) { }
		public FeedItem(string title, string id, string description, Uri uri, DateTime publishDate, Dictionary<XmlQualifiedName, string> customElements) : this(title, id, description, uri, publishDate, null, customElements) { }
		public FeedItem(string title, string description, Uri uri, DateTime publishDate, IAuthor author, Dictionary<XmlQualifiedName, string> customElements) : this(title, string.Empty, description, uri, publishDate, author, customElements) { }
		public FeedItem(string title, Guid id, string description, Uri uri, DateTime publishDate, IAuthor author, Dictionary<XmlQualifiedName, string> customElements) : this(title, "urn:uuid:" + id, description, uri, publishDate, author, customElements) { }
		public FeedItem(string title, string id, string description, Uri uri, DateTime publishDate, IAuthor author, Dictionary<XmlQualifiedName, string> customElements) {
			if(title == null && description == null)
				throw new ArgumentNullException("title");
			this.title = title;
			this.description = description;
			this.uri = uri;
			this.publishDate = publishDate;
			this.author = author;
			this.id = !string.IsNullOrEmpty(id) ? id : uri != null ? uri.AbsoluteUri : string.Empty;
			this.customElements = customElements ?? new Dictionary<XmlQualifiedName, string>();
		}
		#endregion

		#region Properties
		public string Title { get { return this.title; } }
		public string Id { get { return this.id; } }
		public Uri Uri { get { return this.uri; } }
		public string Description { get { return this.description; } }
		public DateTime PublishDate { get { return this.publishDate; } }
		public IAuthor Author { get { return this.author; } }
		public Dictionary<XmlQualifiedName, string> CustomElements { get { return this.customElements; } }
		#endregion
	}

}
