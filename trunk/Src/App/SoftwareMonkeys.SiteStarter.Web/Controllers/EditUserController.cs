using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Navigation;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Controller("Edit", "User")]
	public class EditUserController : EditController
	{
		public EditUserController()
		{
		}
		
		public override bool Update(IEntity entity)
		{
			if (entity is User)
			{
				return Update((User)entity);
			}
			else
				throw new ArgumentException("The provided entity type '" + entity.GetType().FullName + "' is not supported. The entity must be of type 'User'.");
		}
		
		/// <summary>
		/// Updates the user from the form.
		/// </summary>
		public bool Update(User user)
		{
			AutoNavigate = false;
			
			// Get the original user data
			User originalUser = RetrieveStrategy.New<User>().Retrieve<User>("ID", user.ID);
			
			// If the password wasn't added then reset it
			if (user.Password == null || user.Password == String.Empty)
				user.Password = originalUser.Password;
			else
				user.Password = Crypter.EncryptPassword(user.Password);
			
			bool success = base.Update(user);
			
			// If the current user edited their username then fix their authentication session
			if (originalUser.Username == AuthenticationState.Username
			    && user.Username != AuthenticationState.Username)
				AuthenticationState.Username = user.Username;
			
			if (success)
				NavigateAfterUpdate();
			
			return success;
		}
		
		public override void NavigateAfterUpdate()
		{
			Navigator.Current.Go("Index", "User");
		}
	}
}
