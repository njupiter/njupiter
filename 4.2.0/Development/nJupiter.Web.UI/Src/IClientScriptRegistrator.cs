using System;

namespace nJupiter.Web.UI {
	public interface IClientScriptRegistrator {
		void RegisterClientScriptBlock(Type type, string key, string script, RegisterTargetPreference targetPreference);
		void RegisterClientScriptBlock(Type type, string key, string script);
		void RegisterClientScriptInclude(Type type, string key, string url, RegisterTargetPreference targetPreference);
		void RegisterClientScriptInclude(Type type, string key, string url);
		void RegisterClientScriptResource(Type type, string resourceName, RegisterTargetPreference targetPreference);
		void RegisterClientScriptResource(Type type, string resourceName);
	}
}