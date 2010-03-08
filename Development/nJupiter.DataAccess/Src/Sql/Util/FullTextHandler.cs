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
using System.Collections;
using System.Globalization;
using System.Text;

namespace nJupiter.DataAccess.Sql.Util {

	/// <summary>
	/// Utility class to helt build full text search queries for SQL Server.
	/// </summary>
	public static class FullTextHandler {
		#region Enums
		/// <summary>
		/// Represent the type of full text search to be preformed.
		/// </summary>
		public enum FullTextSearchType {
			/// <summary>
			/// Normal full text search
			/// </summary>
			Normal,
			/// <summary>
			/// Prefixed full text search
			/// </summary>
			Prefix,
			/// <summary>
			/// Inflectional full text search
			/// </summary>
			Inflectional,
			/// <summary>
			/// Thesaurus full text search. Only supported on SQL Server 2005 and later.
			/// </summary>
			Thesaurus
		}

		/// <summary>
		/// Represent implicit operators used in full text search queries.
		/// </summary>
		public enum ImplicitOperator {
			/// <summary>
			/// And operator
			/// </summary>
			And,
			/// <summary>
			/// Or operator
			/// </summary>
			Or
		}
		#endregion

		#region Constructors
		#endregion

		#region Methods
		/// <summary>
		/// Gets a full text search condition that contains the text in <paramref name="searchText"/>.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <returns>A search condition.</returns>
		public static string GetContainsSearchCondition(string searchText) {
			return GetContainsSearchCondition(searchText, ImplicitOperator.And, false);
		}

		/// <summary>
		/// Gets a full text search condition that contains the text in <paramref name="searchText"/>.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="treatTextualOperatorsAsSearchText">If set to <c>true</c> treat textual operators as search text.</param>
		/// <returns>A search condition.</returns>
		public static string GetContainsSearchCondition(string searchText, bool treatTextualOperatorsAsSearchText) {
			return GetContainsSearchCondition(searchText, ImplicitOperator.And, treatTextualOperatorsAsSearchText);
		}

		/// <summary>
		/// Gets a full text search condition that contains the text in <paramref name="searchText"/>.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="implicitOperator">The implicit operator.</param>
		/// <returns>A search condition.</returns>
		public static string GetContainsSearchCondition(string searchText, ImplicitOperator implicitOperator) {
			return GetContainsSearchCondition(searchText, FullTextSearchType.Normal, implicitOperator, false);
		}

		/// <summary>
		/// Gets a full text search condition that contains the text in <paramref name="searchText"/>.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="implicitOperator">The implicit operator.</param>
		/// <param name="treatTextualOperatorsAsSearchText">If set to <c>true</c> treat textual operators as search text.</param>
		/// <returns>A search condition.</returns>
		public static string GetContainsSearchCondition(string searchText, ImplicitOperator implicitOperator, bool treatTextualOperatorsAsSearchText) {
			return GetContainsSearchCondition(searchText, FullTextSearchType.Normal, implicitOperator, treatTextualOperatorsAsSearchText);
		}

		/// <summary>
		/// Gets the contains search condition.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="fullTextSearchType">Full text search type.</param>
		/// <returns>A search condition.</returns>
		public static string GetContainsSearchCondition(string searchText, FullTextSearchType fullTextSearchType) {
			return GetContainsSearchCondition(searchText, fullTextSearchType, ImplicitOperator.And, false);
		}

		/// <summary>
		/// Gets the contains search condition.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="fullTextSearchType">Full text search type.</param>
		/// <param name="treatTextualOperatorsAsSearchText">If set to <c>true</c> treat textual operators as search text.</param>
		/// <returns>A search condition.</returns>
		public static string GetContainsSearchCondition(string searchText, FullTextSearchType fullTextSearchType, bool treatTextualOperatorsAsSearchText) {
			return GetContainsSearchCondition(searchText, fullTextSearchType, ImplicitOperator.And, treatTextualOperatorsAsSearchText);
		}

