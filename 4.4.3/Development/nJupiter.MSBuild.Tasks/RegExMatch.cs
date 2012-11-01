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

using System.Xml;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace nJupiter.MSBuild.Tasks {

	public class RegExMatch : Task {

		private string		m_InputString;
		private bool		m_Result;
		private string		m_Match;
		private bool		m_UseWildcards;

		private static Regex	s_WildcardRegEx = new Regex(@"[\*\+\?\(\)\|\\\!\.\[\]\^\{\}\$\#]");

		[Required]
		public string InputString {
			get { return m_InputString; }
			set { m_InputString = value; }
		}

		[Required]
		public string Match {
			get { return m_Match; }
			set { m_Match = value; }
		}

		public bool UseWildcards {
			get { return m_UseWildcards; }
			set { m_UseWildcards = value; }
		}

		[Output]
		public bool Result {
			get { return m_Result; }
			set { m_Result = value; }
		}

		public override bool Execute() {

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(m_Match);
			if(xmlDoc.DocumentElement.Name == "MatchAny") {
				foreach(XmlNode n in xmlDoc.DocumentElement.ChildNodes) {
					XmlElement e = n as XmlElement;
					if(e != null) {
						string pattern = e.InnerText;
						if(this.m_UseWildcards) {

							pattern = "^" + s_WildcardRegEx.Replace(pattern, new MatchEvaluator(this.ReplaceEscapeChar)) + "$";

							/*
							BuildMessageEventArgs args = new BuildMessageEventArgs(
								"-->" + pattern + "<--", string.Empty, "SetEnvironmentVariable", MessageImportance.Normal);
							this.BuildEngine.LogMessageEvent(args);
							*/

						}
						Regex re = new Regex(e.InnerText, RegexOptions.IgnoreCase);
						if(re.IsMatch(m_InputString)){
							m_Result = true;
							break;
						}
					}

				}
			}
			
			return true;
		}

		private string ReplaceEscapeChar(Match m) {
			string escapeChar = m.ToString();
			if(escapeChar == "*")
				return ".*";
			else
				return "\\" + escapeChar;
		}
	}

}
