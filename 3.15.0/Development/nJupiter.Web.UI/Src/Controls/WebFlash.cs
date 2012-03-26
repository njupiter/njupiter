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
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using System.Text;
using System.Globalization;

namespace nJupiter.Web.UI.Controls {

	#region Enums
	public enum DimensionUnit {
		Pixels,
		Percent
	}
	#endregion

	/// <summary>
	///	Renders html tags for flash
	/// </summary>
	public class WebFlash : WebGenericControl {

		#region Enums
		public enum Mode {
			Ufo,
			SWFObject,
			SWFObject2,
			SWFObject2Embedded
		}

		public enum WindowMode {
			Transparent,
			Opaque,
			Window,
			Direct,
			Gpu
		}
		#endregion
	
		#region Constants
		private const string SwfObjectKey			= "swfobject";
		private const string SwfObjectInclude		= "<script type=\"text/javascript\" src=\"/nJupiter/nJupiter.Web.UI/Web/Scripts/SwfObject.js\"></script><script type=\"text/javascript\">var so;</script>";
		// 0 = constructor, 1 = params, 2 = variables, 3 = div id
		private const string SwfScriptTag			= 
		@"<script type=""text/javascript"">
		<!--//--><![CDATA[//><!--
			{0}
			{1}
			{2}
			{3}
			so.write(""{4}"");
		//--><!]]>
		</script>";
		private const string SwfConstructor				= "so = new SWFObject(\"{0}\", \"{1}{2}\", \"{3}{4}\", \"{5}{6}\", \"{7}\", \"{8}\");";
		// 0 = movie file, 1 = movie name, 2 = movie name suffix, 3 = width, 4 = width unit, 5 = height, 6 = height unit, 7 = flash version, 8 = bg color
		private const string SwfAddProperty				= "so.addParam(\"{0}\", \"{1}\");"; // 0 = key, 1 = value
		private const string SwfAddParam				= "so.addVariable(\"{0}\", \"{1}\");"; // 0 = key, 1 = value
		private const string SwfAutoinstall				= "so.useExpressInstall(\"{0}\")";
		private const string SwfMovieNameSuffix			= "movie";
		private const string SwfExpressinstall			= "/nJupiter/nJupiter.Web.UI/Web/Scripts/SwfObject.swf";

		private const string UfoKey						= "ufoobject";
		private const string UfoExpressinstall			= "/nJupiter/nJupiter.Web.UI/Web/Scripts/ufo.swf";
		private const string UfoInclude					= "<script type=\"text/javascript\" src=\"/nJupiter/nJupiter.Web.UI/Web/Scripts/ufo.js\"></script>";
		private const string UfoScriptTag				= 
		@"<script type=""text/javascript""{15}>
		<!--//--><![CDATA[//><!--
			var UFO_{14} = {{ movie:""{0}"", width:""{1}{2}"", height:""{3}{4}"", majorversion:""{5}"", build:""{6}"", scale:""{7}"", salign:""{8}"", wmode:""{9}"", flashvars:""{10}"", xi:""{11}"", ximovie:""{12}""{13} }};
			UFO.create(UFO_{14}, ""{14}"");
		//--><!]]>
		</script>";
		private const string UfoCssTag					= 	
		@"<style type=""text/css"" media=""screen"">
			#{0} {{ width:{1}{2}; height:{3}{4}; overflow:auto; }}
		</style>";

