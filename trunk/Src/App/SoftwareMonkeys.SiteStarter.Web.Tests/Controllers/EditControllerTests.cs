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
	public class EditControllerTests : BaseWebTestFixture
	{
		public EditControllerTests()
		{
		}
		
		[Test]
		[ExpectedException(typeof(UnauthorisedException))]
		public void Test_EnsureAuthorised_RestrictUnauthorisedEntity()
		{
			MockRestrictedEntity restrictedEntity = new MockRestrictedEntity();
			
			EditController controller = new EditController();
			controller.Container = new MockEditProjection(typeof(MockRestrictedEntity));
			bool isAuthorised = controller.EnsureAuthorised(restrictedEntity);
			
			Assert.IsFalse(isAuthorised, "Returned true when it should have been false.");
		}
		
		[Test]
		[ExpectedException(typeof(UnauthorisedException))]
		public void Test_EnsureAuthorised_RestrictUnauthorisedType()
		{
			MockRestrictedEntity restrictedEntity = new MockRestrictedEntity();
			
			EditController controller = new EditController();
			controller.Container = new MockEditProjection(typeof(MockRestrictedEntity));
			bool isAuthorised = controller.EnsureAuthorised();
			
			Assert.IsFalse(isAuthorised, "Returned true when it should have been false.");
		}
		
		[Test]
		public void Test_EnsureAuthorised_AllowPublicEntity()
		{
			MockPublicEntity entity = new MockPublicEntity();
			
			EditController controller = new EditController();
			controller.Container = new MockEditProjection(typeof(MockPublicEntity));
			bool isAuthorised = controller.EnsureAuthorised(entity);
			
			Assert.IsTrue(isAuthorised, "Returned false when it should be true.");
		}
		
		[Test]
		public void Test_EnsureAuthorised_AllowPublicType()
		{			
			EditController controller = new EditController();
			controller.Container = new MockEditProjection(typeof(MockPublicEntity));
			bool isAuthorised = controller.EnsureAuthorised();
			
			Assert.IsTrue(isAuthorised, "Returned false when it should be true.");
		}
	}
}
