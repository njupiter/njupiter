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
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;


namespace nJupiter.MSBuild.Tasks {

	public class AddCompilerConstants : Task {

		private string		m_ProjectPath;
		private ITaskItem[]	m_ConstantMaps;
		private string		m_CurrentConstants;
		private string		m_Result;
		private ITaskItem[] m_References;

		[Required]
		public string ProjectPath { get { return m_ProjectPath; } set { m_ProjectPath = value; } }

		[Required]
		public ITaskItem[] References { get { return m_References; } set { m_References = value; } }

		[Required]
		public ITaskItem[] ConstantMaps { get { return m_ConstantMaps; } set { m_ConstantMaps = value; } }

		[Required]
		public string CurrentConstants { get { return m_CurrentConstants; } set { m_CurrentConstants = value; } }
	
		[Output]
		public string Result { get { return m_Result; } set { m_Result = value; } }


		public override bool Execute() {
			this.Result = this.CurrentConstants;
			foreach(ITaskItem reference in this.References) {
				foreach(ITaskItem constantMap in this.ConstantMaps) {
					string strRef = reference.ToString();
					string cstRef = constantMap.ToString();
					if(String.Compare(strRef, cstRef, true) == 0 || strRef.StartsWith(cstRef + ",", StringComparison.InvariantCultureIgnoreCase)) {
						string hintPath = reference.GetMetadata("HintPath");
						if(hintPath != null && hintPath.Length > 0) {
							hintPath = Path.Combine(this.ProjectPath, hintPath);
							Assembly assembly = Assembly.ReflectionOnlyLoadFrom(hintPath);
							Version maxVersion = new Version(constantMap.GetMetadata("MaxVersion"));
							Version minVersion = new Version(constantMap.GetMetadata("MinVersion"));
							Version currentVersion = assembly.GetName().Version;
							if(currentVersion >= minVersion && currentVersion <= maxVersion) {
								this.Result = this.Result + ";" + constantMap.GetMetadata("Constants");
							}
						}
					}
				}
			}			
			return true;
		}	

	}
}
