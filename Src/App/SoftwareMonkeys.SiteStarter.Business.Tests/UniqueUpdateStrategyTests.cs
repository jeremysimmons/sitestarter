using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class UniqueUpdateStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Update", "IEntity"];
			
			Assert.IsNotNull(strategy);
		}
		
		[Test]
		public void Test_Update_KeepUnique()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Another Title 2";
			
			using (Batch b = BatchState.StartBatch())
			{
				Data.DataAccess.Data.Saver.Save(article);
				Data.DataAccess.Data.Saver.Save(article2);	
			}
			
			
			TestArticle article3 = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			// Check that the original article was found
			Assert.IsNotNull(article3, "The updated article wasn't found.");
			
			article3.Title = article2.Title;
			
			TestArticle[] foundArticles = Data.DataAccess.Data.Indexer.GetEntities<TestArticle>("Title", article3.Title);
			
			// Ensure that the updated article hasn't been committed yet
			Assert.AreEqual(1, foundArticles.Length, "Invalid number found.");
			
			IUniqueUpdateStrategy strategy = new Business.UniqueUpdateStrategy();
			
			bool articleWasUpdated = strategy.Update(article3);
			
			// Check that the article was rejected because the title is not unique
			Assert.IsFalse(articleWasUpdated, "The article was updated even though the title was not unique.");
			
			
			
		}
	}
}
