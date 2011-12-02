using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Security
{
	/// <summary>
	/// Used to authenticate users.
	/// </summary>
	public static class Authentication
	{
		/// <summary>
		/// Ensures that the current user is authenticated, and redirects them to the sign in page if they aren't.
		/// </summary>
		public static void EnsureIsAuthenticated()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Ensuring that the current user is authenticated."))
			{
				if (!IsAuthenticated)
				{
					LogWriter.Debug("Is authenticated.");
					
					Authorisation.InvalidPermissionsRedirect();
				}
				else
					LogWriter.Debug("Is NOT authenticated.");
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the current user is authenticated.
		/// </summary>
		public static bool IsAuthenticated
		{
			get { return AuthenticationState.IsAuthenticated; }
		}
		
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
			bool isAuthenticated = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Attempting to sign in user '" + username + "'."))
			{
				isAuthenticated = SignIn(username, password, false);
				
				LogWriter.Debug("Is authenticated: " + isAuthenticated);
			}
			
			return isAuthenticated;
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
			bool isAuthenticated = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Attempting to sign in user '" + username + "'."))
			{
				LogWriter.Debug("Persist authentication: " + persistAuthentication);
				
				if(Authenticate(username, password))
				{
					LogWriter.Debug("Authenticated. Setting authenticated username.");
					
					SetAuthenticatedUsername(username, persistAuthentication);
					
					isAuthenticated = true;
				}
				
				LogWriter.Debug("Is authenticated: " + isAuthenticated);
			}
			
			return isAuthenticated;
		}
		
		/// <summary>
		/// Authenticates and the user with the provided username and password and signs them in, then redirects them back to the URL they came from.
		/// </summary>
		/// <param name="username">The username of the user to authenticate and sign in.</param>
		/// <param name="password">The unencrypted password of the user to authenticate and sign in.</param>
		/// <returns>A value indicating whether the user's credentials are authentic.</returns>
		public static bool SignInAndRedirect(string username, string password)
		{
			bool isAuthenticated = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Attempting to sign in user '" + username + "' and redirect to previous page."))
			{
				if (SignIn(username, password))
				{
					LogWriter.Debug("Signed in.");
					
					ReturnUser();
					isAuthenticated = true;
				}
				
				LogWriter.Debug("Is authenticated: " + isAuthenticated);
			}
			
			return isAuthenticated;
		}
		
		
		/// <summary>
		/// Authenticates and the user with the provided username and password and signs them in, then redirects them back to the URL they came from.
		/// </summary>
		/// <param name="username">The username of the user to authenticate and sign in.</param>
		/// <param name="password">The unencrypted password of the user to authenticate and sign in.</param>
		/// <param name="persistAuthentication">A value that indicates whether to persist the authentication (ie. remember the user).</param>
		/// <returns>A value indicating whether the user's credentials are authentic.</returns>
		public static bool SignInAndRedirect(string username, string password, bool persistAuthentication)
		{
			bool isAuthenticated = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Attempting to sign in user '" + username + "' and redirect to previous page."))
			{
				LogWriter.Debug("Persist authentication: " + persistAuthentication);
				
				if (SignIn(username, password, persistAuthentication))
				{
					LogWriter.Debug("Signed in.");
					
					ReturnUser();
					
					isAuthenticated = true;
				}
				
				LogWriter.Debug("Is authenticated: " + isAuthenticated);
			}
			
			return isAuthenticated;
		}
		
		/// <summary>
		/// Signs the current user out.
		/// </summary>
		public static void SignOut()
		{
			if (Authentication.IsAuthenticated)
				SetAuthenticatedUsername(String.Empty);
		}
		
		/// <summary>
		/// Sets the provided username to the session state for the current user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		public static void SetAuthenticatedUsername(string username)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Setting the authenticated username '" + username + "'."))
			{
				SetAuthenticatedUsername(username, false);
			}
		}
		
		/// <summary>
		/// Sets the provided username to the session state for the current user.
		/// </summary>
		/// <param name="username">The username of the current user.</param>
		public static void SetAuthenticatedUsername(string username, bool persistAuthentication)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Setting the authenticated username '" + username + "'."))
			{
				DateTime expirationDate = DateTime.Now;
				
				if (persistAuthentication)
				{
					int durationDays = GetPersistDurationDays();
				
					LogWriter.Debug("Persist duration (days): " + durationDays);
				
					expirationDate = DateTime.Now.AddDays(durationDays);
				}
				else
				{
					int durationHours = GetStandardDurationHours();
					
					LogWriter.Debug("Login duration (hours): " + durationHours);
					
					expirationDate = DateTime.Now.AddHours(durationHours);
				}
				
				LogWriter.Debug("Expiration date: " + expirationDate.ToString());
				
				AuthenticationState.SetAuthenticatedUsername(username, expirationDate);
				
				if (username != String.Empty)
				{
					LogWriter.Debug("Setting auth cookie via FormsAuthentication.SetAuthCookie(...)");
					
					// Set the auth cookie in standard forms authentication to ensure dependent features work
					System.Web.Security.FormsAuthentication.SetAuthCookie(username, persistAuthentication);
					
					LogWriter.Debug("HttpContext.Current.User.Identity.Name: " + HttpContext.Current.User.Identity.Name);
				}
				else
				{
					LogWriter.Debug("Removing auth cookie via FormsAuthentication.SignOut()");
					
					System.Web.Security.FormsAuthentication.SignOut();
				}
			}
		}
		
		public static void ReturnUser()
		{
			string returnUrl = GetReturnUrl();
			
			if (returnUrl != null && returnUrl.Trim() != String.Empty)
				HttpContext.Current.Response.Redirect(returnUrl);
		}
		
		public static string GetReturnUrl()
		{
			string returnUrl = (string)HttpContext.Current.Session["ReturnUrl"];
			
			return returnUrl;
		}
		
		public static string GetUrl()
		{
			string url = Navigation.Navigator.Current.GetCurrentLink();
			
			url = WebUtilities.ConvertAbsoluteUrlToRelativeUrl(url, "/");
			
			return url;
		}
		
		public static int GetPersistDurationDays()
		{
			int duration = 1;
			
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
			int duration = 1;
			
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
