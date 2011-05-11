namespace nJupiter.Configuration {
	internal interface IConfigLoader {
		ConfigCollection LoadAll();
		IConfig Load(string configKey);
	}
}