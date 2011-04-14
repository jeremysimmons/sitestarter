using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("Save", "TestArticle")]
	public class MockSaveTestArticleStrategy : UniqueSaveStrategy
	{
		public MockSaveTestArticleStrategy()
		{
		}
		
		public override bool Save(IEntity entity)
		{
			// [Important] The reaction should be triggered even in mock environment
			React(entity);
			
			return true;
		}
		
		static public MockSaveTestArticleStrategy New()
		{
			MockSaveTestArticleStrategy strategy = new MockSaveTestArticleStrategy();
			strategy.TypeName = "TestArticle";
			
			StrategyState
				.Strategies
					.Creator
						.AttachReactions("Save", strategy);
					
			return strategy;
		}
	}
}
