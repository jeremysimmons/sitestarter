using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Holds the authentication state including the current user.
	/// </summary>
	public static class AuthenticationState
	{
		/// <summary>
		/// Gets the key used to store the authentication state value.
		/// </summary>
		static public string AuthenticationStateKey
		{
			get { return "Username"; }
		}
		
		/// <summary>
		/// Gets/sets the username of the current user.
		/// </summary>
		static public string Username
		{
			get { return (string)StateAccess.State.GetUser("Username"); }
			set { StateAccess.State.SetUser("Username", value); }
		}

		/// <summary>
		/// Gets/sets the user currently logged in.
		/// </summary>
		static public Entities.User User
		{
			get
			{
				if (!IsAuthenticated)
					return null;
				
				if (!StateAccess.State.ContainsRequest("User")
				    && StateAccess.State.GetRequest("User") == null
				    && Configuration.Config.IsInitialized)
				{
					User user = RetrieveStrategy.New<User>().Retrieve<User>("Username", Username);
					
					if (user == null)
						throw new Exception("No user was retrieved with the username '" + Username + "'.");
					
					StateAccess.State.SetRequest("User", user);
				}
				return (User)StateAccess.State.GetRequest("User");
			}
			set
			{
				StateAccess.State.SetRequest("User", value);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the current user is authenticated.
		/// </summary>
		static public bool IsAuthenticated
		{
			get { return Username != null && Username != String.Empty;  }
		}
		
		static public bool UserIsInRole(string roleName)
		{
			if (IsAuthenticated)
			{
				User user = AuthenticationState.User;
				
				if (user.Roles == null || user.Roles.Length == 0)
					ActivateStrategy.New<User>().Activate(user, "Roles");
				
				return user.IsInRole(roleName);
			}
			else
				return false;
		}
		
		
		/// <summary>
		/// Sets the provided username to the session state for the current user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		public static void SetAuthenticatedUsername(string username)
		{
			SetAuthenticatedUsername(username, DateTime.MinValue);
		}
		
		/// <summary>
		/// Sets the provided username to the session state for the current user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		/// <param name="expirationDate">The expiration date of the authentication data.</param>
		public static void SetAuthenticatedUsername(string username, DateTime expirationDate)
		{
			StateAccess.State.SetUser(AuthenticationStateKey, username, expirationDate);
		}
	}
}
