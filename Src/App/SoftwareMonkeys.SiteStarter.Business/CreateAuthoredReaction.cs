
using System;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Reacts to the creation of any authored entity (ie. an entity with an author).
	/// </summary>
	[Reaction("Create", "IAuthored")]
	public class CreateAuthoredReaction : BaseCreateReaction
	{
		public CreateAuthoredReaction()
		{
		}
		
		public override void React(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{		
			IAuthored authoredEntity = (IAuthored)entity;
			
			// Set the author of the entity
			authoredEntity.Author = AuthenticationState.User;
			
			base.React(entity);
		}
	}
}
