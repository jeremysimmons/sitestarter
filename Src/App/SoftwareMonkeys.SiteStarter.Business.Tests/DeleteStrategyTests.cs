using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class DeleteStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Delete()
		{
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			
			IDeleteStrategy strategy = new DeleteStrategy();
			
			Data.DataAccess.Data.Saver.Save(article);
			
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNotNull(foundArticle);
			
			strategy.Delete(article);
			
			TestArticle foundArticle2 = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNull(foundArticle2, "The article wasn't deleted.");
		}
	}
}
