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
			InitCriteria(property, domain, condition, required);
		}

		public SearchCriteria(IProperty property) : this(property, CompareCondition.Equal, false) { }
		public SearchCriteria(IProperty property, CompareCondition condition) : this(property, condition, false) { }
		public SearchCriteria(IProperty property, bool required) : this(property, CompareCondition.Equal, required) { }
		public SearchCriteria(IProperty property, CompareCondition condition, bool required) {
			InitCriteria(property, null, condition, required);
		}

		public SearchCriteria(string propertyName, string propertyValue, string domain) : this(propertyName, propertyValue, domain, null, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, bool required) : this(propertyName, propertyValue, domain, null, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, CompareCondition condition) : this(propertyName, propertyValue, domain, null, condition, false) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, CompareCondition condition, bool required) : this(propertyName, propertyValue, domain, null, condition, required) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, IContext context) : this(propertyName, propertyValue, domain, context, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, IContext context, bool required) : this(propertyName, propertyValue, domain, context, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, string domain, IContext context, CompareCondition condition) : this(propertyName, propertyValue, domain, context, condition, false) { }

		public SearchCriteria(string propertyName, string propertyValue) : this(propertyName, propertyValue, null, null, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, bool required) : this(propertyName, propertyValue, null, null, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, CompareCondition condition) : this(propertyName, propertyValue, null, null, condition, false) { }
		public SearchCriteria(string propertyName, string propertyValue, CompareCondition condition, bool required) : this(propertyName, propertyValue, null, null, condition, required) { }
		public SearchCriteria(string propertyName, string propertyValue, IContext context) : this(propertyName, propertyValue, null, context, CompareCondition.Equal, false) { }
		public SearchCriteria(string propertyName, string propertyValue, IContext context, bool required) : this(propertyName, propertyValue, null, context, CompareCondition.Equal, required) { }
		public SearchCriteria(string propertyName, string propertyValue, IContext context, CompareCondition condition) : this(propertyName, propertyValue, null, context, condition, false) { }

		public SearchCriteria(string propertyName, string propertyValue, string domain, IContext context, CompareCondition condition, bool required) {
			if(propertyName == null) {
				throw new ArgumentNullException("propertyName");
			}
			var stringProperty = new Property<string>(propertyName, context, CultureInfo.InvariantCulture);
			stringProperty.Value = propertyValue;
			InitCriteria(stringProperty, domain, condition, required);
		}

		public IProperty Property {
			get { return property; }
			set {
				if(value == null) {
					throw new ArgumentNullException("value");
				}
				SetPropertyValueAndCondition(value, Condition);
			}
		}
		
		public CompareCondition Condition {
			get { return condition; }
			set {
				SetPropertyValueAndCondition(Property, value);
			}
		}

		public bool Required { get { return required; } set { required = value; } }
		public string Domain { get { return domain; } set { domain = value; } }

		private void InitCriteria(IProperty p, string d, CompareCondition c, bool r) {
			SetPropertyValueAndCondition(p, c);
			domain = d;
			required = r;
		}

		private void SetPropertyValueAndCondition(IProperty p, CompareCondition c) {
			CheckPropertyCompareCondition(p, c);
			property = p;
			condition = c;
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
				if(property.Type != typeof(string)) {
					throw new InvalidOperationException("Can not use string comparisons on a property whose underlying type is not a string.");
				}
				break;
			}
		}

	}
}
