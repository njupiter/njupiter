using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using NUnit.Framework;

namespace nJupiter.DataAccess.Users.SQLDAO {
	[Serializable]
	public class SerializableTestClass {
		private readonly string testString;
		private readonly DateTime testDate;

		public string TestString { get { return this.testString; } }
		public DateTime TestDate { get { return this.testDate; } }

		public SerializableTestClass(string testString, DateTime testDate) {
			this.testString = testString;
			this.testDate = testDate;
		}
	}

	[Ignore] //Old tests depending on db
	[TestFixture]
	public class UserTest {
		private const string UserName = "UnitTestUser";
		private const string Password = "UnitTestUser Password";
		private const string TestString = "TestString";
		private readonly DateTime testDate = DateTime.Now;

		private IUser user;
		private IUser user2;
		private UsersDAO usersDao;

		[TestFixtureSetUp]
		public void Init() {
			this.usersDao = UsersDAO.GetInstance("SQLDAO");
		}

		[TestFixtureTearDown]
		public void Dispose() {
			if(this.user != null)
				this.usersDao.DeleteUser(this.user);
			if(this.user2 != null)
				this.usersDao.DeleteUser(this.user2);
		}

		private static string GetRandomFirstname(Random random) {
			string[] firstnames = {	"Sven", "Karl", "Sune", "Johan", "Carl",
									 "Bertil", "Fredrik", "Arne", "Ingmar", "Bo",
									 "Ingvar", "Yngve", "Erik", "Eric", "John",
									 "Bengt", "Severin", "Anders", "Nicklas", "Isak",
									 "Andreas", "Martin", "Per", "Conny", "Hans" };
			return firstnames[random.Next(firstnames.Length - 1)];
		}

		private static string GetRandomLastname(Random random) {
			string[] lastnames = {	"Svensson", "Karlsson", "Sunesson", "Johansson", "Carlsson",
									"Bertilsson", "Fredriksson", "Arnesson", "Ingmarsson", "Bosson",
									"Ingvarsson", "Yngvesson", "Eriksson", "Ericsson", "Johnsson",
									"Bengtsson", "Severinsson", "Andersson", "Nicklasson", "Isaksson",
									"Andreasson", "Martinsson", "Persson", "Connysson", "Hansson" };
			return lastnames[random.Next(lastnames.Length - 1)];
		}

		[Test]
		public void UserCreateTestUsers() {
			Random random = new Random((int)DateTime.Now.Ticks);
			for(int i = 0; i < 1000; i++) {
				string userName = "user" + i;
				IUser userInstance = this.usersDao.CreateUserInstance(userName, "testUsers");
				string firstName = GetRandomFirstname(random);
				string lastName = GetRandomLastname(random);
				userInstance.Properties[this.usersDao.PropertyNames.FirstName].Object = firstName;
				userInstance.Properties[this.usersDao.PropertyNames.LastName].Object = lastName;
				userInstance.Properties[this.usersDao.PropertyNames.Email].Object = userName + "@njupiter.org";
				this.usersDao.SaveUser(userInstance);
			}
		}

		[Test]
		public void UserDeleteTestUsers() {
			var users = this.usersDao.GetUsersByDomain("testUsers");
			foreach(IUser u in users) {
				this.usersDao.DeleteUser(u);
			}
		}


		[Test]
		public void UserFieldSerialization() {

			/*
			UserCollection users = usersDao.GetUsersByDomain("sv-SE");
			Random random = new Random((int)DateTime.Now.Ticks);
			foreach(User tmpUsr in users){
				string firstName = GetRandomFirstname(random);
				string lastName = GetRandomLastname(random);
				tmpUsr.Properties[usersDao.PropertyNames.FirstName].Value = firstName;
				tmpUsr.Properties[usersDao.PropertyNames.LastName].Value = lastName;
				tmpUsr.Properties[usersDao.PropertyNames.Email].Value = firstName + "." + lastName + "@stockholm.se";
				tmpUsr.Properties[usersDao.PropertyNames.Telephone].Value = "+46 08 123 " + random.Next(100, 999);
				tmpUsr.Properties[usersDao.PropertyNames.MobileTelephone].Value = "+46 070 123 " + random.Next(100, 999);
				tmpUsr.Properties[usersDao.PropertyNames.Title].Value = "Chef";
				usersDao.SaveUser(tmpUsr);
			}*/

			// Create a user. This will not save anything to database, just create an instance of user
			this.user = this.usersDao.CreateUserInstance(UserName);

			// Check so property does exist
			if(this.user.Properties["object"] != null) {
				try {
					// If the property exist then set it
					DateTime date = DateTime.Now;
					const string testString = "Test string";
					SerializableTestClass testObject = new SerializableTestClass(testString, date);

					this.user.Properties["object"].Object = testObject;

					// Save user to database
					this.usersDao.SaveUser(this.user);

					// Load user again by its user name
					this.user = this.usersDao.GetUserByUserName(UserName);

					testObject = (SerializableTestClass)this.user.Properties["object"].Object;

					Assert.IsTrue(testObject.TestString == testString && testObject.TestDate == date);
				} finally {
					// And finaly remobe user from database
					this.usersDao.DeleteUser(this.user);
					this.user = null;
				}
			}
		}


