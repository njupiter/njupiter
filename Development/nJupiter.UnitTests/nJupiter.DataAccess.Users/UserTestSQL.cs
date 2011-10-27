using System;
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

	[TestFixture]
	public class UserTest {
		private const string UserName = "UnitTestUser";
		private const string Password = "UnitTestUser Password";
		private const string TestString = "TestString";
		private readonly DateTime testDate = DateTime.Now;

		private User user;
		private User user2;
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
				User userInstance = this.usersDao.CreateUserInstance(userName, "testUsers");
				string firstName = GetRandomFirstname(random);
				string lastName = GetRandomLastname(random);
				userInstance.Properties[this.usersDao.PropertyNames.FirstName].Value = firstName;
				userInstance.Properties[this.usersDao.PropertyNames.LastName].Value = lastName;
				userInstance.Properties[this.usersDao.PropertyNames.Email].Value = userName + "@njupiter.org";
				this.usersDao.SaveUser(userInstance);
			}
		}

		[Test]
		public void UserDeleteTestUsers() {
			UserCollection users = this.usersDao.GetUsersByDomain("testUsers");
			foreach(User u in users) {
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

					this.user.Properties["object"].Value = testObject;

					// Save user to database
					this.usersDao.SaveUser(this.user);

					// Load user again by its user name
					this.user = this.usersDao.GetUserByUserName(UserName);

					testObject = (SerializableTestClass)this.user.Properties["object"].Value;

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
					this.user.Properties[this.usersDao.PropertyNames.FirstName].Value = "Example Name";
					// Save user to database
					this.usersDao.SaveUser(this.user);
					// Load user again by its user name
					this.user = this.usersDao.GetUserByUserName(UserName);
					// Check so the property still has the value previously set
					Assert.AreEqual(this.user.Properties[this.usersDao.PropertyNames.FirstName].Value, "Example Name");
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
			ContextCollection contexts = this.usersDao.GetContexts();
			// Get the registration-context
			Context context = contexts["registration"];
			// Check that the registration-context exists
			if(context != null) {
				// Create a new user
				this.user = this.usersDao.CreateUserInstance(UserName);
				// Get properties for user dependent on the context
				PropertyCollection properties = this.usersDao.LoadProperties(this.user, context);
				// Check so firstName property does belong to this context
				if(properties[this.usersDao.PropertyNames.FirstName] != null) {
					// If it does then set it
					properties[this.usersDao.PropertyNames.FirstName].Value = "Example Name";
				}
				// Save to database
				this.usersDao.SaveUser(this.user);


				if(properties[this.usersDao.PropertyNames.FirstName] != null) {
					// Check so the firstName still is the same after attaching
					Assert.IsTrue((string)this.user.Properties[this.usersDao.PropertyNames.FirstName, context].Value == "Example Name");
					// And then set it to a new name
					this.user.Properties[this.usersDao.PropertyNames.FirstName, context].Value = "New Name";
					// If you want to set a global value for firstname then access it without context
					this.user.Properties[this.usersDao.PropertyNames.FirstName].Value = "Global Name";
					// Global firstName and context specific firstName are not the same
					Assert.IsTrue(this.user.Properties[this.usersDao.PropertyNames.FirstName].Value.ToString() != this.user.Properties["firstName", context].Value.ToString(), "Global and context specific properties are same but shall be different.");
				}

				// And finally delete testuser from database
				this.usersDao.DeleteUser(this.user);
			}
		}

		[Test]
		public void DefaultUserCreate() {
			User defaultUser = this.usersDao.CreateUserInstance("DefaultUser");
			this.usersDao.SaveUser(defaultUser);

		}

		[Test]
		public void DefaultUserDelete() {
			User defaultUser = this.usersDao.GetUserByUserName("DefaultUser");
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
			PropertySchemaTable fdc = this.usersDao.GetPropertySchemas();

			// Get all properties in the collection of type string
			PropertySchemaTable stringFdc = fdc.FilterOnType(typeof(string));
			// Get all properties in the collection of type DateTime
			PropertySchemaTable datetimeFdc = fdc.FilterOnType(typeof(DateTime));
			// Get all properties in the collection of type bool
			PropertySchemaTable boolFdc = fdc.FilterOnType(typeof(bool));

			// If there is any property for the user of type string, then set it to TestString
			if(stringFdc.Count > 0) {
				PropertySchema stringPropertySchema = GetFirstPropertySchema(stringFdc);
				this.user.Properties[stringPropertySchema.PropertyName].Value = TestString;
				Assert.AreEqual(this.user.Properties[stringPropertySchema.PropertyName].Value, TestString); // And test
			}
			// If there is any property for the user of type DateTime, then set it to testDate
			if(datetimeFdc.Count > 0) {
				PropertySchema datetimePropertySchema = GetFirstPropertySchema(datetimeFdc);
				this.user.Properties[datetimePropertySchema.PropertyName].Value = this.testDate;
				Assert.AreEqual(this.user.Properties[datetimePropertySchema.PropertyName].Value, this.testDate); // And test
			}
			// If there is any property for the user of type bool, then set it to true
			if(boolFdc.Count > 0) {
				PropertySchema boolPropertySchema = GetFirstPropertySchema(boolFdc);
				this.user.Properties[boolPropertySchema.PropertyName].Value = true;
				Assert.IsTrue(((BoolProperty)this.user.Properties[boolPropertySchema.PropertyName]).Value); // And test
			}

			// Save user to database
			this.usersDao.SaveUser(this.user);

			// Load the saved user into a new object. Get it by user name
			User tmpUser1 = this.usersDao.GetUserByUserName(this.user.UserName);

			// Check so the Password is loaded correctly from the database
			Assert.IsTrue(this.usersDao.CheckPassword(tmpUser1, Password), "Passwordcheck failed after object has been saved to database.");
			// And so still not wrong Password is accepted
			Assert.IsFalse(this.usersDao.CheckPassword(tmpUser1, "WrongPassword"), "Wrong Password was accepted after object has been saved to database.");

			// Check that the first and second object doesn't differ
			Assert.AreEqual(this.user, tmpUser1, "User differs after object has been saved to database.");
			Assert.AreEqual(this.user.UserName, tmpUser1.UserName, "UserName differs after object has been saved to database.");

			// If properties of different types was set above, check so they are still the same.
			if(stringFdc.Count > 0) {
				Assert.AreEqual(tmpUser1.Properties[GetFirstPropertySchema(stringFdc).PropertyName].Value, TestString);
			}
			if(datetimeFdc.Count > 0) {
				Assert.AreEqual(((DateTime)tmpUser1.Properties[GetFirstPropertySchema(datetimeFdc).PropertyName].Value), this.testDate);
			}
			if(boolFdc.Count > 0) {
				Assert.IsTrue(((BoolProperty)tmpUser1.Properties[GetFirstPropertySchema(boolFdc).PropertyName]).Value);
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
			ContextCollection contexts = this.usersDao.GetContexts();

			// If there is any contexts available ten do test
			if(contexts.Count > 0) {
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
				PropertyCollection properties = this.usersDao.GetProperties(this.user2, context);

				// Get properties dependent on the context
				PropertyCollection emptyFields = this.usersDao.GetProperties(context);

				Assert.IsTrue(properties.Count == emptyFields.Count, "The number of properties is not the same as an empty propertycollection");

				if(context != null) {
					Assert.IsTrue(context.PropertySchemas.Count == properties.Count,
								  "The number of properties not equal to the number of property definitions.");

					// Get all properties in the collection of type string
					PropertySchemaTable stringFdc = context.PropertySchemas.FilterOnType(typeof(string));
					// Get all properties in the collection of type DateTime
					PropertySchemaTable datetimeFdc = context.PropertySchemas.FilterOnType(typeof(DateTime));
					// Get all properties in the collection of type bool
					PropertySchemaTable boolFdc = context.PropertySchemas.FilterOnType(typeof(bool));

					Context newContext = this.usersDao.CreateContext(context.Name + "-ClonedContext", context.PropertySchemas);
					this.usersDao.LoadProperties(this.user2, newContext);

					// If there is any property for the user of type string, then set it to TestString
					if(stringFdc.Count > 0) {
						PropertySchema stringPropertySchema = GetFirstPropertySchema(stringFdc);
						properties[stringPropertySchema.PropertyName].Value = TestString;
						Assert.AreEqual(properties[stringPropertySchema.PropertyName].Value, TestString); // And test
						this.user2.Properties[stringPropertySchema.PropertyName, newContext].Value = TestString + "2";
						Assert.AreEqual(this.user2.Properties[stringPropertySchema.PropertyName].Value.ToString(), TestString);
					}
					// If there is any property for the user of type DateTime, then set it to testDate
					if(datetimeFdc.Count > 0) {
						PropertySchema datetimePropertySchema = GetFirstPropertySchema(datetimeFdc);
						properties[datetimePropertySchema.PropertyName].Value = this.testDate;
						Assert.AreEqual(properties[datetimePropertySchema.PropertyName].Value, this.testDate); // And test
					}
					// If there is any property for the user of type bool, then set it to true
					if(boolFdc.Count > 0) {
						PropertySchema boolPropertySchema = GetFirstPropertySchema(boolFdc);
						properties[boolPropertySchema.PropertyName].Value = true;
						Assert.IsTrue(((BoolProperty)properties[boolPropertySchema.PropertyName]).Value); // And test
					}

					// Save to database
					this.usersDao.SaveUser(this.user2);

					this.user2 = this.usersDao.GetUserById(this.user2.Id);

					if(stringFdc.Count > 0) {
						PropertySchema stringPropertySchema = GetFirstPropertySchema(stringFdc);
						Assert.AreEqual(this.user2.Properties[stringPropertySchema.PropertyName, newContext].Value.ToString(),
										TestString + "2");
						Assert.AreEqual(this.user2.Properties[stringPropertySchema.PropertyName].Value.ToString(), TestString);
					}

					this.usersDao.DeleteContext(newContext);
					// Check so the context is removed by creating a new one with the same name
					newContext = this.usersDao.CreateContext(newContext.Name, newContext.PropertySchemas);
					this.usersDao.DeleteContext(newContext);

					foreach(PropertySchema fd in context.PropertySchemas) {
						if(this.user2.Properties[fd.PropertyName].Value != null)
							Console.WriteLine(fd.PropertyName + ": " + this.user2.Properties[fd.PropertyName].Value);
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
			SearchCriteriaCollection scc = new SearchCriteriaCollection();
			scc.Add(new SearchCriteria("firstName", "", SearchCriteria.CompareCondition.StartsWith, false));
			//scc.Add(new SearchCriteria("lastName", "oe", SearchCriteria.CompareCondition.EndsWith, true));
			//scc.Add(new SearchCriteria("email", "sson", SearchCriteria.CompareCondition.Contains, true));
			UserCollection usersContains = this.usersDao.GetUsersBySearchCriteria(scc);
			Console.WriteLine("StartsWith");
			foreach(User u in usersContains) {
				Console.WriteLine(u.UserName);
			}
		}



		private static PropertySchema GetFirstPropertySchema(PropertySchemaTable pst) {
			foreach(PropertySchema ps in pst) {
				return ps;
			}
			return null;
		}

	}
}
