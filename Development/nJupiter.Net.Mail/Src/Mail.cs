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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Globalization;

namespace nJupiter.Net.Mail {

	/// <summary>
	/// Mail message
	/// </summary>
	public class Mail {

		#region Constants
		private const string AddressPattern = @"([\w\W]+)<([\w\W]+)>";
		private const string EndofblockPattern = @"\</[h1|h2|h3|h4|h5|h6|hr|p|pre|ul|dt|li]*?\>|<br*?>";
		private const string ClosingtagPattern = @"<script.*?>.*?</script.*?>|<style.*?>.*?</style.*?>";
		private const string HtmltagPattern = @"[\f\t\v]|<(?:[^""']+?|.+?(?:""|').*?(?:""|')?.*?)*?>";
		private const string EndoflinePattern = @"[\n\r\f\t\v]{3,}";
		#endregion

		#region Constants
		private static readonly Regex AddressRegEx = new Regex(AddressPattern);
		private static readonly Regex EndOfBlockRegEx = new Regex(EndofblockPattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
		private static readonly Regex ClosingTagRegEx = new Regex(ClosingtagPattern, RegexOptions.Singleline);
		private static readonly Regex HtmlTagRegEx = new Regex(HtmltagPattern);
		private static readonly Regex EndOfLineRegEx = new Regex(EndoflinePattern);
		#endregion

		#region Members
		private readonly MailAddressCollection to;
		private readonly AttachmentCollection attachments;
		private readonly NameValueCollection headers;
		#endregion

		#region Constructors
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, MailAddress replyTo, MailAddress sender, bool senderHidden, string alternativeMessage, AttachmentCollection attachments, MailFormat format) {
			this.to = to;
			this.From = from;
			this.ReplyTo = replyTo;
			this.Sender = sender;
			this.Message = message;
			this.AlternativeMessage = alternativeMessage;
			this.Subject = subject;
			this.CC = cc;
			this.BCC = bcc;
			this.attachments = (attachments ?? new AttachmentCollection());
			this.headers = new NameValueCollection();
			this.Format = format;
			this.SenderHidden = senderHidden;
		}

		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, MailAddress replyTo, MailAddress sender, bool senderHidden, string alternativeMessage, AttachmentCollection attachments, MailFormat format)
			: this(null as MailAddressCollection, from, message, subject, cc, bcc, replyTo, sender, senderHidden, alternativeMessage, attachments, format) {
			if(to != null) {
				this.to = new MailAddressCollection(to);
			}
		}

