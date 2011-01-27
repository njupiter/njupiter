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
using System.Text;
using System.IO;

namespace nJupiter.DataAccess.Ldap.NameParser {

	// based on http://www.codeproject.com/KB/IP/dnparser.aspx?msg=1758400 by by (C) 2005 Pete Everett (pete@CynicalPirate.com)
	// Code will be cleaned up later
	internal sealed class Rdn {

		private enum ParserState { DetermineAttributeType, GetTypeByOId, GetTypeByName, DetermineValueType, GetQuotedValue, GetUnquotedValue, GetHexValue };

		private readonly List<RdnComponent> components;

		internal Rdn(string rdnString) {
			this.components = ParseRdn(rdnString);
		}

		public List<RdnComponent> Components {
			get {
				return components;
			}
		}

		public override string ToString() {
			return ToString(Dn.DefaultEscapeChars);
		}

		public string ToString(Dn.EscapeChars escapeChars) {
			StringBuilder returnValue = new StringBuilder();

			foreach(RdnComponent component in components) {
				returnValue.Append(component.ToString(escapeChars));
				returnValue.Append("+");
			}

			// Get rid of the last plus sign			
			if(returnValue.Length > 0)
				returnValue.Length--;

			return returnValue.ToString();
		}


		private static void WriteUtf8Bytes(Stream s, char c) {
			char[] charArray = new char[1];
			charArray[0] = c;

			byte[] utf8Bytes = Encoding.UTF8.GetBytes(charArray);

			s.Write(utf8Bytes, 0, utf8Bytes.Length);
		}