		[Test]
		public void UserDeleteTestUser() {
			this.user = this.usersDao.GetUserByUserName(UserName);
			if(this.user != null) {
				this.usersDao.DeleteUser(this.user);
				this.user = null;
			}
		}

		[Test]
		public void UserSimpleExampleWithoutUsingContexts() {
			// Create a user. This will not save anything to database, just create an instance of user
			this.user = this.usersDao.CreateUserInstance(UserName);
			// Set Password for newly created user
			this.usersDao.SetPassword(this.user, Password);

			// Check so property does exist
			if(this.user.Properties[this.usersDao.PropertyNames.FirstName] != null) {
				try {
					// If the property exist then set it
					this.user.Properties[this.usersDao.PropertyNames.FirstName].Object = "Example Name";
					// Save user to database
					this.usersDao.SaveUser(this.user);
					// Load user again by its user name
					this.user = this.usersDao.GetUserByUserName(UserName);
					// Check so the property still has the value previously set
					Assert.AreEqual(this.user.Properties[this.usersDao.PropertyNames.FirstName].Object, "Example Name");
				} finally {
					// And finaly remobe user from database
					this.usersDao.DeleteUser(this.user);
					this.user = null;
				}
			}
		}

		[Test]
		public void UserSimpleExampleUsingContexts() {
			// Get a collection of all contexts
			var contexts = this.usersDao.GetContexts();
			// Get the registration-context
			Context context = contexts.SingleOrDefault(c => c.Name == "registration");
			// Check that the registration-context exists
			if(context != null) {
				// Create a new user
				this.user = this.usersDao.CreateUserInstance(UserName);
				// Get properties for user dependent on the context
				var properties = this.usersDao.LoadProperties(this.user, context);
				// Check so firstName property does belong to this context
				if(properties[this.usersDao.PropertyNames.FirstName] != null) {
					// If it does then set it
					properties[this.usersDao.PropertyNames.FirstName].Object = "Example Name";
				}
				// Save to database
				this.usersDao.SaveUser(this.user);


				if(properties[this.usersDao.PropertyNames.FirstName] != null) {
					// Check so the firstName still is the same after attaching
					Assert.IsTrue((string)this.user.Properties[this.usersDao.PropertyNames.FirstName, context].Object == "Example Name");
					// And then set it to a new name
					this.user.Properties[this.usersDao.PropertyNames.FirstName, context].Object = "New Name";
					// If you want to set a global value for firstname then access it without context
					this.user.Properties[this.usersDao.PropertyNames.FirstName].Object = "Global Name";
					// Global firstName and context specific firstName are not the same
					Assert.IsTrue(this.user.Properties[this.usersDao.PropertyNames.FirstName].Object.ToString() != this.user.Properties["firstName", context].Object.ToString(), "Global and context specific properties are same but shall be different.");
				}

				// And finally delete testuser from database
				this.usersDao.DeleteUser(this.user);
			}
		}

		[Test]
		public void DefaultUserCreate() {
			IUser defaultUser = this.usersDao.CreateUserInstance("DefaultUser");
			this.usersDao.SaveUser(defaultUser);

		}

		[Test]
		public void DefaultUserDelete() {
			IUser defaultUser = this.usersDao.GetUserByUserName("DefaultUser");
			if(defaultUser != null)
				this.usersDao.DeleteUser(defaultUser);
		}

