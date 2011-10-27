using System.Text.RegularExpressions;

namespace nJupiter.Text.SimpleTextParser {
	internal class RegexFormatter : IFormatter{
		private readonly Rule rule;
		private readonly Regex regex;

		public RegexFormatter(Rule rule) {
			this.rule = rule;			
			
			RegexOptions options = RegexOptions.Compiled;

			if(!this.rule.CaseSensitive) {
				options |= RegexOptions.IgnoreCase;
			}
			
			this.regex = new Regex(this.rule.Pattern, options);
		}

		string IFormatter.Format(string text) {
			return this.regex.Replace(text, this.rule.Replacement);
		}

	}
}
