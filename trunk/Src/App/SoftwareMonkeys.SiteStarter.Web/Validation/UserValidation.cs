using System;

namespace SoftwareMonkeys.SiteStarter.Web.Validation
{
	/// <summary>
	/// Provides the validation messages related to users.
	/// </summary>
	public class UserValidation : ValidationFacade
	{
		public UserValidation()
		{
			AddError("Username", "Unique", "UsernameTaken");
			AddError("Username", "Required", "UsernameRequired");
			AddError("FirstName", "Required", "FirstNameRequired");
			AddError("LastName", "Required", "LastNameRequired");
			AddError("Email", "Required", "EmailRequired");
			AddError("Email", "Unique", "EmailTaken");
			AddError("Email", "Email", "InvalidEmail");
			AddError("Password", "Required", "PasswordRequired");
		}
	}
}
