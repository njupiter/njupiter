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
using System.Collections.Generic;
using System.Text;

namespace nJupiter.DataAccess.Ldap.NameParser {

	// based on http://www.codeproject.com/KB/IP/dnparser.aspx?msg=1758400 by by (C) 2005 Pete Everett (pete@CynicalPirate.com)
	// Code will be cleaned up later
	internal sealed class Dn {

		private enum ParserState { LookingForSeparator, InQuotedString };

		[Flags]
		internal enum EscapeChars {
			None = 0,
			ControlChars = 1,
			SpecialChars = 2,
			MultibyteChars = 4
		}

		public const EscapeChars DefaultEscapeChars = EscapeChars.ControlChars | EscapeChars.SpecialChars;

		private readonly List<Rdn> rdns;
		private readonly EscapeChars escapeChars;

		public List<Rdn> Rdns {
			get {
				return this.rdns;
			}
		}

		public Dn(string dnString) : this(dnString, DefaultEscapeChars) { }
		private Dn(string dnString, EscapeChars escapeChars) {
			if(dnString == null) {
				throw new ArgumentNullException("dnString");
			}
			this.escapeChars = escapeChars;
			this.rdns = ParseDn(dnString);
		}

		public override string ToString() {
			return ToString(this.escapeChars);
		}

		private string ToString(EscapeChars chars) {
			StringBuilder returnValue = new StringBuilder();

			foreach(Rdn rdn in this.Rdns) {
				returnValue.Append(rdn.ToString(chars));
				returnValue.Append(",");
			}

			// Remove the trailing comma
			if(returnValue.Length > 0)
				returnValue.Length--;

			return returnValue.ToString();
		}


		private static List<Rdn> ParseDn(string dnString) {
			if(string.IsNullOrEmpty(dnString)) {
				return new List<Rdn>();
			}
			List<Rdn> rdns = new List<Rdn>();
			ParserState state = ParserState.LookingForSeparator;
			StringBuilder rawRdn = new StringBuilder();


			for(int position = 0; position < dnString.Length; ++position) {
				switch(state) {
					case ParserState.LookingForSeparator:
					// If we find a separator character, we've hit the end of an Rdn.
					// We'll store the Rdn, and we'll check to see if the Rdn is actually
					// valid later.
					if(dnString[position] == ',' || dnString[position] == ';') {
						Rdn rdn = new Rdn(rawRdn.ToString());
						rdns.Add(rdn); // Add the string to the list of raw Rdns
						rawRdn.Length = 0;              // Clear the StringBuilder to prepare for the next Rdn
					} else {
						// Add the character to our temporary Rdn string
						rawRdn.Append(dnString[position]);

						// If we find an escape character, store character that follows it,
						// but don't consider it as a possible separator character.  If the
						// string ends with an escape character, that's bad, and we should
						// throw an exception
						if(dnString[position] == '\\') {
							try {
								rawRdn.Append(dnString[++position]);
							} catch(IndexOutOfRangeException) {
								throw new ArgumentException("Invalid Dn: DNs aren't allowed to end with an escape character.", dnString);
							}

						}
							// If we find a quote, we'll change state so that we look for the closing quote
							// and ignore any separator characters within.
						else if(dnString[position] == '"') {
							state = ParserState.InQuotedString;
						}
					}

					break;


					case ParserState.InQuotedString:
					// Store the character
					rawRdn.Append(dnString[position]);

					// You're allowed to escape special characters in a quoted string, but not required
					// to.  But if there's an escaped quote, we need to take special care to make sure
					// that we don't mistake that for the end of the quoted string.
					if(dnString[position] == '\\') {
						try {
							rawRdn.Append(dnString[++position]);
						} catch(IndexOutOfRangeException) {
							throw new ArgumentException("Invalid Dn: DNs aren't allowed to end with an escape character.", dnString);
						}
					} else if(dnString[position] == '"')
						state = ParserState.LookingForSeparator;
					break;

				}
			}

			// Take the last Rdn and add it to the list
			Rdn lastRdn = new Rdn(rawRdn.ToString());
			rdns.Add(lastRdn);

			// Check parser's end state
			if(state == ParserState.InQuotedString)
				throw new ArgumentException("Invalid Dn: Unterminated quoted string.", dnString);

			return rdns;
		}

	}




}
