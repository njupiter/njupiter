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
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace nJupiter.Web.UI {

	public static class HtmlHandler {
		#region Constants
		private const string	Colon							= ":";
		private const string	FormatReplaceUrl				= "<" + HtmlTag.A + " {0}" + HtmlAttribute.Href + @"=""{1}"">{2}</" + HtmlTag.A + ">";
		private const string	NoFollowAttribute				= "rel=\"external nofollow\" ";
		private const string	RegexpatternInformalurlPrefix	= @"\w+:";
		private const string	RegexpatternInformalurlEmail	= @"((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)";
		private const string	RegexpatternInformalurl			= RegexpatternInformalurlEmail + "|((" + RegexpatternInformalurlPrefix + @"//|[nN][eE][wW][sS]:|[mM][aA][iI][lL][tT][oO]:|[wW][wW][wW]\.|[fF][tT][pP]\.)[^\\{}|[\]^<>""'\s]*[^\\{}|[\]^<>""'\s.,;?:!])";
		#endregion

		#region Members
		private static readonly Regex			informalUrlRegex		= new Regex(RegexpatternInformalurl, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex			informalUrlEmailRegex	= new Regex(RegexpatternInformalurlEmail, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex			informalUrlPrefixRegex	= new Regex(RegexpatternInformalurlPrefix, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		#endregion

		#region Methods
		public static string StripHtmlTags(string html) {
			if(string.IsNullOrEmpty(html)) {
				return html;
			}
			StringBuilder text = new StringBuilder(html);
			int beginRemove = -1;
			for(int i = 0; i < text.Length; i++) {
				switch(text[i]) {
					case '<':
						beginRemove = i;
						break;
					case '>':
						if(!beginRemove.Equals(-1)) {
							int length = i - beginRemove + 1;
							text.Remove(beginRemove, length);
							i -= length;
						}
						beginRemove = -1;
					break;
				}
			}
			return text.ToString();
		}
		public static string AutoHyperlinkText(string text) {
			return AutoHyperlinkText(text, false);
		}

		public static string AutoHyperlinkText(string text, bool noFollow) {
			if(noFollow) {
				return informalUrlRegex.Replace(text, HyperlinkMatchNoFollowEvaluator);
			}
			return informalUrlRegex.Replace(text, HyperlinkMatchEvaluator);
		}
		public static string ConvertNewLinesToBr(string text) {
			return text == null ? null : text.Replace(Environment.NewLine, "<" + HtmlTag.Br + "/>");
		}
		#endregion

		#region Helper Methods
		private static string HyperlinkMatchEvaluator(Match match) {
			return EvaluateHyperlinkMatch(match, false);
		}

		private static string HyperlinkMatchNoFollowEvaluator(Match match) {
			return EvaluateHyperlinkMatch(match, true);
		}

		private static string EvaluateHyperlinkMatch(Capture match, bool noFollow) {
			string matchValue	= match.Value;
			string address;

			if(informalUrlPrefixRegex.IsMatch(matchValue)) {
			    address	= matchValue;
			} else if(informalUrlEmailRegex.IsMatch(matchValue)) {
			    address	= Uri.UriSchemeMailto + Colon + matchValue;
			} else if(string.Compare(matchValue.Substring(0, 3), Uri.UriSchemeFtp, true, CultureInfo.InvariantCulture).Equals(0)) {
			    address	= Uri.UriSchemeFtp + Uri.SchemeDelimiter + matchValue;
			} else {
			    address	= Uri.UriSchemeHttp + Uri.SchemeDelimiter + matchValue;
			}
			return string.Format(CultureInfo.InvariantCulture, FormatReplaceUrl, noFollow ? NoFollowAttribute : string.Empty, address, matchValue);
		}
		#endregion
	}
}