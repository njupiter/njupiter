using System;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Services.Description;
using System.ComponentModel;

namespace nJupiter.Messaging.Adapters.WebService {

	/// <summary>
	/// Proxy class used for loading Web Services dynamically.
	/// This proxy expect that the web service has one HandleEvents method and one RequestedEvents method
	/// </summary>
	[DebuggerStepThroughAttribute]
	[DesignerCategoryAttribute("code")]
	[WebServiceBindingAttribute(Name = "WebServiceProxySoap", Namespace = "urn:njupiter:messaging:web")]
	public class WebServiceProxy : SoapHttpClientProtocol {

		public WebServiceProxy(Uri url) {
			if(url == null)
				throw new ArgumentNullException("url");
			this.Url = url.AbsoluteUri;
		}

		[SoapDocumentMethodAttribute("urn:njupiter:messaging:web/Notify", RequestNamespace = "urn:njupiter:messaging:web", ResponseNamespace = "urn:njupiter:messaging:web", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Message Notify(Message message) {
			object[] results = this.Invoke("Notify", new object[] { message });
			return ((Message)(results[0]));
		}

		public IAsyncResult BeginNotify(Message message, AsyncCallback callback, object asyncState) {
			return this.BeginInvoke("Notify", new object[] { message }, callback, asyncState);
		}

		public Message EndNotify(IAsyncResult asyncResult) {
			object[] results = this.EndInvoke(asyncResult);
			return ((Message)(results[0]));
		}

		[SoapDocumentMethodAttribute("urn:njupiter:messaging:web/Register", RequestNamespace = "urn:njupiter:messaging:web", ResponseNamespace = "urn:njupiter:messaging:web", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public MessageConsumer Register(MessageConsumer messageConsumer) {
			object[] results = this.Invoke("Register", new object[] { messageConsumer });
			return ((MessageConsumer)(results[0]));
		}

		public IAsyncResult BeginRegister(MessageConsumer messageConsumer, AsyncCallback callback, object asyncState) {
			return this.BeginInvoke("Register", new object[] { messageConsumer }, callback, asyncState);
		}

		public MessageConsumer EndRegister(IAsyncResult asyncResult) {
			object[] results = this.EndInvoke(asyncResult);
			return ((MessageConsumer)(results[0]));
		}

		[SoapDocumentMethodAttribute("urn:njupiter:messaging:web/Publish", RequestNamespace = "urn:njupiter:messaging:web", ResponseNamespace = "urn:njupiter:messaging:web", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Message Publish(Message message) {
			object[] results = this.Invoke("Publish", new object[] { message });
			return ((Message)(results[0]));
		}

		public IAsyncResult BeginPublish(Message message, AsyncCallback callback, object asyncState) {
			return this.BeginInvoke("Publish", new object[] { message }, callback, asyncState);
		}

		public Message EndPublish(IAsyncResult asyncResult) {
			object[] results = this.EndInvoke(asyncResult);
			return ((Message)(results[0]));
		}

	}
}
