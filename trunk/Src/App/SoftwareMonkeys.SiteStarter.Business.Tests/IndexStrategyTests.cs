using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;

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
			article2.Title = "Test Title 2";
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			IIndexStrategy strategy = IndexStrategy.New<TestArticle>(false);
			strategy.PageSize = 20;
			
			TestArticle[] foundArticles = strategy.Index<TestArticle>();
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(2, foundArticles.Length, "Invalid number of test articles found.");
		}
		
		[Test]
		public void Test_GetEntities_FilterValues()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title 2";
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			IIndexStrategy strategy = IndexStrategy.New<TestArticle>(false);
			strategy.TypeName = "TestArticle";
			strategy.PageSize = 20;
			
			Dictionary<string, object> filterValues = new Dictionary<string, object>();
			
			filterValues.Add("Title", article.Title);
			
			TestArticle[] foundArticles = Collection<TestArticle>.ConvertAll(strategy.Index(filterValues));
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(1, foundArticles.Length, "Invalid number of test articles found.");
		}
	}
}
