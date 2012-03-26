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

namespace nJupiter.Net.Mail {

	public class MailAddress {

		#region Members
		private readonly string name;
		private readonly string mail;
		#endregion

		#region Constructors
		public MailAddress(string mail) {
			if(mail == null)
				throw new ArgumentNullException("mail");
			this.mail = mail;
		}

		public MailAddress(string mail, string name)
			: this(mail) {
			this.name = name;
		}
		#endregion

		#region Properties
		public string Mail { get { return this.mail; } }
		public string Name { get { return this.name; } }
		#endregion

		#region Methods
		public override int GetHashCode() {
			return this.Mail.GetHashCode();
		}

		public override bool Equals(object obj) {
			MailAddress objMailAddress = obj as MailAddress;
			return objMailAddress != null && objMailAddress.Mail.Equals(this.Mail);
		}

		public override string ToString() {
			return this.mail;
		}
		#endregion
	}

}