		/// <summary>
		/// Gets the contains search condition.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="fullTextSearchType">Full text search type.</param>
		/// <param name="implicitOperator">The implicit operator.</param>
		/// <param name="treatTextualOperatorsAsSearchText">If set to <c>true</c> treat textual operators as search text.</param>
		/// <returns>A search condition.</returns>
		public static string GetContainsSearchCondition(string searchText, FullTextSearchType fullTextSearchType, ImplicitOperator implicitOperator, bool treatTextualOperatorsAsSearchText) {
			//TODO: when a logical operator is not valid at a certain place, handle it as a search word?
			const string and					= "and";
			const string andSign				= "&";
			const string plus					= "+";
			const string or						= "or";
			const string orSign					= "|";
			const string not					= "not";
			const string notSign				= "!";
			const string minus					= "-";
			const string near					= "near";
			const string nearSign				= "~";
			const string prefix					= "*";
			const string parenStart				= "(";
			const string parenEnd				= ")";

			const string formatNormal			= @"""{0}""";
			const string formatPrefix			= @"""{0}*""";
			const string formatInflectional		= @"FORMSOF(INFLECTIONAL,""{0}"")";
			const string formatThesaurus		= @"FORMSOF(THESAURUS,""{0}"")";

			string[] logicalSearchTerms		= treatTextualOperatorsAsSearchText ? 
				new[] {minus, notSign, andSign, parenStart, parenEnd, prefix, orSign, nearSign, plus} :
				new[] {minus, notSign, andSign, parenStart, parenEnd, prefix, orSign, nearSign, plus, and, near, not, or};
			
			string firstLogical				= null;
			string secondLogical			= null;
			int normalSearchTerms			= 0;
			int openParens					= 0;
			int openedParens				= 0;
			int closeParens					= 0;
			int closedParens				= 0;
			StringBuilder searchCondition	= new StringBuilder();
			string[] searchTokens			= GetSearchTokens(searchText);

			for(int i = 0; i < searchTokens.Length; i++) {
				string searchTerm = searchTokens[i];
				if(Array.BinarySearch(logicalSearchTerms, searchTerm, CaseInsensitiveComparer.DefaultInvariant) < 0) {
					normalSearchTerms++;
					bool beginningOrEndingParens = openParens > 0 || closeParens > 0;
					for(; closeParens > 0; closeParens--) {
						closedParens++;
						searchCondition.Append(parenEnd);
					}
					if(normalSearchTerms > 1) {
						if(!fullTextSearchType.Equals(FullTextSearchType.Inflectional) && !fullTextSearchType.Equals(FullTextSearchType.Thesaurus) && 
							!beginningOrEndingParens &&	firstLogical != null && ((!treatTextualOperatorsAsSearchText && IsEqualCaseInsensitive(firstLogical, near)) || firstLogical.Equals(nearSign))) {
							searchCondition.Append(nearSign);
							//"near" not supported between parens nor inflectional/thesaurus terms
						} else if((firstLogical != null && ((!treatTextualOperatorsAsSearchText && IsEqualCaseInsensitive(firstLogical, or)) || firstLogical.Equals(orSign))) ||
							(implicitOperator.Equals(ImplicitOperator.Or) && !(firstLogical != null && ((!treatTextualOperatorsAsSearchText && IsEqualCaseInsensitive(firstLogical, and)) || firstLogical.Equals(andSign) || firstLogical.Equals(plus))))) {
							searchCondition.Append(orSign);
							//"or not" not supported
						} else {
							searchCondition.Append(andSign);
							if((firstLogical != null && ((!treatTextualOperatorsAsSearchText && IsEqualCaseInsensitive(firstLogical, not)) || firstLogical.Equals(notSign) || firstLogical.Equals(minus))) ||
								(secondLogical != null && ((!treatTextualOperatorsAsSearchText && IsEqualCaseInsensitive(secondLogical, not)) || secondLogical.Equals(notSign) || secondLogical.Equals(minus)))) {
								searchCondition.Append(notSign);
							}
						}
					}
					for(; openParens > 0; openParens--) {
						openedParens++;
						searchCondition.Append(parenStart);
					}
					string format;
					switch(fullTextSearchType) {
						case FullTextSearchType.Inflectional:
							//prefix terms not supported in inflectional terms
							searchTerm	= searchTerm.TrimEnd(prefix[0]);
							format		= formatInflectional;
							break;
						case FullTextSearchType.Thesaurus:
							//prefix terms not supported in thesaurus terms
							searchTerm	= searchTerm.TrimEnd(prefix[0]);
							format		= formatThesaurus;
							break;
						case FullTextSearchType.Prefix:
							format		= formatPrefix;
							break;
						default:
							format		= formatNormal;
							break;
					}
					searchCondition.AppendFormat(CultureInfo.InvariantCulture, format, searchTerm);
					firstLogical = secondLogical = null;
				} else if(searchTerm.Equals(parenStart)) {
					openParens++;
				} else if(searchTerm.Equals(parenEnd)) {
					if(!openParens.Equals(0)) {
						openParens--;
					} else if(openedParens > closedParens + closeParens) {
						closeParens++;
					}
				} else if(firstLogical == null) {
					firstLogical	= searchTerm;
				} else if(secondLogical == null) {
					secondLogical	= searchTerm;
				}
			}
			for(int i = openedParens - closedParens; i > 0; i--) {
				searchCondition.Append(parenEnd);
			}
			return searchCondition.ToString();
		}
		#endregion

