using System.Collections.Generic;

namespace nJupiter.DataAccess.Users {
	public interface IPropertyHandler : IPredefinedProperties, ILockable<IPropertyHandler> {
		IProperty this[string propertyName] { get; }
		IProperty this[string propertyName, IContext context] { get; }
		IPredefinedNames PropertyNames { get; }
		IEnumerable<IContext> AttachedContexts { get; }
		IPropertyCollection GetProperties();
		IPropertyCollection GetProperties(IContext context);
		void AttachProperties(IPropertyCollection properties);
		T GetValue<T>(string propertyName);
		T GetValue<T>(string propertyName, IContext context);
		T GetValue<T>(string propertyName, string contextName);
		IProperty GetProperty(string propertyName);
		IProperty GetProperty(string propertyName, string contextName);
		IProperty GetProperty(string propertyName, IContext context);
		void SetProperty(string propertyName, object value);
		void SetProperty(string propertyName, string contextName, object value);
		void SetProperty(string propertyName, IContext context, object value);
	}
}