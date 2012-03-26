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
using System.Globalization;

namespace nJupiter.Services.Forum.UI {

	public class FriendlyDateFormatter : DateFormatter {
		#region Constants
#if DEBUG
		private const string		DebugPrefix				= "_";
#else
		private const string DebugPrefix = "";
#endif

		private const string DefaultDefaultDateFormat = DebugPrefix + "{0:g}";
		private const string DefaultTodayDateFormat = DebugPrefix + "{0:t}";
		private const string DefaultYesterdayDateFormat = DebugPrefix + "yesterday {0:t}";
		private const string DefaultXDaysAgoDateFormat = DebugPrefix + "{0} days agos {1:t}";
		private const int DefaultXDaysAgoLimit = 5;
		private const bool DefaultConvertToLocalTime = true;
		#endregion

		#region Variables
		private string defaultDateFormat = DefaultDefaultDateFormat;
		private string todayDateFormat = DefaultTodayDateFormat;
		private string yesterdayDateFormat = DefaultYesterdayDateFormat;
		private string xDaysAgoDateFormat = DefaultXDaysAgoDateFormat;
		private int xDaysAgoLimit = DefaultXDaysAgoLimit;
		private bool convertToLocalTime = DefaultConvertToLocalTime;
		#endregion

		#region Properties
		public string DefaultDateFormat { get { return this.defaultDateFormat; } set { this.defaultDateFormat = value; } }
		public string TodayDateFormat { get { return this.todayDateFormat; } set { this.todayDateFormat = value; } }
		public string YesterdayDateFormat { get { return this.yesterdayDateFormat; } set { this.yesterdayDateFormat = value; } }
		public string XDaysAgoDateFormat { get { return this.xDaysAgoDateFormat; } set { this.xDaysAgoDateFormat = value; } }
		public int XDaysAgoLimit { get { return this.xDaysAgoLimit; } set { this.xDaysAgoLimit = value; } }
		public bool ConvertToLocalDateTime { get { return this.convertToLocalTime; } set { this.convertToLocalTime = value; } }
		#endregion

		#region Constructors
		public FriendlyDateFormatter() { }
		public FriendlyDateFormatter(string defaultDateFormat, string todayDateFormat, string yesterdayDateFormat, string xDaysAgoDateFormat, int xDaysAgoLimit)
			: this(defaultDateFormat, todayDateFormat, yesterdayDateFormat, xDaysAgoDateFormat, xDaysAgoLimit, DefaultConvertToLocalTime) { }
		public FriendlyDateFormatter(string defaultDateFormat, string todayDateFormat, string yesterdayDateFormat, string xDaysAgoDateFormat, int xDaysAgoLimit, bool convertToLocalTime) {
			this.defaultDateFormat = defaultDateFormat;
			this.todayDateFormat = todayDateFormat;
			this.yesterdayDateFormat = yesterdayDateFormat;
			this.xDaysAgoDateFormat = xDaysAgoDateFormat;
			this.xDaysAgoLimit = xDaysAgoLimit;
			this.convertToLocalTime = convertToLocalTime;
		}
		#endregion

		#region DateFormatter Implementation
		public string FormatDate(DateTime dateTime) {
			if(this.ConvertToLocalDateTime) {
				dateTime = dateTime.ToLocalTime();
			}
			DateTime today = this.ConvertToLocalDateTime ? DateTime.Today : DateTime.UtcNow.Date;
			int daysAgo;
			IFormatProvider formatProvider = CultureInfo.CurrentCulture;
			if(dateTime.Date.Equals(today)) {
				return string.Format(formatProvider, this.TodayDateFormat, dateTime);
			}
			if(dateTime.Date.Equals(today.AddDays(-1))) {
				return string.Format(formatProvider, this.YesterdayDateFormat, dateTime);
			}
			if((daysAgo = (int)today.Subtract(dateTime.Date).TotalDays) <= this.XDaysAgoLimit) {
				return string.Format(formatProvider, this.XDaysAgoDateFormat, daysAgo, dateTime);
			}
			return string.Format(formatProvider, this.DefaultDateFormat, dateTime);
		}
		#endregion
	}

}