		#region Helper Methods
		private static string[] GetSearchTokens(string searchText) {
			const char separator	= ' ';
			const char quote		= '"';
			const char parenStart	= '(';
			const char parenEnd		= ')';
			const char and			= '&';
			const char plus			= '+';
			const char or			= '|';
			const char not			= '!';
			const char minus		= '-';
			const char near			= '~';

			if(searchText == null || (searchText = searchText.Trim()).Length.Equals(0)) {
				return new string[0];
			}
			bool insideQuotes				= false;
			bool lastWasWhitespace			= false;
			bool lastWasSpecialSign			= false;
			StringBuilder fixedSearchText	= new StringBuilder(searchText);

			for(int i = 0; i < fixedSearchText.Length; i++) {
				char character = fixedSearchText[i];
				switch(character) {
					case parenStart:
					case parenEnd:
					case and:
					case plus:
					case or:
					case not:
					case minus:
					case near:
					case quote:
						if(lastWasSpecialSign || (i > 0 && !insideQuotes && !lastWasWhitespace)) {
							if(!lastWasSpecialSign && character.Equals(minus)) {
								goto default;
							} else {
								fixedSearchText.Insert(i++, separator);
							}
						}
						if(character.Equals(quote)) {
							fixedSearchText.Remove(i--, 1);
							lastWasWhitespace	= insideQuotes = !insideQuotes;
						} else {
							lastWasWhitespace	= false;
						}
						lastWasSpecialSign = !insideQuotes;
						break;
					default:
						if(char.IsWhiteSpace(character)) {
							if(lastWasWhitespace) {
								fixedSearchText.Remove(i--, 1);
							} else {
								fixedSearchText[i]	= insideQuotes ? char.MinValue : separator;
								lastWasWhitespace	= true;
							}
						} else {
							if(lastWasSpecialSign) {
								fixedSearchText.Insert(i++, separator);
							}
							lastWasWhitespace = false;
						}
						lastWasSpecialSign = false;
						break;
				}
			}
			string[] tokens = fixedSearchText.ToString().Trim().Split(separator);
			for(int i = 0; i < tokens.Length; i++) {
				tokens[i] = tokens[i].Replace(char.MinValue, separator).Trim();
			}
			return tokens;
		}
		private static bool IsEqualCaseInsensitive(string a, string b) {
			return string.Compare(a, b, true, CultureInfo.InvariantCulture).Equals(0);
		}
		#endregion
	}
}
