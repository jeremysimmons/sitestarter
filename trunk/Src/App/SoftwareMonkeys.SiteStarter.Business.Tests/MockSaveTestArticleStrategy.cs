using System;
using SoftwareMonkeys.SiteStarter.Data;
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
			return base.Save(entity);
		}
		
		static public MockSaveTestArticleStrategy New()
		{
			return New(true);
		}
		
		static public MockSaveTestArticleStrategy New(bool requiresAuthorisation)
		{
			MockSaveTestArticleStrategy strategy = new MockSaveTestArticleStrategy();
			strategy.TypeName = "TestArticle";
			strategy.RequireAuthorisation = requiresAuthorisation;
			
			StrategyState
				.Strategies
				.Creator
				.AttachReactions("Save", strategy);
			
			return strategy;
		}
	}
}
