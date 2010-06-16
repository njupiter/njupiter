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

using System.Collections;

namespace nJupiter.DataAccess.Users.Comparers.Users {

	public class NameComparer : IComparer {

		#region Members
		private readonly bool ascending;
		#endregion

		#region Constructors
		private NameComparer(bool ascending) {
			this.ascending = ascending;
		}
		#endregion

		#region Static Members
		private static NameComparer ascendingInstance;
		private static NameComparer descendingInstance;
		#endregion

		#region Singleton Instances
		public static NameComparer Ascending {
			get {
				if(ascendingInstance == null)
					ascendingInstance = new NameComparer(true);
				return ascendingInstance;
			}
		}

		public static NameComparer Descending {
			get {
				if(descendingInstance == null)
					descendingInstance = new NameComparer(false);
				return descendingInstance;
			}
		}
		#endregion

		#region IComparer Members
		public int Compare(object x, object y) {
			User xUser = x as User;
			User yUser = y as User;

			if(xUser == null || yUser == null)
				return 0;

			string xName = xUser.Properties.LastName ?? xUser.Properties.DisplayName;
			string yName = yUser.Properties.LastName ?? yUser.Properties.DisplayName;

			if(this.ascending)
				return xName.CompareTo(yName);
			return yName.CompareTo(xName);
		}
		#endregion

	}
}
