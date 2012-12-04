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
		private const string SwfObjectKey = "swfobject";
		private const string SwfObjectInclude = "<script type=\"text/javascript\" src=\"" + WebFlashResourceRegistrator.SwfObjectJsPath + "\"></script><script type=\"text/javascript\">var so;</script>";
		// 0 = constructor, 1 = params, 2 = variables, 3 = div id
		private const string SwfScriptTag =
		@"<script type=""text/javascript"">
		<!--//--><![CDATA[//><!--
			{0}
			{1}
			{2}
			{3}
			so.write(""{4}"");
		//--><!]]>
		</script>";
		private const string SwfConstructor = "so = new SWFObject(\"{0}\", \"{1}{2}\", \"{3}{4}\", \"{5}{6}\", \"{7}\", \"{8}\");";
		// 0 = movie file, 1 = movie name, 2 = movie name suffix, 3 = width, 4 = width unit, 5 = height, 6 = height unit, 7 = flash version, 8 = bg color
		private const string SwfAddProperty = "so.addParam(\"{0}\", \"{1}\");"; // 0 = key, 1 = value
		private const string SwfAddParam = "so.addVariable(\"{0}\", \"{1}\");"; // 0 = key, 1 = value
		private const string SwfAutoinstall = "so.useExpressInstall(\"{0}\")";
		private const string SwfMovieNameSuffix = "movie";
		private const string SwfExpressinstall = WebFlashResourceRegistrator.SwfObjectSwfPath;

		private const string UfoKey = "ufoobject";
		private const string UfoExpressinstall = WebFlashResourceRegistrator.UfoSwfPath;
		private const string UfoInclude = "<script type=\"text/javascript\" src=\"" + WebFlashResourceRegistrator.UfoJsPath + "\"></script>";
		private const string UfoScriptTag =
		@"<script type=""text/javascript""{15}>
		<!--//--><![CDATA[//><!--
			var UFO_{14} = {{ movie:""{0}"", width:""{1}{2}"", height:""{3}{4}"", majorversion:""{5}"", build:""{6}"", scale:""{7}"", salign:""{8}"", wmode:""{9}"", flashvars:""{10}"", xi:""{11}"", ximovie:""{12}""{13} }};
			UFO.create(UFO_{14}, ""{14}"");
		//--><!]]>
		</script>";
		private const string UfoCssTag =
		@"<style type=""text/css"" media=""screen"">
			#{0} {{ width:{1}{2}; height:{3}{4}; overflow:auto; }}
		</style>";

		private const string SwffixKey = "swfobject2";
		private const string SwffixExpressinstall = WebFlashResourceRegistrator.SwfObject2SwfPath;
		private const string SwffixInclude = "<script type=\"text/javascript\" src=\""+ WebFlashResourceRegistrator.SwfObject2JsPath + "\"></script>";
		private const string SwffixScriptTag =
		@"<script type=""text/javascript"">
		<!--//--><![CDATA[//><!--
			swfobject.registerObject(""{0}"", ""{1}.0.0"", {2});
		//--><!]]>
		</script>";
		private const string SwffixBeginTag = @"<object id=""{0}"" classid=""clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"" width=""{1}{2}"" height=""{3}{4}""><param name=""movie"" value=""{5}"" /><!--[if !IE]>--><object type=""application/x-shockwave-flash"" data=""{5}"" width=""{1}{2}"" height=""{3}{4}""><!--<![endif]--><param name=""flashvars"" value=""{6}"" />{7}{8}{9}";
		private const string SwffixEndTag = @"<!--[if !IE]>--></object><!--<![endif]--></object>";
		private const string SwffixEmbeddedScriptTag =
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
		private int width;
		private int height;
		private string flashUrl;
		private DimensionUnit unit = DimensionUnit.Pixels;
		private string scale;
		private string sAlign;
		private bool transparent = true;
		private int requiredVersion = 7;
		private bool autoInstall;
		private string autoInstallMovie;
		private Mode renderMode = Mode.SWFObject2Embedded;
		private WindowMode wMode = WindowMode.Transparent;

		private RegisterTargetPreference scriptRegisterTargetPreference = RegisterTargetPreference.Head;

		private readonly NameValueCollection flashParams;
		#endregion

		#region Properties
		public NameValueCollection FlashParams { get { return flashParams; } }
		public string FlashUrl { get { return flashUrl; } set { flashUrl = value; } }
		public int Width { get { return width; } set { width = value; } }
		public int Height { get { return height; } set { height = value; } }
		public DimensionUnit Unit { get { return unit; } set { unit = value; } }
		public string Scale { get { return scale; } set { scale = value; } }
		public string SAlign { get { return sAlign; } set { sAlign = value; } }
		[Obsolete("Use WindowMode instead", false)]
		public bool Transparent { get { return transparent; } set { transparent = value; } }
		public int RequiredVersion { get { return requiredVersion; } set { requiredVersion = value; } }
		public Mode RenderMode { get { return renderMode; } set { renderMode = value; } }
		public WindowMode WMode { get { return wMode; } set { wMode = value; } }
		public bool AutoInstall { get { return autoInstall; } set { autoInstall = value; } }
		public string AutoInstallMovie { get { return autoInstallMovie; } set { autoInstallMovie = value; } }
		public bool DisableScripts { get; set; }
		public bool DisableMainScript { get; set; }
		[Obsolete("Use TargetPreference instead")]
		public ControlHandler.RegisterTargetPreference ScriptRegisterTargetPreference { get { return ControlHandler.GetOldTargetPreference(scriptRegisterTargetPreference); } set { scriptRegisterTargetPreference = ControlHandler.GetNewTargetPreference(value); } }
		public RegisterTargetPreference TargetPreference { get { return scriptRegisterTargetPreference; } set { scriptRegisterTargetPreference = value; } }
		#endregion

		#region Constructors
		public WebFlash() {
			WebFlashResourceRegistrator.Register();
			TagName = HtmlTag.Div;
			RenderOriginalId = true;
			flashParams = new NameValueCollection();
		}

