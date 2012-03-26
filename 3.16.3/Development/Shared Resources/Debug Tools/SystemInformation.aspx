<%@ Page Language="C#" AutoEventWireup="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "xhtml11.dtd">
<html>
<head>
	<title>System Information</title>
	<script language="C#" runat="server">
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			Label serverName = (Label)FindControl("serverName");
			serverName.Text = HttpContext.Current.Server.MachineName;

			Label localIP = (Label)FindControl("localIP");
			localIP.Text = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];

			Label remoteIP = (Label)FindControl("remoteIP");
			remoteIP.Text = HttpContext.Current.Request.UserHostAddress;
			
			Label remotePort = (Label)FindControl("remotePort");
			remotePort.Text = HttpContext.Current.Request.ServerVariables["REMOTE_PORT"];
						
			Label host = (Label)FindControl("host");
			host.Text = HttpContext.Current.Request.Url.Host;

			Label port = (Label)FindControl("port");
			port.Text = HttpContext.Current.Request.Url.Port.ToString();
						
			Label urlScheme = (Label)FindControl("urlScheme");
			urlScheme.Text = HttpContext.Current.Request.Url.Scheme;

			Label serverVariables = (Label)FindControl("serverVariables");
			if(serverVariables != null) {
				String[] array = HttpContext.Current.Request.ServerVariables.AllKeys;
				StringBuilder stringBuilder = new StringBuilder();
				for(int i = 0; i < array.Length; i++) {
					stringBuilder.Append("Key: " + array[i] + "<br />");
					String[] arr2 = HttpContext.Current.Request.ServerVariables.GetValues(array[i]);
					for(int j = 0; j < arr2.Length; j++) {
						stringBuilder.Append("Value " + j + ": " + Server.HtmlEncode(arr2[j]) + "<br />");
					}
				}
				serverVariables.Text = stringBuilder.ToString();
			}
			
			
			Label httpHeaders = (Label)FindControl("httpHeaders");
			if(httpHeaders != null) {
				String[] array = HttpContext.Current.Request.Headers.AllKeys;
				StringBuilder stringBuilder = new StringBuilder();
				for(int i = 0; i < array.Length; i++) {
					stringBuilder.Append("Key: " + array[i] + "<br />");
					String[] arr2 = HttpContext.Current.Request.Headers.GetValues(array[i]);
					for(int j = 0; j < arr2.Length; j++) {
						stringBuilder.Append("Value " + j + ": " + Server.HtmlEncode(arr2[j]) + "<br />");
					}
				}
				httpHeaders.Text = stringBuilder.ToString();
			}

		}
	</script>
</head>
<body>
	<form id="Form1" method="post" runat="server">
		<strong>Server Name:</strong>
		<asp:Label runat="server" ID="serverName" /><br />
		<strong>Local IP:</strong>
		<asp:Label runat="server" ID="localIP" /><br />
		<strong>Remote IP:</strong>
		<asp:Label runat="server" ID="remoteIP" /><br />
		<strong>Remote Port:</strong>
		<asp:Label runat="server" ID="remotePort" /><br />
		<strong>Host:</strong>
		<asp:Label runat="server" ID="host" /><br />
		<strong>Port:</strong>
		<asp:Label runat="server" ID="port" /><br />
		<strong>Url Scheme:</strong>
		<asp:Label runat="server" ID="urlScheme" /><br />
		<%--
		<strong>Server Variables:</strong><br />
		<asp:Label runat="server" ID="serverVariables" /><br />
		--%>
		<%--
		<strong>HTTP Headers:</strong><br />
		<asp:Label runat="server" ID="httpHeaders" /><br />
		--%>
	</form>
</body>
</html>
