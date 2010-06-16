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
using System.Text;
using System.DirectoryServices;
using System.Collections;
using System.Globalization;

using log4net;

namespace nJupiter.DataAccess.DirectoryService.Adapters {

	public sealed class LdapDirectoryService : DirectoryService {
		#region Constants
		private const string BaseFilter = "(objectClass=*)";
		private const string FilterFormat = "({0}={1})";
		private const string FilterAndFormat = "(&{0})";
		private const string FilterOrFormat = "(|{0})";
		#endregion

		#region Constructors
		internal LdapDirectoryService() { }
		#endregion

		#region Static Members
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		#endregion

		#region Members
		private string[] propertyNames;

		private string[] PropertyNames {
			get {
				return this.propertyNames ??
				       (this.propertyNames = this.Settings.GetValueArray("directoryObjectProperties", "property"));
			}
		}
		private string identifyingPropertyName;

		private string IdentifyingPropertyName {
			get {
				return this.identifyingPropertyName ??
				       (this.identifyingPropertyName =
				        this.Settings.GetValue("directoryObjectProperties", "property[@identity='true']"));
			}
		}
		#endregion

		#region Methods
		public override DirectoryObject GetDirectoryObjectById(string id) {
			if(id == null) {
				throw new ArgumentNullException("id");
			}
			if(Log.IsDebugEnabled) { Log.Debug("Getting Directory Object By Id [" + id + "]"); }
			DirectoryObject directoryObject = null;
			if(id.Length > 0) {
				using(DirectoryEntry directoryEntry = this.GetDirectoryEntry()) {
					using(DirectorySearcher directorySearcher = new DirectorySearcher(
							  directoryEntry,
							  string.Format(CultureInfo.InvariantCulture, FilterAndFormat, BaseFilter + string.Format(CultureInfo.InvariantCulture, FilterFormat, this.IdentifyingPropertyName, id)),
							  PropertyNames,
							  SearchScope.Subtree)) {
						SearchResult searchResult = directorySearcher.FindOne();
						if(searchResult != null) {
							directoryObject = this.CreateDirectoryObjectInstance();
							directoryObject.Id = GetPropertyValue(searchResult, this.IdentifyingPropertyName);
							foreach(string propertyName in searchResult.Properties.PropertyNames) {
								if(directoryObject.Contains(propertyName)) {
									if(Log.IsDebugEnabled) { Log.Debug(string.Format("Directory Object [{0}] contains property name [{1}]", id, propertyName)); }
									directoryObject[propertyName] = GetPropertyValue(searchResult, propertyName);
								}
							}
						} else {
							if(Log.IsDebugEnabled) { Log.Debug(string.Format("Directory Object [{0}] not found.", id)); }
						}
					}
				}
			}
			return directoryObject;
		}
		public override DirectoryObject[] GetDirectoryObjectsBySearchCriteria(SearchCriteria[] searchCriteria) {
			if(searchCriteria == null) {
				throw new ArgumentNullException("searchCriteria");
			}
			using(DirectoryEntry directoryEntry = GetDirectoryEntry()) {

				SearchCriteria[] requiredSearchCriteria = Array.FindAll(searchCriteria, searchCriterion => searchCriterion.Required);
				SearchCriteria[] notRequiredSearchCriteria = Array.FindAll(searchCriteria, searchCriterion => !searchCriterion.Required);

				StringBuilder requiredFilter = new StringBuilder();
				foreach(SearchCriteria sc in requiredSearchCriteria) {
					requiredFilter.AppendFormat(CultureInfo.InvariantCulture, FilterFormat, sc.Name, sc.Value);
				}
				StringBuilder notRequiredFilter = new StringBuilder();
				foreach(SearchCriteria sc in notRequiredSearchCriteria) {
					notRequiredFilter.AppendFormat(CultureInfo.InvariantCulture, FilterFormat, sc.Name, sc.Value);
				}

				string filter = string.Format(CultureInfo.InvariantCulture, FilterAndFormat, BaseFilter + requiredFilter + (notRequiredFilter.Length > 0 ? string.Format(CultureInfo.InvariantCulture, FilterOrFormat, notRequiredFilter) : string.Empty));
				using(DirectorySearcher directorySearcher = new DirectorySearcher(
						  directoryEntry,
						  filter,
						  PropertyNames,
						  SearchScope.Subtree)) {
					using(SearchResultCollection searchResultCollection = directorySearcher.FindAll()) {
						ArrayList directoryObjects = new ArrayList();

						foreach(SearchResult searchResult in searchResultCollection) {
							if(searchResult != null && searchResult.Properties != null && searchResult.Properties[this.IdentifyingPropertyName] != null) {
								DirectoryObject directoryObject = this.CreateDirectoryObjectInstance();
								directoryObject.Id = GetPropertyValue(searchResult, this.IdentifyingPropertyName);
								foreach(string propertyName in searchResult.Properties.PropertyNames) {
									if(directoryObject.Contains(propertyName) && searchResult.Properties[propertyName] != null) {
										directoryObject[propertyName] = GetPropertyValue(searchResult, propertyName);
									}
								}
								directoryObjects.Add(directoryObject);
							}
						}
						DirectoryObject[] result = (DirectoryObject[])directoryObjects.ToArray(typeof(DirectoryObject));
						if(Log.IsDebugEnabled) { Log.Debug(string.Format("Getting directory objects by search criteria [{0}]. Objects found {1}", filter, result.Length)); }
						return result;
					}
				}
			}
		}

