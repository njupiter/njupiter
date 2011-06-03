using System;
using System.Data;

namespace nJupiter.DataAccess {
	public interface ITransaction : IDisposable {
		IDbConnection Connection { get; }
		IDbTransaction DbTransaction { get; }
		IsolationLevel IsolationLevel { get; }
		void Begin();
		void Commit();
		void Rollback();
		void Dispose();
	}
}