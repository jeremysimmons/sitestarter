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
		public void Test_Save_KeepUnique()
		{
			TestArticle article = CreateStrategy.New<TestArticle>(false).Create<TestArticle>();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = CreateStrategy.New<TestArticle>(false).Create<TestArticle>();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title";
			
			ISaveStrategy strategy = SaveStrategy.New<TestArticle>(false);
			
			strategy.Save(article);
			
			bool duplicateWasSaved = strategy.Save(article2);
			
			// Check that the second article was rejected because the title is not unique
			Assert.IsFalse(duplicateWasSaved);
			
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			// Check that the original article was found
			Assert.IsNotNull(foundArticle, "Original article wasn't found.");
			
		}
	}
}
