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
using System.Runtime.Remoting;
using System.Globalization;
using System.ServiceProcess;
using System.Xml;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using log4net;
using log4net.Config;

using nJupiter.Configuration;

namespace nJupiter.Messaging.Server {

	internal sealed class Server : System.ServiceProcess.ServiceBase {

		#region Constants
		private const string MessageServicesSection = "messageServices";
		private const string MessageServiceSection = MessageServicesSection + "/messageService";
		private const string MessageServiceSectionFormat = MessageServiceSection + "[@value='{0}']";
		private const string SettingsSectionFormat = MessageServiceSectionFormat + "/settings";
		#endregion

		#region Static Members
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Instance Members
		private RemoteMessageService mRemoteMessageService;
		#endregion

		#region Service Methods
		static void Main() {
			ServiceBase[] servicesToRun = new System.ServiceProcess.ServiceBase[] { new Server() };
			System.ServiceProcess.ServiceBase.Run(servicesToRun);
		}

		//private void InitializeComponent() {this.ServiceName = "nJupiter Messaging Server";}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(this.mRemoteMessageService != null) {
						this.mRemoteMessageService.Dispose();
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnStart(string[] args) {
			try {
				//while (!System.Diagnostics.Debugger.IsAttached)
				//{
				//    System.Threading.Thread.Sleep(2000);
				//}
				IConfig config = ConfigHandler.GetConfig(Assembly.GetAssembly(typeof(log4net.Config.XmlConfigurator)));
				if(config.ConfigXml != null && config.ConfigXml.SelectSingleNode("log4net") != null)
					XmlConfigurator.Configure((XmlElement)config.ConfigXml.SelectSingleNode("log4net"));
				else
					XmlConfigurator.Configure(); // this loads the Log4Net config information from the app.config file 

				if(Log.IsInfoEnabled) { Log.Info("nJupiter Messaging Server - Starting server."); }

				BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
				provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

				IDictionary props = new Hashtable();
				props["port"] = ConfigHandler.GetConfig(Assembly.GetAssembly(typeof(Message))).GetConfigSection(string.Format(CultureInfo.InvariantCulture, SettingsSectionFormat, "server")).GetValue<int>("port");

				TcpChannel chan = new TcpChannel(props, null, provider);
				ChannelServices.RegisterChannel(chan, false);

				if(Log.IsInfoEnabled) { Log.Info("nJupiter Messaging Server - Tcp Channel has been registered."); }

				RemotingConfiguration.RegisterWellKnownServiceType(
					typeof(RemoteMessageService),
					"RemoteMessageService",
					WellKnownObjectMode.Singleton);

				if(Log.IsInfoEnabled) { Log.Info("Remoting type has been registred."); }

				this.mRemoteMessageService = new RemoteMessageService();
				RemotingServices.Marshal(this.mRemoteMessageService, "RemoteMessageService");
				if(Log.IsInfoEnabled) { Log.Info("Remote type has been marshalled."); }


			} catch(Exception ex) {
				if(Log.IsErrorEnabled) { Log.Error(string.Format(CultureInfo.InvariantCulture, SettingsSectionFormat, "server"), ex); }
				throw; //TODO: Do not catch general exceptions
			}
		}
		protected override void OnStop() {
			this.mRemoteMessageService.Dispose();
			RemotingServices.Disconnect(this.mRemoteMessageService);
			if(Log.IsInfoEnabled) { Log.Info("Server stopped."); }
		}
		#endregion
	}
}
