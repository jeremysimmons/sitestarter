using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all authentication strategies.
	/// </summary>
	public interface IAuthenticateStrategy : IStrategy
	{
		/// <summary>
		/// Authenticates the user with the provided username and password.
		/// </summary>
		/// <param name="username">The username of the user to authenticate.</param>
		/// <param name="password">The unencrypted password of the user to authenticate.</param>
		/// <returns>A value indicating whether the user's credentials are authentic.</returns>
		bool Authenticate(string username, string password);
	}
}
