using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataSaverTests : BaseDataTestFixture
	{		
		[Test]
		public void Test_Save_2ParallelReferences()
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
			
			DataAccess.Data.Saver.Save(user2);
			DataAccess.Data.Saver.Save(user);
			DataAccess.Data.Saver.Save(role2);
			DataAccess.Data.Saver.Save(role);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(user.GetType().Name, role.GetType().Name);
			
			Assert.AreEqual(2, references.Count, "Incorrect number of references found.");
			
			EntityIDReference reference1 = (EntityIDReference)references[0];
			EntityIDReference reference2 = (EntityIDReference)references[1];
			
			// Switch the references around if necessary to match (so they can be found in any order)
			if (!reference1.Includes(user2.ID, "Roles"))
			{
				EntityIDReference tmp = reference1;
				reference1 = reference2;
				reference2 = tmp;
			}
			
			Assert.IsTrue(reference1.Includes(user2.ID, "Roles"), "First reference does not include expected user.");
			Assert.IsTrue(reference1.Includes(role2.ID, "Users"), "First reference does not include expected role.");
			Assert.IsTrue(reference2.Includes(user.ID, "Roles"), "Second reference does not include expected user.");
			Assert.IsTrue(reference2.Includes(role.ID, "Users"), "Second reference does not include expected role.");
			
			Assert.IsFalse(reference1.Includes(user.ID, "Roles"), "First reference includes unexpected user.");
			Assert.IsFalse(reference1.Includes(role.ID, "Users"), "First reference includes unexpected role.");
			Assert.IsFalse(reference2.Includes(user2.ID, "Roles"), "Second reference includes unexpected user.");
			Assert.IsFalse(reference2.Includes(role2.ID, "Users"), "Second reference includes unexpected role.");
			
			//Assert.AreEqual(role2.ID.ToString(), ((EntityIDReference)references[0]).Entity1ID.ToString(), "First reference has invalid entity 1 ID.");
			//Assert.AreEqual(user2.ID.ToString(), ((EntityIDReference)references[0]).Entity2ID.ToString(), "First reference has invalid entity 2 ID.");
			
			//Assert.AreEqual(role1.ID.ToString(), ((EntityIDReference)references[1]).Entity1ID.ToString(), "Second reference has invalid entity 1 ID.");
			//Assert.AreEqual(user1.ID.ToString(), ((EntityIDReference)references[1]).Entity2ID.ToString(), "Second reference has invalid entity 2 ID.");
			
		}
		
		
		[Test]
		public void Test_Save_2ConvergingReferences()
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
			
			user.Roles = new TestRole[] {role, role2};
			//user2.Roles = new TestRole[] {role2};
			
			//DataAccess.Data.Saver.Save(user2);
			DataAccess.Data.Saver.Save(user);
			DataAccess.Data.Saver.Save(role2);
			DataAccess.Data.Saver.Save(role);

			EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences("TestUser", "TestRole");
			
			Assert.AreEqual(2, references.Count, "Incorrect number of references found.");
			
			EntityIDReference reference1 = (EntityIDReference)references[0];
			EntityIDReference reference2 = (EntityIDReference)references[1];
			
			// Switch the references around if necessary to match (so they can be found in any order)
			if (!reference1.Includes(role.ID, "Users"))
			{
				EntityIDReference tmp = reference1;
				reference1 = reference2;
				reference2 = tmp;
			}
			
			Assert.IsTrue(reference1.Includes(user.ID, "Roles"), "First reference does not include expected user.");
			Assert.IsTrue(reference1.Includes(role.ID, "Users"), "First reference does not include expected role.");
			
			Assert.IsTrue(reference2.Includes(user.ID, "Roles"), "Second reference does not include expected user.");
			Assert.IsTrue(reference2.Includes(role2.ID, "Users"), "Second reference does not include expected role.");
			
			Assert.IsFalse(reference1.Includes(role2.ID, "Users"), "First reference includes unexpected role.");
			
			Assert.IsFalse(reference2.Includes(role.ID, "Users"), "Second reference includes unexpected role.");
			
		}
		/*
		[Test]
		public void Test_PreSave()
		{
			
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			user.Roles = Collection<TestRole>.Add(user.Roles, role);
			
			
			DataAccess.Data.Saver.PreSave(user);
			
			TestUser user2 = (TestUser)DataAccess.Data.Reader.GetEntity(typeof(TestUser), "ID", user.ID);
			
			DataAccess.Data.Activator.Activate(user2, "Roles");
			
			// Check the roles list on the newly loaded user object
			// Should be Length == 0
			Assert.AreEqual(0, user2.Roles.Length, "Incorrect number of roles found on retrieved user entity.");
			
		}*/
		
		
		[Test]
		public void Test_Save_2References_CheckLocationOfReferencedEntities()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the Save function and checking that the references are created successfully.", NLog.LogLevel.Debug))
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
				
				DataAccess.Data.Saver.Save(user2);
				DataAccess.Data.Saver.Save(user);
				DataAccess.Data.Saver.Save(role2);
				DataAccess.Data.Saver.Save(role);
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(user.GetType().Name, role.GetType().Name);
				
				Assert.AreEqual(2, references.Count, "Incorrect number of references found.");
				
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
				
				IEntity[] rolesInRolesStore = DataAccess.Data.Stores[role.GetType()].Indexer.GetEntities<TestRole>();
				
				Assert.AreEqual(2, rolesInRolesStore.Length, "Role(s) not found in roles store after save.");
				
				TestUser foundUser = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
				
				Assert.IsNotNull(foundUser, "The foundUser variable is null.");
				
				
				foundUser.Username = user.Username + " 2";
				
				DataAccess.Data.Saver.Save(foundUser);
				
				
				// Load the roles out of the users store (there should be none)
				IEntity[] rolesInUsersStore2 = DataAccess.Data.Stores[typeof(TestUser)].Indexer.GetEntities<TestRole>();
				
				Assert.AreEqual(0, rolesInUsersStore2.Length, "Role(s) found in users store after update.");
				
				
				IEntity[] rolesInRolesStore2 = DataAccess.Data.Stores[typeof(TestRole)].Indexer.GetEntities<TestRole>();
				
				Assert.AreEqual(2, rolesInRolesStore2.Length, "Role(s) not found in roles store after update.");
				
				
			}
		}
		
		
		[Test]
		public void Test_Save_EntityIDReference()
		{
			
			using (LogGroup logGroup = LogGroup.Start("Testing saving of an EntityIDReference.", NLog.LogLevel.Debug))
			{
				EntityIDReference reference = new EntityIDReference();
				reference.ID = Guid.NewGuid();
				reference.Type1Name = "TestArticle";
				reference.Type2Name = "TestArticlePage";
				reference.Entity1ID = Guid.NewGuid();
				reference.Entity2ID = Guid.NewGuid();
				reference.Property1Name = "Pages";
				reference.Property2Name = "Article";
				
				DataAccess.Data.Saver.Save(reference);
				
				IDataStore store = DataAccess.Data.Stores["Testing_Articles-Testing_Articles"];
				
				Assert.IsNotNull(store, "The data store wasn't created/initialized.");
			}
		}
		
		[Test]
		public void Test_Save_DontBindToDataStore()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test Article";
			
			DataAccess.Data.Saver.Save(article);
			
			TestArticle foundArticle = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNotNull(foundArticle);
			
			// Edit the entity
			article.Title = "Test Article 2";
			
			// Retrieve the entity from the store again
			TestArticle foundArticle2 = Data.DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			// Ensure that the in memory edit wasn't persisted in the store (as the update function wasn't called)
			Assert.AreNotEqual(foundArticle2.Title, article.Title, "Changes were persisted in the data store despite the update function not being called.");
			
		}
		
	}
}
