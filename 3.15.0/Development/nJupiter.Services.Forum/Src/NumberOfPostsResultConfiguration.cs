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

namespace nJupiter.Services.Forum {

	public sealed class NumberOfPostsResultConfiguration {
		#region Constants
		private const bool					DefaultIncludeHidden	= false;
		private static readonly	DateTime	defaultDateFilterFrom	= DateTime.MinValue;
		private static readonly	DateTime	defaultDateFilterTo		= DateTime.MaxValue;
		#endregion

		#region Constructors
		public NumberOfPostsResultConfiguration(bool includeHidden, DateTime dateFilterFrom, DateTime dateFilterTo) {
			this.IncludeHidden	= includeHidden;
			this.DateFilterFrom	= dateFilterFrom;
			this.DateFilterTo	= dateFilterTo;
		}
		public NumberOfPostsResultConfiguration(bool includeHidden) : this(includeHidden, defaultDateFilterFrom, defaultDateFilterTo) {}
		public NumberOfPostsResultConfiguration(DateTime dateFilterFrom, DateTime dateFilterTo) : this(DefaultIncludeHidden, dateFilterFrom, dateFilterTo) {}
		public NumberOfPostsResultConfiguration() : this(DefaultIncludeHidden) {}
		#endregion

		#region Properties
		public bool IncludeHidden { get; set; }
		public DateTime DateFilterFrom { get; set; }
		public DateTime DateFilterTo { get; set; }
		#endregion
	}

}