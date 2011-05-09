#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/
#endregion

using System;
using System.IO;

using log4net;

namespace nJupiter.Configuration {

	internal sealed class WatchedConfigHandler : IDisposable {

		#region Members
		private readonly FileInfo configFile;
		private readonly string configKey;
		private readonly FileSystemWatcher watcher;
		private readonly object padLock = new object();
		private bool eventTriggered;
		private bool disposed;
		#endregion

		#region Static Members
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Constructors
		private WatchedConfigHandler(string configKey, FileInfo configFile) {
			this.configKey = configKey;
			this.configFile = configFile;

			// Create a new FileSystemWatcher and set its properties.
			this.watcher = new FileSystemWatcher();

			this.watcher.Path = this.configFile.DirectoryName;
			this.watcher.Filter = this.configFile.Name;

			// Set the notification filters
			this.watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

			// Add event handlers. OnChanged will do for all event handlers that fire a FileSystemEventArgs
			this.watcher.Changed += this.WatchedConfigHandlerOnChanged;
			this.watcher.Created += this.WatchedConfigHandlerOnChanged;
			this.watcher.Deleted += this.WatchedConfigHandlerOnChanged;
			this.watcher.Renamed += this.WatchedConfigHandlerOnRenamed;

			// Begin watching.
			this.watcher.EnableRaisingEvents = true;
		}
		#endregion

		#region Event Handlers
		private void WatchedConfigHandlerOnChanged(object source, FileSystemEventArgs e) {
			this.OnWatchedFileChange();
		}
		private void WatchedConfigHandlerOnRenamed(object source, RenamedEventArgs e) {
			this.OnWatchedFileChange();
		}
		#endregion

		#region Methods
		public static WatchedConfigHandler StartWatching(string configKey, FileInfo configFile) {
			return new WatchedConfigHandler(configKey, configFile);
		}
		#endregion

		#region Event Activators
		private void OnWatchedFileChange() {
			if(this.eventTriggered)
				return;
			lock(this.padLock) {
				if(this.eventTriggered)
					return;
				try {
					Configurator.Configure(this.configKey, this.configFile);
					this.eventTriggered = true;
				} catch(ConfigurationException ex) {
					// Catch error and log to avoid crashing the application
					if(Log.IsFatalEnabled) { Log.Fatal(string.Format("Error while configure configfile with key {0}", this.configKey), ex); }
				}
			}
		}
		#endregion

		#region IDisposable Members
		private void Dispose(bool disposing) {
			if(!this.disposed) {
				this.disposed = true;

				if(this.watcher != null) {
					this.watcher.EnableRaisingEvents = false;
					this.watcher.Dispose();
				}

				// Suppress finalization of this disposed instance.
				if(disposing) {
					GC.SuppressFinalize(this);
				}
			}
		}

		public void Disposing(object sender, EventArgs e) {
			this.Dispose();
		}

		public void Dispose() {
			Dispose(true);
		}

		// Disposable types implement a finalizer.
		~WatchedConfigHandler() {
			Dispose(false);
		}
		#endregion
	}
}
