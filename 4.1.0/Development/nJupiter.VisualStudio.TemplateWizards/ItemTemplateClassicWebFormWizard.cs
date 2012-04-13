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

using System.Collections.Generic;
using Microsoft.VisualStudio.TemplateWizard;

using EnvDTE;

namespace nJupiter.VisualStudio.TemplateWizards {

	public class ItemTemplateClassicWebFormWizard : IWizard {

		ProjectItem m_Form;
		ProjectItem m_CodeBehind;

		// This method is called before opening any item that 
		// has the OpenInEditor attribute.
		public void BeforeOpeningFile(ProjectItem projectItem) {
		}

		public void ProjectFinishedGenerating(Project project) {
		}

		// This method is only called for item templates,
		// not for project templates.
		public void ProjectItemFinishedGenerating(ProjectItem projectItem) {
			if(projectItem.FileCount == 1) {
				string fileName = projectItem.get_FileNames(0);
				if(fileName.EndsWith(".aspx") ||
					fileName.EndsWith(".ascx") ||
					fileName.EndsWith(".asax") ||
					fileName.EndsWith(".asmx")) {
					
					m_Form = projectItem;
				}
				if(fileName.EndsWith(".cs")) {
					m_CodeBehind = projectItem;
				}

			}
		}

		// This method is called after the project is created.
		public void RunFinished() {
			if(m_Form != null && m_CodeBehind != null) {
				m_Form.ProjectItems.AddFromFile(m_CodeBehind.get_FileNames(0));
			}
		}

		public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {

		}

		// This method is only called for item templates,
		// not for project templates.
		public bool ShouldAddProjectItem(string filePath) {
			return true;
		}

	}
}
