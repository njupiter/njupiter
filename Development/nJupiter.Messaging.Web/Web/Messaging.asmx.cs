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

using System.Reflection;
using System.Web.Services;

// Messaging
using nJupiter.Configuration;

namespace nJupiter.Messaging.Web {

	[WebService(Namespace="urn:njupiter:messaging:web")]
	public class Messaging	{
		
		private const string WebServicesSection	= "webService";
		private const string ConsumerSection	= WebServicesSection + "/messageConsumer";
		private const string ServerConfig		= "server";
	
		[WebMethod]
		public void Publish(Message message) {
			MessageService.GetInstance(ServerConfig).Publish(message);
		}

		[WebMethod]
		public void Register(MessageConsumer messageConsumer) {
			MessageService.GetInstance(ServerConfig).Register(messageConsumer);
		}

		[WebMethod]
		public void Notify(Message message) {
            string assemblyName	= ConfigHandler.GetConfig(Assembly.GetAssembly(typeof(Message))).GetValue(ConsumerSection, "assembly");
            string className	= ConfigHandler.GetConfig(Assembly.GetAssembly(typeof(Message))).GetValue(ConsumerSection, "className");
			Assembly assembly = System.Reflection.Assembly.Load(assemblyName);
			if(assembly != null && assembly.Location != null){
				Assembly managerAssembly = Assembly.LoadFrom(assembly.Location);
				MessageConsumer msgCons	= (MessageConsumer)managerAssembly.CreateInstance(className);
				msgCons.Notify(message);
			}
		}

	}

}
