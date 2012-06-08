namespace nJupiter.Abstraction.Logging {
	public interface ILog<out T> : ILog {
		ILog BaseLog { get; }
	}
}