using System;
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
	[Controller("Register", "User")]
	public class RegisterUserController : CreateUserController
	{
		public RegisterUserController()
		{
		}
		
		public override bool ExecuteSave(User user)
		{
			bool success = false;
			
			using (LogGroup logGroup = LogGroup.Start("Saving the new user.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Auto navigate: " + AutoNavigate.ToString());
				
				// Cancel the base automatic navigation
				AutoNavigate = false;
				
				success = base.ExecuteSave(user); // Base function should take care of encrypting password etc
				
				LogWriter.Debug("Success: " + success.ToString());
				
				if (success)
				{
					LogWriter.Debug("Is approved: " + user.IsApproved.ToString());
					
					LogWriter.Debug("Signing the new user in.");
					
					// Only sign the user in and send them to their account page if the user was automatically approvied (based on settings)
					if (user.IsApproved)
					{
						Authentication.SetAuthenticatedUsername(user.Username);
						
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
			if (((User)DataSource).IsApproved)
				Navigator.Current.Go("Account", "User");
		}
		
		public override bool AuthoriseStrategies()
		{
			return Security.Authorisation.UserCan("Register", TypeName);
		}
		
		public override bool AuthoriseStrategies(IEntity entity)
		{
			return Security.Authorisation.UserCan("Create", entity);
		}
	}
}
