using System;

namespace nJupiter.DataAccess.Users {
	public interface ILockable : ICloneable {
		void MakeReadOnly();
		bool IsReadOnly { get; }

	}
}