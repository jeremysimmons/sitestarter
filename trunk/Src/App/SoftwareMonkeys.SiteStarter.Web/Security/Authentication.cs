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
			return SignIn(username, password, false);
		}
		
		
		/// <summary>
		/// Authenticates and the user with the provided username and password and signs them in.
		/// </summary>
		/// <param name="username">The username of the user to authenticate and sign in.</param>
		/// <param name="password">The unencrypted password of the user to authenticate and sign in.</param>
		/// <param name="persistAuthentication">A value that indicates whether to persist the authentication (ie. remember the user).</param>
		/// <returns>A value indicating whether the user's credentials are authentic.</returns>
		public static bool SignIn(string username, string password, bool persistAuthentication)
		{
			if(Authenticate(username, password))
			{
				SetAuthenticatedUsername(username, persistAuthentication);
				
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
			SetAuthenticatedUsername(username, false);
		}
		
		/// <summary>
		/// Sets the provided username to the session state for the current user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		public static void SetAuthenticatedUsername(string username, bool persistAuthentication)
		{
			DateTime expirationDate = DateTime.Now.AddHours(GetStandardDurationHours());
			
			if (persistAuthentication)
				expirationDate = DateTime.Now.AddDays(GetPersistDurationDays());
			
			AuthenticationState.SetAuthenticatedUsername(username, expirationDate);
			
			if (username != String.Empty)
			{
				// Set the auth cookie in standard forms authentication to ensure dependent features work
				System.Web.Security.FormsAuthentication.SetAuthCookie(username, persistAuthentication);
			}
			else
				System.Web.Security.FormsAuthentication.SignOut();
			
		}
		
		public static int GetPersistDurationDays()
		{
			int duration = 0;
			
			try
			{
				duration = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["Authentication.PersistDuration.Days"]);
			}
			catch
			{
				throw new Exception("Authentication.PersistDuration.Days configuration setting is invalid.");
			}
			
			return duration;
		}
		
		public static int GetStandardDurationHours()
		{
			int duration = 0;
			
			try
			{
				duration = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["Authentication.StandardDuration.Hours"]);
			}
			catch
			{
				throw new Exception("Authentication.StandardDuration.Hours configuration setting is invalid.");
			}
			
			return duration;
		}
	}
}
