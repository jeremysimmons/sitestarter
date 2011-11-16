using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.State;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[Reaction("Save", "MockEntity")]
	public class SaveMockEntityReaction : BaseReaction
	{
		public SaveMockEntityReaction()
		{
		}
		
		public override void React(IEntity entity)
		{
			StateAccess.State.Session["SaveMockEntityReaction_Reacted"] = true;
		}
	}
}
