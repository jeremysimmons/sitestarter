using System;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("AuthoriseUpdate", "Projection")]
	public class AuthoriseUpdateProjectionStrategy : BaseAuthoriseStrategy
	{
		public AuthoriseUpdateProjectionStrategy()
		{
		}
		
		public override bool Authorise(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			return Authorise(entity.ShortTypeName);
		}
		
		public override bool Authorise(string shortTypeName)
		{
			return AuthenticationState.UserIsInRole("Administrator");
		}
	}
}
