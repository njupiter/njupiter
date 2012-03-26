<%@ Page Language="C#" AutoEventWireup="false" %>
<%@ Register TagPrefix="Control" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>Simple Mail Client</title>
<script language="C#" runat="server">
	protected override void OnInit(EventArgs e) {
		base.OnInit(e);
		WebButton btnSend = (WebButton)this.FindControl("btnSend");
		btnSend.Click += new EventHandler(btnSend_Click);
	}

	void btnSend_Click(object sender, EventArgs e) {
		TextBox txtServer	= (TextBox)this.FindControl("txtServer");
		TextBox txtFrom		= (TextBox)this.FindControl("txtFrom");
		TextBox txtTo		= (TextBox)this.FindControl("txtTo");
		TextBox txtSubject	= (TextBox)this.FindControl("txtSubject");
		TextBox txtBody		= (TextBox)this.FindControl("txtBody");
		
		nJupiter.Net.Mail.MailAddress from	= new nJupiter.Net.Mail.MailAddress(txtFrom.Text);
		nJupiter.Net.Mail.MailAddress to		= new nJupiter.Net.Mail.MailAddress(txtTo.Text);
		
		nJupiter.Net.Mail.Mail mail = new nJupiter.Net.Mail.Mail(to, from, txtBody.Text, txtSubject.Text);
		
		nJupiter.Net.Mail.SmtpClient smtpClient = new nJupiter.Net.Mail.SmtpClient(txtServer.Text);
		
		smtpClient.Send(mail);
	}
 </script>
</head>
<body>
	<form id="form1" runat="server">
		<p>
			<Control:WebLabel For="txtServer" runat="server">Server</Control:WebLabel><br />
			<asp:TextBox ID="txtServer" runat="server" Text="localhost" />
		</p>
		<p>
			<Control:WebLabel For="txtFrom" runat="server">From</Control:WebLabel><br />
			<asp:TextBox ID="txtFrom" runat="server" Text="m4@njupiter.org" />
		</p>
		<p>
			<Control:WebLabel For="txtTo" runat="server">To</Control:WebLabel><br />
			<asp:TextBox ID="txtTo" runat="server" Text="m4@njupiter.org" />
		</p>
		<p>
			<Control:WebLabel For="txtSubject" runat="server">Subject</Control:WebLabel><br />
			<asp:TextBox ID="txtSubject" runat="server" Text="Test Mail" />
		</p>
		<p>
			<Control:WebLabel For="txtBody" runat="server">Bodytext</Control:WebLabel><br />
			<asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Text="Lorem ipsum" />
		</p>
		<p>
			<Control:WebButton ID="btnSend" runat="server">Send</Control:WebButton>
		</p>
	</form>
</body>
</html>