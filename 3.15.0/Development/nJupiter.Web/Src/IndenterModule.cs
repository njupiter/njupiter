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
using System.Text;
using System.Web;
using System.Xml;

namespace nJupiter.Web {

	public sealed class IndenterModule : IHttpModule {

		static void ReleaseRequestState(object sender, EventArgs e) {
			HttpResponse response = HttpContext.Current.Response;
			if(response.ContentType.Contains("html")) {
				response.Filter = new IndenterFilter(response.Filter);      
			}
		}

		#region Implementation of IHttpModule
		void IHttpModule.Init(HttpApplication context) {
			context.ReleaseRequestState += ReleaseRequestState;
		}

		void IHttpModule.Dispose() {
		}
		#endregion
	}

	internal class IndenterFilter : Stream {

		readonly Stream		responseStream;
		StringBuilder		content;

		public IndenterFilter(Stream inputStream) {
			this.responseStream	= inputStream;
		}

		#region Overrides
		public override	bool	CanRead		{ get { return this.responseStream.CanRead; } }
		public override	bool	CanSeek		{ get { return this.responseStream.CanSeek; } }
		public override	bool	CanWrite	{ get { return this.responseStream.CanWrite; } }
		public override	long	Length		{ get { return this.responseStream.Length; } }
		public override	long	Position	{ get { return this.responseStream.Position; }	set { this.responseStream.Position = value; } }

		public override	void	Close() { this.responseStream.Close(); }
		
		public override	long	Seek(long offset, SeekOrigin origin) {
			return this.responseStream.Seek(offset, origin);
		}

		public override	void	SetLength(long length) {
			this.responseStream.SetLength(length);
		}

		public override	int		Read(byte[] buffer, int offset, int count) {
			return this.responseStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count) {
			if(this.content == null)
				this.content = new StringBuilder();
			this.content.Append(System.Text.Encoding.UTF8.GetString(buffer, offset, count));
		}

		public override	void	Flush() {
			if(this.content == null) {
				this.responseStream.Flush();
				return;
			}

			string contentString = this.content.ToString();
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(contentString);
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Indent = true;
			settings.IndentChars = "\t";
			XmlWriter writer = XmlWriter.Create(this.responseStream, settings);
			doc.Save(writer);

			this.content = null;
			this.responseStream.Flush();
		}
		#endregion
	}
}
