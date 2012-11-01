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

			watcher = new FileSystemWatcher();

			watcher.Path = configFile.DirectoryName;
			watcher.Filter = configFile.Name;

			watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

			watcher.Changed += WatchedConfigOnChanged;
			watcher.Created += WatchedConfigOnChanged;
			watcher.Deleted += WatchedConfigOnChanged;
			watcher.Renamed += WatchedConfigOnRenamed;

			watcher.EnableRaisingEvents = true;
		}

		private void WatchedConfigOnChanged(object source, FileSystemEventArgs e) {
			OnWatchedFileChange();
		}
		
		private void WatchedConfigOnRenamed(object source, RenamedEventArgs e) {
			OnWatchedFileChange();
		}

		private void OnWatchedFileChange() {
			if(eventTriggered)
				return;
			lock(padLock) {
				if(!eventTriggered) {
					eventTriggered = true;
					if(ConfigSourceUpdated != null) {
						ConfigSourceUpdated(this, EventArgs.Empty);
					}
					watcher.EnableRaisingEvents = false;
				}
			}
		}

		#region IDisposable Members
		private void Dispose(bool disposing) {
			if(!disposed) {
				disposed = true;

				if(watcher != null) {
					watcher.EnableRaisingEvents = false;
					watcher.Changed -= WatchedConfigOnChanged;
					watcher.Created -= WatchedConfigOnChanged;
					watcher.Deleted -= WatchedConfigOnChanged;
					watcher.Renamed -= WatchedConfigOnRenamed;
					watcher.Dispose();
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
