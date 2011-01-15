using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

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
			get {
				// If the username is found in the request scope state then use it, because it means the value has just been set
				// and the user scope state won't reflect the change until next request because it uses cookies
				// Using the request scope state as well gets around this hurdle
				if (StateAccess.State.ContainsRequest("Username"))
					return (string)StateAccess.State.GetRequest("Username");
				return (string)StateAccess.State.GetUser("Username"); }
			set { StateAccess.State.SetUser("Username", value);
				// Set the request scope state as well so that the value to get around the hurdle with user scope state not reflecting
				// changes until the next request
				StateAccess.State.SetRequest("Username", value);
			}
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
					
					// TODO: Check if needed. Without it there's a risk of a NullReferenceException
					//if (user == null)
					//	throw new Exception("No user was retrieved with the username '" + Username + "'.");
					
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
			get { return Username != null && Username != String.Empty; }
		}
		
		/// <summary>
		/// Checks to see whether the current user is in the specified role.
		/// </summary>
		/// <param name="roleName">The name of the role to check if the current user is in.</param>
		/// <returns>A flag indicating whether the current user is in the specified role.</returns>
		static public bool UserIsInRole(string roleName)
		{
			bool isInRole = false;
			
			using (LogGroup logGroup = LogGroup.Start("Checking to see whether the current user is in the specified role.", NLog.LogLevel.Debug))
			{
				
				AppLogger.Debug("User is authenticated: " + IsAuthenticated.ToString());
				
				if (IsAuthenticated)
				{
					
					User user = AuthenticationState.User;
					
					if (user != null)
					{
						AppLogger.Debug("Username: " + user.Username);
						
						if (user.Roles == null || user.Roles.Length == 0)
							ActivateStrategy.New<User>().Activate(user, "Roles");
						
						isInRole = user.IsInRole(roleName);
					}
				}
				
				AppLogger.Debug("Is in role: " + isInRole);
			}
			
			return isInRole;
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
			using (LogGroup logGroup = LogGroup.Start("Setting the current user's authenticated username.", NLog.LogLevel.Debug))
			{	
				if (StateAccess.State == null)
					throw new InvalidOperationException("The StateAccess.State provider has not been initialized. Use the WebStateInitializer.Initialize() function.");
				
				AppLogger.Debug("Username: " + username);
				
				StateAccess.State.SetUser(AuthenticationStateKey, username, expirationDate);
				// Set the username in the request scope as well to get around hurdle with user scope values not being reflected until the next request
				// due to the use of cookies
				StateAccess.State.SetRequest(AuthenticationStateKey, username);
			}
		}
	}
}
