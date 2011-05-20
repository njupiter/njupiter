
namespace nJupiter.DataAccess.Users {
	public interface IUser {
		string Id { get; }
		string UserName { get; }
		string Domain { get; }
		Properties Properties { get; }
	}
}