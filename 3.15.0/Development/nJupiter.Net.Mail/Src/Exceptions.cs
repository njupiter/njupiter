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
using System.Runtime.Serialization;

namespace nJupiter.Net.Mail {
	
	[Serializable] 
	public class SmtpException : Exception {
		public SmtpException(){}
		public SmtpException(string message) : base(message){}
		public SmtpException(string message, Exception inner) : base(message, inner){}
		protected SmtpException(SerializationInfo info, StreamingContext context) : base(info, context){}
	}

	[Serializable] 
	public class SmtpConnectionException : SmtpException {
		public SmtpConnectionException(){}
		public SmtpConnectionException(string message) : base(message){}
		public SmtpConnectionException(string message, Exception inner) : base(message, inner){}
		protected SmtpConnectionException(SerializationInfo info, StreamingContext context) : base(info, context){}
	}

	[Serializable] 
	public class SmtpTimeoutException : SmtpConnectionException {
		public SmtpTimeoutException(){}
		public SmtpTimeoutException(string message) : base(message){}
		public SmtpTimeoutException(string message, Exception inner) : base(message, inner){}
		protected SmtpTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context){}
	}

	[Serializable] 
	public class SmtpAuthenticationException : SmtpException {
		public SmtpAuthenticationException(){}
		public SmtpAuthenticationException(string message) : base(message){}
		public SmtpAuthenticationException(string message, Exception inner) : base(message, inner){}
		protected SmtpAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context){}
	}

	[Serializable]
	public class SmtpDataTransferException : SmtpException {
		public SmtpDataTransferException(){}
		public SmtpDataTransferException(string message) : base(message){}
		public SmtpDataTransferException(string message, Exception inner) : base(message, inner){}
		protected SmtpDataTransferException(SerializationInfo info, StreamingContext context) : base(info, context){}
	}


}
