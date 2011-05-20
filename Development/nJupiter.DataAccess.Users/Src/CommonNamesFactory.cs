using nJupiter.Configuration;

namespace nJupiter.DataAccess.Users {

	internal class CommonNamesFactory {
		
		public static ICommonNames CreateCommonPropertyNames(IConfig config, ICommonNames context) {
			return CreateCommonNames(config, GetCommonPropertyKey, context);
		}

		public static ICommonNames CreateCommonContextNames(IConfig config) {
			return CreateCommonNames(config, GetCommonContextAttribute, null);
		}

		private delegate string GetValueDelegate(string property, IConfig config);

		private static ICommonNames CreateCommonNames(IConfig config, GetValueDelegate getValue, ICommonNames context) {
			return new CommonNames(	getValue("userName", config),
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

		private static string GetCommonPropertyKey(string property, IConfig config) {
			string result = null;
			if(config.ContainsKey("commonProperties", property))
				result = config.GetValue("commonProperties", property);
			return result;
		}

		private static string GetCommonContextAttribute(string property, IConfig config) {
			string result = null;
			if(config.ContainsAttribute("commonProperties", property, "context"))
				result = config.GetAttribute("commonProperties", property, "context");
			return result;
		}

	}
}
