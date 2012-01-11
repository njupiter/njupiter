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
using System.Collections;
using System.Threading;

using nJupiter.Configuration;

namespace nJupiter.Net.Mail {

	/// <summary>
	/// Abstract class for fabricating MailSender instances.
	/// </summary>
	public abstract class MailSender {
		/// <summary>
		/// Private construct avoids instanciation
		/// </summary>
		internal MailSender() { }

		public abstract void SendEmails();
		public abstract void SendEmails(bool synchronize);

		/// <summary>
		/// Returns instance of MailSender.
		/// </summary>
		public static MailSender CreateMailSender(Mail mail) {
			MailCollection arrList = new MailCollection();
			arrList.Add(mail);
			return new MailSenderImpl(arrList);
		}

		/// <summary>
		/// Returns a instance of MailSender
		/// </summary>
		public static MailSender CreateMailSender(MailCollection mailList) {
			return new MailSenderImpl(mailList);
		}
	}

	#region Default implementation of the MailSender interface

	internal class MailSenderImpl : MailSender {

		private readonly ArrayList mailList;

		internal MailSenderImpl(MailCollection mailList) {
			this.mailList = new ArrayList(mailList);
		}

		public override void SendEmails() {
			ThreadStart startDelegate = this.Send;
			Thread mailThread = new Thread(startDelegate);
			mailThread.Name = "nJupiter.Net.Mail.MailSender Thread";
			mailThread.Start();
		}

		public override void SendEmails(bool synchronize) {
			if(synchronize) {
				Send(true);
			} else {
				SendEmails();
			}
		}

		/// <summary>
		/// Method to attach to the thread to send out the mails.
		/// </summary>
		private void Send() {
			Send(false);
		}

		private void Send(bool synchronize) {
			string smtpServer = "localhost";
			string userName = string.Empty;
			string password = string.Empty;
			try {
				IConfig systemConfig = ConfigRepository.Instance.GetSystemConfig();
				if(systemConfig.ContainsKey("mailConfig", "smtpServer")) {
					smtpServer = systemConfig.GetValue("mailConfig", "smtpServer");
				}
				if(systemConfig.ContainsAttribute("mailConfig", "smtpServer", "userName")) {
					userName = systemConfig.GetAttribute("mailConfig", "smtpServer", "userName");
				}
				if(systemConfig.ContainsAttribute("mailConfig", "smtpServer", "password")) {
					password = systemConfig.GetAttribute("mailConfig", "smtpServer", "password");
				}
				SmtpClient smtpClient = new SmtpClient(smtpServer, userName, password);
				foreach(Mail mail in this.mailList) {
					// get Smtp server
					try {
						smtpClient.Send(mail);
					} catch(Exception) {
						if(synchronize) {
							throw;
						}
					}
				}
			} catch(Exception) {
				if(synchronize) {
					throw;
				}
			}
		}
	}
	#endregion
}
