using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Web.Navigation;
using SoftwareMonkeys.SiteStarter.Web.Validation;
using SoftwareMonkeys.SiteStarter.Web.WebControls;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[Controller("Edit", "User")]
	public class EditUserController : EditController
	{
		/// <summary>
		/// Gets a value indicating whether the current user object belongs to the current user.
		/// </summary>
		public bool IsSelf
		{
			get { return ((User)DataSource).ID == AuthenticationState.User.ID; }
		}
		
		public EditUserController()
		{
			Validation = new UserValidation();
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
			
			Result.Display(IsSelf
			               ? Properties.Language.YourAccountUpdated
			               : Properties.Language.UserUpdated);
			
			if (success)
				NavigateAfterUpdate();
			
			return success;
		}
		
		public override void NavigateAfterUpdate()
		{
			// If the user being saved was the logged in user then send them to their account
			if (IsSelf)
				Navigator.Current.Go("Details", "User");
			// Otherwise send them to the users index
			else
				Navigator.Current.Go("Index", "User");
		}
		
		public override Guid GetID()
		{
			Guid id = base.GetID();
			string uniqueKey = base.GetUniqueKey();
			
			// If no ID was specified in the URL then edit the currently logged in user
			if (id == Guid.Empty && uniqueKey == String.Empty
				&& AuthenticationState.IsAuthenticated
				&& AuthenticationState.User != null)
				id = AuthenticationState.User.ID;
			
			return id;
		}
	}
}
