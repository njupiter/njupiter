#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System.Collections;

namespace nJupiter.DataAccess.Users.Comparers {
	public class NameComparer : IComparer {
		private readonly bool ascending;

		private NameComparer(bool ascending) {
			this.ascending = ascending;
		}

		private static NameComparer ascendingInstance;
		private static NameComparer descendingInstance;

		public static NameComparer Ascending {
			get {
				if(ascendingInstance == null) {
					ascendingInstance = new NameComparer(true);
				}
				return ascendingInstance;
			}
		}

		public static NameComparer Descending {
			get {
				if(descendingInstance == null) {
					descendingInstance = new NameComparer(false);
				}
				return descendingInstance;
			}
		}

		public int Compare(object x, object y) {
			var xUser = x as IUser;
			var yUser = y as IUser;

			if(xUser == null || yUser == null) {
				return 0;
			}

			var xName = xUser.Properties.LastName ?? xUser.Properties.DisplayName;
			var yName = yUser.Properties.LastName ?? yUser.Properties.DisplayName;

			if(@ascending) {
				return xName.CompareTo(yName);
			}
			return yName.CompareTo(xName);
		}
	}
}