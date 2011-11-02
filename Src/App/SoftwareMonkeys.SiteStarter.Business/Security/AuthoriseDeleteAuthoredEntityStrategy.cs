using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// 
	/// </summary>
	[AuthoriseStrategy("Delete", "IAuthored")]
	public class AuthoriseDeleteAuthoredEntityStrategy : BaseAuthoriseStrategy, IAuthoriseDeleteStrategy
	{
		public AuthoriseDeleteAuthoredEntityStrategy()
		{
		}
		
		public override bool IsAuthorised(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			IAuthored authoredEntity = (IAuthored)entity;
			
			// If the user is an administrator they are authorised
			if (AuthenticationState.UserIsInRole("Administrator"))
				return true;
			
			// If the current use is authenticated
			if (AuthenticationState.IsAuthenticated
			    // and if the current user is the author
			    && authoredEntity.Author != null
			    && AuthenticationState.User != null
			    && authoredEntity.Author.ID == AuthenticationState.User.ID)
				// then they are authoried
				return true;
			else
				// otherwise NOT authorised
				return false;
		}
		
		public override bool IsAuthorised(string shortTypeName)
		{
			return AuthenticationState.IsAuthenticated;
		}
	}
}
