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

	public sealed class ThreadedPostsResultConfiguration {	
		#region Constants
		private const int					DefaultLevels				= -1;
		private const bool					DefaultIncludeHidden		= false;
		private const bool					DefaultLoadAttributes		= true;
		private const Post.DateProperty		DefaultDateFilterProperty	= Post.DateProperty.TimePosted;
		private static readonly	DateTime	defaultDateFilterFrom		= DateTime.MinValue;
		private static readonly	DateTime	defaultDateFilterTo			= DateTime.MaxValue;
		#endregion

		#region Variables
		private int					levels;
		#endregion

		#region Constructors
		public ThreadedPostsResultConfiguration(int levels, bool includeHidden, bool loadAttributes, Post.Property sortProperty, string sortAttributeName, bool sortAscending, DateTime dateFilterFrom, DateTime dateFilterTo, Post.DateProperty dateFilterProperty) {
			this.levels				= levels;
			this.IncludeHidden		= includeHidden;
			this.LoadAttributes		= loadAttributes;
			this.SortProperty		= sortProperty;
			this.SortAttributeName	= sortAttributeName;
			this.SortAscending		= sortAscending;
			this.DateFilterFrom		= dateFilterFrom;
			this.DateFilterTo		= dateFilterTo;
			this.DateFilterProperty	= dateFilterProperty;
		}
		public ThreadedPostsResultConfiguration(int levels, bool includeHidden, bool loadAttributes, Post.Property sortProperty, bool sortAscending) : this(levels, includeHidden, loadAttributes, sortProperty, null, sortAscending, defaultDateFilterFrom, defaultDateFilterTo, DefaultDateFilterProperty) {}
		public ThreadedPostsResultConfiguration(int levels, bool includeHidden, string sortAttributeName, bool sortAscending) : this(levels, includeHidden, DefaultLoadAttributes, Post.DefaultSortProperty, sortAttributeName, sortAscending, defaultDateFilterFrom, defaultDateFilterTo, DefaultDateFilterProperty) {}
		public ThreadedPostsResultConfiguration(int levels) : this(levels, DefaultIncludeHidden, DefaultLoadAttributes, Post.DefaultSortProperty, Post.DefaultSortAscending) {}
		public ThreadedPostsResultConfiguration(bool includeHidden, bool loadAttributes, Post.Property sortProperty, bool sortAscending) : this(DefaultLevels, includeHidden, loadAttributes, sortProperty, sortAscending) {}
		public ThreadedPostsResultConfiguration(bool includeHidden, string sortAttributeName, bool sortAscending) : this(DefaultLevels, includeHidden, sortAttributeName, sortAscending) {}
		public ThreadedPostsResultConfiguration() : this(DefaultLevels) {}
		public ThreadedPostsResultConfiguration(DateTime dateFilterFrom, DateTime dateFilterTo, Post.DateProperty dateFilterProperty) : this(DefaultLevels, DefaultIncludeHidden, DefaultLoadAttributes, Post.DefaultSortProperty, null, Post.DefaultSortAscending, dateFilterFrom, dateFilterTo, dateFilterProperty) {}
		#endregion

		#region Properties
		public int Levels { 
			get { return this.levels; } 
			set {
				if(value < -1) {
					throw new ArgumentOutOfRangeException("value");
				}
				this.levels = value; 
			} 
		}

		public bool IncludeHidden { get; set; }
		public bool LoadAttributes { get; set; }
		public Post.Property SortProperty { get; set; }
		public string SortAttributeName { get; set; }
		public bool SortAscending { get; set; }
		public DateTime DateFilterFrom { get; set; }
		public DateTime DateFilterTo { get; set; }
		public Post.DateProperty DateFilterProperty { get; set; }
		#endregion
	}

}