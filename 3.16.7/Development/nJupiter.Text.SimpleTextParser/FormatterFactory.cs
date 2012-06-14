using nJupiter.Configuration;

namespace nJupiter.Text.SimpleTextParser {
	public class FormatterFactory {

		public static IFormatter GetFormatter() {
			return GetFormatter(string.Empty);
		}

		public static IFormatter GetFormatter(string name) {
			Configuration configuration =  Configuration.GetConfig(name);
			return configuration.Formatter;
		}

		internal static IFormatter GetFormatter(Config config) {
			CompositeFormatter formatters = new CompositeFormatter();
			string[] patterns = config.GetAttributeArray(".", "rule", "pattern");
			foreach(string pattern in patterns) {
				string replacement = config.GetAttribute(".", string.Format("rule[@pattern='{0}']", pattern), "replacement");
				bool caseSensitive = false;

				if(config.ContainsAttribute(".", string.Format("rule[@pattern='{0}']", pattern), "caseSensitive")) {
					caseSensitive = config.GetBoolAttribute(".", string.Format("rule[@pattern='{0}']", pattern), "caseSensitive");
				}

				Rule rule = new Rule(pattern, replacement, caseSensitive);
				IFormatter ruleFormatter = new RegexFormatter(rule);

				formatters.Add(ruleFormatter);
			}
			return formatters;
		}
	}
}
