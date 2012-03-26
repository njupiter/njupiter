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
using System.Globalization;
using System.Text.RegularExpressions;

namespace nJupiter.Web.UI.EmailObfuscator {

	public sealed class EmailObfuscatorModule : IHttpModule {

		private const string ObfuscationDisabledKey = "NJUPITER_EMAIL_OBFUSCATION_DISABLED";

		static void ReleaseRequestState(object sender, EventArgs e) {
            HttpResponse response = HttpContext.Current.Response;
            if(response.ContentType.Contains("html") && !EmailObfuscatorModule.DisableObfuscationForCurrentRequest){
                response.Filter = new EmailObfuscatorFilter(response.Filter);      
			}
		}

		public static bool DisableObfuscationForCurrentRequest {
			get{
				return HttpContext.Current.Items[ObfuscationDisabledKey] != null;
			}
			set {
				HttpContext.Current.Items[ObfuscationDisabledKey] = (!value ? null : (object)true);
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

	internal class EmailObfuscatorFilter : Stream {

		#region Private Constants
		private const string HeadEndPattern	= @"</(head|HEAD)>";
		private const string EmailPattern		= @"^((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)$";
		private const string ScriptTag		= @"<script type=""text/javascript"" src=""{0}""></script>";
		private const string ScriptPath		= @"/nJupiter/nJupiter.Web.UI.EmailObfuscator/Web/Scripts/EmailObfuscator.js";

		private static readonly string script	= string.Format(CultureInfo.InvariantCulture, ScriptTag, ScriptPath);
		#endregion

		readonly Stream		responseStream;
		StringBuilder		content;

		public EmailObfuscatorFilter(Stream inputStream) {
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

		private static bool CharInString(Char c, string str) {
			return str.IndexOf(c) > -1;
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
			
			StringBuilder contentBuilder = new StringBuilder();
			
			bool openTag					= false;
			bool areaOpen					= false;
			bool containsEmail				= false;
			bool currentElementCompleted	= false;
			bool currentAttributeCompleted	= true;
			char openingQuote				= char.MinValue;
			StringBuilder currentElement	= new StringBuilder();
			StringBuilder currentAttribute	= new StringBuilder();
			
			// Parse the document, much faster than regular expressions
			for(int i = 0; i < contentString.Length; i++) {
				char currentChar = contentString[i];
				if(openTag && !currentElementCompleted && currentChar != '>') {
					if(char.IsWhiteSpace(currentChar)) {
						currentElementCompleted = true;
					} else {
						currentElement.Append(currentChar);
					}
				}
				if(currentChar == '<'){
					openTag = true;
					currentElementCompleted = false;
					currentElement.Remove(0, currentElement.Length);
				}
				if(currentChar == '>'){
					openTag = false;
					openingQuote = char.MinValue;
					currentAttributeCompleted = true;
					if(currentAttribute.Length > 0) {
						currentAttribute.Remove(0, currentAttribute.Length);
					}
				}
				// Check for all potential e-mail addresses and replace them
				string element = currentElement.ToString();
				string attribute = currentAttribute.ToString();

				// If inside an anchor tag, check the attributes so we know if we are inside an href attribute or not
				if(openTag && currentElementCompleted && currentAttributeCompleted && element.Equals("a", StringComparison.OrdinalIgnoreCase) && currentChar == '=') {
					currentAttributeCompleted = false;
					openingQuote = char.MinValue;
					for(int j = contentBuilder.Length - 1; j > 0; j--) {
						char attribChar = contentBuilder[j];
						if((currentAttribute.Length > 0 && char.IsWhiteSpace(attribChar)) || attribChar == '<') {
							break;
						}
						if(!char.IsWhiteSpace(attribChar)) {
							currentAttribute.Insert(0, attribChar);
						}
					}
				}
				if(!currentAttributeCompleted) {
					if(!openingQuote.Equals(char.MinValue) && currentChar.Equals(openingQuote)) {
						currentElementCompleted = true;
						openingQuote = char.MinValue;
					}else if(openingQuote.Equals(char.MinValue) && (currentChar == '\'' || currentChar == '"')) {
						openingQuote = currentChar;
					}
				}

				// Check if we are inside an textarea or not
				if(element.Equals("textarea", StringComparison.InvariantCultureIgnoreCase)) {
					areaOpen = true;
				} else if(areaOpen && element.Equals("/textarea", StringComparison.InvariantCultureIgnoreCase)) {
					areaOpen = false;
				}
				
				if(!areaOpen && currentChar == '@' && i > 0
					&& !element.Equals("input", StringComparison.InvariantCultureIgnoreCase)
					&& !element.Equals("option", StringComparison.InvariantCultureIgnoreCase)) {
					StringBuilder beforeAt		= new StringBuilder();
					StringBuilder beforeColon	= new StringBuilder();
					bool checkForMailTo = false;
					for(int j = i - 1; j >= 0; j--) {
						// build the address before the at-character
						char c = contentString[j];
						if(checkForMailTo) {
							if(beforeColon.Length > 5)
								break;
							beforeColon.Insert(0, c);
						}else if(char.IsLetterOrDigit(c) || CharInString(c, "._%+-")){
							beforeAt.Insert(0, c);
						} else if(j < i - 2 && c == ':') {
							checkForMailTo = true;
						} else {
							break;
						}
					}
					if(beforeAt.Length > 0 && contentString.Length > i + 1) { //If the chars before the at-char match an email, keep on looking after the at-char
						StringBuilder afterAt = new StringBuilder();
						for(int k = i + 1; k < contentString.Length; k++) {
							// build the address after the at-character
							char c = contentString[k];
							if(char.IsLetterOrDigit(c) || CharInString(c, "._%+-")) {
								afterAt.Append(c);
							} else {
								break;
							}					
						}
						if(afterAt.Length > 0) { //Potential email
							bool mailto = false;
							string name		= beforeAt.ToString().TrimStart('.','_','%','+','-');
							string domain	= afterAt.ToString().TrimEnd('.','_','%','+','-');
							if(name.Length > 0 && domain.Length > 0) {
								string email = string.Format(CultureInfo.InvariantCulture, "{0}@{1}", name, domain);
								int beforeIndex = name.Length;
								if(beforeColon.Length > 0 && beforeColon.ToString().Equals("mailto", StringComparison.InvariantCultureIgnoreCase)) {
									mailto = true;
									beforeIndex = beforeIndex + "mailto:".Length;
								}
								//Only obfuscate if it is definitly an email, but do not do it if we are inside an href without an mailto (for example we do not want to obfuscate usernames in ftp-links)
								if(!(attribute.Equals("href", StringComparison.OrdinalIgnoreCase) && !mailto)
									&& Regex.IsMatch(email, EmailPattern, RegexOptions.Compiled)) { //Definitly an email
									contentBuilder.Remove(contentBuilder.Length - beforeIndex, beforeIndex);
									int positionAfterMail = i + domain.Length + 1;
									if(mailto && !openingQuote.Equals(char.MinValue) && contentString.Length > positionAfterMail && contentString[positionAfterMail] == '?') {
										StringBuilder queryString = new StringBuilder();
										bool endFound = false;
										for(int m = positionAfterMail; m < contentString.Length; m++) {
											// build the querystring
											char c = contentString[m];
											if(c.Equals(openingQuote) || c.Equals('>')) {
												endFound = true;
												break;
											}
											queryString.Append(c);
										}
										if(endFound) {
											string q = queryString.ToString();
											email = email + q;
											i = i + q.Length;
										}
									}
									i = i + domain.Length;
									string encryptedEmail = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(email)));
									if(mailto || openTag) {
										contentBuilder.Append(string.Format(CultureInfo.InvariantCulture, "EOContact.ashx?encstr={0}&amp;encanchor=true", encryptedEmail));
									} else {
										contentBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<span class=\"eoImage\"><img alt=\"Contact\" src=\"EOContact.ashx?encstr={0}&amp;encimage=true\" /></span>", encryptedEmail));
									}
									containsEmail = true;
									goto _processed; //My first goto in nJupiter, wow :)
								}
							}
						}
					}
				}
				contentBuilder.Append(currentChar);
			_processed:
				continue;
			}
			contentString = contentBuilder.ToString();
			if(containsEmail) {
				contentString = Regex.Replace(contentString, HeadEndPattern, new MatchEvaluator(HeadEndMatch), RegexOptions.Compiled);
			}

			byte[] data = System.Text.Encoding.UTF8.GetBytes(contentString);
			this.content = null;
			this.responseStream.Write(data, 0, data.Length);
			this.responseStream.Flush();
		}
		#endregion

		private static string HeadEndMatch(Match m){
			return string.Format(CultureInfo.InvariantCulture, "{0}<style type=\"text/css\">.hasJavascript span.eoImage img{{ display: none; }}</style></head>", script);
		}

	}


}