		public override void SaveDirectoryObject(DirectoryObject directoryObject) {
			if(directoryObject == null) {
				throw new ArgumentNullException("directoryObject");
			}
			using(DirectoryEntry directoryEntry = GetDirectoryEntry()) {
				string filter = string.Format(CultureInfo.InvariantCulture, FilterAndFormat, BaseFilter + string.Format(CultureInfo.InvariantCulture, FilterFormat, this.IdentifyingPropertyName, directoryObject.Id));
				using(DirectorySearcher directorySearcher = new DirectorySearcher(
						  directoryEntry,
						  filter,
						  PropertyNames,
						  SearchScope.Subtree)) {
					using(DirectoryEntry directoryEntryWrite = directorySearcher.FindOne().GetDirectoryEntry()) {
						bool commitChanges = false;
						foreach(string propertyName in this.PropertyNames) {
							PropertyValueCollection pvc = directoryEntryWrite.Properties[propertyName];
							string newValue = directoryObject[propertyName];
							if(pvc.Value != null) {
								if(!pvc.Value.ToString().Equals(newValue)) {
									if(!string.IsNullOrEmpty(newValue)) {
										if(Log.IsDebugEnabled) { Log.Debug(string.Format("Setting property [{0}] with value [{1}] on directory objects with filter [{2}].", propertyName, newValue, filter)); }
										pvc.Value = newValue;
									} else {
										if(Log.IsDebugEnabled) { Log.Debug(string.Format("Remove value for property [{0}] on directory objects with filter [{1}].", propertyName, filter)); }
										pvc.RemoveAt(0);
									}
									commitChanges = true;
								}
							} else if(!string.IsNullOrEmpty(newValue)) {
								if(Log.IsDebugEnabled) { Log.Debug(string.Format("Add property [{0}] with value [{1}] on directory objects with filter [{2}].", propertyName, newValue, filter)); }
								pvc.Add(newValue);
								commitChanges = true;
							}
						}
						if(commitChanges) {
							directoryEntryWrite.CommitChanges();
							if(Log.IsDebugEnabled) { Log.Debug(string.Format("Commiting changes directory objects with filter [{0}].", filter)); }
						}
					}
				}
			}
		}

		public override DirectoryObject CreateDirectoryObjectInstance() {
			DirectoryObject propertiedObject = new DirectoryObject();
			Property[] properties = new Property[PropertyNames.Length];
			for(int i = 0; i < PropertyNames.Length; i++) {
				properties[i] = new Property(PropertyNames[i], null);
			}
			propertiedObject.Properties = properties;
			return propertiedObject;
		}
		#endregion

		#region Private Methods
		private DirectoryEntry GetDirectoryEntry() {
			const string ldapUrlScheme = "LDAP://{0}:{1}/{2}";
			const string host = "host";
			const string port = "port";
			const string ldapBase = "base";
			const string username = "userName";
			const string password = "password";

			// If username and password is not supplied, run as executing user
			if(string.IsNullOrEmpty(this.Settings.GetValue(username)) && string.IsNullOrEmpty(this.Settings.GetValue(password))) {
				return new DirectoryEntry(string.Format(CultureInfo.InvariantCulture, ldapUrlScheme, Settings.GetValue(host), Settings.GetValue(port), Settings.GetValue(ldapBase)));
			}
			return new DirectoryEntry(
				string.Format(CultureInfo.InvariantCulture, ldapUrlScheme, Settings.GetValue(host), Settings.GetValue(port), Settings.GetValue(ldapBase)),
				this.Settings.GetValue(username), this.Settings.GetValue(password), AuthenticationTypes.ServerBind);
		}
		private static string GetPropertyValue(SearchResult searchResult, string propertyName) {
			const string octetStringDivider = "\\";
			byte[] octetStringResult = searchResult.Properties[propertyName][0] as byte[];
			if(octetStringResult != null) {
				return octetStringDivider + BitConverter.ToString(octetStringResult).Replace("-", octetStringDivider);
			}
			return searchResult.Properties[propertyName][0].ToString();
		}
		#endregion
	}
}