		private const string SwffixKey					= "swfobject2";
		private const string SwffixExpressinstall		= "/nJupiter/nJupiter.Web.UI/Web/Scripts/swfobject2.swf";
		private const string SwffixInclude				= "<script type=\"text/javascript\" src=\"/nJupiter/nJupiter.Web.UI/Web/Scripts/swfobject2.js\"></script>";
		private const string SwffixScriptTag			= 
		@"<script type=""text/javascript"">
		<!--//--><![CDATA[//><!--
			swfobject.registerObject(""{0}"", ""{1}.0.0"", {2});
		//--><!]]>
		</script>";
		private const string SwffixBeginTag				= @"<object id=""{0}"" classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" width=""{1}{2}"" height=""{3}{4}""><param name=""movie"" value=""{5}"" /><!--[if !IE]>--><object type=""application/x-shockwave-flash"" data=""{5}"" width=""{1}{2}"" height=""{3}{4}""><!--<![endif]--><param name=""flashvars"" value=""{6}"" />{7}{8}{9}";
		private const string SwffixEndTag				= @"<!--[if !IE]>--></object><!--<![endif]--></object>";
		private const string SwffixEmbeddedScriptTag	= 
		@"<script type=""text/javascript"">
		<!--//--><![CDATA[//><!--
			var params_{5} = {{
				flashvars: ""{0}"",
				scale: ""{1}"",
				salign:""{2}"",
				wmode: ""{3}""
			}};
			swfobject.embedSWF(""{4}"", ""{5}"", ""{6}{7}"", ""{8}{9}"", ""{10}.0.0"", {11}, null, params_{5});
		//--><!]]>
		</script>";
		#endregion

		#region Members
		private int										width;
		private int										height;
		private string									flashUrl;
		private DimensionUnit							unit								= DimensionUnit.Pixels;
		private string									scale;
		private string									sAlign;
		private bool									transparent							= true;
		private int										requiredVersion						= 7;
		private bool									autoInstall;
		private string									autoInstallMovie;
		private bool									disableScripts;
		private bool									disableMainScript;
		private Mode									renderMode							= Mode.SWFObject2Embedded;
		private WindowMode								wMode								= WindowMode.Transparent;

		private ControlHandler.RegisterTargetPreference scriptRegisterTargetPreference		= ControlHandler.RegisterTargetPreference.Head;

		private readonly NameValueCollection			flashParams;
		#endregion

		#region Properties
		public NameValueCollection						FlashParams						{ get { return this.flashParams; } }
		public string									FlashUrl						{ get { return this.flashUrl; }						set { this.flashUrl						= value; } }
		public int										Width							{ get { return this.width; }							set { this.width							= value; } }
		public int										Height							{ get { return this.height; }							set { this.height							= value; } }
		public DimensionUnit							Unit							{ get { return this.unit; }							set { this.unit							= value; } }
		public string									Scale							{ get { return this.scale; }							set { this.scale							= value; } }
		public string									SAlign							{ get { return this.sAlign; }							set { this.sAlign							= value; } }
		[Obsolete("Use WindowMode instead", false)]
		public bool										Transparent						{ get { return this.transparent; }						set { this.transparent						= value; } }
		public int										RequiredVersion					{ get { return this.requiredVersion; }					set { this.requiredVersion					= value; } }
		public Mode										RenderMode						{ get { return this.renderMode; }						set { this.renderMode						= value; } }
		public WindowMode								WMode							{ get { return this.wMode; }							set { this.wMode							= value; } }
		public bool										AutoInstall						{ get { return this.autoInstall; }						set { this.autoInstall						= value; } }
		public string									AutoInstallMovie				{ get { return this.autoInstallMovie; }				set { this.autoInstallMovie				= value; } }
		public bool										DisableScripts					{ get { return this.disableScripts; }					set { this.disableScripts					= value; } }
		public bool										DisableMainScript				{ get { return this.disableMainScript; }				set { this.disableMainScript				= value; } }
		public ControlHandler.RegisterTargetPreference	ScriptRegisterTargetPreference	{ get { return this.scriptRegisterTargetPreference; }	set { this.scriptRegisterTargetPreference	= value; } }
		#endregion

		#region Constructors
		public WebFlash() {
			this.TagName = HtmlTag.Div;
			this.RenderOriginalId = true;
			this.flashParams = new NameValueCollection();
		}

#pragma warning disable 168
		//This is needed for functioning (part of the contract of being an HtmlControl)
		//Should always be a DIV
		public WebFlash(string tag) : this(){}
#pragma warning restore 168
		#endregion

