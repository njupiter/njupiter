using System.Web.UI;

namespace nJupiter.Web.UI {
	public interface IControlLoader {
		/// <summary>
		/// Loads a control that is not catchable just by its virtual path, but by a custom string.
		/// </summary>
		/// <param name="templateControl">The template control that shall load the control.</param>
		/// <param name="virtualPath">The virtual path for the control.</param>
		/// <param name="varyByCustom">The custom string that vary between different versions of the cached control.</param>
		/// <returns>The loaded control.</returns>
		Control LoadControl(TemplateControl templateControl, string virtualPath, string varyByCustom);
	}
}