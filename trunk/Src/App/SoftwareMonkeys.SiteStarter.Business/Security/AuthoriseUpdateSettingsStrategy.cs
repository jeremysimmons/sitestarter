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
		
		public override bool Authorise(string shortTypeName)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
		public override bool Authorise(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
		
	}
}
