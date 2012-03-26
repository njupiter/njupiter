#region Copyright & License
/*
	Copyright (c) 2005-2010 nJupiter

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

using System;
using System.Net;

using nJupiter.DataAccess.DirectoryService.Adapters.WebService;

using WS = nJupiter.DataAccess.DirectoryService.Adapters.WebService;

namespace nJupiter.DataAccess.DirectoryService.Adapters {

	public sealed class WSDirectoryService : DirectoryService {
		#region Constructors
		internal WSDirectoryService() { }
		#endregion

		#region Methods
		public override DirectoryObject GetDirectoryObjectById(string id) {
			if(id == null) {
				throw new ArgumentNullException("id");
			}
			WebService.DirectoryObject wsDirectoryObject = this.GetWebService().GetDirectoryObjectById(id);
			return wsDirectoryObject == null ? null : ConvertWSDirectoryObject(wsDirectoryObject);
		}
		public override DirectoryObject[] GetDirectoryObjectsBySearchCriteria(SearchCriteria[] searchCriteria) {
			if(searchCriteria == null) {
				throw new ArgumentNullException("searchCriteria");
			}
			WebService.SearchCriteria[] wsSearchCriteria = new WebService.SearchCriteria[searchCriteria.Length];
			for(int i = 0; i < searchCriteria.Length; i++) {
				WebService.SearchCriteria wsSC = new WebService.SearchCriteria();
				wsSC.Name = searchCriteria[i].Name;
				wsSC.Value = searchCriteria[i].Value;
				wsSC.Required = searchCriteria[i].Required;
				wsSearchCriteria[i] = wsSC;
			}
			WebService.DirectoryObject[] wsDirectoryObjects = this.GetWebService().GetDirectoryEntriesBySearchCriteria(wsSearchCriteria);
			DirectoryObject[] directoryObjects = new DirectoryObject[wsDirectoryObjects.Length];
			for(int i = 0; i < wsDirectoryObjects.Length; i++) {
				WebService.DirectoryObject wsDirectoryObject = wsDirectoryObjects[i];
				directoryObjects[i] = ConvertWSDirectoryObject(wsDirectoryObject);
			}
			return directoryObjects;
		}
		public override void SaveDirectoryObject(DirectoryObject directoryObject) {
			if(directoryObject == null) {
				throw new ArgumentNullException("directoryObject");
			}
			this.GetWebService().SaveDirectoryObject(ConvertDirectoryObject(directoryObject));
		}
		public override DirectoryObject CreateDirectoryObjectInstance() {
			return ConvertWSDirectoryObject(this.GetWebService().CreateDirectoryObjectInstance());
		}
		#endregion

		#region Private Methods
		private static DirectoryObject ConvertWSDirectoryObject(WebService.DirectoryObject wsDirectoryObject) {
			DirectoryObject directoryObject = new DirectoryObject();
			directoryObject.Id = wsDirectoryObject.Id;
			Property[] properties = new Property[wsDirectoryObject.Properties.Length];
			for(int i = 0; i < wsDirectoryObject.Properties.Length; i++) {
				Property property = new Property();
				property.Name = wsDirectoryObject.Properties[i].Name;
				property.Value = wsDirectoryObject.Properties[i].Value;
				properties[i] = property;
			}
			directoryObject.Properties = properties;
			return directoryObject;
		}

		private static WebService.DirectoryObject ConvertDirectoryObject(DirectoryObject directoryObject) {
			WebService.DirectoryObject wsDirectoryObject = new WebService.DirectoryObject();
			wsDirectoryObject.Id = directoryObject.Id;
			WebService.Property[] wsProperties = new WebService.Property[directoryObject.Properties.Length];
			for(int i = 0; i < directoryObject.Properties.Length; i++) {
				WebService.Property wsProperty = new WebService.Property();
				wsProperty.Name = directoryObject.Properties[i].Name;
				wsProperty.Value = directoryObject.Properties[i].Value;
				wsProperties[i] = wsProperty;
			}
			wsDirectoryObject.Properties = wsProperties;
			return wsDirectoryObject;
		}

		private DirectoryServiceWebService GetWebService() {
			const string webServiceUrl = "webServiceUrl";
			const string username = "userName";
			const string password = "password";

			return new DirectoryServiceWebService(
				this.Settings.GetValue(webServiceUrl),
					new NetworkCredential(this.Settings.GetValue(username), this.Settings.GetValue(password)));
		}
		#endregion
	}
}