		private static List<RdnComponent> ParseRdn(string rdnString) {
			List<string> rawTypes = new List<string>();
			List<string> rawValues = new List<string>();
			List<RdnComponent.RdnValueType> rawValueTypes = new List<RdnComponent.RdnValueType>();

			MemoryStream rawData = new MemoryStream();

			ParserState state = ParserState.DetermineAttributeType;

			int position = 0;

			while(position < rdnString.Length) {
				switch(state) {
					# region case ParserState.DetermineAttributeType:

					case ParserState.DetermineAttributeType:

					# region Ignore any spaces at the beginning

					try {
						while(rdnString[position] == ' ')
							++position;
					} catch(IndexOutOfRangeException) {
						throw new ArgumentException("Invalid Rdn: It's entirely blank spaces", rdnString);
					}

					# endregion

					# region Are we looking at a letter?

					if(IsAlpha(rdnString[position])) {
						string startPoint = rdnString.Substring(position);

						// OID. is an optional string at the beginning of an object identifier.
						// if we find it, we ignore it, but we assume that the rest of the type
						// is going to be in dotted OID format.  We advance 3 positions, which puts
						// us at the dot, so at the next iteration, we start at the first number
						// of the OID.
						if(startPoint.StartsWith("OID.") ||
							startPoint.StartsWith("oid.")) {
							position += 4;
							state = ParserState.GetTypeByOId;
						} else {
							// We're looking at the start of an attribute name.  We'll set the state
							// to GetTypeByName so we can parse the name from the beginning in the 
							// next iteration.
							state = ParserState.GetTypeByName;
						}
					}

					# endregion

					# region If not, are we looking at a digit?

 else if(IsDigit(rdnString[position])) {
						// If we're looking at a digit, that means we're looking at an OID.
						// We'll set the state to GetTypeByOId.
						state = ParserState.GetTypeByOId;
					}

					# endregion

					# region If not, we're looking at something that shouldn't be there.

 else {
						// If it's not a letter or a digit, then it's a wacky and invalid character,
						// and we'd do well to freak out.
						throw new ArgumentException("Invalid Rdn: Invalid character in attribute name", rdnString);
					}

					# endregion

					break;

					# endregion

					# region case ParserState.GetTypeByName:

					case ParserState.GetTypeByName:

					try {
						# region The first character needs to be a letter

						if(IsAlpha(rdnString[position])) {
							WriteUtf8Bytes(rawData, rdnString[position]);
							position++;
						} else {
							throw new ArgumentException("Invalid Attribute Name: Name must start with a letter", rdnString);
						}

						# endregion

						# region The remaining characters can be letters, digits, or hyphens

						while(IsAlpha(rdnString[position]) || IsDigit(rdnString[position]) || rdnString[position] == '-') {
							WriteUtf8Bytes(rawData, rdnString[position]);
							position++;
						}

						# endregion

						# region The name can be followed by any number of blank spaces

						while(rdnString[position] == ' ') {
							position++;
						}

						# endregion

						# region And it needs to end with an equals sign

						// If we've found an equals sign, add the type name to the list, and
						// set the state to start looking for the attribute value.
						if(rdnString[position] == '=') {
							position++;
							rawTypes.Add(Encoding.UTF8.GetString(rawData.ToArray()));
							rawData.SetLength(0);

							if(position >= rdnString.Length) {
								// If we're at the end of the string, the Rdn has an empty value.
								// We'll store a blank string, then we'll exit the loop gracefully.
								rawValues.Add("");
								rawValueTypes.Add(RdnComponent.RdnValueType.StringValue);
								state = ParserState.GetUnquotedValue;
							} else {
								state = ParserState.DetermineValueType;
							}
						} else {
							throw new ArgumentException("Invalid Rdn: unterminated attribute name", rdnString);
						}

						# endregion
					} catch(IndexOutOfRangeException) {
						throw new ArgumentException("Invalid Rdn: unterminated attribute name", rdnString);
					}

					break;

					# endregion

					# region case ParserState.GetTypeByOId:

					case ParserState.GetTypeByOId:

					try {
						# region The first character needs to be a digit

						if(IsDigit(rdnString[position])) {
							WriteUtf8Bytes(rawData, rdnString[position]);
							position++;
						} else {
							throw new ArgumentException("Invalid Attribute OID: OID must start with a digit", rdnString);
						}

						# endregion

						# region The remaining characters need to be a digit or a period

						while(IsDigit(rdnString[position]) || rdnString[position] == '.') {
							WriteUtf8Bytes(rawData, rdnString[position]);
							position++;
						}

						# endregion

						# region The OID can be followed by any number of blank spaces

						while(rdnString[position] == ' ') {
							position++;
						}

						# endregion

						# region And it needs to end with an equals sign

						// If we've found an equals sign, verify that the OID is well-formed,
						// then add it to the list of types.
						if(rdnString[position] == '=') {
							position++;

							string oid = Encoding.UTF8.GetString(rawData.ToArray());

							# region OIDs aren't allowed to end with a period

							if(oid.EndsWith(".")) {
								throw new ArgumentException("Invalid Rdn: OID cannot end with a period", rdnString);
							}

							# endregion

							# region You're also not allowed to have two periods in a row

							if(oid.IndexOf("..") > -1)
								throw new ArgumentException("Invalid Rdn: OIDs cannot have two periods in a row", rdnString);

							# endregion

							# region Also, numbers aren't allowed to have a leading zero

							// Let's clarify that with an example.
							// The OID "12.3.0.2" is totally allowed, 
							// but "12.3.04.2" isn't.  It would have to be
							// "12.3.4.2".  If we see a leading zero, we don't just ignore it.
							// We complain about it.  LOUDLY.

							string[] oidPieces = oid.Split('.');

							foreach(string oidPiece in oidPieces) {
								if(oidPiece.Length > 1 && oidPiece[0] == '0')
									throw new ArgumentException("Invalid Rdn: OIDs cannot have a leading zero", rdnString);
							}

							# endregion

							rawTypes.Add(oid);
							rawData.SetLength(0);
							state = ParserState.DetermineValueType;
						} else {
							throw new ArgumentException("Invalid Rdn: unterminated attribute name", rdnString);
						}


						# endregion
					} catch(IndexOutOfRangeException) {
						throw new ArgumentException("Invalid Rdn: unterminated attribute OID", rdnString);
					}

					break;

					# endregion

					# region case ParserState.DetermineValueType:

					case ParserState.DetermineValueType:

					try {
						# region Get rid of any leading spaces

						while(rdnString[position] == ' ') {
							position++;
						}

						# endregion

						# region Find out what the first character of the value is and set the state accordingly

						if(rdnString[position] == '"') {
							// It's a quoted string
							state = ParserState.GetQuotedValue;
						} else if(rdnString[position] == '#') {
							// It's a hex representation of a binary value
							state = ParserState.GetHexValue;
						} else {
							// It's a regular ol' unquoted string
							state = ParserState.GetUnquotedValue;
						}

						# endregion
					} catch(IndexOutOfRangeException) {
						// It's okay if we hit the end of the string without finding a value.
						// An empty value is allowed.  So we'll just drop this quietly and
						// go about our business, and the value will be reported as an empty string.
						state = ParserState.GetUnquotedValue;
						rawValues.Add("");
						rawValueTypes.Add(RdnComponent.RdnValueType.StringValue);
					}

					break;

					# endregion

					# region case ParserState.GetHexValue:

					case ParserState.GetHexValue:

					# region The first character has to be a pound sign

					if(rdnString[position] == '#') {
						WriteUtf8Bytes(rawData, rdnString[position]);
						position++;
					} else {
						throw new ArgumentException("Invalid Rdn: Hex values must start with #", rdnString);
					}

					# endregion

					# region The rest of the characters have to be hex digits

					while(position + 1 < rdnString.Length && IsHexDigit(rdnString[position]) && IsHexDigit(rdnString[position + 1])) {
						WriteUtf8Bytes(rawData, rdnString[position]);
						WriteUtf8Bytes(rawData, rdnString[position + 1]);
						position += 2;
					}

					# endregion

					# region Get rid of any trailing blank spaces

					while(position < rdnString.Length && rdnString[position] == ' ') {
						position++;
					}

					# endregion

					# region Check for end-of-string or + sign (which indicates a multi-valued Rdn)

					if(position >= rdnString.Length) {
						// If we're at the end of the string, just store what we've found and 
						// we'll exit the loop gracefully.
						rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
						rawData.SetLength(0);
						rawValueTypes.Add(RdnComponent.RdnValueType.HexValue);
					} else if(rdnString[position] == '+') {
						// if we've found a plus sign, that means that there's another name/value
						// pair after it.  We'll store what we've found, advance to the next character,
						// and set the state to DetermineAttributeType.
						position++;
						rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
						rawData.SetLength(0);
						state = ParserState.DetermineAttributeType;
						rawValueTypes.Add(RdnComponent.RdnValueType.HexValue);
					} else {
						throw new ArgumentException("Invalid Rdn: Invalid characters at end of value", rdnString);
					}

					# endregion

					break;

					# endregion

					# region case ParserState.GetQuotedValue:

					case ParserState.GetQuotedValue:

					# region The first character has to be a quote

					if(rdnString[position] == '"') {
						position++;
					} else {
						throw new ArgumentException("Invalid Rdn: Quoted values must start with \"", rdnString);
					}

					# endregion

					# region Read characters until we find a closing quote

					try {
						while(rdnString[position] != '"') {
							if(rdnString[position] == '\\') {
								try {
									position++;

									if(IsHexDigit(rdnString[position])) {
										rawData.WriteByte(Convert.ToByte(rdnString.Substring(position, 2), 16));

										position += 2;
									} else if(IsSpecialChar(rdnString[position]) || rdnString[position] == ' ') {
										WriteUtf8Bytes(rawData, rdnString[position]);
										position++;
									} else {
										throw new ArgumentException("Invalid Rdn: Escape sequence \\" + rdnString[position] + " is invalid.", rdnString);
									}
								} catch(IndexOutOfRangeException) {
									throw new ArgumentException("Invalid Rdn: Invalid escape sequence", rdnString);
								}
							} else {
								WriteUtf8Bytes(rawData, rdnString[position]);
								position++;
							}
						}
					} catch(IndexOutOfRangeException) {
						throw new ArgumentException("Invalid Rdn: Unterminated quoted value", rdnString);
					}

					position++;

					# endregion

					# region Remove any trailing spaces

					while(position < rdnString.Length && rdnString[position] == ' ') {
						position++;
					}

					# endregion

					# region Check for end-of-string or + sign (which indicates a multi-valued Rdn)

					if(position >= rdnString.Length) {
						// If we're at the end of the string, just store what we've found and 
						// we'll exit the loop gracefully.
						rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
						rawData.SetLength(0);
						rawValueTypes.Add(RdnComponent.RdnValueType.StringValue);
					} else if(rdnString[position] == '+') {
						// if we've found a plus sign, that means that there's another name/value
						// pair after it.  We'll store what we've found, advance to the next character,
						// and set the state to DetermineAttributeType.
						rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
						rawData.SetLength(0);
						state = ParserState.DetermineAttributeType;
						rawValueTypes.Add(RdnComponent.RdnValueType.StringValue);
					} else {
						throw new ArgumentException("Invalid Rdn: Invalid characters at end of value", rdnString);
					}

					# endregion

					break;

					# endregion

					# region case ParserState.GetUnquotedValue:

					case ParserState.GetUnquotedValue:

					# region Is the first character an escaped space or pound sign?

					try {
						if(rdnString[position] == '\\' && (rdnString[position + 1] == ' ' || rdnString[position + 1] == '#')) {
							WriteUtf8Bytes(rawData, rdnString[position + 1]);
							position += 2;
						}
					} catch(IndexOutOfRangeException) {
						throw new ArgumentException("Invalid Rdn: Invalid escape sequence", rdnString);
					}

					# endregion

					# region Read all characters, keeping track of trailing spaces

					int trailingSpaces = 0;

					while(position < rdnString.Length && rdnString[position] != '+') {
						if(rdnString[position] == ' ') {
							trailingSpaces++;

							WriteUtf8Bytes(rawData, rdnString[position]);

							position++;
						} else {
							trailingSpaces = 0;

							if(rdnString[position] == '\\') {
								try {
									position++;

									if(IsHexDigit(rdnString[position])) {
										rawData.WriteByte(Convert.ToByte(rdnString.Substring(position, 2), 16));

										position += 2;
									} else if(IsSpecialChar(rdnString[position]) || rdnString[position] == ' ') {
										WriteUtf8Bytes(rawData, rdnString[position]);
										position++;
									} else {
										throw new ArgumentException("Invalid Rdn: Escape sequence \\" + rdnString[position] + " is invalid.", rdnString);
									}
								} catch(IndexOutOfRangeException) {
									throw new ArgumentException("Invalid Rdn: Invalid escape sequence", rdnString);
								}
							} else if(IsSpecialChar(rdnString[position])) {
								throw new ArgumentException("Invalid Rdn: Unquoted special character '" + rdnString[position] + "'", rdnString);
							} else {
								WriteUtf8Bytes(rawData, rdnString[position]);
								position++;
							}
						}
					}

					# endregion

					# region Remove any trailing spaces

					rawData.SetLength(rawData.Length - trailingSpaces);

					# endregion

					# region If the last character is a + sign, set state to look for another value, otherwise, finish up

					if(position < rdnString.Length) {
						if(rdnString[position] == '+') {
							// if we've found a plus sign, that means that there's another name/value
							// pair after it.  We'll store what we've found, advance to the next character,
							// and set the state to DetermineAttributeType.
							position++;
							rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
							rawData.SetLength(0);
							state = ParserState.DetermineAttributeType;
							rawValueTypes.Add(RdnComponent.RdnValueType.StringValue);
						} else {
							throw new ArgumentException("Invalid Rdn: invalid trailing character", rdnString);
						}
					} else {
						rawValues.Add(Encoding.UTF8.GetString(rawData.ToArray()));
						rawData.SetLength(0);
						rawValueTypes.Add(RdnComponent.RdnValueType.StringValue);
					}

					# endregion

					break;

					# endregion
				}
			}

			# region Check ending state

			if(!(state == ParserState.GetHexValue ||
				state == ParserState.GetQuotedValue ||
				state == ParserState.GetUnquotedValue
				)) {
				throw new ArgumentException("Invalid Rdn", rdnString);
			}

			# endregion

			# region Store the results we've collected
			List<RdnComponent> result = new List<RdnComponent>();

			for(int i = 0; i < rawTypes.Count; i++) {
				RdnComponent rdnComponent = new RdnComponent(rawTypes[i], rawValues[i], rawValueTypes[i]);
				result.Add(rdnComponent);
			}

			return result;

			# endregion
		}

		private static bool IsAlpha(char c) {
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		private static bool IsDigit(char c) {
			return (c >= '0' && c <= '9');
		}

		private static bool IsHexDigit(char c) {
			return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
		}

		public static bool IsSpecialChar(char c) {
			return c == ',' || c == '=' || c == '+' || c == '<' || c == '>' || c == '#' || c == ';' || c == '\\' || c == '"';
		}

	}
}

