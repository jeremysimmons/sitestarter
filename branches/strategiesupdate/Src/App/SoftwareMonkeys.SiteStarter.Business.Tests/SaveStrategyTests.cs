using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class SaveStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Save", "IEntity"];
			
			Assert.IsNotNull(strategy);
		}
		
		[Test]
		public void Test_Save()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			
			StrategyInfo info = StrategyState.Strategies["Save", "IEntity"];
			ISaveStrategy strategy = SaveStrategy.New<TestArticle>(false);
			strategy.Validator = new ValidateStrategy();
			
			strategy.Save(article);
			
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNotNull(foundArticle);
		}
	}
}
