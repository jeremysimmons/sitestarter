
using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Reacts to the creation of any entity.
	/// </summary>
	[Reaction("Create", "IEntity")] // Currently not enabled because this class has no implementation
	public class CreateReaction : BaseCreateReaction
	{
		public CreateReaction()
		{
		}
		
		public override void React(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{			
			// Set the entity ID if needed
			if (entity.ID == Guid.Empty)
				entity.ID = Guid.NewGuid();
			
			base.React(entity);
		}
	}
}
