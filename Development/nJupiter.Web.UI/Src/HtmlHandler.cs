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

	#region Delegates
	public delegate HtmlLink HtmlLinkEvaluator(HtmlLink htmlLink);
	#endregion

	public static class HtmlHandler {
		#region Constants
		private const string Colon = ":";
		private const string FormatReplaceUrl = "<" + HtmlTag.A + " {0}" + HtmlAttribute.Href + @"=""{1}"">{2}</" + HtmlTag.A + ">";
		private const string NoFollowAttribute = "rel=\"external nofollow\" ";
		private const string RegexpatternInformalurlPrefix = @"\w+:";
		private const string RegexpatternInformalurlEmail = @"((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.?[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)";
		private const string RegexpatternInformalurl = RegexpatternInformalurlEmail + "|((" + RegexpatternInformalurlPrefix + @"//|[nN][eE][wW][sS]:|[mM][aA][iI][lL][tT][oO]:|[wW][wW][wW]\.|[fF][tT][pP]\.)[^\\{}|[\]^<>""'\s]*[^\\{}|[\]^<>""'\s.,;?:!])";
		#endregion

		#region Members
		private static readonly Regex InformalUrlRegex = new Regex(RegexpatternInformalurl, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex InformalUrlEmailRegex = new Regex(RegexpatternInformalurlEmail, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex InformalUrlPrefixRegex = new Regex(RegexpatternInformalurlPrefix, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
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
			return AutoHyperlinkText(text, false, null);
		}
		public static string AutoHyperlinkText(string text, bool noFollow) {
			return AutoHyperlinkText(text, noFollow, null);
		}
		public static string AutoHyperlinkText(string text, HtmlLinkEvaluator htmlLinkEvaluator) {
			return AutoHyperlinkText(text, false, htmlLinkEvaluator);
		}
		public static string AutoHyperlinkText(string text, bool noFollow, HtmlLinkEvaluator htmlLinkEvaluator) {
			return InformalUrlRegex.Replace(text, delegate(Match match) {
				string linkText = match.Value;
				string linkUrl;
				if(InformalUrlPrefixRegex.IsMatch(linkText)) {
					linkUrl = linkText;
				} else if(InformalUrlEmailRegex.IsMatch(linkText)) {
					linkUrl = Uri.UriSchemeMailto + Colon + linkText;
				} else if(string.Compare(linkText.Substring(0, 3), Uri.UriSchemeFtp, true, CultureInfo.InvariantCulture).Equals(0)) {
					linkUrl = Uri.UriSchemeFtp + Uri.SchemeDelimiter + linkText;
				} else {
					linkUrl = Uri.UriSchemeHttp + Uri.SchemeDelimiter + linkText;
				}
				if(htmlLinkEvaluator != null) {
					HtmlLink htmlLink = htmlLinkEvaluator(new HtmlLink(linkUrl, linkText));
					linkUrl = htmlLink.Url;
					linkText = htmlLink.Text;
				}
				return string.Format(CultureInfo.InvariantCulture, 
					FormatReplaceUrl, 
					noFollow ? NoFollowAttribute : string.Empty, 
					linkUrl, 
					linkText);
			});
		}
		public static string ConvertNewLinesToBr(string text) {
			return text == null ? null : text.Replace(Environment.NewLine, "<" + HtmlTag.Br + "/>");
		}
		#endregion

	}
}