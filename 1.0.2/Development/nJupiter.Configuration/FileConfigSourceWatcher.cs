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

namespace nJupiter.Configuration {
	internal sealed class FileConfigSourceWatcher : IConfigSourceWatcher, IDisposable {

		private readonly FileSystemWatcher watcher;
		private readonly object padLock = new object();
		private bool eventTriggered;
		private bool disposed;

		public event EventHandler ConfigSourceUpdated;

		public FileConfigSourceWatcher(FileInfo configFile) {

			this.watcher = new FileSystemWatcher();

			this.watcher.Path = configFile.DirectoryName;
			this.watcher.Filter = configFile.Name;

			this.watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

			this.watcher.Changed += this.WatchedConfigOnChanged;
			this.watcher.Created += this.WatchedConfigOnChanged;
			this.watcher.Deleted += this.WatchedConfigOnChanged;
			this.watcher.Renamed += this.WatchedConfigOnRenamed;

			this.watcher.EnableRaisingEvents = true;
		}

		private void WatchedConfigOnChanged(object source, FileSystemEventArgs e) {
			this.OnWatchedFileChange();
		}
		
		private void WatchedConfigOnRenamed(object source, RenamedEventArgs e) {
			this.OnWatchedFileChange();
		}

		private void OnWatchedFileChange() {
			if(this.eventTriggered)
				return;
			lock(this.padLock) {
				if(!this.eventTriggered) {
					this.eventTriggered = true;
					if(this.ConfigSourceUpdated != null) {
						this.ConfigSourceUpdated(this, EventArgs.Empty);
					}
					this.watcher.EnableRaisingEvents = false;
				}
			}
		}

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

		public void Dispose() {
			Dispose(true);
		}

		// Disposable types implement a finalizer.
		~FileConfigSourceWatcher() {
			Dispose(false);
		}
		#endregion
	}
}
