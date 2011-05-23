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
using System.Globalization;

namespace nJupiter.DataAccess.Users {

	public class SearchCriteria {

		private IProperty property;
		private bool required;
		private string domain;
		private CompareCondition condition;

		public SearchCriteria(IProperty property, string domain) : this(property, domain, CompareCondition.Equal, false) { }
		public SearchCriteria(IProperty property, string domain, CompareCondition condition) : this(property, domain, condition, false) { }
		public SearchCriteria(IProperty property, string domain, bool required) : this(property, domain, CompareCondition.Equal, required) { }
		public SearchCriteria(IProperty property, string domain, CompareCondition condition, bool required) {
			this.InitCriteria(property, domain, condition, required);
		}

		public SearchCriteria(IProperty property) : this(property, CompareCondition.Equal, false) { }
		public SearchCriteria(IProperty property, CompareCondition condition) : this(property, condition, false) { }
		public SearchCriteria(IProperty property, bool required) : this(property, CompareCondition.Equal, required) { }
		public SearchCriteria(IProperty property, CompareCondition condition, bool required) {
			this.InitCriteria(property, null, condition, required);
		}

		public SearchCriteria(string propertyName, string propertyValue, string domain) : this(propertyName, propertyValue, domain, null, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, bool required) : this(propertyName, propertyValue, domain, null, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, CompareCondition condition) : this(propertyName, propertyValue, domain, null, condition, false) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, CompareCondition condition, bool required) : this(propertyName, propertyValue, domain, null, condition, required) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, Context context) : this(propertyName, propertyValue, domain, context, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, Context context, bool required) : this(propertyName, propertyValue, domain, context, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, Context context, CompareCondition condition) : this(propertyName, propertyValue, domain, context, condition, false) { }

		public SearchCriteria(string propertyName, string propertyValue) : this(propertyName, propertyValue, null, null, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, bool required) : this(propertyName, propertyValue, null, null, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, CompareCondition condition) : this(propertyName, propertyValue, null, null, condition, false) { }
		public SearchCriteria(string propertyName, string propertyValue, CompareCondition condition, bool required) : this(propertyName, propertyValue, null, null, condition, required) { }
		public SearchCriteria(string propertyName, string propertyValue, Context context) : this(propertyName, propertyValue, null, context, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, Context context, bool required) : this(propertyName, propertyValue, null, context, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, Context context, CompareCondition condition) : this(propertyName, propertyValue, null, context, condition, false) { }

		public SearchCriteria(string propertyName, string propertyValue, string domain, Context context, CompareCondition condition, bool required) {
			if(propertyName == null) {
				throw new ArgumentNullException("propertyName");
			}
			var stringProperty = new GenericProperty<string>(propertyName, context, CultureInfo.InvariantCulture);
			stringProperty.Value = propertyValue;
			this.InitCriteria(stringProperty, domain, condition, required);
		}

		public IProperty Property {
			get { return this.property; }
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				this.SetPropertyValueAndCondition(value, this.Condition);
			}
		}
		
		public CompareCondition Condition {
			get { return this.condition; }
			set {
				this.SetPropertyValueAndCondition(this.Property, value);
			}
		}

		public bool Required { get { return this.required; } set { this.required = value; } }
		public string Domain { get { return this.domain; } set { this.domain = value; } }

		private void InitCriteria(IProperty property, string domain, CompareCondition condition, bool required) {
			this.SetPropertyValueAndCondition(property, condition);
			this.domain = domain;
			this.required = required;
		}

		private void SetPropertyValueAndCondition(IProperty property, CompareCondition condition) {
			CheckPropertyCompareCondition(property, condition);
			this.property = property;
			this.condition = condition;
		}

		private static void CheckPropertyCompareCondition(IProperty property, CompareCondition condition) {
			switch(condition) {
				case CompareCondition.GreaterThan:
				case CompareCondition.GreaterThanOrEqual:
				case CompareCondition.LessThan:
				case CompareCondition.LessThanOrEqual:
				if(!typeof(IComparable).IsAssignableFrom(property.Type)) {
					throw new InvalidOperationException("Can not use inequality comparison on a property whose underlying type is not comparable.");
				}
				break;
				case CompareCondition.ContainsStartsWith:
				case CompareCondition.Contains:
				case CompareCondition.NotContains:
				case CompareCondition.EndsWith:
				case CompareCondition.NotEndsWith:
				case CompareCondition.StartsWith:
				case CompareCondition.NotStartsWith:
				if(!property.Type.Equals(typeof(string))) {
					throw new InvalidOperationException("Can not use string comparisons on a property whose underlying type is not a string.");
				}
				break;
			}
		}

	}
}
