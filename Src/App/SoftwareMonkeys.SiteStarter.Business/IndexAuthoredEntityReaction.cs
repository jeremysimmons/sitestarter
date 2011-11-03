using System;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[Reaction("Index", "IAuthored")]
	public class IndexAuthoredEntityReaction : BaseIndexReaction
	{
		public IndexAuthoredEntityReaction()
		{
		}
		
		public override void React(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			ActivateStrategy.New(entity).Activate(entity, "Author");
			
			base.React(entity);
		}
	}
}
