using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class CreateSubEntityStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_New()
		{
			ICreateStrategy strategy = CreateStrategy.New("TestArticlePage", false);
			
			Assert.IsTrue(strategy is CreateSubEntityStrategy, "Wrong strategy type returned.");
		}
		
		[Test]
		public void Test_Create_FromParentID()
		{
			TestArticle article = CreateStrategy.New<TestArticle>(false).Create<TestArticle>();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			SaveStrategy.New<TestArticle>(false).Save(article);
			
			CreateSubEntityStrategy strategy = (CreateSubEntityStrategy)CreateStrategy.New("TestArticlePage", false);
			
			TestArticlePage page = strategy.Create<TestArticlePage>(article.ID, String.Empty);
			
			Assert.IsNotNull(page.Article, "The article wasn't assigned to the page.");
			
			Assert.AreEqual(article.Title, page.Article.Title, "Article titles don't match.");
			
			Assert.AreEqual(1, page.PageNumber, "Invalid page number");
		}
	}
}
