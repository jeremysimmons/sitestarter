using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class DeleteSubEntityStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Delete_Reorder()
		{
			TestArticle article = new TestArticle();
			
			article.ID = Guid.NewGuid();
			article.Title = "Test Article";
			
			SaveStrategy.New(article, false).Save(article);
			
			Collection<TestArticlePage> pages = new Collection<TestArticlePage>();
			
			// Create and save 3 article pages associated with the article
			for (int i = 0; i < 3; i++)
			{
				TestArticlePage page = new TestArticlePage();
				page.Article = article;
				page.Title = "Page " + (i+1);
				page.ID = Guid.NewGuid();
				page.PageNumber = i+1;
				
				pages.Add(page);
				
				SaveStrategy.New(page, false).Save(page);
			}
			
			// Delete the second page (0 based position = 1)
			DeleteStrategy.New(pages[1], false).Delete(pages[1]);
			
			// Load the article from the store
			TestArticle foundArticle = RetrieveStrategy.New<TestArticle>(false).Retrieve<TestArticle>("ID", article.ID);
			
			// Activate the pages on the article
			ActivateStrategy.New(foundArticle, false).Activate(foundArticle, "Pages");
			
			Assert.IsNotNull(foundArticle.Pages, "Pages property isn't set.");
			
			Assert.AreEqual(2, foundArticle.Pages.Length, "Invalid number of pages.");
			
			foundArticle.Pages = Collection<TestArticlePage>.Sort(foundArticle.Pages, "PageNumberAscending");
			
			Assert.AreEqual(1, foundArticle.Pages[0].PageNumber, "First page has wrong number.");
			Assert.AreEqual("Page 1", foundArticle.Pages[0].Title, "First page has wrong title.");
			Assert.AreEqual(2, foundArticle.Pages[1].PageNumber, "Third page has wrong number (should now be 2 as it's moved up one spot).");
			Assert.AreEqual("Page 3", foundArticle.Pages[1].Title, "Third page has wrong title.");
		}
	}
}
