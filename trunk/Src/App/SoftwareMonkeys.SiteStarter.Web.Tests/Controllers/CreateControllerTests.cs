using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Web.Tests.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class CreateControllerTests : BaseWebTestFixture
	{
		public CreateControllerTests()
		{
		}
		
		[Test]
		[ExpectedException(typeof(UnauthorisedException))]
		public void Test_EnsureAuthorised_RestrictUnauthorisedEntity()
		{
			MockRestrictedEntity restrictedEntity = new MockRestrictedEntity();
			
			CreateController controller = new CreateController();
			controller.Container = new MockCreateProjection(typeof(MockRestrictedEntity));
			bool isAuthorised = controller.EnsureAuthorised(restrictedEntity);
			
			Assert.IsFalse(isAuthorised, "Returned true when it should have been false.");
		}
		
		[Test]
		[ExpectedException(typeof(UnauthorisedException))]
		public void Test_EnsureAuthorised_RestrictUnauthorisedType()
		{
			MockRestrictedEntity restrictedEntity = new MockRestrictedEntity();
			
			CreateController controller = new CreateController();
			controller.Container = new MockCreateProjection(typeof(MockRestrictedEntity));
			bool isAuthorised = controller.EnsureAuthorised();
			
			Assert.IsFalse(isAuthorised, "Returned true when it should have been false.");
		}
		
		[Test]
		public void Test_EnsureAuthorised_AllowPublicEntity()
		{
			MockPublicEntity entity = new MockPublicEntity();
			
			CreateController controller = new CreateController();
			controller.Container = new MockCreateProjection(typeof(MockPublicEntity));
			bool isAuthorised = controller.EnsureAuthorised(entity);
			
			Assert.IsTrue(isAuthorised, "Returned false when it should be true.");
		}
		
		[Test]
		public void Test_EnsureAuthorised_AllowPublicType()
		{			
			CreateController controller = new CreateController();
			controller.Container = new MockCreateProjection(typeof(MockPublicEntity));
			bool isAuthorised = controller.EnsureAuthorised();
			
			Assert.IsTrue(isAuthorised, "Returned false when it should be true.");
		}
	}
}
