using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Globalization;
using System.Web.Profile;

using FakeItEasy;

using nJupiter.DataAccess.Users;

using NUnit.Framework;

using ProfileProvider = nJupiter.DataAccess.Users.Web.ProfileProvider;

namespace nJupiter.Tests.nJupiter.DataAccess.Users {
	
	[TestFixture]
	public class ProfileProviderTests {

		[Test]
		public void GetPropertyValues_NonExistantUserIsCreated_CreateUserInstanceIsCalled() {
			var userRepository = A.Fake<IUserRepository>();
			var profileProvider = GetProfileProvider(userRepository);
			var context  = new SettingsContext();
			var settings = new SettingsPropertyCollection();
			SettingsProperty settingsProperty = GetSettingsProperty<string>("name");
			settings.Add(settingsProperty);
			context.Add("UserName", "njupiter\\modhelius");
			A.CallTo(() => userRepository.GetUserByUserName("modhelius", "njupiter")).Returns(null);
			profileProvider.GetPropertyValues(context, settings);
			A.CallTo(() => userRepository.CreateUserInstance("modhelius", "njupiter")).MustHaveHappened(Repeated.Exactly.Once);
		}

		[Test]
		public void SetPropertyValues_SetSettingsPropertyValue_SameValueIsSavedForUser() {
			var userRepository = A.Fake<IUserRepository>();
			var originalUser = A.Fake<IUser>();
			var clonedUser = A.Fake<IUser>();
		
			A.CallTo(() => userRepository.GetUserByUserName("modhelius", null)).Returns(originalUser);
			A.CallTo(() => originalUser.IsReadOnly).Returns(true);
			A.CallTo(() => originalUser.Clone()).Returns(clonedUser);
			var genericproperty = new GenericProperty<string>("propertyName", null, CultureInfo.InvariantCulture);
			A.CallTo(() => clonedUser.Properties.GetProperty("propertyName", Context.DefaultContext)).Returns(genericproperty);
			var profileProvider = GetProfileProvider(userRepository);
			var settings = new SettingsPropertyValueCollection();
			SettingsPropertyValue settingProperyValue = GetSettingProperyValue("propertyName", "propertyValue");
			settings.Add(settingProperyValue);

			SettingsContext context = GetContext("modhelius");

			profileProvider.SetPropertyValues(context, settings);

			Assert.AreEqual("propertyValue", genericproperty.Value);
			A.CallTo(() => userRepository.SaveUser(clonedUser)).MustHaveHappened(Repeated.Exactly.Once);
		}

	[Test]
		public void SetPropertyValues_SetSettingsPropertyValueForPropertyNotExistOnUser_ThrowsProviderException() {
			var userRepository = A.Fake<IUserRepository>();
			var user = A.Fake<IUser>();
		
			A.CallTo(() => userRepository.GetUserByUserName("modhelius", null)).Returns(user);
			A.CallTo(() => user.Properties.GetProperty("propertyName", Context.DefaultContext)).Returns(null);

			var profileProvider = GetProfileProvider(userRepository);
			var settings = new SettingsPropertyValueCollection();
			SettingsPropertyValue settingProperyValue = GetSettingProperyValue("propertyName", "propertyValue");
			settings.Add(settingProperyValue);

			SettingsContext context = GetContext("modhelius");

			Assert.Throws<ProviderException>(() => profileProvider.SetPropertyValues(context, settings));
		}

		[Test]
		public void Initialize_TestInitializedValues_CorrectValuesSet() {
			var userRepository = A.Fake<IUserRepository>();
			A.CallTo(() => userRepository.Name).Returns("MyRepository");
			var profileProvider = GetProfileProvider(userRepository);
			Assert.AreEqual("MyProvider", profileProvider.Name);
			Assert.AreEqual("MyApplication", profileProvider.ApplicationName);
			Assert.AreEqual(userRepository, profileProvider.UserRepository);
			Assert.IsTrue(profileProvider.AutomaticallyCreateNonExistingUsers);
			Assert.IsTrue(profileProvider.DeleteUsersOnProfileDeleting);
		}

