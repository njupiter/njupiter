using System.Collections.Generic;

namespace nJupiter.Text.SimpleTextParser {
	internal class CompositeFormatter : List<IFormatter>, IFormatter {
		public string Format(string text) {
			foreach(IFormatter formatter in this) {
				text = formatter.Format(text);
			}
			return text;
		}	
	}
}
