using System;

namespace nJupiter.Configuration {
	public class UriConfigSource : ConfigSource {
		public UriConfigSource(Uri uri) : base(uri) {}
		public Uri ConfigUrl { get { return this.GetConfigSource<Uri>(); } }
	}
}