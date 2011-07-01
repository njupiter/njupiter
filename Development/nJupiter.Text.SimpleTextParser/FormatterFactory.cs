using nJupiter.Configuration;

namespace nJupiter.Text.SimpleTextParser {
	public class FormatterFactory {

		public static IFormatter GetFormatter() {
			return GetFormatter(string.Empty);
		}

		public static IFormatter GetFormatter(string name) {
			var configuration =  Configuration.GetConfig(name);
			return configuration.Formatter;
		}

		internal static IFormatter GetFormatter(IConfig config) {
			var formatters = new CompositeFormatter();
			string[] patterns = config.GetAttributeArray(".", "rule", "pattern");
			foreach(string pattern in patterns) {
				string replacement = config.GetAttribute(".", string.Format("rule[@pattern='{0}']", pattern), "replacement");
				bool caseSensitive = false;

				if(config.ContainsAttribute(".", string.Format("rule[@pattern='{0}']", pattern), "caseSensitive")) {
					caseSensitive = config.GetAttribute<bool>(".", string.Format("rule[@pattern='{0}']", pattern), "caseSensitive");
				}

				var rule = new Rule(pattern, replacement, caseSensitive);
				var ruleFormatter = new RegexFormatter(rule);

				formatters.Add(ruleFormatter);
			}
			return formatters;
		}
	}
}
