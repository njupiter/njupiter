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

namespace nJupiter.Web.UI.GoogleAnalytics {
	public class Transaction {
		/// <summary>
		/// Country 
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// State or province
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// City to correlate the transaction with
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// The shipping amount of the transaction
		/// </summary>
		public decimal Shipping { get; set; }

		/// <summary>
		/// Tax amount of the transaction
		/// </summary>
		public decimal Tax { get; set; }

		/// <summary>
		/// Total amount of the transaction
		/// </summary>
		public decimal Total { get; set; }

		/// <summary>
		/// Optional partner or store affilation
		/// </summary>
		public string Affiliation { get; set; }

		/// <summary>
		/// Your internal unique order id number
		/// </summary>
		public string OrderId { get; set; }

		private readonly List<Item> items = new List<Item>();

		public List<Item> Items {
			get { return this.items; }
		}

		internal string GetLines() {
			string output = string.Format("UTM:T|{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}\r\n", OrderId, Affiliation, this.Total, this.Tax, this.Shipping, City, State, Country);
			foreach(Item item in Items) {
				output += item.GetLine(OrderId);
			}
			return output;
		}

	}
}
