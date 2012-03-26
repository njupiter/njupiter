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

namespace nJupiter.Text {

	public static class TextHandler {
		public static bool IsAscii(string value) {
			if(value == null)
				throw new ArgumentNullException("value");

			foreach(char chr in value) {
				if(chr > '\x007f')
					return false;
			}
			return true;
		}

		public static bool IsAnsi(string value) {
			if(value == null)
				throw new ArgumentNullException("value");

			foreach(char chr in value) {
				if(chr > '\x00ff')
					return false;
			}
			return true;
		}

		/// <summary>
		/// Encode a string to QuotedPrintable as explained in RFC2045
		/// </summary>
		/// <param name="value">The string that shall be encoded</param>
		/// <returns>An encoded string</returns>
		/// <seealso cref="http://en.wikipedia.org/wiki/Quoted-printable"/>
		public static string EncodeToQuotedPrintable(string value) {
			return EncodeToQuotedPrintable(value, Encoding.UTF8, false);
		}



		/// <summary>
		/// Encode a string to QuotedPrintable as explained in RFC2045
		/// </summary>
		/// <param name="value">The string that shall be encoded</param>
		/// <param name="singleLineEncoding">If single line encoding, then replace all spaces with underscore</param>
		/// <returns>An encoded string</returns>
		/// <seealso cref="http://en.wikipedia.org/wiki/Quoted-printable"/>
		public static string EncodeToQuotedPrintable(string value, bool singleLineEncoding) {
			return EncodeToQuotedPrintable(value, Encoding.UTF8, singleLineEncoding);
		}

		/// <summary>
		/// Encode a string to QuotedPrintable as explained in RFC2045
		/// </summary>
		/// <param name="value">The string that shall be encoded</param>
		/// <param name="encoding">The encoding of the source string</param>
		/// <returns>An encoded string</returns>
		public static string EncodeToQuotedPrintable(string value, Encoding encoding) {
			return EncodeToQuotedPrintable(value, encoding, false);
		}

		/// <summary>
		/// Encode a string to QuotedPrintable as explained in RFC2045
		/// </summary>
		/// <param name="value">The string that shall be encoded</param>
		/// <param name="encoding">The encoding of the source string</param>
		/// <param name="singleLineEncoding">If single line encoding, then replace all spaces with underscore</param>
		/// <returns>An encoded string</returns>
		public static string EncodeToQuotedPrintable(string value, Encoding encoding, bool singleLineEncoding) {
			if(value == null)
				return null;
			if(encoding == null)
				encoding = Encoding.UTF8;
			byte[] buffer = encoding.GetBytes(value);
			return EncodeToQuotedPrintable(buffer, singleLineEncoding);
		}

		private static string EncodeToQuotedPrintable(byte[] bytes, bool singleLineEncoding) {
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < bytes.Length; i++) {
				byte current = bytes[i];
				byte next = (bytes.Length > (i + 1) ? bytes[i + 1] : (byte)0);
				if(current >= 33 && current <= 126 && current != 61 && current != 95) { // do not encode visible charachters
					sb.Append((char)current);
				} else {
					if(!singleLineEncoding && current == 95) { // If not single line encoding, no need to encode underscore
						sb.Append((char)current);
					} else if(current >= 10 && current <= 13) { // If line feed or return do not encode
						sb.Append((char)current);
					} else if((current == 9 || current == 32) && !(next >= 10 && next <= 13)) { // if space or tab in the middle of a string, do not encode
						if(singleLineEncoding && current == 32)
							sb.Append("_");
						else
							sb.Append((char)current);
					} else {
						sb.Append("=");
						sb.Append(IntToHex((current >> 4) & 15));
						sb.Append(IntToHex(current & 15));
					}
				}
			}
			return sb.ToString();
		}

		private static char IntToHex(int n) {
			if(n <= 9)
				return (char)((ushort)(n + 0x30));
			return (char)((ushort)((n - 10) + 0x61));
		}
	}
}
