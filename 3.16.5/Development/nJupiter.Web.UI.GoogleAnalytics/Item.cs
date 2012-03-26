#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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

namespace nJupiter.Web.UI.GoogleAnalytics {
	public class Item {
		/// <summary>
		/// Product SKU code
		/// </summary>
		public string SKU { get; set; }

		/// <summary>
		/// Product name or description
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Category of the product or variation
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Unit-price of the product
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Quantity ordered
		/// </summary>
		public int Quantity { get; set; }

		public Item(string sku, string name, string category, decimal price, int quantity) {
			SKU = sku;
			Name = name;
			Category = category;
			Price = price;
			Quantity = quantity;
		}

		internal string GetLine(string orderId) {
			return string.Format("UTM:I|{0}|{1}|{2}|{3}|{4}|{5}\r\n", orderId, SKU, Name, Category, this.Price, this.Quantity);
		}
	}
}
