using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Tests the retriever strategy.
	/// </summary>
	[TestFixture]
	public class RetrieveStrategyTests : BaseBusinessTestFixture
	{
		
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Retrieve", "IEntity"];
			
			Assert.IsNotNull(strategy);
		}
		
		[Test]
		public void Test_Retrieve_ByID()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			
			DataAccess.Data.Saver.Save(article);
			
			
			IRetrieveStrategy strategy = new RetrieveStrategy();
			
			IEntity foundArticle = strategy.Retrieve<TestArticle>(article.ID);
			
			Assert.IsNotNull(foundArticle, "Test article wasn't retrieved.");
		}
		
		[Test]
		public void Test_Retrieve_ByUniqueKey()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			
			DataAccess.Data.Saver.Save(article);
			
			StrategyInfo info = StrategyState.Strategies["Retrieve", typeof(TestUser)];
			
			IRetrieveStrategy strategy = (IRetrieveStrategy)info.New();
			
			IEntity foundArticle = strategy.Retrieve<TestArticle>(article.UniqueKey);
			
			Assert.IsNotNull(foundArticle, "Test article wasn't retrieved.");
		}
		
		[Test]
		public void Test_Retrieve_ByCustomProperty()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			DataAccess.Data.Saver.Save(article);
			
			StrategyInfo info = StrategyState.Strategies["Retrieve", typeof(TestUser)];
			
			IRetrieveStrategy strategy = (IRetrieveStrategy)info.New();
			
			IEntity foundArticle = strategy.Retrieve<TestArticle>("Title", article.Title);
			
			Assert.IsNotNull(foundArticle, "Test article wasn't retrieved.");
			
		}
	}
}
