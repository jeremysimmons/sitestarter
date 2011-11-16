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
				string username = String.Empty;
				
				// If the username is found in the request scope state then use it, because it means the value has just been set
				// and the user scope state won't reflect the change until next request because it uses cookies
				// Using the request scope state as well gets around this hurdle
				if (StateAccess.State.ContainsOperation("Username"))
					username = (string)StateAccess.State.GetOperation("Username");
				else
					username = (string)StateAccess.State.GetUser("Username");
				
				return username;
			}
			set { StateAccess.State.SetUser("Username", value);
				// Set the request scope state as well so that the value to get around the hurdle with user scope state not reflecting
				// changes until the next request
				StateAccess.State.SetOperation("Username", value);
				
				// If the username is different from the one on the user object then reset the user object
				if (User != null && User.Username != value)
					User = null;
			}
		}

		/// <summary>
		/// Gets/sets the user currently logged in.
		/// </summary>
		static public Entities.User User
		{
			get
			{
				if (!StateAccess.State.ContainsOperation("User")
				    && StateAccess.State.GetOperation("User") == null
				    && Configuration.Config.IsInitialized)
				{
					User u = RetrieveStrategy.New<User>().Retrieve<User>("Username", Username);
					
					StateAccess.State.SetOperation("User", u);
				}
				
				User user = (User)StateAccess.State.GetOperation("User");
				
				// If no corresponding user exists then sign the user out, as it means they're signed in on an old session, likely due
				// to a recompile
				if (user == null && Username != String.Empty)
					Username = String.Empty;
				
				return user;
			}
			set
			{
				StateAccess.State.SetOperation("User", value);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the current user is authenticated.
		/// </summary>
		static public bool IsAuthenticated
		{
			get
			{
				return Username != null
					&& Username != String.Empty;
			}
		}
		
		/// <summary>
		/// Checks to see whether the current user is in the specified role.
		/// </summary>
		/// <param name="roleName">The name of the role to check if the current user is in.</param>
		/// <returns>A flag indicating whether the current user is in the specified role.</returns>
		static public bool UserIsInRole(string roleName)
		{
			bool isInRole = false;
			
			using (LogGroup logGroup = LogGroup.Start("Checking to see whether the current user is in the '" + roleName + "' role."))
			{
				
				LogWriter.Debug("User is authenticated: " + IsAuthenticated.ToString());
				
				if (IsAuthenticated)
				{
					
					User user = AuthenticationState.User;
					
					if (user != null)
					{
						LogWriter.Debug("Username: " + user.Username);
						
						if (user.Roles == null || user.Roles.Length == 0)
							ActivateStrategy.New<User>().Activate(user, "Roles");
						
						isInRole = user.IsInRole(roleName);
					}
				}
				
				LogWriter.Debug("Is in role: " + isInRole);
			}
			
			return isInRole;
		}
		
		
		/// <summary>
		/// Sets the provided username to the session state for the current user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		public static void SetAuthenticatedUsername(string username)
		{
			SetAuthenticatedUsername(username, DateTime.Now.AddHours(1));
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
				
				LogWriter.Debug("Username: " + username);
				
				StateAccess.State.SetUser(AuthenticationStateKey, username, expirationDate);
				// Set the username in the operation scope as well to get around hurdle with user scope values not being reflected until the next request
				// due to the use of cookies
				StateAccess.State.SetOperation(AuthenticationStateKey, username);
				
				LogWriter.Debug("AuthenticationState.Username: " + Username);
				LogWriter.Debug("AuthenticationState.IsAuthenticated: " + IsAuthenticated);
			}
		}
	}
}
