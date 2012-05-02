#region Copyright & License
/*
	Copyright (c) 2005-2012 nJupiter

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
using System.Web.UI;

namespace nJupiter.Web.UI {
	public interface IControlFinder {
		Control FindControl(Control rootControl, string uniqueId);
		Control FindControl(Control rootControl, string uniqueId, bool ensureChildControls);
		T[] FindControlsOnType<T>(Control rootControl);
		T[] FindControlsOnType<T>(Control rootControl, bool ensureChildControls);
		T FindFirstControlOnType<T>(Control rootControl);
		T FindFirstControlOnType<T>(Control rootControl, bool ensureChildControls);
		Control[] FindControlsOnType(Control rootControl, Type type);
		Control[] FindControlsOnType(Control rootControl, Type type, bool ensureChildControls);
		Control FindFirstControlOnType(Control rootControl, Type type);
		Control FindFirstControlOnType(Control rootControl, Type type, bool ensureChildControls);
	}
}