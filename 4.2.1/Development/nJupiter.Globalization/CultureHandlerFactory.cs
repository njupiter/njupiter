namespace nJupiter.Globalization {
	public class CultureHandlerFactory {
		/// <summary>
		/// Returns the default instance of StringParser
		/// </summary>
		public static ICultureHandler Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly ICultureHandler instance = new ConfigurableCultureHandler();
		}
	}
}
