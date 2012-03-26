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

using System.ComponentModel;

namespace nJupiter.Messaging.Server {

	[RunInstaller(true)]
	public class ServerInstaller : System.Configuration.Install.Installer {
		private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller serviceInstaller;

		public ServerInstaller() {
			InitializeComponent();
		}

		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(this.serviceProcessInstaller != null) {
						this.serviceProcessInstaller.Dispose();
					}
					if(this.serviceInstaller != null) {
						this.serviceInstaller.Dispose();
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
			this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
			this.serviceProcessInstaller.Password = null;
			this.serviceProcessInstaller.Username = null;
			this.serviceInstaller.ServiceName = "nJupiter Messaging Server";
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
																					  this.serviceProcessInstaller,
																					  this.serviceInstaller});

		}
		#endregion
	}
}
