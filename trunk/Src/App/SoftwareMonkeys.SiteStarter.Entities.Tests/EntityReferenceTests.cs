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
		
		[Test]
		public void Test_Includes_Match()
		{
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			
			string propertyName1 = "Property1";
			string propertyName2 = "Property2";
			
			string type1 = "Type1";
			string type2 = "Type2";
			
			EntityReference reference = new EntityReference();
			reference.Entity1ID = id1;
			reference.Entity2ID = id2;
			reference.Type1Name = type1;
			reference.Type2Name = type2;
			reference.Property1Name = propertyName1;
			reference.Property2Name = propertyName2;
			
			bool includes = reference.Includes(id1, propertyName1);
			
			Assert.IsTrue(includes, "Returned false when it should have returned true");
			
			includes = reference.Includes(id2, propertyName2);
			
			Assert.IsTrue(includes, "Returned false when it should have returned true");
		}
		
		[Test]
		public void Test_Includes_Mismatch()
		{
			Guid id1 = Guid.NewGuid();
			Guid id2 = Guid.NewGuid();
			
			string propertyName1 = "Property1";
			string propertyName2 = "Property2";
			
			string type1 = "Type1";
			string type2 = "Type2";
			
			EntityReference reference = new EntityReference();
			reference.Entity1ID = id1;
			reference.Entity2ID = id2;
			reference.Type1Name = type1;
			reference.Type2Name = type2;
			reference.Property1Name = propertyName1;
			reference.Property2Name = propertyName2;
			
			bool includes = reference.Includes(id1, propertyName2);
			
			Assert.IsFalse(includes, "Returned true when it should have returned false");
			
			includes = reference.Includes(id2, propertyName1);
			
			Assert.IsFalse(includes, "Returned true when it should have returned false");
			
		}
		
		// TODO: Remove if not needed
		/*[Test]
		public void Test_SwitchFor()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
						
			EntityReference reference = new EntityReference();
			reference.Entity1ID = user.ID;
			reference.Entity2ID = role.ID;
			reference.Type1Name = user.ShortTypeName;
			reference.Type2Name = role.ShortTypeName;
			reference.Property1Name = "Roles"; // ie. user.Roles
			reference.Property2Name = "Users"; // ie. role.Users
			
			EntityReference switchedReference = reference.SwitchFor(role);
						
			Assert.AreEqual(switchedReference.Entity1ID, role.ID, "The IDs of entity 1 weren't switched.");
			Assert.AreEqual(switchedReference.Entity2ID, user.ID, "The IDs of entity 2 weren't switched.");
			Assert.AreEqual(switchedReference.Type1Name, role.ShortTypeName, "The type names of entity 1 weren't switched.");
			Assert.AreEqual(switchedReference.Type2Name, user.ShortTypeName, "The type names of entity 2 weren't switched.");
			Assert.AreEqual(switchedReference.Property1Name,
			              "Users", // ie. role.Users
			              "The property names of entity 1 weren't switched.");
			Assert.AreEqual(switchedReference.Property2Name,
			              "Roles", // ie. user.Roles
			              "The property names of entity 2 weren't switched.");
			
			
		}*/
	}
}
