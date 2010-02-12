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
	/// Utility class that encrypts and decrypts a string. It uses the DES3 algorithm with ECB cipher mode.
	/// </summary>
	public static class DES3 {
		/// <summary>
		///	Encrypt a string with a given key.
		/// </summary>
		/// <param name="source">String to encrypt.</param>
		/// <param name="key">Encryption key.</param>
		/// <returns>An encrypted string.</returns>
		public static string Encrypt(string source, string key) {
			TripleDESCryptoServiceProvider des = GetDESService(key);
			byte[] buff = Encoding.UTF8.GetBytes(source);
			return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));

		}

		/// <summary>
		/// Decrypt a string with a given key.
		/// </summary>
		/// <param name="source">String to decrypt.</param>
		/// <param name="key">Encryption key.</param>
		/// <returns>A decrypted string.</returns>
		public static string Decrypt(string source, string key) {
			TripleDESCryptoServiceProvider des = GetDESService(key);
			byte[] buff = Convert.FromBase64String(source);
			return Encoding.UTF8.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));
		}

		/// <summary>
		/// Instatiates and configures a TripleDESCryptoServiceProvider  object,
		/// </summary>
		/// <param name="key">Encryption key</param>
		/// <returns>A TripleDESCryptoServiceProvider object.</returns>
		private static TripleDESCryptoServiceProvider GetDESService(string key) {
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
			byte[] pwdhash = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));

			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			des.Key = pwdhash;
			des.Mode = CipherMode.ECB;
			return des;
		}
	}
}