		[Test]
		public void TestUser() {
			// Create new user
			this.user = this.usersDao.CreateUserInstance(UserName);
			// Set Password for newly created user
			this.usersDao.SetPassword(this.user, Password);
			// Check  so the Password can be checked
			Assert.IsTrue(this.usersDao.CheckPassword(this.user, Password), "Passwordcheck failed.");
			// And so not wrong Password is accepted
			Assert.IsFalse(this.usersDao.CheckPassword(this.user, "WrongPassword"), "Wrong Password was accepted.");

			// Get the definition for all properties that can be set for this user
			var fdc = this.usersDao.GetDefaultContextSchema();

			// Get all properties in the collection of type string
			var stringFdc = fdc.Values.Where(p => typeof(string).Equals(p.DataType));
			// Get all properties in the collection of type DateTime
			var datetimeFdc = fdc.Values.Where(p => typeof(DateTime).Equals(p.DataType));
			// Get all properties in the collection of type bool
			var boolFdc = fdc.Values.Where(p => typeof(bool).Equals(p.DataType));

			// If there is any property for the user of type string, then set it to TestString
			if(stringFdc.Any()) {
				PropertyDefinition stringPropertyDefinition = stringFdc.First();
				this.user.Properties[stringPropertyDefinition.PropertyName].Object = TestString;
				Assert.AreEqual(this.user.Properties[stringPropertyDefinition.PropertyName].Object, TestString); // And test
			}
			// If there is any property for the user of type DateTime, then set it to testDate
			if(datetimeFdc.Any()) {
				PropertyDefinition datetimePropertyDefinition = datetimeFdc.First();
				this.user.Properties[datetimePropertyDefinition.PropertyName].Object = this.testDate;
				Assert.AreEqual(this.user.Properties[datetimePropertyDefinition.PropertyName].Object, this.testDate); // And test
			}
			// If there is any property for the user of type bool, then set it to true
			if(boolFdc.Any()) {
				PropertyDefinition boolPropertyDefinition = boolFdc.First();
				this.user.Properties[boolPropertyDefinition.PropertyName].Object = true;
				Assert.IsTrue(((BoolProperty)this.user.Properties[boolPropertyDefinition.PropertyName]).Value); // And test
			}

			// Save user to database
			this.usersDao.SaveUser(this.user);

			// Load the saved user into a new object. Get it by user name
			IUser tmpUser1 = this.usersDao.GetUserByUserName(this.user.UserName);

			// Check so the Password is loaded correctly from the database
			Assert.IsTrue(this.usersDao.CheckPassword(tmpUser1, Password), "Passwordcheck failed after object has been saved to database.");
			// And so still not wrong Password is accepted
			Assert.IsFalse(this.usersDao.CheckPassword(tmpUser1, "WrongPassword"), "Wrong Password was accepted after object has been saved to database.");

			// Check that the first and second object doesn't differ
			Assert.AreEqual(this.user, tmpUser1, "User differs after object has been saved to database.");
			Assert.AreEqual(this.user.UserName, tmpUser1.UserName, "UserName differs after object has been saved to database.");

			// If properties of different types was set above, check so they are still the same.
			if(stringFdc.Any()) {
				Assert.AreEqual(tmpUser1.Properties[stringFdc.First().PropertyName].Object, TestString);
			}
			if(datetimeFdc.Any()) {
				Assert.AreEqual(((DateTime)tmpUser1.Properties[datetimeFdc.First().PropertyName].Object), this.testDate);
			}
			if(boolFdc.Any()) {
				Assert.IsTrue(((BoolProperty)tmpUser1.Properties[boolFdc.First().PropertyName]).Value);
			}

			// And save user again
			this.usersDao.SaveUser(tmpUser1);

			// Delete testuser from database
			this.usersDao.DeleteUser(this.user);

			// Try to get the user from database
			this.user = this.usersDao.GetUserById(this.user.Id);

			// If user not null the user still exists in database
			Assert.IsNull(this.user, "The user not deleted from the database.");
		}

