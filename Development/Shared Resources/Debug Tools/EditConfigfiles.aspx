<%@ Page Language="C#" AutoEventWireup="false" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.Text" %>
<%@ Import namespace="System.Xml" %>
<%@ Import namespace="System.Collections.Generic" %>
<%@ Import namespace="System.Security.Cryptography" %>
<%@ Import namespace="nJupiter.Configuration" %>
<%@ Import namespace="nJupiter.Web" %>
<%@ Register TagPrefix="Control" Namespace="nJupiter.Web.UI.Controls" Assembly="nJupiter.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "xhtml11.dtd">
<html>
<head>
	<Control:WebGenericControl TagName="title" id="hgcTitle" runat="server">Config</Control:WebGenericControl>
	<script language="C#" runat="server">
		#region Constnants
		private const string EDITCONFIG_QUERY_KEY = "editConfig";
		#endregion

		#region Event Handlers
		override protected void OnInit(EventArgs e) {
			base.OnInit(e);
			string configKey = this.Request[EDITCONFIG_QUERY_KEY];
			if(configKey != null) {
				this.PopulateFieldset(configKey);
				rptConfigList.Visible = false;
				flsFieldset.Visible = true;
			} else {
				this.PopulateFileList();
				rptConfigList.Visible = true;
				flsFieldset.Visible = false;
			}
		}

		private void rptConfigList_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			if(e == null) throw new ArgumentNullException("e");

			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
				FileInfo file = (FileInfo)e.Item.DataItem;

				// Find Ui controls
				WebPlaceHolder	phlFileInfo = (WebPlaceHolder)e.Item.FindControl("phlFileInfo");
				WebAnchor		ancFileLink = (WebAnchor)e.Item.FindControl("ancFileLink");

				phlFileInfo.InnerText	= file.LastWriteTime.ToShortDateString() + "\t\t" + file.LastWriteTime.ToShortTimeString() + "\t\t" + file.Length + "\t\t";
				ancFileLink.Text		= file.Name;
				ancFileLink.ID			= e.Item.ItemIndex.ToString();
				ancFileLink.NavigateUrl	= "?" + EDITCONFIG_QUERY_KEY + "=" + HttpUtility.UrlEncode(MD5Hash(file.FullName));
			}
		}
		#endregion

		#region Helper Methods
		private void PopulateFileList() {
			hgcTitle.InnerText = ltrHeading.Text = Request.Url.Host + " " + Request.Path.Substring(0, Request.Path.LastIndexOf("/") + 1);

			rptConfigList.ItemDataBound +=this.rptConfigList_ItemDataBound;
			
			List<FileInfo> files = this.GetConfigFiles();
			
			if(files.Count == 0) {
				phlErrorMessage.InnerText = "No configuration files found";
			} else {
				Comparison<FileInfo> c = (x, y) => x.FullName.CompareTo(y.FullName);
				files.Sort(c);
				rptConfigList.DataSource = files;
				rptConfigList.DataBind();
			}
		}

		private void PopulateFieldset(string configKey) {
			FileInfo file = this.GetConfigFile(configKey);
			btnCancel.Click += this.btnCancel_Click;
			if(file == null) {
				phlErrorMessage.InnerText = "File not found";
				prgTextBox.Visible = btnSave.Visible = false;
			} else {
				hgcTitle.InnerText = ltrHeading.Text = "Edit " + file.Name;
				btnSave.Click += this.btnSave_Click;
				prgTextBox.Visible = btnSave.Visible = true;
				using(StreamReader sr = file.OpenText()) {
					txbEditConfig.Text = sr.ReadToEnd();
				}				
			}
		}

		void btnCancel_Click(object sender, EventArgs e) {
			this.Back();
		}

		void btnSave_Click(object sender, EventArgs e) {
			string configKey = this.Request[EDITCONFIG_QUERY_KEY];
			FileInfo file = this.GetConfigFile(configKey);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(txbEditConfig.Text);
			
			using(StreamWriter sw = file.CreateText()){
				sw.Write(txbEditConfig.Text);
			}
			this.Back();
		}

		private void Back() {
			this.Response.Redirect(UrlHandler.RemoveQueryKey(this.Request.Url.AbsolutePath, EDITCONFIG_QUERY_KEY));
		}
		
		private List<FileInfo> GetConfigFiles() {
			List<FileInfo> files = new List<FileInfo>();
			foreach(Config config in ConfigHandler.Configurations){
				if(config.ConfigFile != null && !files.Contains(config.ConfigFile)) {
					files.Add(config.ConfigFile);
				}
			}
			string path = this.Request.MapPath("/Web.config");
			if(File.Exists(path)) {
				FileInfo file = new FileInfo(path);
				files.Add(file);
			}
			return files;		
		}

		private FileInfo GetConfigFile(string configKey) {
			foreach(FileInfo file in this.GetConfigFiles()){
				if(configKey.Equals(MD5Hash(file.FullName))){
					return file;
				}
			}
			return null;
		}
		
		private static string MD5Hash(string text) {
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hashArray = md5.ComputeHash(UnicodeEncoding.Unicode.GetBytes(text));
			return BitConverter.ToString(hashArray);
		}
		#endregion
	</script>
</head>
<body>
	<Control:WebForm id="Form1" runat="server">
		<h1><asp:Literal ID="ltrHeading" Runat="server" /></h1>
		<hr />
		<div>
		<a href="../">[To Parent Directory]</a>
		<br /><br /><Control:WebPlaceHolder ID="phlErrorMessage" Runat="server" />
		</div>
		<asp:Repeater ID="rptConfigList" Runat="server">			
<HeaderTemplate><pre></HeaderTemplate>			
<ItemTemplate><Control:WebPlaceHolder ID="phlFileInfo" Runat="server" /><Control:WebAnchor ID="ancFileLink" Runat="server" />
</ItemTemplate><FooterTemplate></pre></FooterTemplate>			
		</asp:Repeater>
		<Control:WebGenericControl TagName="fieldset" ID="flsFieldset" runat="server">
			<Control:WebParagraph ID="prgTextBox" runat="server">
				<Control:WebLabel For="txbEditConfig" runat="server">Edit Config</Control:WebLabel>
				<asp:TextBox Wrap="false" ID="txbEditConfig" TextMode="MultiLine" rows="30" style="width:100%" runat="server" />
			</Control:WebParagraph>
			<p>
				<Control:WebButton ID="btnSave" runat="server">Save</Control:WebButton>
				<Control:WebButton ID="btnCancel" runat="server">Cancel</Control:WebButton>
			</p>
		</Control:WebGenericControl>
		<hr />
	</Control:WebForm>
</body>
</html>