namespace nJupiter.Configuration {
	public interface IConfigSource {
		T GetConfigSource<T>();
	}
}