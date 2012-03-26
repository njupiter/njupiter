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
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

using log4net;

namespace nJupiter.Net.Mail {

	/// <summary>
	/// SMTP-client implemented after RFC2821
	/// </summary>
	public class SmtpClient {

		#region Members
		private readonly string host = "localhost";
		private readonly string userName = string.Empty;
		private readonly string password = string.Empty;
		private readonly int timeout = 100;
		#endregion

		#region Static Members
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Constructors
		public SmtpClient() { }

		public SmtpClient(string host) {
			if(host == null) {
				throw new ArgumentNullException("host");
			}
			this.host = host;
		}

		public SmtpClient(string host, int timeout)
			: this(host) {
			if(timeout <= 0) {
				throw new ArgumentOutOfRangeException("timeout", "Timeout can not be less than 1 second.");
			}
			this.timeout = timeout;
		}

		public SmtpClient(string host, string userName, string password)
			: this(host) {
			if(userName == null) {
				throw new ArgumentNullException("userName");
			}
			if(password == null) {
				throw new ArgumentNullException("password");
			}
			this.userName = userName;
			this.password = password;
		}

		public SmtpClient(string host, string userName, string password, int timeout)
			: this(host, userName, password) {
			if(timeout <= 0) {
				throw new ArgumentOutOfRangeException("timeout", "Timeout can not be less than 1 second.");
			}
			this.host = host;
			this.timeout = timeout;
		}
		#endregion

		#region Enums
		private enum SmtpReplyCode {
			ConnectSuccess = 220,
			QuitSuccess = 221,
			AuthSuccess = 235,
			GenericSuccess = 250,
			AuthRequest = 334,
			DataSuccess = 354
		}
		#endregion

		#region Methods
		public void Send(Mail mail) {
			if(mail == null)
				throw new ArgumentNullException("mail");
			string from = (mail.Sender != null && mail.Sender.Mail.Length > 0 ? mail.Sender.Mail : mail.From.Mail);
			if(from == null)
				throw new ArgumentException("No from-address specified.", "mail");

			IPAddress ipAddress;
			if(!IPAddress.TryParse(this.host, out ipAddress)) {
				IPHostEntry ipHost = Dns.GetHostEntry(this.host);
				ipAddress = ipHost.AddressList[0];
			}
			IPEndPoint endPoint = new IPEndPoint(ipAddress, 25);

			using(Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)) {
				if(Log.IsDebugEnabled) { Log.Debug(string.Format("Connecting to SMTP-server '{0}'", this.host)); }

				socket.Connect(endPoint);
				WaitForResponse(socket, SmtpReplyCode.ConnectSuccess);

				this.Authenticate(socket);

				SendData(socket, string.Format(CultureInfo.InvariantCulture, "MAIL From: {0}\r\n", from), SmtpReplyCode.GenericSuccess);

				this.ValidateReceivingAddresses(socket, mail.To);
				this.ValidateReceivingAddresses(socket, mail.CC);
				this.ValidateReceivingAddresses(socket, mail.BCC);

				SendData(socket, "DATA\r\n", SmtpReplyCode.DataSuccess);

				SendData(socket, new StringBuilder(mail.ToString()).Append(".\r\n").ToString(), SmtpReplyCode.GenericSuccess);

				SendData(socket, "QUIT\r\n", SmtpReplyCode.QuitSuccess);
			}
		}
		#endregion

		#region Helper Methods
		private SmtpResponse SendData(Socket socket, string message, SmtpReplyCode expectedResponse) {
			byte[] msg = Encoding.UTF8.GetBytes(message);
			if(Log.IsDebugEnabled) { Log.Debug(string.Format("Sending data '{0}' to SMTP-server {1}", message, this.host)); }
			socket.Send(msg, 0, msg.Length, SocketFlags.None);
			if(!expectedResponse.Equals(SmtpReplyCode.QuitSuccess)) {
				return this.WaitForResponse(socket, expectedResponse);
			}
			return null;
		}

		private SmtpResponse WaitForResponse(Socket socket, SmtpReplyCode expectedResponse) {
			DateTime timeStamp = DateTime.Now.AddSeconds(this.timeout);
			while(socket.Available == 0) {
				if(DateTime.Now > timeStamp) {
					throw new SmtpTimeoutException(string.Format("Connection timeout while sending data to the server '{0}'", this.host));
				}
				Thread.Sleep(10);
			}

			byte[] bytes = new byte[1024];

			socket.Receive(bytes, 0, socket.Available, SocketFlags.None);

			string response = Encoding.UTF8.GetString(bytes);
			int replyCode = int.Parse(response.Substring(0, 3), NumberFormatInfo.InvariantInfo);
			string message = response.Substring(4, response.Length - 4);

			if(replyCode != (int)expectedResponse) {
				switch(expectedResponse) {
					case SmtpReplyCode.ConnectSuccess:
					throw new SmtpConnectionException(string.Format("Failed to connect to SMTP-server '{0}'. Code: {1} Message: {2}", this.host, replyCode, message));
					case SmtpReplyCode.AuthSuccess:
					case SmtpReplyCode.AuthRequest:
					throw new SmtpAuthenticationException(string.Format("Authentication to SMTP-server failed '{0}'. Code: {1} Message: {2}", this.host, replyCode, message));
					case SmtpReplyCode.DataSuccess:
					throw new SmtpDataTransferException(string.Format("Failed to send data to SMTP-server '{0}'. Code: {1} Message: {2}", this.host, replyCode, message));
					default:
					throw new SmtpException(string.Format("Failed to send mail through SMTP-server '{0}'. Code: {1} Message: {2}", this.host, replyCode, message));
				}
			}

			return new SmtpResponse(replyCode, message);
		}

		private void ValidateReceivingAddresses(Socket socket, MailAddressCollection addresses) {
			if(addresses != null) {
				foreach(MailAddress address in addresses) {
					SendData(socket, string.Format(CultureInfo.InvariantCulture, "RCPT TO: {0}\r\n", address.Mail), SmtpReplyCode.GenericSuccess);
				}
			}
		}

		private void Authenticate(Socket socket) {
			if(this.userName.Length > 0 || this.password.Length > 0) {
				SmtpResponse smtpResponse = SendData(socket, string.Format(CultureInfo.InvariantCulture, "EHLO {0}\r\n", Dns.GetHostName()), SmtpReplyCode.GenericSuccess);

				if(smtpResponse.Message.IndexOf("AUTH=LOGIN") >= 0) {
					SendData(socket, "AUTH LOGIN\r\n", SmtpReplyCode.AuthRequest);

					byte[] bytes = Encoding.UTF8.GetBytes(this.userName);
					SendData(socket, Convert.ToBase64String(bytes) + Environment.NewLine, SmtpReplyCode.AuthRequest);

					bytes = Encoding.UTF8.GetBytes(this.password);
					SendData(socket, Convert.ToBase64String(bytes) + Environment.NewLine, SmtpReplyCode.AuthSuccess);
				} else {
					throw new SmtpAuthenticationException(string.Format("Authentication to SMTP-server failed '{0}'. Code: {1} Message: {2}", this.host, smtpResponse.ReplyCode, smtpResponse.Message));
				}
			} else {
				SendData(socket, string.Format(CultureInfo.InvariantCulture, "HELO {0}\r\n", Dns.GetHostName()), SmtpReplyCode.GenericSuccess);
			}
		}
		#endregion

		#region Helper Classes
		private sealed class SmtpResponse {
			public readonly int ReplyCode;
			public readonly string Message;

			public SmtpResponse(int code, string message) {
				ReplyCode = code;
				Message = message;
			}
		}
		#endregion

	}
}
