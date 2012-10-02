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

using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {
	internal class PredefinedNamesFactory {
		public static IPredefinedNames Create(IConfig config) {
			var contextNames = CreatePredefinedContextNames(config);
			return CreatePredefinedPropertyNames(config, contextNames);
		}

		private static IPredefinedNames CreatePredefinedPropertyNames(IConfig config, IPredefinedNames context) {
			return CreatePredefinedNames(config, GetPredefinedPropertyKey, context);
		}

		private static IPredefinedNames CreatePredefinedContextNames(IConfig config) {
			return CreatePredefinedNames(config, GetPredefinedContextAttribute, null);
		}

		private delegate string GetValueDelegate(string property, IConfig config);

		private static IPredefinedNames CreatePredefinedNames(IConfig config,
		                                                      GetValueDelegate getValue,
		                                                      IPredefinedNames context) {
			return new PredefinedNames(getValue("userName", config),
			                           getValue("fullName", config),
			                           getValue("firstName", config),
			                           getValue("lastName", config),
			                           getValue("description", config),
			                           getValue("email", config),
			                           getValue("homePage", config),
			                           getValue("streetAddress", config),
			                           getValue("company", config),
			                           getValue("department", config),
			                           getValue("city", config),
			                           getValue("telephone", config),
			                           getValue("fax", config),
			                           getValue("homeTelephone", config),
			                           getValue("mobileTelephone", config),
			                           getValue("postOfficeBox", config),
			                           getValue("postalCode", config),
			                           getValue("country", config),
			                           getValue("title", config),
			                           getValue("active", config),
			                           getValue("passwordQuestion", config),
			                           getValue("passwordAnswer", config),
			                           getValue("lastActivityDate", config),
			                           getValue("creationDate", config),
			                           getValue("lastLockoutDate", config),
			                           getValue("lastLoginDate", config),
			                           getValue("lastPasswordChangedDate", config),
			                           getValue("locked", config),
			                           getValue("lastUpdatedDate", config),
			                           getValue("isAnonymous", config),
			                           getValue("password", config),
			                           getValue("passwordSalt", config),
			                           context);
		}

		private static string GetPredefinedPropertyKey(string property, IConfig config) {
			string result = null;
			if(config.ContainsKey("predefinedProperties", property)) {
				result = config.GetValue("predefinedProperties", property);
			}
			return result;
		}

		private static string GetPredefinedContextAttribute(string property, IConfig config) {
			string result = null;
			if(config.ContainsAttribute("predefinedProperties", property, "context")) {
				result = config.GetAttribute("predefinedProperties", property, "context");
			}
			return result;
		}
	}
}