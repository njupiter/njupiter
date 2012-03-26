#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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
using System.Collections.Generic;
using System.Timers;

using Microsoft.Build.BuildEngine;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using System.Runtime.InteropServices;

namespace nJupiter.MSBuild.Tasks.MSBuildState {

	public class CheckIfInitialized : Task {
		[Output]
		public ITaskItem IsInitialized {
			get { return new TaskItem(StateMachine.Instance.IsInitialized.ToString()); }
		}
		public override bool Execute() {
			//#if DEBUG
			//if(!StateMachine.Instance.IsInitialized)
			//	System.Diagnostics.Debugger.Break();
			//#endif
			return true;
		}
	}

	public class InitializeState : Task {
		private ITaskItem[] m_keys;
		[Required]
		public ITaskItem[] Keys {
			get { return m_keys; }
			set { m_keys = value; }
		}
		private ITaskItem[] m_values;
		[Required]
		public ITaskItem[] Values {
			get { return m_values; }
			set { m_values = value; }
		}

		private ITaskItem m_OnlyIfNotInitialized;

		public ITaskItem OnlyIfNotInitialized {
			get { return m_OnlyIfNotInitialized; }
			set { m_OnlyIfNotInitialized = value; }
		}

		private bool runOnlyIfNotInitialized {
			get {
				if(m_OnlyIfNotInitialized != null) {
					return Boolean.Parse(m_OnlyIfNotInitialized.ToString());
				} else
					return true;

			}
		}

		public override bool Execute() {
			if(runOnlyIfNotInitialized &&
				StateMachine.Instance.IsInitialized) {
				return true;
			} else {
				StateMachine.Instance.SetState(Keys, Values);
				StateMachine.Instance.IsInitialized = true;
				return true;
			}
		}
	}

	public class SetState : Task {
		private ITaskItem[] m_keys;
		[Required]
		public ITaskItem[] Keys {
			get { return m_keys; }
			set { m_keys = value; }
		}
		private ITaskItem[] m_values;
		[Required]
		public ITaskItem[] Values {
			get { return m_values; }
			set { m_values = value; }
		}

		public override bool Execute() {
			StateMachine.Instance.SetState(Keys, Values);
			return true;
		}
	}

	public class GetStateItems : Task {
		[Output]
		public ITaskItem[] Keys {
			get { return StateMachine.Instance.Keys; }
		}
		[Output]
		public ITaskItem[] Values {
			get { return StateMachine.Instance.Values; }
		}

		public override bool Execute() { return true; }
	}

	public class GetPropertyFromState : Task {
		[Output]
		public ITaskItem Value {
			get { return StateMachine.Instance.GetValue(m_propertyName); }
		}
		private ITaskItem m_propertyName;

		[Required]
		public ITaskItem PropertyName {
			set { m_propertyName = value; }
		}

		public override bool Execute() { return true; }
	}

	public class PrintState : Task {
		public override bool Execute() {
			StateMachine.Instance.DumpStateViaConsoleWrite();
			return true;
		}
	}

	public class AddTargetsToExecuteAtEnd : Task {
		private string m_targets;
		private string m_project;
		[Required]
		public ITaskItem Targets {
			set {
				m_targets = value.ToString();
			}
		}
		public ITaskItem ProjectFileName {
			set {
				m_project = value.ToString();
			}
		}
		public override bool Execute() {
			if(m_project == null || m_project.Length < 1) {
				m_project = GetCurrentProjectName();
			}
			StateMachine.Instance.AddTargets(m_project, m_targets);
			return true;
		}

		private string GetCurrentProjectName() {
			return BuildEngine.ProjectFileOfTaskNode;
		}
	}

	internal class StateMachine : IDisposable {

		#region Members
		private bool	m_Disposed	= false;
		private Timer	m_Timer		= new Timer();
		#endregion

		private static StateMachine	s_StateMachine	= null;
		private static object		s_Padlock		= new object();

		#region Singleton implementation
		public static StateMachine Instance {
			get {
				lock(s_Padlock){
					if(s_StateMachine == null || s_StateMachine.m_Disposed){
						s_StateMachine = new StateMachine();
					}
				}
				return s_StateMachine;
			}
		}

