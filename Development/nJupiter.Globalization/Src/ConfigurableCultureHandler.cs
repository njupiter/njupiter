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
using System.Collections.Generic;
using System.Globalization;

using nJupiter.Configuration;

namespace nJupiter.Globalization {
	internal class ConfigurableCultureHandler : ICultureHandler {

		private readonly Dictionary<String, CultureInfo> cultureInfoCache = new Dictionary<String, CultureInfo>(StringComparer.InvariantCultureIgnoreCase);

		public CultureInfo CurrentCulture {
			get {
				return GetCultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture);
			}
		}

		public CultureInfo CurrentUICulture {
			get {
				return GetCultureInfo(System.Threading.Thread.CurrentThread.CurrentUICulture);
			}
		}

		public CultureInfo GetCultureInfo(int culture) {
			return GetCultureInfo((new CultureInfo(culture))); // Can this be done nicer?
		}

		private CultureInfo GetCultureInfo(CultureInfo culture) {
			if(culture == null) {
				throw new ArgumentNullException("culture");
			}
			return GetCultureInfo(culture.Name);
		}

		public CultureInfo GetCultureInfo(string name) {
			if(name == null) {
				throw new ArgumentNullException("name");
			}
			CultureInfo cultureInfo = null;
			if(cultureInfoCache.ContainsKey(name)) {
				cultureInfo = cultureInfoCache[name];
			}
			if(cultureInfo != null)
				return (CultureInfo)cultureInfo.Clone();

			cultureInfo = new CultureInfo(name);

			IConfig config = ConfigRepository.Instance.GetSystemConfig();

			string dateTimeFormatConfigKey = string.Format("cultureConfig/culture[@value=\"{0}\"]/dateTimeFormat", name);
			if(config.ContainsKey(dateTimeFormatConfigKey)) {
				IConfig dateTimeFormatConfig = config.GetConfigSection(dateTimeFormatConfigKey);

				string[] abbreviatedMonthGenitiveNames = dateTimeFormatConfig.GetValueArray("abbreviatedMonthGenitiveNames", "abbreviatedMonthGenitiveName");
				if(abbreviatedMonthGenitiveNames.Length > 0) {
					try {
						cultureInfo.DateTimeFormat.AbbreviatedMonthGenitiveNames = abbreviatedMonthGenitiveNames;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/abbreviatedMonthGenitiveNames] {1}", dateTimeFormatConfigKey, ex.Message));
					}
				}

				string[] monthGenitiveNames = dateTimeFormatConfig.GetValueArray("monthGenitiveNames", "monthGenitiveName");
				if(monthGenitiveNames.Length > 0) {
					try {
						cultureInfo.DateTimeFormat.MonthGenitiveNames = monthGenitiveNames;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/monthGenitiveNames] {1}", dateTimeFormatConfigKey, ex.Message));
					}
				}

