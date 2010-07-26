using System;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// Description of EntityReferenceTests.
	/// </summary>
	[TestFixture]
	public class EntityReferenceTests
	{
		[Test]
		public void Test_GetOtherEntity()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			
			EntityReference reference = new EntityReference();
			
			reference.SourceEntity = user;
			reference.ReferenceEntity = role;
			
			IEntity otherEntity1 = reference.GetOtherEntity(user);
			IEntity otherEntity2 = reference.GetOtherEntity(role);
			
			Assert.AreEqual(role.ShortTypeName, otherEntity1.ShortTypeName, "Failed to get other entity. #1");
			Assert.AreEqual(user.ShortTypeName, otherEntity2.ShortTypeName, "Failed to get other entity. #2");
		}
	}
}
