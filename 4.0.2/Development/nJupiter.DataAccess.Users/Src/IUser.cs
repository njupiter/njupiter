
namespace nJupiter.DataAccess.Users {
	public interface IUser : ILockable<IUser> {
		string Id { get; }
		string UserName { get; }
		string Domain { get; }
		IPropertyHandler Properties { get; }
	}
}