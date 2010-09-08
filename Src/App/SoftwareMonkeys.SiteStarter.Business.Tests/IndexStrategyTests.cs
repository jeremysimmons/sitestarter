using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class IndexStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_GetEntities()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title";
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			IndexStrategy strategy = new IndexStrategy();
			
			TestArticle[] foundArticles = strategy.Get<TestArticle>();
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(2, foundArticles.Length, "Invalid number of test articles found.");
		}
	}
}