		#endregion

		private StateMachine() {
			m_Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
			m_Timer.Interval = 1000;
			m_Timer.Enabled = true;
		}

		// Specify what you want to happen when the Elapsed event is raised.
		private void OnTimedEvent(object source, ElapsedEventArgs e) {
			lock(this) {
				if(!Engine.GlobalEngine.BuildEnabled) {
					System.Threading.Thread.Sleep(1000);
					if(!Engine.GlobalEngine.BuildEnabled) {
						this.Dispose();
					}
				}
			}
		}

		private bool m_isInitialized = false;
		public bool IsInitialized {
			get { return m_isInitialized; }
			set { m_isInitialized = value; }
		}

		~StateMachine() {
#if DEBUG
			Console.ReadLine();
#endif
			Dispose(false);
		}
	
		protected virtual void Dispose(bool disposing) {
			if (!m_Disposed) {
				m_Disposed = true;

				m_Timer.Enabled = false;
				ExecuteEndTargets();
				m_Timer.Dispose();
   
				// Suppress finalization of this disposed instance.
				if (disposing) 
					GC.SuppressFinalize(this);
			}
		}
		
		public void Dispose() {
			Dispose(true);
		}

		#region Statebag
		public ITaskItem[] Keys {
			get {
				ITaskItem[] keys = new ITaskItem[statebag.Count];
				int i = 0;
				foreach(string s in statebag.Keys) {
					keys[i] = new TaskItem(s);
					i++;
				}

				return keys;
			}
		}

		public ITaskItem[] Values {
			get {
				ITaskItem[] values = new ITaskItem[statebag.Count];
				statebag.Values.CopyTo(values, 0);
				return values;
			}
		}

		private Dictionary<String, ITaskItem> statebag = new Dictionary<String, ITaskItem>();

		internal ITaskItem GetValue(ITaskItem propertyName) {
			return statebag[propertyName.ToString()];
		}

		internal void SetState(ITaskItem[] keysToSet, ITaskItem[] valuesToUse) {
			if(
				(keysToSet == null && valuesToUse != null) ||
				(keysToSet != null && valuesToUse == null)
			   ) {
				throw new ArgumentNullException("Both keys and values must be set");
			}

			if(keysToSet != null && valuesToUse != null &&
				keysToSet.Length > 0 && valuesToUse.Length > 0) {
				if(keysToSet.Length != valuesToUse.Length) {
					throw new ArgumentException("There must be same number of keys and values");
				} else {
					for(int i = 0; i < keysToSet.Length; i++) {
						statebag[keysToSet[i].ToString()] = valuesToUse[i];
					}
				}
			}
		}

		public void DumpStateViaConsoleWrite() {
			if(Keys != null && Keys.Length > 0) {
				for(int i = 0; i < Keys.Length; i++) {
					Console.WriteLine("Prop: {0}, Value: {1}",
						Keys[i].ToString(), Values[i].ToString());
				}
			}
		}
		#endregion

		#region ExecuteAtEnd
		private Dictionary<string, string> m_targetsToExecuteAtEnd = new Dictionary<string, string>();

		public void AddTargets(string projectFileName, string targets) {
			if(!File.Exists(projectFileName))
				throw new FileNotFoundException();
			string fullPath = Path.GetFullPath(projectFileName);
			m_targetsToExecuteAtEnd.Add(fullPath, targets);
		}

		private void ExecuteEndTargets() {
			Engine.GlobalEngine.BinPath = RuntimeEnvironment.GetRuntimeDirectory();
			Engine.GlobalEngine.OnlyLogCriticalEvents = false;

			foreach(string projectName in m_targetsToExecuteAtEnd.Keys) {
				ConsoleLogger consoleLogger = new ConsoleLogger();
				Engine.GlobalEngine.RegisterLogger(consoleLogger);
				Project proj = new Project(Engine.GlobalEngine);
				try {
					proj.Load(projectName);
					proj.BuildEnabled = true;
					proj.Build(m_targetsToExecuteAtEnd[projectName]);
				} catch(InvalidProjectFileException) { }
			}
		}
		#endregion
	}

}
