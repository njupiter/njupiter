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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace nJupiter.Web.UI {

	#region Delegates
	public delegate HtmlLink HtmlLinkEvaluator(HtmlLink htmlLink);
	#endregion

	public static class HtmlHandler {
		#region Constants
		private const string	Colon									= ":";
		private const string	FormatReplaceUrl						= "<" + HtmlTag.A + " {0}{1}" + HtmlAttribute.Href + @"=""{2}"">{3}</" + HtmlTag.A + ">";
		private const string	NoFollowAttribute						= HtmlAttribute.Rel + "=\"external nofollow\" ";
		private const string	FormatAttribute							= " {0}=\"{1}\" ";
		private const string	RegexpatternInformalurlPrefix			= @"\w+:";
		private const string	RegexpatternInformalurlEmail			= @"((?>[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+\x20*|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*""\x20*)*(?<angle><))?((?!\.)(?>\.*[a-zA-Z\d!#$%&'*+\-/=?^_`{|}~]+)+|""((?=[\x01-\x7f])[^""\\]|\\[\x01-\x7f])*"")@(((?!-)[a-zA-Z\d\-]+(?<!-)\.)+[a-zA-Z]{2,}|\[(((?(?<!\[)\.)(25[0-5]|2[0-4]\d|[01]?\d?\d)){4}|[a-zA-Z\d\-]*[a-zA-Z\d]:((?=[\x01-\x7f])[^\\\[\]]|\\[\x01-\x7f])+)\])(?(angle)>)";
		private const string	RegexpatternInformalurl					= RegexpatternInformalurlEmail + "|((" + RegexpatternInformalurlPrefix + @"//|[nN][eE][wW][sS]:|[mM][aA][iI][lL][tT][oO]:|[wW][wW][wW]\.|[fF][tT][pP]\.)[^\\{}|[\]^<>""'\s]*[^\\{}|[\]^<>""'\s.,;?:!)])";
		private const string 	RegexpatternFullATagOrTag				= @"(</?[aA]+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>.*?</[aA]>)|(</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>)";
		private const string	RegexpatternFullATagOrTagOrInformalUrl	= "(" + RegexpatternFullATagOrTag + ")|(" + RegexpatternInformalurl + ")";
		#endregion

		#region Members
		private static readonly Regex	InformalUrlRegex				= new Regex(RegexpatternInformalurl, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex	InformalUrlEmailRegex			= new Regex(RegexpatternInformalurlEmail, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex	InformalUrlPrefixRegex			= new Regex(RegexpatternInformalurlPrefix, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex	FullATagOrTagRegex				= new Regex(RegexpatternFullATagOrTag, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
		private static readonly Regex	FullATagOrTagOrInformalUrlRegex = new Regex(RegexpatternFullATagOrTagOrInformalUrl, RegexOptions.ExplicitCapture | RegexOptions.Compiled);
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
				return HyperlinkUrl(match.Value, noFollow, htmlLinkEvaluator);
			});
		}
		public static string AutoHyperlinkHtml(string html) {
			return AutoHyperlinkHtml(html, false, null);
		}
		public static string AutoHyperlinkHtml(string html, bool noFollow) {
			return AutoHyperlinkHtml(html, noFollow, null);
		}
		public static string AutoHyperlinkHtml(string html, HtmlLinkEvaluator htmlLinkEvaluator) {
			return AutoHyperlinkHtml(html, false, htmlLinkEvaluator);
		}
		public static string AutoHyperlinkHtml(string html, bool noFollow, HtmlLinkEvaluator htmlLinkEvaluator) {
			return FullATagOrTagOrInformalUrlRegex.Replace(html, delegate(Match match) {
				if(!FullATagOrTagRegex.IsMatch(match.Value)) {
					return HyperlinkUrl(match.Value, noFollow, htmlLinkEvaluator);
				} 
				return match.Value;
			});
		}
		public static string ConvertNewLinesToBr(string text) {
			return text == null ? null : text.Replace(Environment.NewLine, "<" + HtmlTag.Br + "/>");
		}
		#endregion

		#region Helper Methods
		public static string HyperlinkUrl(string url, bool noFollow, HtmlLinkEvaluator htmlLinkEvaluator) {
			string linkText = url;
			string linkUrl;
			StringBuilder linkAttributes = null;
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
				if(!htmlLink.Attributes.Count.Equals(0)) {
					linkAttributes = new StringBuilder();
					foreach(string attribute in htmlLink.Attributes.Keys) {
						linkAttributes.AppendFormat(CultureInfo.InvariantCulture, FormatAttribute, attribute, htmlLink.Attributes[attribute]);
					}
				}
			}
			return string.Format(CultureInfo.InvariantCulture, 
				FormatReplaceUrl, 
				noFollow ? NoFollowAttribute : string.Empty, 
				linkAttributes,
				linkUrl, 
				linkText);
		}
		#endregion
	}
}