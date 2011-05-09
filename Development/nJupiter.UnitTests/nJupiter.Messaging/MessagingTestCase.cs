using System;

using NUnit.Framework;

namespace nJupiter.Messaging
{
	[Ignore] //Old tests depending on ws
	[TestFixture]
	public class MessagingTestCase {

		private static			MessageDestination	messageDestination;
		private static			MessageService		service;
		private static			MessageConsumer		messageConsumer;
		private static readonly Uri					uri					= new Uri("http://localhost/Messaging.asmx");
	
		[TestFixtureSetUp]
		public void Init() {
			service				= MessageService.GetInstance("WS");
			messageDestination	= service.CreateMessageDestination("News");
			messageConsumer		= service.CreateMessageConsumer(messageDestination, uri);

		}

		[TestFixtureTearDown]
		public void Dispose() {
		}
		
		[Test]
		public void RegisterTest() {
			service.Register(messageConsumer);
		}

		[Test]
		public void PublishTest() {
			service.Publish(service.CreateMessage(DateTime.Now.AddMinutes(1), DateTime.Now.AddDays(1), "New message.", messageDestination));
		}
	}

}
