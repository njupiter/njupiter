<%@ Page Language="C#" AutoEventWireup="false" %>
<%@ Register TagPrefix="Control" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>Simple Mail Client</title>
<script language="C#" runat="server">
	protected override void OnInit(EventArgs e) {
		base.OnInit(e);
		WebButton sendButton = (WebButton)this.FindControl("sendButton");
		sendButton.Click += this.SendButtonClick;
	}

	void SendButtonClick(object sender, EventArgs e) {
		TextBox serverTextBox	= (TextBox)this.FindControl("serverTextBox");
		TextBox fromTextBox		= (TextBox)this.FindControl("fromTextBox");
		TextBox toTextBox		= (TextBox)this.FindControl("toTextBox");
		TextBox subjectTextBox	= (TextBox)this.FindControl("subjectTextBox");
		TextBox bodyTextBox		= (TextBox)this.FindControl("bodyTextBox");
		CheckBox checkBox		= (CheckBox)this.FindControl("nJupiterMailCheckBox");

		if(checkBox.Checked) {
			nJupiter.Net.Mail.MailAddress from = new nJupiter.Net.Mail.MailAddress(fromTextBox.Text);
			nJupiter.Net.Mail.MailAddress to = new nJupiter.Net.Mail.MailAddress(toTextBox.Text);

			nJupiter.Net.Mail.Mail mail = new nJupiter.Net.Mail.Mail(to, from, bodyTextBox.Text, subjectTextBox.Text);

			nJupiter.Net.Mail.SmtpClient smtpClient = new nJupiter.Net.Mail.SmtpClient(serverTextBox.Text);

			smtpClient.Send(mail);
		} else {
			System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(fromTextBox.Text);
			System.Net.Mail.MailAddress to = new System.Net.Mail.MailAddress(toTextBox.Text);
			
			System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(from, to);
			mail.Subject = subjectTextBox.Text;
			mail.Body = bodyTextBox.Text;
			
			System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(serverTextBox.Text);
			
			smtpClient.Send(mail);
		}
 }
 </script>
</head>
<body>
	<form id="form1" runat="server">
		<p>
			<Control:WebLabel For="nJupiterMailCheckBox" runat="server">Use nJupiter.Net.Mail</Control:WebLabel><br />
			<asp:CheckBox ID="nJupiterMailCheckBox" Checked="true" runat="server" />
		</p>
		<p>
			<Control:WebLabel For="serverTextBox" runat="server">Server</Control:WebLabel><br />
			<asp:TextBox ID="serverTextBox" runat="server" Text="localhost" />
		</p>
		<p>
			<Control:WebLabel For="fromTextBox" runat="server">From</Control:WebLabel><br />
			<asp:TextBox ID="fromTextBox" runat="server" Text="m4@njupiter.org" />
		</p>
		<p>
			<Control:WebLabel For="toTextBox" runat="server">To</Control:WebLabel><br />
			<asp:TextBox ID="toTextBox" runat="server" Text="m4@njupiter.org" />
		</p>
		<p>
			<Control:WebLabel For="subjectTextBox" runat="server">Subject</Control:WebLabel><br />
			<asp:TextBox ID="subjectTextBox" runat="server" Text="Test Mail" />
		</p>
		<p>
			<Control:WebLabel For="txbodyTextBoxtBody" runat="server">Bodytext</Control:WebLabel><br />
			<asp:TextBox ID="bodyTextBox" runat="server" TextMode="MultiLine" Text="Lorem ipsum" />
		</p>
		<p>
			<Control:WebButton ID="sendButton" runat="server">Send</Control:WebButton>
		</p>
	</form>
</body>
</html>