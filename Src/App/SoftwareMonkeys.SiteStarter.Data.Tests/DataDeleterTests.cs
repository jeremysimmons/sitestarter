using System;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataDeleterTests : BaseDataTestFixture
	{		
		public DataDeleterTests()
		{
		}
		
		[Test]
		public void Test_Delete()
		{
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
						
			TestUser[] foundUsers1 = DataAccess.Data.Indexer.GetEntities<TestUser>();
			
			Assert.AreEqual(0, foundUsers1.Length, "There should be no users found yet.");
			
			DataAccess.Data.Saver.Save(user);
			
			TestUser[] foundUsers2 = DataAccess.Data.Indexer.GetEntities<TestUser>();
			
			Assert.AreEqual(1, foundUsers2.Length, "User wasn't saved or too many found.");
					
			DataAccess.Data.Deleter.Delete(user);
			
			TestUser[] foundUsers3 = DataAccess.Data.Indexer.GetEntities<TestUser>();
			
			Assert.AreEqual(0, foundUsers3.Length, "Invalid number of users.");
		}
		
		[Test]
		public void Test_Delete_Reference()
		{
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			user.Roles = new TestRole[] {role};
			
			DataAccess.Data.Saver.Save(role);
			DataAccess.Data.Saver.Save(user);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(typeof(TestUser).Name, typeof(TestRole).Name);
			
			Assert.AreEqual(1, references.Count, "Incorrect number of references found.");
			
			DataAccess.Data.Deleter.Delete(references[0]);
			
			EntityReferenceCollection references2 = DataAccess.Data.Referencer.GetReferences(typeof(TestUser).Name, typeof(TestRole).Name);
			
			Assert.AreEqual(0, references2.Count, "Reference not deleted.");
		}
		
		
		[Test]
		public void Test_Delete_EntityAndReference_Sync()
		{
			
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			user.Roles = new TestRole[] {role};
			
			DataAccess.Data.Saver.Save(role);
			DataAccess.Data.Saver.Save(user);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(typeof(TestUser).Name, typeof(TestRole).Name);
			
			Assert.AreEqual(1, references.Count, "Incorrect number of references found.");
			
			DataAccess.Data.Deleter.Delete(user);
			
			EntityReferenceCollection references2 = DataAccess.Data.Referencer.GetReferences(typeof(TestUser).Name, typeof(TestRole).Name);
			
			Assert.AreEqual(0, references2.Count, "Reference not deleted.");
		}
		
		[Test]
		public void Test_Delete_EntityAndReference_Async()
		{
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockPublicEntity referencedEntity = new MockPublicEntity();
			referencedEntity.ID = Guid.NewGuid();
			
			entity.PublicEntities = new MockPublicEntity[]{referencedEntity};
			
			DataAccess.Data.Saver.Save(referencedEntity);
			DataAccess.Data.Saver.Save(entity);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(typeof(MockEntity).Name, typeof(MockPublicEntity).Name);
			
			Assert.AreEqual(1, references.Count, "Incorrect number of references found.");
			
			DataAccess.Data.Deleter.Delete(entity);
			
			EntityReferenceCollection references2 = DataAccess.Data.Referencer.GetReferences(typeof(MockEntity).Name, typeof(MockPublicEntity).Name);
			
			Assert.AreEqual(0, references2.Count, "Reference not deleted.");
		}
		
		
		[Test]
		public void Test_Delete_RemoveReferences()
		{
			
			using (LogGroup logGroup = LogGroup.Start("Testing saving of an EntityIDReference.", NLog.LogLevel.Debug))
			{
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				// This should remain commented out to check for exclusion
				user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Saver.Save(role);
				DataAccess.Data.Saver.Save(user);
				
				DataAccess.Data.Deleter.Delete(role);
				
				TestUser user2 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
				
				DataAccess.Data.Activator.Activate(user2, "Roles");
				
				Assert.IsNotNull(user2.Roles);
				
				if (user2.Roles != null)
					Assert.AreEqual(0, user2.Roles.Length, "Incorrect number of roles. The role should have been removed.");
				//Assert.AreEqual(newFirstName, user3.FirstName, "First name mismatch.");
				
				//IDataStore store = DataAccess.Data.Stores["Testing_Articles-Testing_Articles"];
				
				//Assert.IsNotNull(store, "The data store wasn't created/initialized.");
				
			}
		}
		
		
	}
}
