using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to authenticate a user.
	/// </summary>
	[Strategy("Authenticate", "User")]
	public class AuthenticateStrategy : BaseStrategy, IAuthenticateStrategy
	{
		
		/// <summary>
		/// Authenticates the user with the provided username and password.
		/// </summary>
		/// <param name="username">The username of the user to authenticate.</param>
		/// <param name="password">The unencrypted password of the user to authenticate.</param>
		/// <returns>A value indicating whether the user's credentials are authentic.</returns>
		public bool Authenticate(string username, string password)
		{
			string encryptedPassword = Crypter.EncryptPassword(password);
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("Username", username);
			parameters.Add("Password", encryptedPassword);
			
			IRetrieveStrategy strategy = RetrieveStrategy.New<User>(false);
			
			User user = strategy.Retrieve<User>(parameters);
			
			bool isAuthenticated = (user != null)
				&& user.IsApproved
				&& !user.IsLockedOut;
			
			if (isAuthenticated)
			{
				user.LastLoginDate = DateTime.Now;
				UpdateStrategy.New(user, false).Update(user);
			}
			
			return isAuthenticated;
		}
		
		#region New functions	
		/// <summary>
		/// Creates a new strategy for authenticating a user.
		/// </summary>
		static public IAuthenticateStrategy New()
		{
			return StrategyState.Strategies.Creator.New<IAuthenticateStrategy>("Authenticate", typeof(User).Name);
		}
		#endregion
	}
}
