<%@ Page Language="C#" AutoEventWireup="false" %>
<%@ Import namespace="System.IO" %>
<%@ Import namespace="System.CodeDom.Compiler" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "xhtml11.dtd">
<html>
<head>
	<title id="hgcTitle" runat="server">Logs</title>
	<script language="C#" runat="server">
		#region Members
		private		ArrayList	m_FileCollection = new ArrayList();
		#endregion

		#region Event Handlers
		override protected void OnInit(EventArgs e) {
			rptFileList.ItemDataBound +=new RepeaterItemEventHandler(rptFileList_ItemDataBound);
			base.OnInit(e);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			hgcTitle.InnerText = ltrHeading.Text = Request.Url.Host + " " + Request.Path.Substring(0, Request.Path.LastIndexOf("/") + 1);
			ArrayList dirs = new ArrayList();

			foreach(log4net.ILog logger in log4net.LogManager.GetCurrentLoggers()){
				foreach(log4net.Appender.IAppender appender in logger.Logger.Repository.GetAppenders()){
					log4net.Appender.FileAppender fileAppender = appender as log4net.Appender.FileAppender;
					if(fileAppender != null) {
						DirectoryInfo logDir = (new FileInfo(fileAppender.File)).Directory;
						if(!dirs.Contains(logDir.FullName)){
							dirs.Add(logDir.FullName);
							foreach(FileInfo fi in logDir.GetFiles()){
								m_FileCollection.Add(fi);
							}
						}
					}
				}
			}
			
			if(m_FileCollection.Count == 0) {
				ltrErrorMessage.Text = "No logging path specified";
			} else {
				rptFileList.DataSource = m_FileCollection;
				rptFileList.DataBind();
			}
		}

		private void rptFileList_ItemDataBound(object sender, RepeaterItemEventArgs e) {
			if(e == null) throw new ArgumentNullException("e");

			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) {
				FileInfo file = (FileInfo)e.Item.DataItem;

				// Find Ui controls
				LinkButton	lnkFileLink = (LinkButton)e.Item.FindControl("lnkFileLink");
				Literal		ltrFileInfo = (Literal)e.Item.FindControl("ltrFileInfo");

				ltrFileInfo.Text	= file.LastWriteTime.ToShortDateString() + "\t\t" + file.LastWriteTime.ToShortTimeString() + "\t\t" + file.Length + "\t\t";
				lnkFileLink.Text	= file.Name;
				lnkFileLink.ID		= e.Item.ItemIndex.ToString();

				lnkFileLink.Click +=new EventHandler(lnkFileLink_Click);
			}
		}

		private void lnkFileLink_Click(object sender, EventArgs e) {
			LinkButton lnkFileLink = sender as LinkButton;
			if(lnkFileLink != null) {
				FileInfo file = (FileInfo)m_FileCollection[int.Parse(lnkFileLink.ID)];
				
				TempFileCollection tempFiles = new TempFileCollection();

				FileInfo newFile = file.CopyTo(tempFiles.BasePath);
				tempFiles.AddFile(newFile.Name, false);
				using(StreamReader sr = newFile.OpenText()) {
					StreamText(sr.ReadToEnd(), file.Name);
				}
				tempFiles.Delete();
			}
			else {
				Response.Clear();
				Response.Status = "403 OK"; // Access forbidden
				Response.End();
			}
		}
	
		#endregion

		#region Helper Methods
		private void StreamText(string text, string fileName) {
			Response.Clear();

			Response.ContentType = "text/plain";
			Response.AddHeader("Content-Disposition", "inline;filename="+ fileName);

			byte[] byteArray = UTF8Encoding.UTF8.GetBytes(text);

			if(byteArray.Length > 0) {
				Response.Buffer = false;
				Response.BinaryWrite(byteArray);
			}

			Response.End();
		}
		#endregion
	</script>
</head>
<body>
	<form id="Form1" method="post" runat="server">
		<h1><asp:Literal ID="ltrHeading" Runat="server" /></h1>
		<hr />
		<a href="../">[To Parent Directory]</a>
		<br /><br /><asp:Literal ID="ltrErrorMessage" Runat="server" />
		<asp:Repeater ID="rptFileList" Runat="server">			
<HeaderTemplate><pre></HeaderTemplate>			
<ItemTemplate><asp:Literal ID="ltrFileInfo" Runat="server" /><asp:LinkButton ID="lnkFileLink" Runat="server" />
</ItemTemplate><FooterTemplate></pre></FooterTemplate>			
		</asp:Repeater>			
		<hr />
	</form>
</body>
</html>