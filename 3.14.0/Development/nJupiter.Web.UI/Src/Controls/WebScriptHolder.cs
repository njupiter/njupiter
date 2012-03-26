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

namespace nJupiter.Web.UI.Controls {
	public class WebScriptHolder : Control {
		#region Members
		private ListDictionary scriptBlocks;
		private readonly object padlock = new object();
		#endregion

		#region Constructors
		public WebScriptHolder() {
			if(HttpContext.Current != null) {
				HttpContext.Current.Items[typeof(WebScriptHolder)] = this;
			}
		}
		#endregion

		#region Properties
		private ListDictionary ScriptBlocks {
			get {
				if(this.scriptBlocks != null)
					return this.scriptBlocks;
				lock(this.padlock) {
					if(this.scriptBlocks == null) {
						this.scriptBlocks = new ListDictionary();
					}
				}
				return this.scriptBlocks;
			}
		}
		#endregion
		
		#region Methods
		protected override void Render(HtmlTextWriter writer) {
			base.Render(writer);
			if(this.scriptBlocks != null) {
				foreach(string script in this.scriptBlocks.Values) {
					writer.WriteLine(script);
					writer.WriteLine();
				}
			}
		}
		public void RegisterClientScriptBlock(Type type, string key, string script) {
			ScriptKey scriptKey = new ScriptKey(type, key);
			if(this.ScriptBlocks[scriptKey] == null) {
				this.ScriptBlocks.Add(scriptKey, script);
			}
		}
		#endregion

		#region Helper Classes
		private sealed class ScriptKey {
			private readonly string		key;
			private readonly Type		type;
			
			internal ScriptKey(Type type, string key) {
				this.type		= type;
				if(key == null) {
					key = string.Empty;
				}
				this.key		= key;
			}

			public override bool Equals(object obj) {
				ScriptKey scriptKey = (ScriptKey)obj;
				return ((scriptKey.type == this.type) && (scriptKey.key == this.key));
			}

			public override int GetHashCode() {
				int result = 17;
				result = (37*result) + this.type.GetHashCode();
				result = (37*result) + this.key.GetHashCode();
				return result;
			}
		}
		#endregion
	}
}
