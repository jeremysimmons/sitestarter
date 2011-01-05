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
		public void Test_Index()
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
			
			TestArticle[] foundArticles = strategy.Index<TestArticle>();
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(2, foundArticles.Length, "Invalid number of test articles found.");
		}
		
		[Test]
		public void Test_Index_FilterValues()
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
			
			Dictionary<string, object> filterValues = new Dictionary<string, object>();
			
			filterValues.Add("Title", article.Title);
			
			TestArticle[] foundArticles = Collection<TestArticle>.ConvertAll(strategy.Index(filterValues));
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(1, foundArticles.Length, "Invalid number of test articles found.");
		}
		
		
		[Test]
		public void Test_Index_PageIndex0_Size1()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title 2";
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			PagingLocation location = new PagingLocation(0, 1);
			
			IIndexStrategy strategy = IndexStrategy.New<TestArticle>(location, "TitleAscending", false);
			strategy.TypeName = "TestArticle";
			strategy.EnablePaging = true;
			
			TestArticle[] foundArticles = Collection<TestArticle>.ConvertAll(strategy.Index());
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(2, location.AbsoluteTotal, "Absolute total invalid.");
			
			Assert.AreEqual(1, foundArticles.Length, "Invalid number of test articles found.");
		}
		
		[Test]
		public void Test_Index_PageIndex0_Size2()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title 2";
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			PagingLocation location = new PagingLocation(0, 2);
			
			IIndexStrategy strategy = IndexStrategy.New<TestArticle>(location, "TitleAscending", false);
			strategy.TypeName = "TestArticle";
			strategy.EnablePaging = true;
			
			
			TestArticle[] foundArticles = Collection<TestArticle>.ConvertAll(strategy.Index());
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(2, location.AbsoluteTotal, "Absolute total invalid.");
						
			Assert.AreEqual(2, foundArticles.Length, "Invalid number of test articles found.");
		}
		
		[Test]
		public void Test_Index_PageIndex1_Size1()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title 2";
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			PagingLocation location = new PagingLocation(1, 1);
			
			IIndexStrategy strategy = IndexStrategy.New<TestArticle>(location, "TitleAscending", false);
			strategy.TypeName = "TestArticle";
			strategy.EnablePaging = true;
			
			TestArticle[] foundArticles = Collection<TestArticle>.ConvertAll(strategy.Index());
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(2, location.AbsoluteTotal, "Absolute total invalid.");
			
			Assert.AreEqual(1, foundArticles.Length, "Invalid number of test articles found.");
			
		}
		
		[Test]
		public void Test_Index_PageIndex1_Size2()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title 2";
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			PagingLocation location = new PagingLocation(1, 2);
			
			IIndexStrategy strategy = IndexStrategy.New<TestArticle>(location, "TitleAscending", false);
			strategy.TypeName = "TestArticle";
			strategy.EnablePaging = true;
			
			TestArticle[] foundArticles = Collection<TestArticle>.ConvertAll(strategy.Index());
			
			Assert.IsNotNull(foundArticles);
			
			// There should be no articles found because it's requesting a page outside the available range
			Assert.AreEqual(0, foundArticles.Length, "Invalid number of test articles found.");
		}
	}
}
