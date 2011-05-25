using System.Collections.Generic;

namespace nJupiter.DataAccess.Users {
	public interface IPropertyCollection : IEnumerable<IProperty>, ILockable {
		ContextSchema Schema { get; }
		int Count { get; }
	}
}