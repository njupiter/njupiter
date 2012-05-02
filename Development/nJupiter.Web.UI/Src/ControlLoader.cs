#region Copyright & License
/*
	Copyright (c) 2005-2012 nJupiter

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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.UI;

namespace nJupiter.Web.UI {
	/// <summary>
	/// This class is here to work around a behaviour (bug?) in ASP.NET that makes it impossible
	/// to output cache different instances of web controls if they are created dynamically (in for
	/// example lists) and if they also share the same virtual path.
	/// 
	/// The output cache is caching a web control with a cache key created with a combination of the
	/// virtual path and a hash code of the call stack (!) of the executing control. By creating a
	/// Dynamic Method for every varyByCustom string we get a different call stack for the different
	/// controls and can thereby have different caches for different web controls even if they are
	/// loaded dynamically.
	/// 
	/// Read more about it here:
	/// http://www.tommycode.se/2012/04/outputcache-on-dynamically-loaded.html
	/// </summary>
	public class ControlLoader : IControlLoader {

		private delegate Control LoadControlDelegate(TemplateControl templateControl, string virtualPath);
		private readonly Dictionary<string, LoadControlDelegate> delegateStore = new Dictionary<string, LoadControlDelegate>();
		private readonly Type[] loadControlDelegateParameterTypes = new[] { typeof(TemplateControl), typeof(string) };
		private readonly MethodInfo invoker = typeof(ControlLoader).GetMethod("LoadControlInvoker");
		private readonly object padLock = new object();

		public Control LoadControl(TemplateControl templateControl, string virtualPath, string varyByCustom) {
			var cacheKey = GetCacheKey(virtualPath, varyByCustom);
			if(!delegateStore.ContainsKey(cacheKey)) {
				lock(padLock) {
					if(!delegateStore.ContainsKey(cacheKey)) {
						delegateStore[cacheKey] = CreateLoadControlDelegate();
					}
				}
			}
			return delegateStore[cacheKey](templateControl, virtualPath);
		}

		private string GetCacheKey(string virtualPath, string varyByCustom) {
			return string.Format("{0}{1}", virtualPath, varyByCustom);
		}

		private LoadControlDelegate CreateLoadControlDelegate() {
			var dynamicMethod = CreateDynamicMethod();
			return (LoadControlDelegate)dynamicMethod.CreateDelegate(typeof(LoadControlDelegate));
		}

		private DynamicMethod CreateDynamicMethod() {
			var dynamicMethod = new DynamicMethod(string.Empty, typeof(Control), loadControlDelegateParameterTypes);
			GenerateMethodBody(dynamicMethod);
			return dynamicMethod;
		}

		private void GenerateMethodBody(DynamicMethod dynamicMethod) {
			var ilGenerator = dynamicMethod.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldarg_0); // Load templateControl argument onto stack
			ilGenerator.Emit(OpCodes.Ldarg_1); // Load virtualPath argument onto stack
			ilGenerator.Emit(OpCodes.Call, invoker); // Invoke LoadControlInvoker with the loaded two arguments
			ilGenerator.Emit(OpCodes.Ret); // End method
		}

		public static Control LoadControlInvoker(TemplateControl templateControl, string virtualPath) {
			return templateControl.LoadControl(virtualPath);
		}

		public static IControlLoader Instance { get { return NestedSingleton.instance; } }

		// thread safe Singleton implementation with fully lazy instantiation and with full performance
		private sealed class NestedSingleton {
			// ReSharper disable EmptyConstructor
			static NestedSingleton() {} // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
			// ReSharper restore EmptyConstructor
			internal static readonly IControlLoader instance = new ControlLoader();
		}
	}
}