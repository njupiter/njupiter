namespace nJupiter.Web {
	public interface IResponseHandler {
		void Redirect(string url);
		void Redirect(string url, bool permanently);
		void Redirect(string url, bool permanently, bool endResponse);
		void PerformXhtmlContentNegotiation();
	}
}