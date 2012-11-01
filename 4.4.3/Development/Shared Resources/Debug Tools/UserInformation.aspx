<%@ Page Language="C#" AutoEventWireup="false" %>
<%@ Import Namespace="System.Security.Principal"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "xhtml11.dtd">
<html>
<head>
	<title>Test page for information about the current user</title>
	<script language="C#" runat="server">
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			Label authenticationType = (Label)FindControl("authenticationType");
			Label principalType = (Label)FindControl("principalType");
			Label roleInfo = (Label)FindControl("roleInfo");

			authenticationType.Text = this.Page.User.Identity.AuthenticationType;
			IPrincipal principal = this.Page.User;
			principalType.Text = principal == null ? "No User found" : principal.GetType().Name;
			PopulateRolesList();
			if(Context.User != null) {
				roleInfo.Text = Context.User.Identity.Name;
			}

		}

		private void PopulateRolesList() {
			if(this.User == null)
				return;
			Repeater listRoles = (Repeater)FindControl("listRoles");
			string[] roleList = Roles.GetRolesForUser(this.User.Identity.Name);
			listRoles.DataSource = roleList;
			listRoles.DataBind();
		}
	</script>
</head>
<body>
	<form id="Form1" method="post" runat="server">
		Your authenticationtype is:
		<asp:Label runat="server" ID="authenticationType" /><br />
		The current principal type is:
		<asp:Label runat="server" ID="principalType" /><br />
		<asp:Label ID="roleInfo" runat="server" />
		:
		<br />
		<ul>
			<asp:Repeater ID="listRoles" runat="server">
				<ItemTemplate>
					<li>
						<%# Container.DataItem %>
					</li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>
	</form>
</body>
</html>