				string[] abbreviatedDayNames = dateTimeFormatConfig.GetValueArray("abbreviatedDayNames", "abbreviatedDayName");
				if(abbreviatedDayNames.Length > 0) {
					try {
						cultureInfo.DateTimeFormat.AbbreviatedDayNames = abbreviatedDayNames;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/abbreviatedDayNames] {1}", dateTimeFormatConfigKey, ex.Message));
					}
				}

				string[] shortestDayNames = dateTimeFormatConfig.GetValueArray("shortestDayNames", "shortestDayName");
				if(shortestDayNames.Length > 0) {
					try {
						cultureInfo.DateTimeFormat.ShortestDayNames = shortestDayNames;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/shortestDayNames] {1}", dateTimeFormatConfigKey, ex.Message));
					}
				}

				string[] abbreviatedMonthNames = dateTimeFormatConfig.GetValueArray("abbreviatedMonthNames", "abbreviatedMonthName");
				if(abbreviatedMonthNames.Length > 0) {
					try {
						cultureInfo.DateTimeFormat.AbbreviatedMonthNames = abbreviatedMonthNames;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/abbreviatedMonthNames] {1}", dateTimeFormatConfigKey, ex.Message));
					}
				}

				string[] dayNames = dateTimeFormatConfig.GetValueArray("dayNames", "dayName");
				if(dayNames.Length > 0) {
					try {
						cultureInfo.DateTimeFormat.DayNames = dayNames;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/dayNames] {1}", dateTimeFormatConfigKey, ex.Message));
					}
				}


				string[] monthNames = dateTimeFormatConfig.GetValueArray("monthNames", "monthName");
				if(monthNames.Length > 0) {
					try {
						cultureInfo.DateTimeFormat.MonthNames = monthNames;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/monthNames] {1}", dateTimeFormatConfigKey, ex.Message));
					}
				}

				if(dateTimeFormatConfig.ContainsKey("calendarWeekRule")) {
					try {
						cultureInfo.DateTimeFormat.CalendarWeekRule = (CalendarWeekRule)Enum.Parse(typeof(CalendarWeekRule), dateTimeFormatConfig.GetValue("calendarWeekRule"), true);
					} catch(ArgumentException) {
						throw new ConfigurationException(string.Format("Value [{0}/calendarWeekRule] is not of type CalendarWeekRule", dateTimeFormatConfigKey));
					}
				}

				if(dateTimeFormatConfig.ContainsKey("firstDayOfWeek")) {
					try {
						cultureInfo.DateTimeFormat.FirstDayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dateTimeFormatConfig.GetValue("firstDayOfWeek"), true);
					} catch(ArgumentException) {
						throw new ConfigurationException(string.Format("Value [{0}/firstDayOfWeek] is not of type DayOfWeek", dateTimeFormatConfigKey));
					}
				}

				if(dateTimeFormatConfig.ContainsKey("amDesignator")) {
					cultureInfo.DateTimeFormat.AMDesignator = dateTimeFormatConfig.GetValue("amDesignator");
				}

				if(dateTimeFormatConfig.ContainsKey("pmDesignator")) {
					cultureInfo.DateTimeFormat.PMDesignator = dateTimeFormatConfig.GetValue("pmDesignator");
				}

				if(dateTimeFormatConfig.ContainsKey("dateSeparator")) {
					cultureInfo.DateTimeFormat.DateSeparator = dateTimeFormatConfig.GetValue("dateSeparator");
				}

				if(dateTimeFormatConfig.ContainsKey("fullDateTimePattern")) {
					cultureInfo.DateTimeFormat.FullDateTimePattern = dateTimeFormatConfig.GetValue("fullDateTimePattern");
				}

				if(dateTimeFormatConfig.ContainsKey("longDatePattern")) {
					cultureInfo.DateTimeFormat.LongDatePattern = dateTimeFormatConfig.GetValue("longDatePattern");
				}

				if(dateTimeFormatConfig.ContainsKey("longTimePattern")) {
					cultureInfo.DateTimeFormat.LongTimePattern = dateTimeFormatConfig.GetValue("longTimePattern");
				}

				if(dateTimeFormatConfig.ContainsKey("monthDayPattern")) {
					cultureInfo.DateTimeFormat.MonthDayPattern = dateTimeFormatConfig.GetValue("monthDayPattern");
				}

				if(dateTimeFormatConfig.ContainsKey("shortDatePattern")) {
					cultureInfo.DateTimeFormat.ShortDatePattern = dateTimeFormatConfig.GetValue("shortDatePattern");
				}

				if(dateTimeFormatConfig.ContainsKey("shortTimePattern")) {
					cultureInfo.DateTimeFormat.ShortTimePattern = dateTimeFormatConfig.GetValue("shortTimePattern");
				}

				if(dateTimeFormatConfig.ContainsKey("timeSeparator")) {
					cultureInfo.DateTimeFormat.TimeSeparator = dateTimeFormatConfig.GetValue("timeSeparator");
				}

				if(dateTimeFormatConfig.ContainsKey("yearMonthPattern")) {
					cultureInfo.DateTimeFormat.YearMonthPattern = dateTimeFormatConfig.GetValue("yearMonthPattern");
				}

			}

			string numberFormatConfigKey = string.Format("cultureConfig/culture[@value=\"{0}\"]/numberFormat", name);

			if(config.ContainsKey(numberFormatConfigKey)) {
				IConfig numberFormatConfig = config.GetConfigSection(numberFormatConfigKey);

				if(numberFormatConfig.ContainsKey("currencyDecimalDigits")) {
					cultureInfo.NumberFormat.CurrencyDecimalDigits = numberFormatConfig.GetValue<int>("currencyDecimalDigits");
				}

				if(numberFormatConfig.ContainsKey("currencyNegativePattern")) {
					cultureInfo.NumberFormat.CurrencyNegativePattern = numberFormatConfig.GetValue<int>("currencyNegativePattern");
				}

				if(numberFormatConfig.ContainsKey("currencyPositivePattern")) {
					cultureInfo.NumberFormat.CurrencyPositivePattern = numberFormatConfig.GetValue<int>("currencyPositivePattern");
				}

				if(numberFormatConfig.ContainsKey("numberDecimalDigits")) {
					cultureInfo.NumberFormat.NumberDecimalDigits = numberFormatConfig.GetValue<int>("numberDecimalDigits");
				}

				if(numberFormatConfig.ContainsKey("numberNegativePattern")) {
					cultureInfo.NumberFormat.NumberNegativePattern = numberFormatConfig.GetValue<int>("numberNegativePattern");
				}

				if(numberFormatConfig.ContainsKey("percentDecimalDigits")) {
					cultureInfo.NumberFormat.PercentDecimalDigits = numberFormatConfig.GetValue<int>("percentDecimalDigits");
				}

				if(numberFormatConfig.ContainsKey("percentNegativePattern")) {
					cultureInfo.NumberFormat.PercentNegativePattern = numberFormatConfig.GetValue<int>("percentNegativePattern");
				}

				if(numberFormatConfig.ContainsKey("percentPositivePattern")) {
					cultureInfo.NumberFormat.PercentPositivePattern = numberFormatConfig.GetValue<int>("percentPositivePattern");
				}

				if(numberFormatConfig.ContainsKey("currencyDecimalSeparator")) {
					cultureInfo.NumberFormat.CurrencyDecimalSeparator = numberFormatConfig.GetValue("currencyDecimalSeparator");
				}

				if(numberFormatConfig.ContainsKey("currencyGroupSeparator")) {
					cultureInfo.NumberFormat.CurrencyGroupSeparator = numberFormatConfig.GetValue("currencyGroupSeparator");
				}

				if(numberFormatConfig.ContainsKey("currencySymbol")) {
					cultureInfo.NumberFormat.CurrencySymbol = numberFormatConfig.GetValue("currencySymbol");
				}

				if(numberFormatConfig.ContainsKey("nanSymbol")) {
					cultureInfo.NumberFormat.NaNSymbol = numberFormatConfig.GetValue("nanSymbol");
				}

				if(numberFormatConfig.ContainsKey("negativeInfinitySymbol")) {
					cultureInfo.NumberFormat.NegativeInfinitySymbol = numberFormatConfig.GetValue("negativeInfinitySymbol");
				}

				if(numberFormatConfig.ContainsKey("negativeSign")) {
					cultureInfo.NumberFormat.NegativeSign = numberFormatConfig.GetValue("negativeSign");
				}

				if(numberFormatConfig.ContainsKey("numberDecimalSeparator")) {
					cultureInfo.NumberFormat.NumberDecimalSeparator = numberFormatConfig.GetValue("numberDecimalSeparator");
				}

				if(numberFormatConfig.ContainsKey("numberGroupSeparator")) {
					cultureInfo.NumberFormat.NumberGroupSeparator = numberFormatConfig.GetValue("numberGroupSeparator");
				}

				if(numberFormatConfig.ContainsKey("percentDecimalSeparator")) {
					cultureInfo.NumberFormat.PercentDecimalSeparator = numberFormatConfig.GetValue("percentDecimalSeparator");
				}

				if(numberFormatConfig.ContainsKey("percentGroupSeparator")) {
					cultureInfo.NumberFormat.PercentGroupSeparator = numberFormatConfig.GetValue("percentGroupSeparator");
				}

				if(numberFormatConfig.ContainsKey("percentSymbol")) {
					cultureInfo.NumberFormat.PercentSymbol = numberFormatConfig.GetValue("percentSymbol");
				}

				if(numberFormatConfig.ContainsKey("perMilleSymbol")) {
					cultureInfo.NumberFormat.PerMilleSymbol = numberFormatConfig.GetValue("perMilleSymbol");
				}

				if(numberFormatConfig.ContainsKey("positiveInfinitySymbol")) {
					cultureInfo.NumberFormat.PositiveInfinitySymbol = numberFormatConfig.GetValue("positiveInfinitySymbol");
				}

				if(numberFormatConfig.ContainsKey("positiveSign")) {
					cultureInfo.NumberFormat.PositiveSign = numberFormatConfig.GetValue("positiveSign");
				}

				int[] percentGroupSizes = numberFormatConfig.GetValueArray<int>("percentGroupSizes", "percentGroupSize");
				if(percentGroupSizes.Length > 0) {
					try {
						cultureInfo.NumberFormat.PercentGroupSizes = percentGroupSizes;
					} catch(ArgumentException ex) {
						throw new ConfigurationException("[" + numberFormatConfigKey + "/percentGroupSizes] " + ex.Message);
					}
				}

				int[] numberGroupSizes = numberFormatConfig.GetValueArray<int>("numberGroupSizes", "numberGroupSize");
				if(numberGroupSizes.Length > 0) {
					try {
						cultureInfo.NumberFormat.NumberGroupSizes = numberGroupSizes;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/numberGroupSizes] {1}", numberFormatConfigKey, ex.Message));
					}
				}

				int[] currencyGroupSizes = numberFormatConfig.GetValueArray<int>("currencyGroupSizes", "currencyGroupSize");
				if(currencyGroupSizes.Length > 0) {
					try {
						cultureInfo.NumberFormat.CurrencyGroupSizes = currencyGroupSizes;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/currencyGroupSizes] {1}", numberFormatConfigKey, ex.Message));
					}
				}

				if(numberFormatConfig.ContainsKey("digitSubstitution")) {
					try {
						cultureInfo.NumberFormat.DigitSubstitution = (DigitShapes)Enum.Parse(typeof(DigitShapes), numberFormatConfig.GetValue("digitSubstitution"), true);
					} catch(ArgumentException) {
						throw new ConfigurationException(string.Format("Value [{0}/digitSubstitution] is not of type DigitShapes", numberFormatConfigKey));
					}
				}

				string[] nativeDigits = numberFormatConfig.GetValueArray("nativeDigits", "nativeDigit");
				if(nativeDigits.Length > 0) {
					try {
						cultureInfo.NumberFormat.NativeDigits = nativeDigits;
					} catch(ArgumentException ex) {
						throw new ConfigurationException(string.Format("[{0}/nativeDigits] {1}", numberFormatConfigKey, ex.Message));
					}
				}
			}

			cultureInfoCache[name] = cultureInfo;

			return (CultureInfo)cultureInfo.Clone();
		}

	}
}
