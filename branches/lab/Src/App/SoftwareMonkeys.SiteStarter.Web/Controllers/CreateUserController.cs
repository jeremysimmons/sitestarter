﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Navigation;
using SoftwareMonkeys.SiteStarter.Web.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Controller("Create", "User")]
	public class CreateUserController : CreateController
	{
		/// <summary>
		/// Gets a value indicating whether the current user object belongs to the current user.
		/// </summary>
		public bool IsSelf
		{
			get { return ((User)DataSource).ID == AuthenticationState.User.ID; }
		}
		
		public CreateUserController()
		{
		}
		
		public override bool ExecuteSave(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			if (entity is User)
			{
				return ExecuteSave((User)entity);
			}
			else
				throw new ArgumentException("The provided entity type '" + entity.GetType().FullName + "' is not supported. The entity must be of type 'User'.");
		}
		
		public bool ExecuteSave(User user)
		{
			bool success = false;
			
			using (LogGroup logGroup = LogGroup.Start("Saving the new user.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Auto navigate: " + AutoNavigate.ToString());
				
				user.Password = Crypter.EncryptPassword(user.Password);
				
				// Cancel the base automatic navigation
				AutoNavigate = false;
				
				success = base.ExecuteSave(user);
				
				// Now the AutoNavigate flag for the custom function depends on the AutoApproveNewUsers setting
				AutoNavigate = Config.Application.Settings.GetBool("AutoApproveNewUsers");
				
				LogWriter.Debug("Success: " + success.ToString());
				
				if (success)
				{
					LogWriter.Debug("Is approved: " + user.IsApproved.ToString());
					LogWriter.Debug("User is already signed in: " + AuthenticationState.IsAuthenticated.ToString());
					
					// If it's a new user registering and the new user was automatically approved (during create strategy, from configuration settings)
					// then sign the user in automatically
					if (!AuthenticationState.IsAuthenticated && user.IsApproved)
					{
						LogWriter.Debug("Signing the new user in.");
						
						Authentication.SetAuthenticatedUsername(user.Username);
						
						// Navigate only if automatically logged in
						NavigateAfterSave();
					}
					
				}
				else
				{
					LogWriter.Debug("Failed to save (username in use).");
				}
			}
			
			return success;
		}

		public override void NavigateAfterSave()
		{
			// If the user being saved was the logged in user then send them to their account
			if (IsSelf)
				Navigator.Current.Go("Account", "User");
			// Otherwise send them to the users index to see the new user in the list
			else
				Navigator.Current.Go("Index", "User");
			// TODO: If a new user was created by an administrator, and not registering their self, then they should be redirected back to the index after saving a new user
			//Navigator.Current.Go("Index", "User");
		}
	}
}
