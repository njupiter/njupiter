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

using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace nJupiter.MSBuild.Tasks {
	
	public class CleanDirectory : Task {

		private string		m_Directory;
		private ITaskItem[] m_Files;

		public string Directory { get { return m_Directory; } set { m_Directory = value; } }

		[Required]
		public ITaskItem[] Files { get { return m_Files; } set { m_Files = value; } }

		public override bool Execute() {
			if(this.Directory != null && this.Directory.Length > 0) {
				DirectoryInfo dir = new DirectoryInfo(this.Directory);
				foreach(ITaskItem item in this.Files) {
					FileInfo[] files = dir.GetFiles(item.ToString());
					foreach(FileInfo file in files) {
						file.Delete();
					}
				}
			} else {
				foreach(ITaskItem item in this.Files) {
					FileInfo file = new FileInfo(item.ToString());
					file.Delete();
				}
			}
			return true;
		}
	}
}
