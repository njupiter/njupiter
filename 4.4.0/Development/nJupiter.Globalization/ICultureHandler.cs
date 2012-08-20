using System.Globalization;

namespace nJupiter.Globalization {
	public interface ICultureHandler {
		CultureInfo CurrentCulture { get; }
		CultureInfo CurrentUICulture { get; }
		CultureInfo GetCultureInfo(int culture);
		CultureInfo GetCultureInfo(string name);
	}
}