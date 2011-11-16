using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Navigation;
using SoftwareMonkeys.SiteStarter.Web.Properties;
using SoftwareMonkeys.SiteStarter.Web.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web.Validation;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Controller("Create", "User")]
	public class CreateUserController : CreateController
	{
		public CreateUserController()
		{
			Validation = new UserValidation();
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
		
		public virtual bool ExecuteSave(User user)
		{
			bool success = false;
			
			using (LogGroup logGroup = LogGroup.Start("Saving the new user.", NLog.LogLevel.Debug))
			{
				// TODO: Clean up
				
				LogWriter.Debug("Auto navigate: " + AutoNavigate.ToString());
				
				user.Password = Crypter.EncryptPassword(user.Password);
				
				success = base.ExecuteSave(user);
				
				LogWriter.Debug("Success: " + success.ToString());
				
				if (!success)
				{
					LogWriter.Debug("Failed to save (username in use).");
				}
			}
			
			return success;
		}
		
		public override void NavigateAfterSave()
		{	
				Navigator.Current.Go("Index", "User");
		}
	}
}
