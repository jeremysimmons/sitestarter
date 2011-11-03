using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Update", "IAuthored")]
	public class AuthoriseUpdateAuthoredEntityStrategy : BaseAuthoriseStrategy, IAuthoriseUpdateStrategy
	{
		public AuthoriseUpdateAuthoredEntityStrategy()
		{
		}
		
		public override bool IsAuthorised(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			IAuthored authoredEntity = (IAuthored)entity;
			
			// If the user is an administrator they are authorised
			if (AuthenticationState.UserIsInRole("Administrator"))
				return true;
			
			// If the current user is the author
			if (UserIsAuthor((IAuthored)entity))
				return true;
			else
				// otherwise NOT authorised
				return false;
		}
		
		
		protected virtual bool UserIsAuthor(IAuthored entity)
		{
			ActivateStrategy.New(entity).Activate(entity, "Author");
			
			// If the current use is authenticated
			return AuthenticationState.IsAuthenticated
				// and if the current user is the author
				&& entity.Author != null
				&& AuthenticationState.User != null
				&& entity.Author.ID == AuthenticationState.User.ID;
		}
		
		public override bool IsAuthorised(string shortTypeName)
		{
			return AuthenticationState.IsAuthenticated;
		}
	}
}
