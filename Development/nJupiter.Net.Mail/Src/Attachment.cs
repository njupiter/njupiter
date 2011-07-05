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
using System.IO;
using System.Web;

using nJupiter.Web;

namespace nJupiter.Net.Mail {

	public class Attachment : IDisposable {

		#region Members
		private readonly FileInfo fileInfo;
		private readonly Stream fileStream;
		private readonly string fileName;
		private readonly IMimeType contentType;
		private readonly IMimeTypeHandler mimeTypeHandler;
		private bool disposed;
		#endregion

		#region Constructors
		private Attachment() {
			mimeTypeHandler = new MimeTypeHandler(new HttpContextWrapper(HttpContext.Current));
		}

		public Attachment(FileInfo file) : this() {
			if(file == null)
				throw new ArgumentNullException("file");
			if(!file.Exists)
				throw new FileNotFoundException("File not found.", file.FullName);
			this.fileInfo = file;
		}

		public Attachment(FileInfo file, string name)
			: this(file) {
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("Parameter can not be of zero length.", "name");
			this.fileName = name;
		}

		public Attachment(FileInfo file, string name, string contentType)
			: this(file, name) {
			if(contentType == null)
				throw new ArgumentNullException("contentType");
			if(contentType.Length == 0)
				throw new ArgumentException("Parameter can not be of zero length.", "contentType");
			this.contentType = new MimeType(contentType);
		}

		public Attachment(Stream fileStream, string name) : this() {
			if(fileStream == null)
				throw new ArgumentNullException("fileStream");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("Parameter can not be of zero length.", "name");

			this.fileStream = fileStream;
			this.fileName = name;
		}

		public Attachment(Stream fileStream, string name, string contentType)
			: this(fileStream, name) {
			if(contentType == null)
				throw new ArgumentNullException("contentType");
			if(contentType.Length == 0)
				throw new ArgumentException("Parameter can not be of zero length.", "contentType");
			this.contentType = new MimeType(contentType);
		}
		#endregion

		#region Properties
		public string FileName {
			get {
				if(this.fileName != null)
					return this.fileName;
				if(this.fileInfo != null)
					return this.fileInfo.Name;
				return null;
			}
		}

		public IMimeType ContentType {
			get {
				IMimeType result = null;

				if(this.contentType != null) {
					result = this.contentType;
				} else if(this.fileInfo != null) {
					result = mimeTypeHandler.GetMimeType(this.fileInfo);
				} else if(this.fileStream != null) {
					result = mimeTypeHandler.GetMimeType(this.fileStream);
				}

				if(result != null && result.Parameters["file"] == null) {
					result.Parameters.Add("file", this.FileName);
				}
				return result;
			}
		}
		#endregion

		#region Methods
		public Stream OpenRead() {
			if(this.disposed)
				throw new ObjectDisposedException(base.GetType().FullName);
			if(this.fileInfo != null)
				return this.fileInfo.OpenRead();
			return this.fileStream;
		}

		public void Dispose() {
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing) {
			if(disposing && !this.disposed) {
				this.disposed = true;
				if(this.fileStream != null)
					((IDisposable)this.fileStream).Dispose();
			}
		}
		#endregion

	}

}
