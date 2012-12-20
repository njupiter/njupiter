#region Copyright & License
// 
// 	Copyright (c) 2005-2012 nJupiter
// 
// 	Permission is hereby granted, free of charge, to any person obtaining a copy
// 	of this software and associated documentation files (the "Software"), to deal
// 	in the Software without restriction, including without limitation the rights
// 	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// 	copies of the Software, and to permit persons to whom the Software is
// 	furnished to do so, subject to the following conditions:
// 
// 	The above copyright notice and this permission notice shall be included in
// 	all copies or substantial portions of the Software.
// 
// 	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	THE SOFTWARE.
// 
#endregion

using System.Collections.Generic;

namespace nJupiter.DataAccess.Users {
	public interface IPropertyHandler : IPredefinedProperties, ILockable<IPropertyHandler> {
		IProperty this[string propertyName] { get; }
		IProperty this[string propertyName, IContext context] { get; }
		IPredefinedNames PropertyNames { get; }
		IEnumerable<IContext> AttachedContexts { get; }
		IPropertyCollection GetProperties();
		IPropertyCollection GetProperties(IContext context);
		void AttachProperties(IPropertyCollection properties);
		T GetValue<T>(string propertyName);
		T GetValue<T>(string propertyName, IContext context);
		T GetValue<T>(string propertyName, string contextName);
		IProperty GetProperty(string propertyName);
		IProperty GetProperty(string propertyName, string contextName);
		IProperty GetProperty(string propertyName, IContext context);
		void SetProperty(string propertyName, object value);
		void SetProperty(string propertyName, string contextName, object value);
		void SetProperty(string propertyName, IContext context, object value);
	}
}