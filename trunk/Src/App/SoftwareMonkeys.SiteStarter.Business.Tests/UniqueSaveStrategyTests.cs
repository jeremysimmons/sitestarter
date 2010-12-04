using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class UniqueSaveStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			strategies["Save", "IUniqueEntity"] = new StrategyInfo(new UniqueSaveStrategy());
			
			StrategyInfo strategyInfo = strategies["Save", typeof(TestArticle).Name];
			
			Assert.IsNotNull(strategyInfo, "No strategy found.");
			
			IStrategy strategy = strategyInfo.New();
			
			Assert.AreEqual("UniqueSaveStrategy", strategy.GetType().Name, "Loaded the wrong type.");
		}
		
		[Test]
		public void Test_Save_KeepUnique()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title";
			
			ISaveStrategy strategy = UniqueSaveStrategy.New<TestArticle>(false);
			
			strategy.Save(article);
			
			bool duplicateWasSaved = strategy.Save(article2);
			
			// Check that the second article was rejected because the title is not unique
			Assert.IsFalse(duplicateWasSaved);
			
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			// Check that the original article was found
			Assert.IsNotNull(foundArticle);
			
		}
	}
}
