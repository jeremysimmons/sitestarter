using System;
using System.Reflection;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class ValidateUniqueStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Validate", "IEntity"];
			
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
			
			IValidateUniqueStrategy strategy = new ValidateUniqueStrategy();
			
			// Check that the strategy was found
			Assert.IsNotNull(strategy);
			
			PropertyInfo titleProperty = article2.GetType().GetProperty("Title");
			
			// Execute the validate function on the strategy
			bool isUnique = strategy.IsValid(article2, titleProperty, new UniqueAttribute());
			
			// Check that the validate function returned true
			Assert.IsTrue(isUnique, "The Validate function returned false when it shouldn't have.");
			
			article3.Title = article2.Title;
			
			PropertyInfo titleProperty2 = article3.GetType().GetProperty("Title");
			
			// Execute the validate function on the strategy and expect it to fail
			bool isNotUnique = strategy.IsValid(article3, titleProperty2, new UniqueAttribute());
			
			// Check that the validate function returned false when it's supposed to
			Assert.IsFalse(isNotUnique, "The Validate function returned true when it shouldn't have.");
			
		}
	}
}
