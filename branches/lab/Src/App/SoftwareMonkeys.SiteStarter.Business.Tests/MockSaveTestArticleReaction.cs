using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[Reaction("Save", "TestArticle")]
	public class MockSaveTestArticleReaction : BaseReaction
	{
		public MockSaveTestArticleReaction()
		{
		}
		
		public override void React(IEntity entity)
		{
			throw new Exception("reacted");
		}
	}
}
