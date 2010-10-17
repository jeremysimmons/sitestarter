using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
	/// <summary>
	/// Used to authenticate users.
	/// </summary>
	public static class Authentication
	{
		/// <summary>
		/// Authenticates the user with the provided username and password.
		/// </summary>
		/// <param name="username">The username of the user to authenticate.</param>
		/// <param name="password">The unencrypted password of the user to authenticate.</param>
		/// <returns>A value indicating whether the user's credentials are authentic.</returns>
		public static bool Authenticate(string username, string password)
		{
			return AuthenticateStrategy.New().Authenticate(username, password);
		}
		
		
		/// <summary>
		/// Authenticates and the user with the provided username and password and signs them in.
		/// </summary>
		/// <param name="username">The username of the user to authenticate and sign in.</param>
		/// <param name="password">The unencrypted password of the user to authenticate and sign in.</param>
		/// <returns>A value indicating whether the user's credentials are authentic.</returns>
		public static bool SignIn(string username, string password)
		{
			if(Authenticate(username, password))
			{
				SetAuthenticatedUsername(username);
				
				return true;
			}
			else
				return false;
		}
		
		/// <summary>
		/// Signs the current user out.
		/// </summary>
		public static void SignOut()
		{
			SetAuthenticatedUsername(String.Empty);
		}
		
		/// <summary>
		/// Sets the provided username to the session state for the current user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		public static void SetAuthenticatedUsername(string username)
		{
			AuthenticationState.Username = username;
		}
	}
}
