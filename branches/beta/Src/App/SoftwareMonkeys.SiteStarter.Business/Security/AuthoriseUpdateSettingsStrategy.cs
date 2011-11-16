using System;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Used to authorise the edit/update of settings.
	/// </summary>
	[Strategy("AuthoriseUpdate", "Settings")]
	public class AuthoriseUpdateSettingsStrategy : AuthoriseUpdateStrategy
	{
		public AuthoriseUpdateSettingsStrategy()
		{
		}
		
		public override bool IsAuthorised(string shortTypeName)
		{
			return AuthenticationState.IsAuthenticated
				&& AuthenticationState.UserIsInRole("Administrator");
		}
		}
	}
