#region Copyright & License
/*
	Copyright (c) 2005-2011 nJupiter

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

using System.Web.Services;
using System.Xml.Serialization;

using nJupiter.Configuration;

namespace nJupiter.DataAccess.DirectoryService.Web {

	[WebService(Namespace="urn:services:dataaccess.directoryservice")]
	internal sealed class DirectoryServiceWebService {

		#region Properties
		private DirectoryService CurrentDirectoryService{
			get{
				string dirService = null;
				IConfig config = ConfigHandlerOld.GetConfig(this.GetType().Assembly);
				if(config.ContainsKey("webService", "directoryService"))
					dirService = config.GetValue("webService", "directoryService");
				if(dirService != null)
					return DirectoryService.GetInstance(dirService);
				return DirectoryService.GetInstance();
			}
		}
		#endregion

		#region Methods
		[WebMethod]
		[return:XmlElement("DirectoryObject", IsNullable=true)]
		public DirectoryObject GetDirectoryObjectById([XmlElement("Id")]string id) {
			return this.CurrentDirectoryService.GetDirectoryObjectById(id);
		}
		[WebMethod]
		[return:XmlArray("DirectoryObjects")]
		public DirectoryObject[] GetDirectoryEntriesBySearchCriteria([XmlElement("SearchCriteria")]SearchCriteria[] searchCriteria) {
			return this.CurrentDirectoryService.GetDirectoryObjectsBySearchCriteria(searchCriteria);
		}
		[WebMethod]
		public void SaveDirectoryObject([XmlElement("DirectoryObject")]DirectoryObject directoryObject) {
			this.CurrentDirectoryService.SaveDirectoryObject(directoryObject);
		}
		[WebMethod]
		[return:XmlElement("DirectoryObject", IsNullable=true)]
		public DirectoryObject CreateDirectoryObjectInstance() {
			return this.CurrentDirectoryService.CreateDirectoryObjectInstance();
		}
		#endregion

	}

}