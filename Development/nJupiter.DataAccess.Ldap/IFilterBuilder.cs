using System.Collections.Generic;

namespace nJupiter.DataAccess.Ldap {
	internal interface IFilterBuilder {
		string CreatePropertyRangeFilter(string propertyName, uint rangeLow, uint rangeHigh, bool isLastQuery);
		string CreatePropertyRangeFilter(string propertyName, uint startIndex, uint endIndex);
		string AttachAttributeFilters(string nameToMatch, string defaultFilter, string rdnAttribute, IEnumerable<IAttributeDefinition> attributeDefinitinon);
		string AttachFilter(string attributeToMatch, string valueToMatch, string defaultFilter);
		string AttachRdnFilter(string valueToMatch, string defaultFilter);
	}
}