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

namespace nJupiter.Globalization {

	public static class DateHandler {
		public static DateTime FirstDateOfWeek(DateTime date) {
			return FirstDateOfWeek(date, CultureInfo.CurrentCulture);
		}

		public static DateTime FirstDateOfWeek(DateTime date, CultureInfo culture) {
			if(culture == null)
				throw new ArgumentNullException("culture");

			return date.AddDays(DaysToFirstDayInWeek(culture.DateTimeFormat.FirstDayOfWeek, date.DayOfWeek));
		}

		public static DateTime LastDateOfWeek(DateTime date) {
			return LastDateOfWeek(date, CultureInfo.CurrentCulture);
		}

		public static DateTime LastDateOfWeek(DateTime date, CultureInfo culture) {
			if(culture == null)
				throw new ArgumentNullException("culture");

			return date.AddDays(DaysToLastDayInWeek(culture.DateTimeFormat.FirstDayOfWeek, date.DayOfWeek));
		}

		public static int WeekNumber(DateTime date) {
			return WeekNumber(date, CultureInfo.CurrentCulture);
		}

		public static int WeekNumber(DateTime date, CultureInfo culture) {
			if(culture == null)
				throw new ArgumentNullException("culture");

			return culture.Calendar.GetWeekOfYear(LastDateOfWeek(date, culture), culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
		}

		public static int DaysToFirstDayInWeek(DayOfWeek firstDayOfWeek, DayOfWeek currentDayOfWeek) {
			return DaysToLastDayInWeek(firstDayOfWeek, currentDayOfWeek) - 6;
		}

		public static int DaysToLastDayInWeek(DayOfWeek firstDayOfWeek, DayOfWeek currentDayOfWeek) {
			return ((((int)firstDayOfWeek + 6) - (int)currentDayOfWeek) % 7);
		}

		public static DayOfWeek[] SortedDaysOfWeek() {
			return SortedDaysOfWeek(CultureInfo.CurrentCulture);
		}

		public static DayOfWeek[] SortedDaysOfWeek(CultureInfo culture) {
			if(culture == null)
				throw new ArgumentNullException("culture");

			DayOfWeek[] daysOfWeek = new DayOfWeek[7];
			int firstDayOfWeek = (int)culture.DateTimeFormat.FirstDayOfWeek;
			for(int i = 0; i < 7; i++) {
				daysOfWeek[i] = (DayOfWeek)(i + (((firstDayOfWeek + 6) - i) % 7) - 6);
			}
			return daysOfWeek;
		}

	}
}
