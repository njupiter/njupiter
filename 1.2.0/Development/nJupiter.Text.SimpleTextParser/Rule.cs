namespace nJupiter.Text.SimpleTextParser {
	internal class Rule {
		private readonly string pattern;
		private readonly string replacement;
		private readonly bool caseSensitive;

		public bool CaseSensitive { get { return this.caseSensitive; } }
		public string Replacement { get { return this.replacement; } }
		public string Pattern { get { return this.pattern; } }

		public Rule(string pattern, string replacement) : this(pattern, replacement, true) {}

		public Rule(string pattern, string replacement, bool caseSensitive) {
			this.pattern = pattern;
			this.replacement = replacement;
			this.caseSensitive = caseSensitive;
		}

	}
}
