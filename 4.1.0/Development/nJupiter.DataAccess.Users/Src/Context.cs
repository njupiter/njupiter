using System;

namespace nJupiter.DataAccess.Users {
	[Serializable]
	public class Context : IContext {

		public static readonly IContext DefaultContext = new Context();

		private readonly string name;

		private Context() {
			this.name = string.Empty;
		}

		public Context(string contextName) {
			if(string.IsNullOrEmpty(contextName)) {
				throw new ArgumentException("contextName can not be empty.", "contextName");
			}
			this.name = contextName;
		}

		public string Name { get { return this.name; } }

		public override int GetHashCode() {
			return this.Name.ToLowerInvariant().GetHashCode();
		}

		public override bool Equals(object obj) {
			var context = obj as IContext;
			if(context == null) {
				return false;
			}
			return string.Equals(this.Name, context.Name, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}