using System;

// NUnit
using NUnit.Framework;

namespace nJupiter.DataAccess.Users.DSDAO {
	
	[Ignore] //Old tests depending on ad
	[TestFixture]
	public class UserTestDS 	{

		private UsersDAO userDao;

		[TestFixtureSetUp]
		public void Init() {
			this.userDao = UsersDAO.GetInstance("DirectoryServiceDAO");
		}

		[TestFixtureTearDown]
		public void Dispose() {
		}
		
		[Test]
		public void UserTest() {
			SearchCriteria sc = new SearchCriteria("givenName", "Andreas");
			UserCollection uc = this.userDao.GetUsersBySearchCriteria(sc);
			Assert.IsTrue(uc.Count > 0, "Andreas does not exist");

			User andreas = this.userDao.GetUserByUserName("afredrik");
			Assert.IsNotNull(andreas, "Can not get afredrik by user name");

			User user = this.userDao.GetUserById("modhelius@njupiter.org");

			Console.WriteLine(user.Properties.FullName);
			Assert.IsNotNull(user, "User null");

			Assert.IsTrue(user.Properties["telephoneNumber"].Value.ToString() == "+46 8 505 790 09", "Wrong prop");
			user.Properties["telephoneNumber"].Value = "+46 8 505 790 00";
			this.userDao.SaveUser(user);
			user = this.userDao.GetUserById("modhelius@njupiter.org");
			Assert.IsTrue(user.Properties["telephoneNumber"].Value.ToString() == "+46 8 505 790 00", "Wrong prop");
			user.Properties["telephoneNumber"].Value = "+46 8 505 790 09";
			this.userDao.SaveUser(user);
		}
	}
}
