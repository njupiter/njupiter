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
using System.Xml;

namespace nJupiter.Web.Syndication {

	public abstract class FeedWriter : IDisposable {

		private readonly XmlWriter xmlWriter;
		private readonly FeedType feedType;
		private bool disposed;

		protected FeedWriter(Stream stream, FeedType feedType) {
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			this.feedType = feedType;
			this.xmlWriter =  XmlWriter.Create(stream, settings);
		}

		protected XmlWriter XmlWriter { get { return this.xmlWriter; } }
		public FeedType FeedType { get { return this.feedType; } }

		#region Methods
		public static FeedWriter Create(Stream stream) {
			return Create(stream, FeedType.Rss);
		}

		public static FeedWriter Create(Stream stream, FeedType type) {
			if(FeedType.Rss.Equals(type)){
				return new RssWriter(stream, type);
			}
			if(FeedType.Atom.Equals(type)){
				return new AtomWriter(stream, type);
			}
			throw new NotSupportedException(string.Format("FeedType {0} not supported", type.Name));
		}

		public void Close() {
			this.XmlWriter.Close();
		}

		public abstract void Write(IFeed feed);
		#endregion

		#region IDisposable Members
		private void Dispose(bool disposing) {
			if (!this.disposed) {
				this.disposed = true;
				if(this.xmlWriter !=  null)
					this.xmlWriter.Close();

				// Suppress finalization of this disposed instance.
				if(disposing) {
					GC.SuppressFinalize(this);
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		
		// Disposable types implement a finalizer.
		~FeedWriter() {
			Dispose(false);
		}
		#endregion

	}

}