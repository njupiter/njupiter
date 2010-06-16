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
using System.IO;
using System.Runtime.InteropServices;

using nJupiter.Web;

namespace nJupiter.IO {

	public static class FileHandler {
		#region Constructors
		#endregion

		#region Methods
		///	<summary>
		///	</summary>
		/// <param name="stream">Stream containing the file</param>
		/// <returns>Returns a MimeType object</returns>
		public static MimeType GetMimeType(Stream stream) {
			if(stream == null) {
				throw new ArgumentNullException("stream");
			}

			int maxContent = (int)stream.Length;

			if(maxContent > 4096)
				maxContent = 4096;

			byte[] buf = new byte[maxContent];
			stream.Read(buf, 0, maxContent);

			string mime;

			//note: the CLR frees the data automatically returned in ppwzMimeOut     
			int result = NativeMethods.FindMimeFromData(IntPtr.Zero, null, buf, maxContent, null, 0, out mime, 0);

			if(result != 0)
				Marshal.ThrowExceptionForHR(result);

			if(mime != null && mime.IndexOf("/") > 0)
				return new MimeType(mime);

			return null;
		}
		/// <summary>
		/// Ensures that file exists and retrieves the content type
		/// </summary>
		/// <param name="file">FileInfo object containing the file</param>
		/// <returns>Returns a MimeType object</returns>
		public static MimeType GetMimeType(FileInfo file) {
			if(file == null) {
				throw new ArgumentNullException("file");
			}
			if(!file.Exists) {
				throw new FileNotFoundException(file + " not found");
			}

			using(FileStream fs = file.OpenRead()) {
				return GetMimeType(fs);
			}
		}
		public static MimeType GetMimeType(byte[] bytes) {
			if(bytes == null) {
				throw new ArgumentNullException("bytes");
			}
			using(Stream s = new MemoryStream(bytes)) {
				return GetMimeType(s);
			}
		}

		public static string FormatSize(long bytes) {
			double size = bytes;
			double result = size / 1024;
			if(result > 1) {
				size = result;
				result = size / 1024;
				if(result > 1) {
					size = result;
					return Math.Round(size, 1) + " MB";
				}
				return Math.Round(size) + " kB";
			}
			return bytes + " B";
		}
		#endregion
	}

	internal static class NativeMethods {
		[DllImport("urlmon.dll", EntryPoint = "FindMimeFromData", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
		internal static extern int FindMimeFromData(IntPtr pbc,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
			[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
			int cbSize,
			[MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed, int dwMimeFlags,
			[MarshalAs(UnmanagedType.LPWStr)] out string ppwzMimeOut,
			int dwReserved);

	}
}