#pragma warning disable 168
		// ReSharper disable UnusedParameter.Local
		//
		//This is needed for functioning (part of the contract of being an HtmlControl)
		//Should always be a DIV
		public WebFlash(string tag) : this() { }
		// ReSharper restore UnusedParameter.Local
#pragma warning restore 168
		#endregion

		#region Methods
		protected override void CreateChildControls() {
			if(!DisableScripts) {
				switch(RenderMode) {
					case Mode.SWFObject:
					if(!DisableMainScript) {
						ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), SwfObjectKey, SwfObjectInclude, TargetPreference);
					}
					break;
					case Mode.SWFObject2:
					if(!DisableMainScript) {
						ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), SwffixKey, SwffixInclude, TargetPreference);
					}
					ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), ClientID, BuildSWFFixScript(), TargetPreference);
					break;
					case Mode.SWFObject2Embedded:
					if(!DisableMainScript) {
						ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), SwffixKey, SwffixInclude, TargetPreference);
					}
					ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), ClientID, BuildSWFFixEmbeddedScript(), TargetPreference);
					break;
					default:
					if(!DisableMainScript) {
						ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), UfoKey, UfoInclude, TargetPreference);
					}
					ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), ClientID, BuildUFOScript(), TargetPreference);
					if(UserAgent.Instance.IsIE) {
						ClientScriptRegistrator.Instance.RegisterClientScriptBlock(GetType(), ClientID + "css", BuildUFOCss(), TargetPreference);
					}
					break;
				}
			}
		}

		protected override void RenderEndTag(HtmlTextWriter writer) {
			base.RenderEndTag(writer);
			if(RenderMode.Equals(Mode.SWFObject)) {
				writer.Write(BuildSwfObjectScript());
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
			if(RenderMode.Equals(Mode.SWFObject2)) {
				writer.Write(BuildSWFFixBeginTag());
				base.RenderChildren(writer);
				writer.Write(SwffixEndTag);
			} else if(RenderMode.Equals(Mode.SWFObject2Embedded)) {
				writer.Write("<div id=\"" + ClientID + "_inner\">");
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
				flashUrl,
				width, GetUnitString(unit),
				height, GetUnitString(unit),
				requiredVersion, 0,
				(scale ?? string.Empty),
				(sAlign ?? string.Empty),
				GetWModeString(),
				UrlHandler.Instance.GetQueryString(FlashParams, false),
				(autoInstall ? "true" : "false"),
				(string.IsNullOrEmpty(autoInstallMovie) ? UfoExpressinstall : autoInstallMovie),
				(UserAgent.Instance.IsIE ? ", setcontainercss:\"true\"" : string.Empty),
				ClientID,
				(UserAgent.Instance.IsIE ? " defer=\"true\"" : string.Empty));
		}

		private string BuildUFOCss() {
			return string.Format(CultureInfo.InvariantCulture, UfoCssTag, ClientID, width, GetUnitString(unit, true), height, GetUnitString(unit, true));
		}
		#endregion

		#region SWFFix Helper Methods
		private string BuildSWFFixScript() {
			return string.Format(
				CultureInfo.InvariantCulture,
				SwffixScriptTag,
				ClientID + "_inner",
				requiredVersion,
				(AutoInstall ? "\"" + (string.IsNullOrEmpty(autoInstallMovie) ? SwffixExpressinstall : autoInstallMovie) + "\"" : "null"));
		}

		private string BuildSWFFixBeginTag() {
			var wmode = GetWModeString();

			return string.Format(
				CultureInfo.InvariantCulture,
				SwffixBeginTag,
				ClientID + "_inner",
				width, GetUnitString(unit),
				height, GetUnitString(unit),
				flashUrl,
				HttpUtility.HtmlEncode(UrlHandler.Instance.GetQueryString(FlashParams, false)),
				(!string.IsNullOrEmpty(scale) ? string.Format(@"<param name=""scale"" value=""{0}"" />", scale) : string.Empty),
				(!string.IsNullOrEmpty(sAlign) ? string.Format(@"<param name=""sAlign"" value=""{0}"" />", sAlign) : string.Empty),
				(!string.IsNullOrEmpty(wmode) ? string.Format(@"<param name=""wmode"" value=""{0}"" />", wmode) : string.Empty));
		}

		private string BuildSWFFixEmbeddedScript() {
			return string.Format(
				CultureInfo.InvariantCulture,
				SwffixEmbeddedScriptTag,
				UrlHandler.Instance.GetQueryString(FlashParams, false),
				(scale ?? string.Empty),
				(sAlign ?? string.Empty),
				GetWModeString(),
				flashUrl,
				ClientID + "_inner",
				width, GetUnitString(unit),
				height, GetUnitString(unit),
				requiredVersion,
				(AutoInstall ? "\"" + (string.IsNullOrEmpty(autoInstallMovie) ? SwffixExpressinstall : autoInstallMovie) + "\"" : "null"));
		}
		#endregion

		#region SWFObject Helper Methods
		private string BuildSwfObjectScript() {
			var constructor = string.Format(CultureInfo.InvariantCulture, SwfConstructor, flashUrl, ClientID, SwfMovieNameSuffix, width, GetUnitString(unit), height, GetUnitString(unit), requiredVersion, string.Empty);
			var script = string.Format(CultureInfo.InvariantCulture, SwfScriptTag, constructor, BuildSwfAttributesScript(), BuildSwfPropertiesScript(), BuildSwfParamScript(), ClientID);

			return script;
		}

		private string BuildSwfAttributesScript() {
			var attributes = new StringBuilder();

			if(AutoInstall) {
				var path = (string.IsNullOrEmpty(AutoInstallMovie) ? SwfExpressinstall : AutoInstallMovie);
				var install = string.Format(SwfAutoinstall, path);
				attributes.Append(install);
			}

			return attributes.ToString();
		}

		private string BuildSwfParamScript() {
			var parameters = new StringBuilder();

			foreach(string key in FlashParams.Keys) {
				parameters.AppendFormat(SwfAddParam, key, FlashParams[key]);
			}

			return parameters.ToString();
		}

		private string BuildSwfPropertiesScript() {
			var props = new StringBuilder();

			if(scale != null) {
				props.AppendFormat(SwfAddProperty, "scale", scale);
			}

			if(sAlign != null) {
				props.AppendFormat(SwfAddProperty, "salign", sAlign);
			}

			var wmode = GetWModeString();
			if(!wmode.Length.Equals(0)) {
				props.AppendFormat(SwfAddProperty, "wmode", wmode);
			}

			return props.ToString();
		}

		private string GetWModeString() {
			switch(WMode) {
				case (WindowMode.Window):
				return string.Empty;
				case (WindowMode.Opaque):
				return "opaque";
				case (WindowMode.Gpu):
				return "gpu";
				case WindowMode.Direct:
				return "direct";
				default:
				return !transparent ? string.Empty : "transparent";
			}
		}
		#endregion

	}
}