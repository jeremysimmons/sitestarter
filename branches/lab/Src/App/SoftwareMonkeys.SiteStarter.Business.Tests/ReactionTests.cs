using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class ReactionTests : BaseBusinessTestFixture
	{
		public ReactionTests()
		{
		}
		
		[SetUp]
		public void Initialize()
		{
			InitializeMockBusiness();
		}
		
		[ExpectedException(typeof(Exception))]
		[Test]
		public void Test_React()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Article";
			
			MockSaveTestArticleStrategy strategy = MockSaveTestArticleStrategy.New();
			
			strategy.Save(article);
		}
	}
}
