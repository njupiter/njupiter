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

namespace nJupiter.Web.Syndication {

	public sealed class FeedType {
		#region Members
		private readonly string name;
		private readonly string contentType;
		private readonly string responseContentType;
		#endregion

		#region Static Members
		public static readonly FeedType Rss = new FeedType("Rss", "application/rss+xml", "application/xml");
		public static readonly FeedType Rdf = new FeedType("Rdf", "application/rss+xml", "application/xml");
		public static readonly FeedType Atom = new FeedType("Atom", "application/atom+xml", "application/xml");
		#endregion

		#region Constructors
		private FeedType(string name, string contentType, string responseContentType) {
			this.name = name;
			this.contentType = contentType;
			this.responseContentType = responseContentType;
		}
		#endregion

		#region Factory Method
		public static FeedType GetFeedType(string feedType) {
			if(string.Equals(feedType, Atom.Name, StringComparison.OrdinalIgnoreCase)) {
				return Atom;
			}
			if(string.Equals(feedType, Rdf.Name, StringComparison.OrdinalIgnoreCase)) {
				return Rdf;
			}
			return Rss;
		}
		#endregion

		#region Properties
		public string Name { get { return this.name; } }
		public string ContentType { get { return this.contentType; } }
		public string ResponseContentType { get { return this.responseContentType; } }
		#endregion
	}

}
