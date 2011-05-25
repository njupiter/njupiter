
namespace nJupiter.DataAccess.Users {
	public interface IUser : ILockable {
		string Id { get; }
		string UserName { get; }
		string Domain { get; }
		PropertyHandler Properties { get; }
	}
}