using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class UniqueValidateStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Validate", "IUniqueEntity"];
			
			Assert.IsNotNull(strategy);
		}
		
		[Test]
		public void Test_Validate()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Title";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test Title 2";
			
			TestArticle article3 = new TestArticle();
			article3.ID = Guid.NewGuid();
			article3.Title = article2.Title;
			
			DataAccess.Data.Saver.Save(article);
			DataAccess.Data.Saver.Save(article2);
			
			IUniqueValidateStrategy strategy = new UniqueValidateStrategy();
			
			// Check that the strategy was found
			Assert.IsNotNull(strategy);
			
			// Execute the validate function on the strategy
			bool isUnique = strategy.Validate(article2, "Title");
			
			// Check that the validate function returned true
			Assert.IsTrue(isUnique, "The Validate function returned false when it shouldn't have.");
			
			article3.Title = article2.Title;
			
			// Execute the validate function on the strategy and expect it to fail
			bool isNotUnique = strategy.Validate(article3, "Title");
			
			// Check that the validate function returned false when it's supposed to
			Assert.IsFalse(isNotUnique, "The Validate function returned true when it shouldn't have.");
			/*
			// Load the original article to ensure it's there
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNotNull(foundArticle);*/
			
		}
	}
}
