using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.Tests.Projections;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class CreateSubEntityControllerTests : BaseWebTestFixture
	{
		[Test]
		public void Test_Create_Parent()
		{
			MockProjection projection = new MockProjection();
			projection.RequireAuthorisation = false;
			projection.Action = "Create";
			projection.Type =  typeof(TestArticlePage);
			
			TestArticle article = CreateStrategy.New<TestArticle>(false).Create<TestArticle>();
			article.ID = Guid.NewGuid();
			article.Title = "Test title";
			
			SaveStrategy.New<TestArticle>(false).Save(article);
			
			CreateController controller = CreateController.New(projection, typeof(TestArticlePage));
			
			Assert.IsTrue(controller is CreateSubEntityController, "Invalid controller type: " + controller.GetType().ToString());
			
			CreateSubEntityController createSubEntityController = (CreateSubEntityController)controller;
			
			ISubEntity subEntity = (ISubEntity)createSubEntityController.Create(article.ID, String.Empty);
			
			Assert.IsNotNull(subEntity, "No sub entity returned.");
			
			Assert.IsTrue((subEntity is TestArticlePage), "Wrong type returned: " + subEntity.GetType().ToString());
			
			Assert.IsNotNull(subEntity.Parent, "No parent assigned to sub entity.");
			
			Assert.AreEqual(article.ID.ToString(), subEntity.Parent.ID.ToString(), "Parent ID doesn't match expected.");
			
			Assert.AreEqual(1, subEntity.Number, "Sub entity has wrong number.");
			
			SaveStrategy.New(subEntity, false).Save(subEntity);
			
			
			ISubEntity subEntity2 = (ISubEntity)createSubEntityController.Create(Guid.Empty, article.UniqueKey);
			
			Assert.IsNotNull(subEntity2, "No sub entity returned.");
			
			Assert.IsTrue((subEntity2 is TestArticlePage), "Wrong type returned: " + subEntity.GetType().ToString());
			
			Assert.IsNotNull(subEntity2.Parent, "No parent assigned to sub entity.");
			
			Assert.AreEqual(article.ID.ToString(), subEntity2.Parent.ID.ToString(), "Parent ID doesn't match expected.");
			
			Assert.AreEqual(2, subEntity2.Number, "Sub entity has wrong number.");
		}
	}
}