		[Test]
		public void TestContext() {
			// Get a collection of all contexts
			var contexts = this.usersDao.GetContexts();

			// If there is any contexts available ten do test
			if(contexts.Any()) {
				// Get the first context in the collection
				Context context = null;
				foreach(Context c in contexts) {
					context = c;
					break;
				}
				// Create a new user
				CultureInfo ci = new CultureInfo("sv-SE");
				this.user2 = this.usersDao.CreateUserInstance(UserName, ci.Name);
				// Set Password for newly created user
				this.usersDao.SetPassword(this.user2, Password);

				// Load properties for a specific context into the user object
				this.usersDao.LoadProperties(this.user2, context);

				// Get properties dependent on the context for user
				var properties = this.usersDao.GetProperties(this.user2, context);

				// Get properties dependent on the context
				var emptyFields = this.usersDao.GetProperties(context);

				Assert.IsTrue(properties.Count == emptyFields.Count, "The number of properties is not the same as an empty propertycollection");

				if(context != null) {
					Assert.IsTrue(context.ContextSchema.Count == properties.Count,
								  "The number of properties not equal to the number of property definitions.");

					// Get all properties in the collection of type string

					var stringFdc = context.ContextSchema.Values.Where(p => typeof(string).Equals(p.DataType));
					// Get all properties in the collection of type DateTime
					var datetimeFdc = context.ContextSchema.Values.Where(p => typeof(DateTime).Equals(p.DataType));
					// Get all properties in the collection of type bool
					var boolFdc = context.ContextSchema.Values.Where(p => typeof(bool).Equals(p.DataType));

					Context newContext = this.usersDao.CreateContext(context.Name + "-ClonedContext", context.ContextSchema);
					this.usersDao.LoadProperties(this.user2, newContext);

					// If there is any property for the user of type string, then set it to TestString
					if(stringFdc.Any()) {
						PropertyDefinition stringPropertyDefinition = stringFdc.First();
						properties[stringPropertyDefinition.PropertyName].Object = TestString;
						Assert.AreEqual(properties[stringPropertyDefinition.PropertyName].Object, TestString); // And test
						this.user2.Properties[stringPropertyDefinition.PropertyName, newContext].Object = TestString + "2";
						Assert.AreEqual(this.user2.Properties[stringPropertyDefinition.PropertyName].Object.ToString(), TestString);
					}
					// If there is any property for the user of type DateTime, then set it to testDate
					if(datetimeFdc.Any()) {
						PropertyDefinition datetimePropertyDefinition = datetimeFdc.First();
						properties[datetimePropertyDefinition.PropertyName].Object = this.testDate;
						Assert.AreEqual(properties[datetimePropertyDefinition.PropertyName].Object, this.testDate); // And test
					}
					// If there is any property for the user of type bool, then set it to true
					if(boolFdc.Any()) {
						PropertyDefinition boolPropertyDefinition = boolFdc.First();
						properties[boolPropertyDefinition.PropertyName].Object = true;
						Assert.IsTrue(((BoolProperty)properties[boolPropertyDefinition.PropertyName]).Value); // And test
					}

					// Save to database
					this.usersDao.SaveUser(this.user2);

					this.user2 = this.usersDao.GetUserById(this.user2.Id);

					if(stringFdc.Any()) {
						PropertyDefinition stringPropertyDefinition = stringFdc.First();
						Assert.AreEqual(this.user2.Properties[stringPropertyDefinition.PropertyName, newContext].Object.ToString(),
										TestString + "2");
						Assert.AreEqual(this.user2.Properties[stringPropertyDefinition.PropertyName].Object.ToString(), TestString);
					}

					this.usersDao.DeleteContext(newContext);
					// Check so the context is removed by creating a new one with the same name
					newContext = this.usersDao.CreateContext(newContext.Name, newContext.ContextSchema);
					this.usersDao.DeleteContext(newContext);

					foreach(PropertyDefinition fd in context.ContextSchema.Values) {
						if(this.user2.Properties[fd.PropertyName].Object != null)
							Console.WriteLine(fd.PropertyName + ": " + this.user2.Properties[fd.PropertyName].Object);
						else
							Console.WriteLine(fd.PropertyName + ": <null>");
					}
				}

				// Delete testuser from database
				this.usersDao.DeleteUser(this.user2);

				// Try to get the user from database
				this.user2 = this.usersDao.GetUserById(this.user2.Id);

				// If user not null the user still exists in database
				Assert.IsNull(this.user2, "The user not deleted from the database.");
			}
		}

		[Test]
		public void TestSearch() {
			var scc = new List<SearchCriteria>();
			scc.Add(new SearchCriteria("firstName", "", SearchCriteria.CompareCondition.StartsWith, false));
			//scc.Add(new SearchCriteria("lastName", "oe", SearchCriteria.CompareCondition.EndsWith, true));
			//scc.Add(new SearchCriteria("email", "sson", SearchCriteria.CompareCondition.Contains, true));
			var usersContains = this.usersDao.GetUsersBySearchCriteria(scc);
			Console.WriteLine("StartsWith");
			foreach(IUser u in usersContains) {
				Console.WriteLine(u.UserName);
			}
		}



		private static PropertyDefinition GetFirstPropertySchema(IDictionary<string, PropertyDefinition> pst) {
			foreach(PropertyDefinition ps in pst.Values) {
				return ps;
			}
			return null;
		}

	}
}