		#region Methods
		protected override void CreateChildControls() {
			if(!this.DisableScripts) {
				switch(this.RenderMode) {
					case Mode.SWFObject:
						if(!this.DisableMainScript) {
							ControlHandler.RegisterClientScriptBlock(this.GetType(), SwfObjectKey, SwfObjectInclude, this.ScriptRegisterTargetPreference);
						}
					break;
					case Mode.SWFObject2:
						if(!this.DisableMainScript) {
							ControlHandler.RegisterClientScriptBlock(this.GetType(), SwffixKey, SwffixInclude, this.ScriptRegisterTargetPreference);
						}
						ControlHandler.RegisterClientScriptBlock(this.GetType(), this.ClientID, this.BuildSWFFixScript(), this.ScriptRegisterTargetPreference);
						break;
					case Mode.SWFObject2Embedded:
						if(!this.DisableMainScript) {
							ControlHandler.RegisterClientScriptBlock(this.GetType(), SwffixKey, SwffixInclude, this.ScriptRegisterTargetPreference);
						}
						ControlHandler.RegisterClientScriptBlock(this.GetType(), this.ClientID, this.BuildSWFFixEmbeddedScript(), this.ScriptRegisterTargetPreference);
						break;
					default:
						if(!this.DisableMainScript) {
							ControlHandler.RegisterClientScriptBlock(this.GetType(), UfoKey, UfoInclude, this.ScriptRegisterTargetPreference);
						}
						ControlHandler.RegisterClientScriptBlock(this.GetType(), this.ClientID, this.BuildUFOScript(), this.ScriptRegisterTargetPreference);
						if(ControlHandler.IsIE) {
							ControlHandler.RegisterClientScriptBlock(this.GetType(), this.ClientID + "css", this.BuildUFOCss(), this.ScriptRegisterTargetPreference);
						}
						break;
				}
			}
		}

		protected override void RenderEndTag(HtmlTextWriter writer) {
			base.RenderEndTag(writer);
			if(this.RenderMode.Equals(Mode.SWFObject)) {
				writer.Write(this.BuildSwfObjectScript());
			}
		}

		private static string GetUnitString(DimensionUnit unit) {
			return GetUnitString(unit, false);
		}

		private static string GetUnitString(DimensionUnit unit, bool renderPixelSufix) {
			switch(unit) {
				case DimensionUnit.Pixels:
					return (renderPixelSufix ? "px" : string.Empty);
				case DimensionUnit.Percent:
					return "%";
			}
			return string.Empty;
		}

		protected override void RenderChildren(HtmlTextWriter writer) {
			if(this.RenderMode.Equals(Mode.SWFObject2)) {
				writer.Write(this.BuildSWFFixBeginTag());
				base.RenderChildren(writer);
				writer.Write(SwffixEndTag);
			}else if(this.RenderMode.Equals(Mode.SWFObject2Embedded)) {
				writer.Write("<div id=\"" + this.ClientID + "_inner\">");
				base.RenderChildren(writer);
				writer.Write("</div>");
			} else {
				base.RenderChildren(writer);
			}
		}
		#endregion

		#region UFO Helper Methods
		private string BuildUFOScript() {
			return string.Format(
				CultureInfo.InvariantCulture,
				UfoScriptTag,
				this.flashUrl,
				this.width, GetUnitString(this.unit),
				this.height, GetUnitString(this.unit),
				this.requiredVersion, 0,
				(this.scale ?? string.Empty),
				(this.sAlign ?? string.Empty),
				this.GetWModeString(),
				UrlHandler.GetQueryString(this.FlashParams, false),
				(this.autoInstall ? "true" : "false"),
				(string.IsNullOrEmpty(this.autoInstallMovie) ? UfoExpressinstall : this.autoInstallMovie),
				(ControlHandler.IsIE ? ", setcontainercss:\"true\"" : string.Empty),
				this.ClientID,
				(ControlHandler.IsIE ? " defer=\"true\"" : string.Empty));
		}

		private string BuildUFOCss() {
			return string.Format(CultureInfo.InvariantCulture, UfoCssTag, this.ClientID, this.width, GetUnitString(this.unit, true), this.height, GetUnitString(this.unit, true));
		}
		#endregion

