namespace nJupiter.DataAccess.Users {
	public interface ILockable<out T> : ILockable {
		T CreateWritable();
	}
}