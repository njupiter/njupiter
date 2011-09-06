using NUnit.Framework;

namespace nJupiter.DataAccess.Users.DirectoryService {

	[TestFixture, Ignore("Old tests depending on AD")]
	public class DirectoryServiceTest {
		private DirectoryService directoryService;

		[TestFixtureSetUp]
		public void Init() {
			this.directoryService = DirectoryService.GetInstance();
		}

		[TestFixtureTearDown]
		public void Dispose() {
		}
		
		[Test]
		public void DirectoryObjectGet() {
			DirectoryObject directoryObject = this.directoryService.GetDirectoryObjectById("SDL18EBE");
			Assert.IsNotNull(directoryObject);
		}

		[Test]
		public void DirectoryObjectsGet() {
			SearchCriteria[] sc = new[] { new SearchCriteria("description", "Technology") };
			DirectoryObject[] directoryObjects = this.directoryService.GetDirectoryObjectsBySearchCriteria(sc);
			Assert.IsNotNull(directoryObjects);
		}

		[Test]
		public void DirectoryObjectUpdate() {
			DirectoryObject directoryObject = this.directoryService.GetDirectoryObjectById("kai.de.leeuw@njupiter.org");
			directoryObject["sn"] = "de Leeuw";
			this.directoryService.SaveDirectoryObject(directoryObject);
		}
	}
}
