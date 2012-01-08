using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Sets the default property values of each new entity immediately after it's created.
	/// </summary>
	[Reaction("Create", "IEntity")]
	public class CreateEntityReaction : BaseCreateReaction
	{
		public CreateEntityReaction()
		{
		}
		
		public override void React(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			entity.DateCreated = DateTime.Now;	
			
			base.React(entity);
		}
	}
}
