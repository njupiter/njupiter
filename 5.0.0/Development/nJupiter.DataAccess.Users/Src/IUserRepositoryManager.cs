namespace nJupiter.DataAccess.Users {
	public interface IUserRepositoryManager {
		/// <summary>
		/// Gets the default userRepository instance.
		/// </summary>
		/// <returns>The default userRepository instance.</returns>
		IUserRepository GetRepository();

		/// <summary>
		/// Gets the userRepository instance with the name <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The userRepository name to get.</param>
		/// <returns>The userRepository instance with the name <paramref name="name"/></returns>
		IUserRepository GetRepository(string name);
	}
}