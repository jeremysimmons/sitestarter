using System;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// Description of EntityReferenceTests.
	/// </summary>
	[TestFixture]
	public class EntityReferenceTests : BaseEntityTestFixture
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
		
		[Test]
		public void Test_SwitchFor()
		{
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockSyncEntity otherEntity = new MockSyncEntity();
			otherEntity.ID = Guid.NewGuid();
			
			entity.SyncEntities = new MockSyncEntity[]{
				otherEntity
			};
			
			EntityReference reference = new EntityReference();
			reference.SourceEntity = entity;
			reference.ReferenceEntity = otherEntity;
			reference.Property1Name = "SyncEntities";
			reference.Property2Name = "Entities";
			
			EntityReference switchedReference = reference.SwitchFor(otherEntity);
			
			Assert.AreEqual(reference.Property1Name, "Entities", "Property1Name doesn't match what's expected.");
			Assert.AreEqual(reference.Property2Name, "SyncEntities", "Property2Name doesn't match what's expected.");
			
			Assert.AreEqual(reference.Type1Name.ToString(), otherEntity.ShortTypeName, "Type1Name doesn't match what's expected.");
			Assert.AreEqual(reference.Type2Name.ToString(), entity.ShortTypeName, "Type2Name doesn't match what's expected.");
			
			Assert.AreEqual(reference.Entity1ID.ToString(), otherEntity.ID.ToString(), "Entity1ID doesn't match what's expected.");
			Assert.AreEqual(reference.Entity2ID.ToString(), entity.ID.ToString(), "Entity2ID doesn't match what's expected.");
				
		}
	}
}
