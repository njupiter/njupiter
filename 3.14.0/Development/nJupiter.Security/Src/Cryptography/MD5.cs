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
using System.Security.Cryptography;
using System.Text;

namespace nJupiter.Security.Cryptography {
	/// <summary>
	/// Utility class for creating Cryptography.
	/// </summary>
	public static class MD5 {
		/// <summary>
		/// Creates an md5 hex hash string of any given string.
		/// </summary>
		public static string HashToString(string text) {
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hashArray = md5.ComputeHash(Encoding.Unicode.GetBytes(text));
			return BitConverter.ToString(hashArray).Replace("-", string.Empty);
		}

	}
}
