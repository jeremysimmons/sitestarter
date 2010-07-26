using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Description of DataUpdaterTests.
	/// </summary>
	[TestFixture]
	public class DataUpdaterTests
	{
		public DataUpdaterTests()
		{
		}
		
		
		[Test]
		public void Test_PreUpdate()
		{
			// Register the types
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			// Create the dummy objects
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			// Add the role to the User.Roles collection
			user.Roles = Collection<TestRole>.Add(user.Roles, role);
			
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
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the DataAccess.Data.Updater.Update function.", NLog.LogLevel.Debug))
			{
				
				TestUtilities.ClearTestEntities();
				
				
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test-Before";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(role);
				
				DataAccess.Data.Saver.Save(user);
				
				TestUser user2 = (TestUser)DataAccess.Data.Reader.GetEntity(typeof(TestUser), "ID", user.ID);
				
				Assert.AreEqual(user.FirstName, user2.FirstName, "The name doesn't appear to have been saved.");
				
				DataAccess.Data.Activator.Activate(user2);
				
				user2.FirstName = "Test-Updated";
				
				//user2.Roles.RemoveAt(0);
				
				
				DataAccess.Data.Updater.Update(user2);
				
				//IDataStore store = DataAccess.Data.Stores[user2];
				//store.Dispose();
				DataAccess.Data.Stores.Remove(DataAccess.Data.Stores[user2]);
				
				
				TestUser user3 = (TestUser)DataAccess.Data.Reader.GetEntity<TestUser>("ID", user2.ID);
				
				Assert.AreEqual(user2.FirstName, user3.FirstName, "The name doesn't appear to have been updated.");
				
				//Assert.IsNotNull(toDelete, "The toDelete list is null.");
				//if (toDelete != null)
				//	Assert.AreEqual(1, toDelete.Length, "Incorrect number of entities in toDelete list. Expecting the obsolete reference to be in the list.");
				
				TestUtilities.ClearTestEntities();
			}
		}
		
		[Test]
		public void Test_Update_2References_CheckLocationOfReferencedEntities()
		{
			TestUtilities.ClearTestEntities();
			
			
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
			
			DataAccess.Data.Saver.Save(user2);
			DataAccess.Data.Saver.Save(user);
			DataAccess.Data.Saver.Save(role2);
			DataAccess.Data.Saver.Save(role);
			
			IEntity[] references = DataAccess.Data.Stores[DataUtilities.GetDataStoreName("TestUser", "TestRole")].Indexer.GetEntities<EntityIDReference>();
			
			Assert.AreEqual(2, references.Length, "Incorrect number of references found.");
			
			//EntityIDReference reference1 = (EntityIDReference)references[0];
			//EntityIDReference reference2 = (EntityIDReference)references[1];
			
			// Switch the references around if necessary to match (so they can be found in any order)
			/*if (!reference1.Includes(user2.ID, "Roles"))
			{
				EntityIDReference tmp = reference1;
				reference1 = reference2;
				reference2 = tmp;
			}*/
			
			//Assert.IsTrue(reference1.Includes(user.ID, "Roles"), "First reference does not include expected user.");
			//Assert.IsTrue(reference1.Includes(role.ID, "Users"), "First reference does not include expected role.");
			//Assert.IsTrue(reference2.Includes(user.ID, "Roles"), "Second reference does not include expected user.");
			//Assert.IsTrue(reference2.Includes(role.ID, "Users"), "Second reference does not include expected role.");
			
			//Assert.IsFalse(reference1.Includes(user.ID, "Roles"), "First reference includes unexpected user.");
			//Assert.IsFalse(reference1.Includes(role.ID, "Users"), "First reference includes unexpected role.");
			//Assert.IsFalse(reference2.Includes(user2.ID, "Roles"), "Second reference includes unexpected user.");
			//Assert.IsFalse(reference2.Includes(role2.ID, "Users"), "Second reference includes unexpected role.");
			
			//Assert.AreEqual(role2.ID.ToString(), ((EntityIDReference)references[0]).Entity1ID.ToString(), "First reference has invalid entity 1 ID.");
			//Assert.AreEqual(user2.ID.ToString(), ((EntityIDReference)references[0]).Entity2ID.ToString(), "First reference has invalid entity 2 ID.");
			
			//Assert.AreEqual(role1.ID.ToString(), ((EntityIDReference)references[1]).Entity1ID.ToString(), "Second reference has invalid entity 1 ID.");
			//Assert.AreEqual(user1.ID.ToString(), ((EntityIDReference)references[1]).Entity2ID.ToString(), "Second reference has invalid entity 2 ID.");
			
			
			// Load the roles out of the users store (there should be none)
			IEntity[] rolesInUsersStore = DataAccess.Data.Stores[typeof(TestUser)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(0, rolesInUsersStore.Length, "Role(s) found in users store after save.");
			
			
			IEntity[] rolesInRolesStore = DataAccess.Data.Stores[typeof(TestRole)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(2, rolesInRolesStore.Length, "Role(s) not found in roles store after save.");
			
			TestUser foundUser = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
			
			Assert.IsNotNull(foundUser, "The foundUser variable is null.");
			
			
			foundUser.Username = user.Username + " 2";
			
			DataAccess.Data.Updater.Update(foundUser);
			
			
			// Load the roles out of the users store (there should be none)
			IEntity[] rolesInUsersStore2 = DataAccess.Data.Stores[typeof(TestUser)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(0, rolesInUsersStore2.Length, "Role(s) found in users store after update.");
			
			
			IEntity[] rolesInRolesStore2 = DataAccess.Data.Stores[typeof(TestRole)].Indexer.GetEntities<TestRole>();
			
			Assert.AreEqual(2, rolesInRolesStore2.Length, "Role(s) not found in roles store after update.");
			
			
			TestUtilities.ClearTestEntities();
		}
	
		
		[Test]
		public void Test_Update_MaintainReferences()
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Testing saving of an EntityIDReference.", NLog.LogLevel.Debug))
			{
				TestUser.RegisterType();
				TestRole.RegisterType();
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				// This should remain commented out to check for exclusion
				user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Saver.Save(user);
				DataAccess.Data.Saver.Save(role);
				
				
				EntityIDReference reference = new EntityIDReference();
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
				
				if (user3.Roles != null)
					Assert.AreEqual(1, user3.Roles.Length, "Incorrect number of roles.");
				Assert.AreEqual(newFirstName, user3.FirstName, "First name mismatch.");
				
				//IDataStore store = DataAccess.Data.Stores["Testing_Articles-Testing_Articles"];
				
				//Assert.IsNotNull(store, "The data store wasn't created/initialized.");
			}
		}
		
		
	}
}