		#region SWFFix Helper Methods
		private string BuildSWFFixScript() {
			return string.Format(
				CultureInfo.InvariantCulture,
				SwffixScriptTag,
				this.ClientID + "_inner",
				this.requiredVersion, 
				(this.AutoInstall ? "\"" + (string.IsNullOrEmpty(this.autoInstallMovie) ? SwffixExpressinstall : this.autoInstallMovie) + "\"" : "null"));
		}

		private string BuildSWFFixBeginTag() {
			string wmode = this.GetWModeString();

			return string.Format(
				CultureInfo.InvariantCulture,
				SwffixBeginTag,
				this.ClientID + "_inner",
				this.width, GetUnitString(this.unit),
				this.height, GetUnitString(this.unit),
				this.flashUrl,
				HttpUtility.HtmlEncode(UrlHandler.GetQueryString(this.FlashParams, false)),
				(!string.IsNullOrEmpty(this.scale) ? string.Format(@"<param name=""scale"" value=""{0}"" />", this.scale) : string.Empty),
				(!string.IsNullOrEmpty(this.sAlign) ? string.Format(@"<param name=""sAlign"" value=""{0}"" />", this.sAlign) : string.Empty),
				(!string.IsNullOrEmpty(wmode) ? string.Format(@"<param name=""wmode"" value=""{0}"" />", wmode) : string.Empty));
		}
		
		private string BuildSWFFixEmbeddedScript() {
			return string.Format(
				CultureInfo.InvariantCulture,
				SwffixEmbeddedScriptTag,
				UrlHandler.GetQueryString(this.FlashParams, false),
				(this.scale ?? string.Empty),
				(this.sAlign ?? string.Empty),
				this.GetWModeString(),
				this.flashUrl,
				this.ClientID + "_inner",
				this.width, GetUnitString(this.unit),
				this.height, GetUnitString(this.unit),
				this.requiredVersion, 
				(this.AutoInstall ? "\"" + (string.IsNullOrEmpty(this.autoInstallMovie) ? SwffixExpressinstall : this.autoInstallMovie) + "\"" : "null"));
		}
		#endregion

		#region SWFObject Helper Methods
		private string BuildSwfObjectScript() {
			string constructor = string.Format(CultureInfo.InvariantCulture, SwfConstructor, this.flashUrl, this.ClientID, SwfMovieNameSuffix, this.width, GetUnitString(this.unit), this.height, GetUnitString(this.unit), this.requiredVersion, string.Empty);
			string script = string.Format(CultureInfo.InvariantCulture, SwfScriptTag, constructor, BuildSwfAttributesScript(), BuildSwfPropertiesScript(), BuildSwfParamScript(), this.ClientID);

			return script;
		}

		private string BuildSwfAttributesScript() {
			StringBuilder attributes = new StringBuilder();

			if(AutoInstall) {
				string path = (string.IsNullOrEmpty(this.AutoInstallMovie) ? SwfExpressinstall : this.AutoInstallMovie);
				string install = string.Format(SwfAutoinstall, path);
				attributes.Append(install);
			}

			return attributes.ToString();
		}

		private string BuildSwfParamScript() {
			StringBuilder parameters = new StringBuilder();

			foreach(string key in FlashParams.Keys) {
				parameters.AppendFormat(SwfAddParam, key, FlashParams[key]);
			}

			return parameters.ToString();
		}

		private string BuildSwfPropertiesScript() {
			StringBuilder props = new StringBuilder();

			if(this.scale != null) {
				props.AppendFormat(SwfAddProperty, "scale", this.scale);
			}

			if(this.sAlign != null) {
				props.AppendFormat(SwfAddProperty, "salign", this.sAlign);
			}

			string wmode = GetWModeString();
			if(!wmode.Length.Equals(0)) {
				props.AppendFormat(SwfAddProperty, "wmode", wmode);
			}

			return props.ToString();
		}

		private string GetWModeString() {
			switch(this.WMode){
				case(WindowMode.Window):
					return string.Empty;
				case(WindowMode.Opaque):
					return "opaque";
				case(WindowMode.Gpu):
					return "gpu";
				case WindowMode.Direct:
					return "direct";
				default:
					return !this.transparent ? string.Empty : "transparent";
			}
		}
		#endregion

	}
}