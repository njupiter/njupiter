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
using System.Text;
using System.Globalization;
using System.Collections.Specialized;

namespace nJupiter.Web {
	/// <summary>
	/// MimeType object implemented after RFC2046 and RFC2616
	/// </summary>
	/// <seealso cref="http://www.ietf.org/rfc/rfc2046.txt"/>
	/// <seealso cref="http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html"/>
	public class MimeType : IMimeType {

		#region Members
		private readonly string type;
		private readonly string discreteType;
		private readonly string compositeType;
		private readonly NameValueCollection parameters;
		#endregion

		#region Properties
		public string Type { get { return this.type; } }
		public string DiscreteType { get { return this.discreteType; } }
		public string CompositeType { get { return this.compositeType; } }
		public NameValueCollection Parameters { get { return this.parameters; } }

		#region Auto Generated Properties
		//This properties are generated on the fly because the object isn't fully imutable
		// and that the parameters collection can and should be able to change on the fly
		public string ContentType {
			get {
				var contentType = new StringBuilder(this.type);
				foreach(var paramKey in this.parameters.AllKeys) {
					var paramValues = this.parameters[paramKey].Split(',');
					foreach(var paramValue in paramValues) {
						contentType.Append("; ");
						contentType.Append(paramKey);
						contentType.Append("=");
						contentType.Append(paramValue);
					}
				}
				return contentType.ToString();
			}
		}

		public decimal Quality {
			get {
				if(this.parameters["q"] != null) {
					double q;
					var paramValue = this.parameters["q"].Split(',')[0]; // According to RFC2616 the first q is always the quality. Other counts as usual parameters
					if(double.TryParse(paramValue, NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out q)) {
						return new decimal(q);
					}
				}
				return 1;
			}
		}

		public Encoding CharSet {
			get {
				if(this.parameters["charset"] != null) {
					var paramValue = this.parameters["charset"].Split(',')[0];
					return Encoding.GetEncoding(paramValue);
				}
				return Encoding.UTF8;
			}
		}
		#endregion
		#endregion

		#region Constructors
		public MimeType(string mimeType, decimal quality) : this(mimeType + "; q=" + quality.ToString("0.###", NumberFormatInfo.InvariantInfo)) { }
		public MimeType(string mimeType, Encoding charSet) : this(mimeType + (charSet == null ? string.Empty : "; charset=" + charSet.BodyName)) { }

		public MimeType(string mimeType) {
			if(mimeType == null)
				throw new ArgumentNullException("mimeType");

			var types = mimeType.Split(';', ':');

			this.parameters = new NameValueCollection();

			this.type = types[0].Trim();

			var mime = this.type.Split('/');

			if(mime.Length != 2)
				throw new ArgumentException("Parameter value [" + mimeType + "] is not a mime-type.", "mimeType");

			this.discreteType = mime[0].Trim();
			this.compositeType = mime[1].Trim();

			if(types.Length > 1) {
				for(var i = 1; i < types.Length; i++) {
					var acceptParam = types[i];
					var aParam = acceptParam.Split('=');
					if(aParam.Length == 2) {
						var paramKey = aParam[0].Trim();
						var paramValue = aParam[1].Trim();
						this.parameters.Add(paramKey, paramValue);
					}
				}
			}
		}
		#endregion

		#region Methods
		public override bool Equals(object obj) {
			var mimeType = obj as IMimeType;
			if(mimeType == null)
				return false;
			return this.ContentType.Equals(mimeType.ContentType);
		}

		public override int GetHashCode() {
			var result = 17;
			result = (37 * result) + this.ContentType.GetHashCode();
			return result;
		}

		public bool EqualsType(IMimeType mimeType) {
			return mimeType != null &&
				this.DiscreteType.Equals(mimeType.DiscreteType) &&
				(this.CompositeType.Equals(mimeType.CompositeType) || this.CompositeType.Equals("*") || mimeType.CompositeType.Equals("*"));
		}

		public bool EqualsExactType(IMimeType mimeType) {
			return mimeType != null &&
				this.DiscreteType.Equals(mimeType.DiscreteType) &&
				this.CompositeType.Equals(mimeType.CompositeType);
		}

		public override string ToString() {
			return this.ContentType;
		}
		#endregion

	}
}
