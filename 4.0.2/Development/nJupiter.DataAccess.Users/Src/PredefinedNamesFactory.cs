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

		private static IPredefinedNames CreatePredefinedNames(IConfig config, GetValueDelegate getValue, IPredefinedNames context) {
			return new PredefinedNames(	getValue("userName", config),
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
			if(config.ContainsKey("predefinedProperties", property))
				result = config.GetValue("predefinedProperties", property);
			return result;
		}

		private static string GetPredefinedContextAttribute(string property, IConfig config) {
			string result = null;
			if(config.ContainsAttribute("predefinedProperties", property, "context"))
				result = config.GetAttribute("predefinedProperties", property, "context");
			return result;
		}

	}
}
