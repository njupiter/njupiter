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

using System;

namespace nJupiter.Services.Forum {

	public sealed class AttributeCriterion {
		#region Constants
		private const Comparison DefaultComparison = Comparison.Equal;
		private const bool DefaultRequired = true;
		#endregion

		#region Variables
		private Attribute attribute;
		private Comparison comparison;
		#endregion

		#region Constructors
		public AttributeCriterion(Attribute attribute) : this(attribute, DefaultComparison, DefaultRequired) { }
		public AttributeCriterion(Attribute attribute, Comparison comparison, bool required) {
			this.comparison = comparison; //order is significant here
			this.Attribute = attribute;
			this.Required = required;
		}
		#endregion

		#region Properties
		public Attribute Attribute {
			get { return this.attribute; }
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				CheckAttributeComparison(value, this.Comparison);
				this.attribute = value;
			}
		}
		public Comparison Comparison {
			get { return this.comparison; }
			set {
				CheckAttributeComparison(this.Attribute, value);
				this.comparison = value;
			}
		}
		public bool Required { get; set; }
		#endregion

		#region Helper Methods
		private static void CheckAttributeComparison(Attribute attribute, Comparison comparison) {
			switch(comparison) {
				case Comparison.Greater:
				case Comparison.GreaterEqual:
				case Comparison.Less:
				case Comparison.LessEqual:
				if(!typeof(IComparable).IsAssignableFrom(attribute.AttributeValueType)) {
					throw new InvalidOperationException("Can not use inequality comparison on an attribute whose underlying type is not comparable.");
				}
				break;
			}
		}
		#endregion
	}

}
