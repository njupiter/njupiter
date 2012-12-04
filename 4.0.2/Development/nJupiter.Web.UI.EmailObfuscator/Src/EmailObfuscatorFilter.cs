using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace nJupiter.Web.UI.EmailObfuscator {
	internal class EmailObfuscatorFilter : Stream {

		private const string HeadEndPattern = @"</(head|HEAD)>";
		private const string EmailPattern = @"^((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)$";
		private const string ScriptTag = @"<script type=""text/javascript"" src=""{0}""></script>";
		

		private static readonly string Script = string.Format(CultureInfo.InvariantCulture, ScriptTag, ResourceRegistrator.ScriptPath);

		private readonly Stream responseStream;
		private StringBuilder content;

		public EmailObfuscatorFilter(Stream inputStream) {
			responseStream = inputStream;
		}

		public override bool CanRead { get { return responseStream.CanRead; } }
		public override bool CanSeek { get { return responseStream.CanSeek; } }
		public override bool CanWrite { get { return responseStream.CanWrite; } }
		public override long Length { get { return responseStream.Length; } }
		public override long Position { get { return responseStream.Position; } set { responseStream.Position = value; } }

		public override void Close() { responseStream.Close(); }

		public override long Seek(long offset, SeekOrigin origin) {
			return responseStream.Seek(offset, origin);
		}

		public override void SetLength(long length) {
			responseStream.SetLength(length);
		}

		public override int Read(byte[] buffer, int offset, int count) {
			return responseStream.Read(buffer, offset, count);
		}

		private static bool CharInString(Char c, string str) {
			return str.IndexOf(c) > -1;
		}

		public override void Write(byte[] buffer, int offset, int count) {
			if(content == null)
				content = new StringBuilder();
			content.Append(Encoding.UTF8.GetString(buffer, offset, count));
		}

		public override void Flush() {
			if(content == null) {
				responseStream.Flush();
				return;
			}

			var contentString = content.ToString();

			var contentBuilder = new StringBuilder();

			var openTag = false;
			var areaOpen = false;
			var containsEmail = false;
			var currentElementCompleted = false;
			var currentAttributeCompleted = true;
			var openingQuote = char.MinValue;
			var currentElement = new StringBuilder();
			var currentAttribute = new StringBuilder();

			// Parse the document, much faster than regular expressions
			for(var i = 0; i < contentString.Length; i++) {
				var currentChar = contentString[i];
				if(openTag && !currentElementCompleted && currentChar != '>') {
					if(char.IsWhiteSpace(currentChar)) {
						currentElementCompleted = true;
					} else {
						currentElement.Append(currentChar);
					}
				}
				if(currentChar == '<') {
					openTag = true;
					currentElementCompleted = false;
					currentElement.Remove(0, currentElement.Length);
				}
				if(currentChar == '>') {
					openTag = false;
					openingQuote = char.MinValue;
					currentAttributeCompleted = true;
					if(currentAttribute.Length > 0) {
						currentAttribute.Remove(0, currentAttribute.Length);
					}
				}
				
				// Check for all potential e-mail addresses and replace them
				var element = currentElement.ToString();
				var attribute = currentAttribute.ToString();

				// If inside an anchor tag, check the attributes so we know if we are inside an href attribute or not
				if(openTag && currentElementCompleted && currentAttributeCompleted && element.Equals("a", StringComparison.OrdinalIgnoreCase) && currentChar == '=') {
					currentAttributeCompleted = false;
					openingQuote = char.MinValue;
					for(var j = contentBuilder.Length - 1; j > 0; j--) {
						var attribChar = contentBuilder[j];
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
					} else if(openingQuote.Equals(char.MinValue) && (currentChar == '\'' || currentChar == '"')) {
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
					var beforeAt = new StringBuilder();
					var beforeColon = new StringBuilder();
					var checkForMailTo = false;
					for(var j = i - 1; j >= 0; j--) {
						// build the address before the at-character
						var c = contentString[j];
						if(checkForMailTo) {
							if(beforeColon.Length > 5)
								break;
							beforeColon.Insert(0, c);
						} else if(char.IsLetterOrDigit(c) || CharInString(c, "._%+-")) {
							beforeAt.Insert(0, c);
						} else if(j < i - 2 && c == ':') {
							checkForMailTo = true;
						} else {
							break;
						}
					}
					if(beforeAt.Length > 0 && contentString.Length > i + 1) { //If the chars before the at-char match an email, keep on looking after the at-char
						var afterAt = new StringBuilder();
						for(var k = i + 1; k < contentString.Length; k++) {
							// build the address after the at-character
							var c = contentString[k];
							if(char.IsLetterOrDigit(c) || CharInString(c, "._%+-")) {
								afterAt.Append(c);
							} else {
								break;
							}
						}
						if(afterAt.Length > 0) { //Potential email
							var mailto = false;
							var name = beforeAt.ToString().TrimStart('.', '_', '%', '+', '-');
							var domain = afterAt.ToString().TrimEnd('.', '_', '%', '+', '-');
							if(name.Length > 0 && domain.Length > 0) {
								var email = string.Format(CultureInfo.InvariantCulture, "{0}@{1}", name, domain);
								var beforeIndex = name.Length;
								if(beforeColon.Length > 0 && beforeColon.ToString().Equals("mailto", StringComparison.InvariantCultureIgnoreCase)) {
									mailto = true;
									beforeIndex = beforeIndex + "mailto:".Length;
								}
								//Only obfuscate if it is definitly an email, but do not do it if we are inside an href without an mailto (for example we do not want to obfuscate usernames in ftp-links)
								if(!(attribute.Equals("href", StringComparison.OrdinalIgnoreCase) && !mailto)
								   && Regex.IsMatch(email, EmailPattern, RegexOptions.Compiled)) { //Definitly an email
									contentBuilder.Remove(contentBuilder.Length - beforeIndex, beforeIndex);
									var positionAfterMail = i + domain.Length + 1;
									if(mailto && !openingQuote.Equals(char.MinValue) && contentString.Length > positionAfterMail && contentString[positionAfterMail] == '?') {
										var queryString = new StringBuilder();
										var endFound = false;
										for(var m = positionAfterMail; m < contentString.Length; m++) {
											// build the querystring
											var c = contentString[m];
											if(c.Equals(openingQuote) || c.Equals('>')) {
												endFound = true;
												break;
											}
											queryString.Append(c);
										}
										if(endFound) {
											var q = queryString.ToString();
											email = email + q;
											i = i + q.Length;
										}
									}
									i = i + domain.Length;
									var encryptedEmail = HttpUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(email)));
									if(mailto || openTag) {
										contentBuilder.Append(string.Format(CultureInfo.InvariantCulture, "EOContact.ashx?encstr={0}&amp;encanchor=true", encryptedEmail));
									} else {
										contentBuilder.Append(string.Format(CultureInfo.InvariantCulture, "<span class=\"eoImage\"><img alt=\"Contact\" src=\"EOContact.ashx?encstr={0}&amp;encimage=true\" /></span>", encryptedEmail));
									}
									containsEmail = true;
									goto _processed;
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
				contentString = Regex.Replace(contentString, HeadEndPattern, HeadEndMatch, RegexOptions.Compiled);
			}

			var data = Encoding.UTF8.GetBytes(contentString);
			content = null;
			responseStream.Write(data, 0, data.Length);
			responseStream.Flush();
		}

		private static string HeadEndMatch(Match m) {
			return string.Format(CultureInfo.InvariantCulture, "{0}<style type=\"text/css\">.hasJavascript span.eoImage img{{ display: none; }}</style></head>", Script);
		}

	}
}