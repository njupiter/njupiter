using System;
using System.IO;

namespace nJupiter.Configuration {
	public class FileConfigSource : UriConfigSource {
		public FileConfigSource(FileInfo file) : base(new Uri(file.FullName)) {
			this.configSources.Add(file);
		}
		public FileInfo ConfigFile { get { return this.GetConfigSource<FileInfo>(); } }
	}
}