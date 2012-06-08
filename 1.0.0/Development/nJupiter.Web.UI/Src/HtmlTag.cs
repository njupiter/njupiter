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

namespace nJupiter.Web.UI {
	public class HtmlTag {
		public const string A				= "a";
		public const string Abbr			= "abbr";
		public const string Acronym			= "acronym";
		public const string Address			= "address";
		public const string Area			= "area";
		public const string B				= "b";
		public const string Base			= "base";
		public const string Basefont		= "basefont";
		public const string Bdo				= "bdo";
		public const string Bgsound			= "bgsound";
		public const string Big				= "big";
		public const string Blockquote		= "blockquote";
		public const string Body			= "body";
		public const string Br				= "br";
		public const string Button			= "button";
		public const string Caption			= "caption";
		public const string Center			= "center";
		public const string Cite			= "cite";
		public const string Code			= "code";
		public const string Col				= "col";
		public const string Colgroup		= "colgroup";
		public const string Dd				= "dd";
		public const string Del				= "del";
		public const string Dfn				= "dfn";
		public const string Dir				= "dir";
		public const string Div				= "div";
		public const string Dl				= "dl";
		public const string Dt				= "dt";
		public const string Em				= "em";
		public const string Embed			= "embed";
		public const string Fieldset		= "fieldset";
		public const string Font			= "font";
		public const string Footer			= "footer";
		public const string Form			= "form";
		public const string Frame			= "frame";
		public const string Frameset		= "frameset";
		public const string H1				= "h1";
		public const string H2				= "h2";
		public const string H3				= "h3";
		public const string H4				= "h4";
		public const string H5				= "h5";
		public const string H6				= "h6";
		public const string Head			= "head";
		public const string Hr				= "hr";
		public const string Html			= "html";
		public const string I				= "i";
		public const string Iframe			= "iframe";
		public const string Img				= "img";
		public const string Input			= "input";
		public const string Ins				= "ins";
		public const string Isindex			= "isindex";
		public const string Kbd				= "kbd";
		public const string Label			= "label";
		public const string Legend			= "legend";
		public const string Li				= "li";
		public const string Link			= "link";
		public const string Map				= "map";
		public const string Marquee			= "marquee";
		public const string Menu			= "menu";
		public const string Meta			= "meta";
		public const string Nobr			= "nobr";
		public const string Noframes		= "noframes";
		public const string Noscript		= "noscript";
		public const string Object			= "object";
		public const string Ol				= "ol";
		public const string Optgroup		= "optgroup";
		public const string Option			= "option";
		public const string P				= "p";
		public const string Param			= "param";
		public const string Pre				= "pre";
		public const string Q				= "q";
		public const string Rt				= "rt";
		public const string Ruby			= "ruby";
		public const string S				= "s";
		public const string Samp			= "samp";
		public const string Script			= "script";
		public const string Select			= "select";
		public const string Small			= "small";
		public const string Span			= "span";
		public const string Strike			= "strike";
		public const string Strong			= "strong";
		public const string Style			= "style";
		public const string Sub				= "sub";
		public const string Sup				= "sup";
		public const string Table			= "table";
		public const string Tbody			= "tbody";
		public const string Td				= "td";
		public const string Textarea		= "textarea";
		public const string Tfoot			= "tfoot";
		public const string Th				= "th";
		public const string Thead			= "thead";
		public const string Time			= "time";
		public const string Title			= "title";
		public const string Tr				= "tr";
		public const string Tt				= "tt";
		public const string U				= "u";
		public const string Ul				= "ul";
		public const string Unknown			= "unknown";
		public const string Var				= "var";
		public const string Wbr				= "wbr";
		public const string Xml				= "xml";

		public static bool IsBlockElement(string tagName){
			if(tagName == null)
				return false;
			if(	tagName.Equals(Address) ||
				tagName.Equals(Blockquote) ||
				tagName.Equals(Center) ||
				tagName.Equals(Dir) ||
				tagName.Equals(Div) ||
				tagName.Equals(Dl) ||
				tagName.Equals(Fieldset) ||
				tagName.Equals(Form) ||
				tagName.Equals(Footer) ||
				tagName.Equals(H1) ||
				tagName.Equals(H2) ||
				tagName.Equals(H3) ||
				tagName.Equals(H4) ||
				tagName.Equals(H5) ||
				tagName.Equals(H6) ||
				tagName.Equals(Hr) ||
				tagName.Equals(Isindex) ||
				tagName.Equals(Menu) ||
				tagName.Equals(Noframes) ||
				tagName.Equals(Noscript) ||
				tagName.Equals(Ol) ||
				tagName.Equals(P) ||
				tagName.Equals(Pre) ||
				tagName.Equals(Table) ||
				tagName.Equals(Ul)){
				return true;
			}
			return false;
		}

		public static bool RequiresEndTag(string tagName){
			if(tagName == null)
				return false;
			if(	tagName.Equals(Base) ||
				tagName.Equals(Basefont) ||
				tagName.Equals(Meta) ||
				tagName.Equals(Link) ||
				tagName.Equals(Hr) ||
				tagName.Equals(Br) ||
				tagName.Equals(Param) ||
				tagName.Equals(Img) ||
				tagName.Equals(Area) ||
				tagName.Equals(Input) ||
				tagName.Equals(Col)){
				return false;
			}
			return true;
		}

	}
}