		[Test]
		public void Initialize_PassingNullConfig_ThrowsArgumentNullException() {
			var profileProvider = new ProfileProvider();
			Assert.Throws<ArgumentNullException>(() => profileProvider.Initialize("provider", null));
		}

		[Test]
		public void GetAllProfiles_GenerateOneProfileToReturn_ReturnsOneProfileWithCorrectUserName() {
			var userRepository = A.Fake<IUserRepository>();

			var profileProvider = GetProfileProvider(userRepository);
			int total;

			var user = new User("1", "modhelius", "njupiter", new PropertyCollection(), null);
			var users = new List<IUser>();
			users.Add(user);
			A.CallTo(() => userRepository.GetAllUsers(0, 10, out total)).Returns(users);

			var profiles = profileProvider.GetAllProfiles(ProfileAuthenticationOption.All, 0, 10, out total);
			Assert.AreEqual(1, profiles.Count);
			Assert.AreEqual("njupiter\\modhelius", profiles["njupiter\\modhelius"].UserName);

		}

		[Test]
		public void FindProfilesByUserName_GenerateOneProfileToReturn_ReturnsOneProfileWithCorrectUserName() {
			var userRepository = A.Fake<IUserRepository>();

			var profileProvider = GetProfileProvider(userRepository);

			var user = new User("1", "modhelius", "njupiter", new PropertyCollection(), null);
			A.CallTo(() => userRepository.GetUserByUserName("modhelius", "njupiter")).Returns(user);

			int total;
			var profiles = profileProvider.FindProfilesByUserName(ProfileAuthenticationOption.All, "njupiter\\modhelius", 0, 10, out total);
			Assert.AreEqual(1, profiles.Count);
			Assert.AreEqual("njupiter\\modhelius", profiles["njupiter\\modhelius"].UserName);
		}

		[Test]
		public void DeleteProfiles_DeleteOneExistingProfile_UserDeleted() {
			var userRepository = A.Fake<IUserRepository>();
			var user = A.Fake<IUser>();
			A.CallTo(() => userRepository.GetUserByUserName("modhelius", "njupiter")).Returns(user);
			var profileProvider = GetProfileProvider(userRepository);
			var profiles = new ProfileInfoCollection();
			var profile = new ProfileInfo("njupiter\\modhelius", false, DateTime.Now, DateTime.Now, 0);
			profiles.Add(profile);
			var count = profileProvider.DeleteProfiles(profiles);
			Assert.AreEqual(1, count);
			A.CallTo(() => userRepository.DeleteUser(user)).MustHaveHappened(Repeated.Exactly.Once);
		}

		private static ProfileProvider GetProfileProvider(IUserRepository userRepository) {
			var userRepositoryManager = A.Fake<IUserRepositoryManager>();
			A.CallTo(() => userRepositoryManager.GetRepository("MyRepository")).Returns(userRepository);
			var profileProvider = new ProfileProvider(userRepositoryManager);
			var nameValues = new NameValueCollection();
			nameValues.Add("userRepository", "MyRepository");
			nameValues.Add("applicationName", "MyApplication");
			profileProvider.Initialize("MyProvider", nameValues);
			return profileProvider;
		}

		private static SettingsContext GetContext(string username) {
			var context  = new SettingsContext();
			context.Add("UserName", username);
			context.Add("IsAuthenticated", true);
			return context;
		}

		private static SettingsPropertyValue GetSettingProperyValue<T>(string name, T value) {
			var settingsProperty = GetSettingsProperty<T>(name);
			var settingProperyValue = new SettingsPropertyValue(settingsProperty);
			settingProperyValue.PropertyValue = value;
			return settingProperyValue;
		}

		private static SettingsProperty GetSettingsProperty<T>(string name) {
			var settingsProperty = new SettingsProperty(name);
			settingsProperty.PropertyType = typeof(T);
			settingsProperty.DefaultValue = default(T);
			settingsProperty.SerializeAs = SettingsSerializeAs.ProviderSpecific;
			return settingsProperty;
		}

	}
}
