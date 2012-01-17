using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	///
	/// </summary>
	[AuthoriseStrategy("Retrieve", "IAuthored")]
	public class AuthoriseRetrieveAuthoredEntityStrategy : AuthoriseRetrieveStrategy
	{
		public AuthoriseRetrieveAuthoredEntityStrategy()
		{
		}
		
		public override bool IsAuthorised(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			if (!AuthenticationState.IsAuthenticated)
				return false;
			
			IAuthored authoredEntity = (IAuthored)entity;
			
			ActivateStrategy.New(authoredEntity).Activate(authoredEntity, "Author");
			
			return authoredEntity.IsPublic
				|| (authoredEntity != null && authoredEntity.Author.ID == AuthenticationState.User.ID);
		}
	}
}
