using System.Web;

namespace nJupiter.Web {
	public interface IHttpContextHandler {
		HttpContextBase Current { get; }
	}
}