		public Mail(MailAddressCollection to, MailAddress from, string message, string subject) : this(to, from, message, subject, null, null, null, null, false, null, null, MailFormat.Text) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, AttachmentCollection attachments) : this(to, from, message, subject, null, null, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailFormat format) : this(to, from, message, subject, null, null, null, null, false, null, null, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, null, null, null, null, false, null, attachments, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc) : this(to, from, message, subject, cc, null, null, null, false, null, null, MailFormat.Text) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, AttachmentCollection attachments) : this(to, from, message, subject, cc, null, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, MailFormat format) : this(to, from, message, subject, cc, null, null, null, false, null, null, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, cc, null, null, null, false, null, attachments, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, AttachmentCollection attachments) : this(to, from, message, subject, cc, bcc, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, cc, bcc, null, null, false, null, attachments, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, null, null, null, false, alternativeMessage, null, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailFormat format, string alternativeMessage) : this(to, from, message, subject, null, null, null, null, false, alternativeMessage, null, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, AttachmentCollection attachments, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, null, null, null, false, alternativeMessage, attachments, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, bcc, null, null, false, alternativeMessage, null, format) { }
		public Mail(MailAddressCollection to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, AttachmentCollection attachments, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, bcc, null, null, false, alternativeMessage, attachments, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject) : this(to, from, message, subject, null, null, null, null, false, null, null, MailFormat.Text) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, AttachmentCollection attachments) : this(to, from, message, subject, null, null, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailFormat format) : this(to, from, message, subject, null, null, null, null, false, null, null, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, null, null, null, null, false, null, attachments, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc) : this(to, from, message, subject, cc, null, null, null, false, null, null, MailFormat.Text) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, AttachmentCollection attachments) : this(to, from, message, subject, cc, null, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, MailFormat format) : this(to, from, message, subject, cc, null, null, null, false, null, null, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, cc, null, null, null, false, null, attachments, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, AttachmentCollection attachments) : this(to, from, message, subject, cc, bcc, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, cc, bcc, null, null, false, null, attachments, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, null, null, null, false, alternativeMessage, null, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailFormat format, string alternativeMessage) : this(to, from, message, subject, null, null, null, null, false, alternativeMessage, null, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, AttachmentCollection attachments, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, null, null, null, false, alternativeMessage, attachments, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, bcc, null, null, false, alternativeMessage, null, format) { }
		public Mail(MailAddress to, MailAddress from, string message, string subject, MailAddressCollection cc, MailAddressCollection bcc, AttachmentCollection attachments, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, bcc, null, null, false, alternativeMessage, attachments, format) { }
		public Mail(string to, string from, string message, string subject, string cc, string bcc, string replyTo, string sender, bool senderHidden, string alternativeMessage, AttachmentCollection attachments, MailFormat format) : this(GetMailAddressCollection(to), GetMailAddress(from), message, subject, GetMailAddressCollection(cc), GetMailAddressCollection(bcc), GetMailAddress(replyTo), GetMailAddress(sender), senderHidden, alternativeMessage, attachments, format) { }
		public Mail(string to, string from, string message, string subject) : this(to, from, message, subject, null, null, null, null, false, null, null, MailFormat.Text) { }
		public Mail(string to, string from, string message, string subject, AttachmentCollection attachments) : this(to, from, message, subject, null, null, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(string to, string from, string message, string subject, MailFormat format) : this(to, from, message, subject, null, null, null, null, false, null, null, format) { }
		public Mail(string to, string from, string message, string subject, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, null, null, null, null, false, null, attachments, format) { }
		public Mail(string to, string from, string message, string subject, string cc) : this(to, from, message, subject, cc, null, null, null, false, null, null, MailFormat.Text) { }
		public Mail(string to, string from, string message, string subject, string cc, AttachmentCollection attachments) : this(to, from, message, subject, cc, null, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(string to, string from, string message, string subject, string cc, MailFormat format) : this(to, from, message, subject, cc, null, null, null, false, null, null, format) { }
		public Mail(string to, string from, string message, string subject, string cc, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, cc, null, null, null, false, null, attachments, format) { }
		public Mail(string to, string from, string message, string subject, string cc, string bcc, AttachmentCollection attachments) : this(to, from, message, subject, cc, bcc, null, null, false, null, attachments, MailFormat.Text) { }
		public Mail(string to, string from, string message, string subject, string cc, string bcc, AttachmentCollection attachments, MailFormat format) : this(to, from, message, subject, cc, bcc, null, null, false, null, attachments, format) { }
		public Mail(string to, string from, string message, string subject, string cc, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, null, null, null, false, alternativeMessage, null, format) { }
		public Mail(string to, string from, string message, string subject, MailFormat format, string alternativeMessage) : this(to, from, message, subject, null, null, null, null, false, alternativeMessage, null, format) { }
		public Mail(string to, string from, string message, string subject, string cc, AttachmentCollection attachments, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, null, null, null, false, alternativeMessage, attachments, format) { }
		public Mail(string to, string from, string message, string subject, string cc, string bcc, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, bcc, null, null, false, alternativeMessage, null, format) { }
		public Mail(string to, string from, string message, string subject, string cc, string bcc, AttachmentCollection attachments, MailFormat format, string alternativeMessage) : this(to, from, message, subject, cc, bcc, null, null, false, alternativeMessage, attachments, format) { }
		#endregion

		#region Properties
		public MailAddressCollection To { get { return this.to; } }
		public MailAddress From { get; set; }
		public MailAddress ReplyTo { get; set; }
		public MailAddress Sender { get; set; }
		public bool SenderHidden { get; set; }
		public string Message { get; set; }
		public string AlternativeMessage { get; set; }
		public string Subject { get; set; }
		public MailAddressCollection CC { get; private set; }
		public MailAddressCollection BCC { get; private set; }
		public MailFormat Format { get; set; }
		public AttachmentCollection Attachments { get { return this.attachments; } }
		public NameValueCollection Headers { get { return this.headers; } }
		public Encoding MessageEncoding { get; set; }
		public Encoding AlternativeMessageEncoding { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Generates a mail as Standard Arpa Internet Message defined in RFC822
		/// </summary>
		/// <returns>A mail as text</returns>
		public override string ToString() {
			return GenerateMail(this);
		}

		/// <summary>
		/// Save the mail to disk as an Standard Arpa Internet Message defined in RFC822.
		/// This method can for example be used to save the mail as .eml to a SMTP pickup folder
		/// </summary>
		/// <param name="fileName">The full path and filename</param>
		/// <returns>The created file</returns>
		public virtual FileInfo Save(string fileName) {
			FileInfo file = new FileInfo(fileName);
			using(FileStream fileStream = file.OpenWrite()) {
				string content = this.ToString();
				byte[] binaryData = Encoding.ASCII.GetBytes(content);
				fileStream.Write(binaryData, 0, binaryData.Length);
			}
			return file;
		}
		#endregion

		#region Helper Methods
		private static MailAddressCollection GetMailAddressCollection(string addresses) {
			MailAddressCollection result = new MailAddressCollection();
			if(!string.IsNullOrEmpty(addresses)) {
				string[] addressArray = addresses.Split(';', ',');
				foreach(string address in addressArray) {
					result.Add(GetMailAddress(address));
				}
			}
			return result;
		}

		private static MailAddress GetMailAddress(string address) {
			if(string.IsNullOrEmpty(address))
				return null;
			string[] split = AddressRegEx.Split(address);
			if(split.Length == 4) {
				string name = split[1].Trim();
				if(name.Length <= 0)
					name = null;
				return new MailAddress(split[2].Trim(), name);
			}
			if(address.Length != 0)
				return new MailAddress(address);
			return null;
		}

		private static string GenerateMail(Mail mail) {
			StringBuilder sb = new StringBuilder();

			AppendHeaders(mail, sb);

			if(mail.Attachments != null && mail.Attachments.Count > 0) {
				AppendAttachments(mail, sb);
			} else {
				AppendBody(mail, sb, true);
			}
			return sb.ToString();
		}

		private static void AppendHeaders(Mail mail, StringBuilder sb) {

			AppendSender(sb, mail.From, "From");
			AppendSender(sb, mail.ReplyTo, "Reply-To");

			if(!mail.SenderHidden && mail.Sender != null && !mail.Sender.Equals(mail.From))
				AppendSender(sb, mail.Sender, "Sender");

			AppendRecivers(sb, mail.To, "To");
			AppendRecivers(sb, mail.CC, "Cc");

			DateTime now = DateTime.Now;
			sb.Append("Date: ");
			sb.Append(now.ToString("ddd, dd MMMM yyyy HH:mm:ss ", DateTimeFormatInfo.InvariantInfo));
			sb.Append(now.ToString("zzzz", DateTimeFormatInfo.InvariantInfo).Replace(":", string.Empty));
			sb.Append("\r\n");
			sb.Append("Subject: ");
			if(!string.IsNullOrEmpty(mail.Subject))
				sb.Append(EncodeHeaderValue(mail.Subject));
			sb.Append("\r\n");
			sb.Append("X-Mailer: nJupiter SMTP-Client V");
			sb.Append(typeof(Mail).Assembly.GetName().Version.ToString());
			sb.Append("\r\n");
			//TODO: fold header values (insert crlf after 78 chars, then continue the same way)
			foreach(string key in mail.Headers.AllKeys) {
				sb.Append(key + ": " + mail.Headers[key]);
				sb.Append("\r\n");
			}
		}

		private static void AppendBody(Mail mail, StringBuilder sb, bool addMimeHeader) {
			if(addMimeHeader)
				sb.Append("MIME-Version: 1.0\r\n");
			if(mail.Format.Equals(MailFormat.TextAndHtml)) {
				string uniqueBoundary = GenerateUniqueBoundary();
				sb.Append("Content-Type: multipart/alternative; boundary=\"");
				sb.Append(uniqueBoundary);
				sb.Append("\"\r\n\r\n");
				if(addMimeHeader)
					sb.Append("This is a multi-part message in MIME format.\r\n");
				sb.Append("--");
				sb.Append(uniqueBoundary);
				sb.Append("\r\n");
				if(!string.IsNullOrEmpty(mail.AlternativeMessage))
					AppendMessage(sb, mail.AlternativeMessage, "plain", mail.AlternativeMessageEncoding);
				else
					AppendMessage(sb, StripHtmlFromMessage(mail.Message), "plain", mail.MessageEncoding);
				sb.Append("--");
				sb.Append(uniqueBoundary);
				sb.Append("\r\n");
				AppendMessage(sb, mail.Message, "html", mail.MessageEncoding);
				sb.Append("--");
				sb.Append(uniqueBoundary);
				sb.Append("--\r\n");
			} else if(mail.Format.Equals(MailFormat.Html)) {
				AppendMessage(sb, mail.Message, "html", mail.MessageEncoding);
			} else {
				AppendMessage(sb, mail.Message, "plain", mail.MessageEncoding);
			}
		}

		private static void AppendMessage(StringBuilder sb, string message, string format, Encoding encoding) {
			if(message == null)
				message = string.Empty;
			sb.Append("Content-Type: text/");
			sb.Append(format);
			if(StringHandler.IsAscii(message)) {
				sb.Append("\r\n");
				sb.Append("Content-Transfer-Encoding: 7Bit\r\n");
				sb.Append("\r\n");
				sb.Append(message);
			} else {
				if((encoding == null && !StringHandler.IsAnsi(message)) || (encoding != null && encoding.Equals(Encoding.UTF8))) {
					sb.Append("; charset=utf-8\r\n");
					sb.Append("Content-Transfer-Encoding: base64\r\n");
					sb.Append("\r\n");
					string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));
					ChunkString(sb, base64String, 73);
				} else {
					if(encoding == null)
						encoding = Encoding.GetEncoding(1252);
					sb.Append("; charset=");
					sb.Append(encoding.BodyName);
					sb.Append("\r\n");
					sb.Append("Content-Transfer-Encoding: quoted-printable\r\n");
					sb.Append("\r\n");
					sb.Append(StringHandler.EncodeToQuotedPrintable(message, encoding));
				}
			}
			if(!message.EndsWith("\r\n"))
				sb.Append("\r\n");
			sb.Append("\r\n");
		}

		private static void AppendAttachments(Mail mail, StringBuilder sb) {
			string uniqueBoundary = GenerateUniqueBoundary();
			sb.Append("MIME-Version: 1.0\r\n");
			sb.Append("Content-Type: multipart/mixed; boundary=\"");
			sb.Append(uniqueBoundary);
			sb.Append("\"\r\n\r\n");
			sb.Append("This is a multi-part message in MIME format.\r\n");
			sb.Append("--");
			sb.Append(uniqueBoundary);
			sb.Append("\r\n");

			AppendBody(mail, sb, false);

			foreach(Attachment attachment in mail.Attachments) {
				byte[] binaryData;
				sb.Append("--");
				sb.Append(uniqueBoundary);
				sb.Append("\r\n");
				sb.Append("Content-Type: ");
				sb.Append(attachment.ContentType);
				sb.Append("\r\n");
				sb.Append("Content-Transfer-Encoding: base64\r\n");
				sb.Append("Content-Disposition: attachment; filename=");
				sb.Append(attachment.FileName);
				sb.Append("\r\n");
				sb.Append("\r\n");
				using(Stream fileStream = attachment.OpenRead()) {
					binaryData = new Byte[fileStream.Length];
					fileStream.Read(binaryData, 0, (int)fileStream.Length);
				}
				string base64String = Convert.ToBase64String(binaryData, 0, binaryData.Length);
				ChunkString(sb, base64String, 73);
				sb.Append("\r\n");
			}
			sb.Append("--");
			sb.Append(uniqueBoundary);
			sb.Append("--\r\n");
		}

		private static void ChunkString(StringBuilder sb, string s, int chunkLength) {
			for(int i = 0; i < s.Length; ) {
				int nextchunk = chunkLength;
				if(s.Length - (i + nextchunk) < 0)
					nextchunk = s.Length - i;
				sb.Append(s.Substring(i, nextchunk));
				sb.Append("\r\n");
				i += nextchunk;
			}
		}

		private static void AppendSender(StringBuilder sb, MailAddress address, string type) {
			if(address != null) {
				sb.Append(type);
				sb.Append(": ");
				AppendAddress(sb, address);
				sb.Append("\r\n");
			}
		}

		private static void AppendRecivers(StringBuilder sb, MailAddressCollection addresses, string type) {
			if(addresses != null && addresses.Count > 0) {
				sb.Append(type);
				sb.Append(": ");
				for(int i = 0; i < addresses.Count; i++) {
					sb.Append(i > 0 ? ", " : string.Empty);
					AppendAddress(sb, addresses[i]);
				}
				sb.Append("\r\n");
			}
		}

		private static void AppendAddress(StringBuilder sb, MailAddress address) {
			if(address != null) {
				if(address.Name != null) {
					sb.Append(EncodeHeaderValue(address.Name));
					sb.Append(" <");
					sb.Append(address.Mail);
					sb.Append(">");
				} else {
					sb.Append(address.Mail);
				}
			}
		}

		private static string GenerateUniqueBoundary() {
			return "----" + Guid.NewGuid().ToString("N");
		}

		private static string StripHtmlFromMessage(string message) {
			message = ClosingTagRegEx.Replace(message, string.Empty);
			message = EndOfBlockRegEx.Replace(message, "\r\n");
			message = HtmlTagRegEx.Replace(message, string.Empty);
			message = EndOfLineRegEx.Replace(message, "\r\n\r\n");
			message = System.Web.HttpUtility.HtmlDecode(message);
			return message;
		}

		private static string EncodeHeaderValue(string value) {
			StringBuilder sb = new StringBuilder();
			if(StringHandler.IsAscii(value))
				return value;
			if(StringHandler.IsAnsi(value)) {
				Encoding enc = Encoding.GetEncoding(1252);
				sb.Append("=?");
				sb.Append(enc.BodyName);
				sb.Append("?Q?");
				sb.Append(StringHandler.EncodeToQuotedPrintable(value, enc, true));
				sb.Append("?=");
			} else {
				sb.Append("=?utf-8?B?");
				sb.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
				sb.Append("?=");
			}
			return sb.ToString();
		}
		#endregion


	}

	#region Enums
	public enum MailFormat {
		Text,
		Html,
		TextAndHtml
	}
	#endregion

}
