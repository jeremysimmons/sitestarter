using System;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Web.Validation
{
	/// <summary>
	/// Provides the validation messages related to users.
	/// </summary>
	public class UserValidation : ValidationFacade
	{
		public UserValidation()
		{
			AddError("Username", "Unique", DynamicLanguage.GetText(GetType(), "UsernameTaken"));
			AddError("Username", "Required", DynamicLanguage.GetText(GetType(), "UsernameRequired"));
			AddError("FirstName", "Required", DynamicLanguage.GetText(GetType(), "FirstNameRequired"));
			AddError("LastName", "Required", DynamicLanguage.GetText(GetType(), "LastNameRequired"));
			AddError("Email", "Required", DynamicLanguage.GetText(GetType(), "EmailRequired"));
			AddError("Email", "Unique", DynamicLanguage.GetText(GetType(), "EmailTaken"));
			AddError("Email", "Email", DynamicLanguage.GetText(GetType(), "InvalidEmail"));
			AddError("Password", "Required", DynamicLanguage.GetText(GetType(), "PasswordRequired"));
		}
	}
}
