using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Description of DataUpdaterTests.
	/// </summary>
	public class DataUpdaterTests : BaseDataTestFixture
	{		
		
		[Test]
		public void Test_PreUpdate()
		{
			
			// Create the dummy objects
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			// Add the role to the User.Roles collection
			user.Roles = new TestRole[] { role };
			
			// Save both objects
			DataAccess.Data.Saver.Save(role);
			DataAccess.Data.Saver.Save(user);
			
			// Load the user to another variable
			TestUser user2 = (TestUser)DataAccess.Data.Reader.GetEntity(typeof(TestUser), "ID", user.ID);
			
			// Activate the loaded user object
			DataAccess.Data.Activator.Activate(user2);
			
			// Remove the role from the list
			user2.Roles = Collection<TestRole>.RemoveAt(user2.Roles, 0);
			
			// Run the DataStore.PreUpdate function
			DataAccess.Data.Updater.PreUpdate(user2);
			
			// Load the user again to a new variable, which should now reflect the changes
			TestUser user3 = (TestUser)DataAccess.Data.Reader.GetEntity(typeof(TestUser), "ID", user2.ID);
			
			DataAccess.Data.Activator.Activate(user3, "Roles");
			
			// Check the roles list on the newly loaded user object
			// Should be Length == 0
			Assert.AreEqual(0, user3.Roles.Length, "Incorrect number of roles found on retrieved user entity.");
			
			
		}
		
		[Test]
		public void Test_Update()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the DataAccess.Data.Updater.Update function.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				Guid userID = user.ID;
				user.FirstName = "Test-Before";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				Guid roleID = role.ID;
				role.Name = "Test Role";
				
				user.Roles = new TestRole[] { role };
				
				DataAccess.Data.Saver.Save(role);
				
				DataAccess.Data.Saver.Save(user);
				
				TestUser user2 = (TestUser)DataAccess.Data.Reader.GetEntity(typeof(TestUser), "ID", user.ID);
				
				Assert.IsNotNull(user2, "user2 == null");
				
				Assert.AreEqual(user.FirstName, user2.FirstName, "The name doesn't appear to have been saved.");
				
				DataAccess.Data.Activator.Activate(user2);
				
				user2.FirstName = "Test-Updated";
				
				//user2.Roles.RemoveAt(0);
				
				
				DataAccess.Data.Updater.Update(user2);
				
				
				
				// TODO: Check if its possible to remove and dispose the store, then have it automatically reload when needed
				//store.Dispose();
				//DataAccess.Data.Stores.Remove(store);
				
				TestUser user3 = (TestUser)DataAccess.Data.Reader.GetEntity<TestUser>("ID", user2.ID);
				
				Assert.IsNotNull(user3, "user3 == null");
				
				Assert.AreEqual(user2.ID, user3.ID, "The IDs don't match.");
				Assert.AreEqual(user2.FirstName, user3.FirstName, "The name doesn't appear to have been updated.");
				
				//Assert.IsNotNull(toDelete, "The toDelete list is null.");
				//if (toDelete != null)
				//	Assert.AreEqual(1, toDelete.Length, "Incorrect number of entities in toDelete list. Expecting the obsolete reference to be in the list.");
				
			}
		}
		
		[Test]
		public virtual void Test_Update_RemoveObsoleteReference_Sync()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the DataAccess.Data.Updater.Update function to ensure obsolete references are removed.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				Guid userID = user.ID;
				user.FirstName = "Test-Before";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				Guid roleID = role.ID;
				role.Name = "Test Role";
				
				user.Roles = new TestRole[] { role };

				// Save the user and the role with a reference between them				
				DataAccess.Data.Saver.Save(role);
				DataAccess.Data.Saver.Save(user);
				
				TestUser user2 = (TestUser)DataAccess.Data.Reader.GetEntity(typeof(TestUser), "ID", user.ID);
				
				Assert.IsNotNull(user2, "user2 == null");
				
				Assert.AreEqual(user.FirstName, user2.FirstName, "The name doesn't appear to have been saved.");
				
				DataAccess.Data.Activator.Activate(user2);
				
				// Clear the roles from the user
				user2.Roles = new TestRole[]{};
				
				// Update the user which should remove the obsolete reference
				DataAccess.Data.Updater.Update(user2);
				
				TestUser user3 = (TestUser)DataAccess.Data.Reader.GetEntity<TestUser>("ID", user2.ID);
				
				Assert.IsNotNull(user3, "user3 == null");
				
				Assert.AreEqual(user2.ID, user3.ID, "The IDs don't match.");
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences("TestUser", "TestRole");
				
				Assert.AreEqual(0, references.Count, "Invalid number of references found.");
				
			}
		}
		
		[Test]
		public void Test_Update_2References_CheckLocationOfReferencedEntities()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			Guid userID = user.ID;
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			Guid roleID = role.ID;
			role.Name = "Test Role";
			
			TestUser user2 = new TestUser();
			user.ID = Guid.NewGuid();
			Guid user2ID = user.ID;
			user2.FirstName = "Test2";
			user2.LastName = "User2";
			
			TestRole role2 = new TestRole();
			role2.ID = Guid.NewGuid();
			Guid role2ID = role2.ID;
			role2.Name = "Test Role2";
			
			user.Roles = new TestRole[] {role};
			user2.Roles = new TestRole[] {role2};
			
			DataAccess.Data.Saver.Save(role2);
			DataAccess.Data.Saver.Save(role);
			DataAccess.Data.Saver.Save(user2);
			DataAccess.Data.Saver.Save(user);
			
			EntityReferenceCollection references = (EntityReferenceCollection)DataAccess.Data.Referencer.GetReferences("TestUser", "TestRole");
			
			Assert.AreEqual(2, references.Count, "Incorrect number of references found.");
			
			
			// Load the roles out of the users store (there should be none)
			IEntity[] rolesInUsersStore = DataAccess.Data.Stores[typeof(TestUser)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(0, rolesInUsersStore.Length, "Invalid number of roles found in users store after save.");
			
			
			IEntity[] rolesInRolesStore = DataAccess.Data.Stores[typeof(TestRole)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(2, rolesInRolesStore.Length, "Invalid number of roles found in roles store after save.");
			
			TestUser foundUser = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
			
			Assert.IsNotNull(foundUser, "The foundUser variable is null.");
			
			
			foundUser.Username = user.Username + " 2";
			
			DataAccess.Data.Updater.Update(foundUser);
			
			
			// Load the roles out of the users store (there should be none)
			IEntity[] rolesInUsersStore2 = DataAccess.Data.Stores[typeof(TestUser)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(0, rolesInUsersStore2.Length, "Role(s) found in users store after update.");
			
			
			IEntity[] rolesInRolesStore2 = DataAccess.Data.Stores[typeof(TestRole)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(2, rolesInRolesStore2.Length, "Role(s) not found in roles store after update.");
			
			
		}
	
		
		[Test]
		public void Test_Update_MaintainReferences()
		{
			
			using (LogGroup logGroup = LogGroup.Start("Testing the update function to see if it maintains references.", NLog.LogLevel.Debug))
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
				
				
				EntityReference reference = new EntityReference();
				reference.ID = Guid.NewGuid();
				reference.Type1Name = "TestUser";
				reference.Type2Name = "TestRole";
				reference.Entity1ID = user.ID;
				reference.Entity2ID = role.ID;
				reference.Property1Name = "Roles";
				reference.Property2Name = "Users";
				
				TestUser user2 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
				
				DataAccess.Data.Activator.Activate(user2);
				
				string newFirstName =  "Something else";
				user2.FirstName  = newFirstName;
				
				DataAccess.Data.Updater.Update(user2);
				
				TestUser user3 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
				
				DataAccess.Data.Activator.Activate(user3);
				
				Assert.IsNotNull(user3.Roles);
				
				Assert.AreEqual(newFirstName, user3.FirstName, "First name mismatch.");
				
				if (user3.Roles != null)
					Assert.AreEqual(1, user3.Roles.Length, "Incorrect number of roles.");
			
			}
		}
		
		[Test]
		public void Test_Update_DontDuplicate()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Mock Title";
			
			DataAccess.Data.Saver.Save(article);
			
			string newTitle = "Updated";
			article.Title = newTitle;
			
			DataAccess.Data.Updater.Update(article);
			
			TestArticle[] foundArticles = DataAccess.Data.Indexer.GetEntities<TestArticle>();
			
			Assert.IsNotNull(foundArticles);
			
			Assert.AreEqual(1, foundArticles.Length, "Incorrect number of articles found.");
		}
		
		[Test]
		public virtual void Test_Update_SetsCountPropertyForReference_TwoWay()
		{
			
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockSyncEntity referencedEntity = new MockSyncEntity();
			referencedEntity.ID = Guid.NewGuid();
			
			entity.SyncEntities = new MockSyncEntity[]{
				referencedEntity
			};
			
			DataAccess.Data.Saver.Save(referencedEntity);
			DataAccess.Data.Saver.Save(entity);
			
			DataAccess.Data.Updater.Update(entity);
			
			MockEntity foundEntity = DataAccess.Data.Reader.GetEntity<MockEntity>("ID", entity.ID);
			MockSyncEntity foundReferencedEntity = DataAccess.Data.Reader.GetEntity<MockSyncEntity>("ID", referencedEntity.ID);
			
			DataAccess.Data.Activator.Activate(foundEntity);
			DataAccess.Data.Activator.Activate(foundReferencedEntity);
			
			Assert.AreEqual(1, foundEntity.TotalSyncEntities, "The TotalSyncEntities property didn't have the expected value.");
			Assert.AreEqual(1, foundEntity.SyncEntities.Length, "The SyncEntities property didn't have the expected length.");
					
			Assert.AreEqual(1, foundReferencedEntity.TotalEntities, "The TotalEntities property didn't have the expected value.");
			Assert.AreEqual(1, foundReferencedEntity.Entities.Length, "The Entities property didn't have the expected length.");
			
		}
		
		[Test]
		public virtual void Test_Update_SetsCountPropertyForReference_OneWay()
		{
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockPublicEntity referencedEntity = new MockPublicEntity();
			referencedEntity.ID = Guid.NewGuid();
			
			entity.PublicEntities = new MockPublicEntity[]{
				referencedEntity
			};

			DataAccess.Data.Saver.Save(referencedEntity);
			DataAccess.Data.Saver.Save(entity);
			
			DataAccess.Data.Updater.Update(entity);
			
			MockEntity foundEntity = DataAccess.Data.Reader.GetEntity<MockEntity>("ID", entity.ID);
			MockPublicEntity foundReferencedEntity = DataAccess.Data.Reader.GetEntity<MockPublicEntity>("ID", referencedEntity.ID);
			
			DataAccess.Data.Activator.Activate(foundEntity);
			DataAccess.Data.Activator.Activate(foundReferencedEntity);
			
			Assert.AreEqual(1, foundEntity.TotalPublicEntities, "The TotalPublicEntities property didn't have the expected value.");
			Assert.AreEqual(1, foundEntity.PublicEntities.Length, "The PublicEntities property didn't have the expected length.");		
		}
	}
}
