
var EmailObfuscator = new function(){ 

	var m_AnchorEnd		= "&encanchor=true";
	var m_ImgEnd		= "&encimage=true";
	var m_AnchorStr		= "?encstr=";
	var m_Base64Chars	= "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

	this.Prepare = function(){
		var node = document.documentElement;
		if(!node){
			node = document.getElementsByTagName("body")[0];
		}
		if(node.className == null || node.className.match(/\bhasJavascript\b/) == null){
			var append = "hasJavascript";
			var className = node.className;
			node.className = className == null ? append : className.replace(/\s+/g, " ") + (className == "" ? "" : " ") + append;
		}
	}

	this.OnLoad = function(){
		EmailObfuscator.InitializeImages();
		EmailObfuscator.InitializeLinks();
	}
	
	this.InitializeLinks = function(){

		if (!document.getElementsByTagName)
			return;

		var anchors = document.getElementsByTagName("a");

		for (var i = 0; i < anchors.length; i++) {
			var anchor = anchors[i];
			var href = anchor.getAttribute("href");
			if(href != null){
				var lastIndex = href.lastIndexOf(m_AnchorEnd);
				if(lastIndex > 0 && (href.length - lastIndex - m_AnchorEnd.length) == 0){
					var index = href.lastIndexOf(m_AnchorStr);
					if(index > 0){
						var enc = unescape(href.substring(index + m_AnchorStr.length, lastIndex));
						var dec = EmailObfuscator.Base64ToText(enc);
						anchor.href = "mailto:" + dec;
					}
				}
			}
		}
	}
	
	this.InitializeImages = function(){

		if (!document.getElementsByTagName)
			return;

		var imgs	= document.getElementsByTagName("img");
		var eImgs	= new Array();

		for (var i = 0; i < imgs.length; i++) {
			var img = imgs[i];
			var src = img.getAttribute("src");
			if(src != null){
				var lastIndex = src.lastIndexOf(m_ImgEnd);
				if(lastIndex > 0 && (src.length - lastIndex - m_ImgEnd.length) == 0){
					var index = src.lastIndexOf(m_AnchorStr);
					if(index > 0){
						var enc = unescape(src.substring(index + m_AnchorStr.length, lastIndex));
						var dec = EmailObfuscator.Base64ToText(enc);
						var element = document.createElement("span");
						element.innerHTML = dec;
						img.parentNode.appendChild(element);
						eImgs.push(img);
					}
				}
			}
		}
		for (var i = 0; i < eImgs.length; i++) {
			var img = eImgs[i];
			img.parentNode.removeChild(img);
		}

	}
	
	// Function by http://www.webtoolkit.info/
	this.Base64ToText = function(input){
		var output = "";
		var chr1, chr2, chr3;
		var enc1, enc2, enc3, enc4;
		var i = 0;

		input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

		do {
		  enc1 = m_Base64Chars.indexOf(input.charAt(i++));
		  enc2 = m_Base64Chars.indexOf(input.charAt(i++));
		  enc3 = m_Base64Chars.indexOf(input.charAt(i++));
		  enc4 = m_Base64Chars.indexOf(input.charAt(i++));

		  chr1 = (enc1 << 2) | (enc2 >> 4);
		  chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
		  chr3 = ((enc3 & 3) << 6) | enc4;

		  output = output + String.fromCharCode(chr1);

		  if (enc3 != 64) {
			 output = output + String.fromCharCode(chr2);
		  }
		  if (enc4 != 64) {
			 output = output + String.fromCharCode(chr3);
		  }
		} while (i < input.length);

		return output;
	}	
}
EmailObfuscator.Prepare();
if(document.addEventListener)
	document.addEventListener("load", EmailObfuscator.OnLoad, false);
if(window.addEventListener)
	window.addEventListener("load", EmailObfuscator.OnLoad, false);
else if(window.attachEvent)
	window.attachEvent("onload", EmailObfuscator.OnLoad);

