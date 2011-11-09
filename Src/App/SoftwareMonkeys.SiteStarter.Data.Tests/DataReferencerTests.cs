using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataReferencerTests : BaseDataTestFixture
	{
		[Test]
		public void Test_GetReferences()
		{
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences();
				
				Assert.IsNotNull(references);
				                 
				Assert.AreEqual(0, references.Count, "Invalid number of references found before creating one.");
			
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(user);
				
				DataAccess.Data.Saver.Save(role);
				
				references = DataAccess.Data.Referencer.GetReferences();
				
				Assert.IsNotNull(references);
				                 
				Assert.AreEqual(1, references.Count, "Invalid number of references found after creating one.");
		}
		
		[Test]
		public void Test_GetReferences_Basic()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the retrieval of references for an entity.", NLog.LogLevel.Debug))
			{
								
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(user);
				
				DataAccess.Data.Saver.Save(role);
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(user.GetType(), user.ID, "Roles", typeof(TestRole), false);
				
				Assert.IsNotNull(references, "The references object returned was null.");
				
				if (references != null)
				{
					Assert.AreEqual(1, references.Count, "Wrong number of references returned.");
					
					Assert.IsTrue(references[0].Includes(userID, "Roles"), "The user ID wasn't found on the reference.");
					Assert.IsTrue(references[0].Includes(roleID, "Users"), "The role ID wasn't found on the reference.");
				}
			}
		}
		
		[Test]
		public void Test_GetReference_EmptyReferencedEntityID_Found()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the retrieval of references for an entity when specifying a Guid.Empty referenced entity ID.", NLog.LogLevel.Debug))
			{
								
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(user);
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(user.GetType(), user.ID, "Roles", typeof(TestRole), false);
				
				Assert.IsNotNull(references, "The references object returned was null.");
				
				if (references != null)
				{
					Assert.AreEqual(1, references.Count, "Wrong number of references returned.");
					
					Assert.IsTrue(references[0].Includes(userID, "Roles"), "The user ID wasn't found on the reference.");
				}
			}
		}
		
		[Test]
		public void Test_GetReferences_2References()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the retrieval of references for an entity.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				TestRole role2 = new TestRole();
				Guid role2ID = role2.ID = Guid.NewGuid();
				role2.Name = "Test Role 2 ";
				
				user.Roles = new TestRole[] {role, role2};
				
				DataAccess.Data.Saver.Save(role);
				DataAccess.Data.Saver.Save(role2);
				DataAccess.Data.Saver.Save(user);
				
				EntityReferenceCollection referenceEntities = DataAccess.Data.Referencer.GetReferences("TestUser", "TestRole");
				
				Assert.AreEqual(2, referenceEntities.Count, "Incorrect number of references found in the store after saving entities.");
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(user.GetType(), user.ID, "Roles", typeof(TestRole), false);
				
				Assert.IsNotNull(references, "The references object returned was null.");
				
				if (references != null)
				{
					Assert.AreEqual(2, references.Count, "Wrong number of references returned.");
					
					EntityIDReference reference1 = references[0];
					EntityIDReference reference2 = references[1];
					
					// Switch the references around if necessary to match (so they can be found in any order)
					if (!reference1.Includes(role.ID, "Users"))
					{
						EntityIDReference tmp = reference1;
						reference1 = reference2;
						reference2 = tmp;
					}
					
					Assert.IsTrue(reference1.Includes(userID, "Roles"), "The user ID wasn't found on the reference.");
					Assert.IsTrue(reference1.Includes(roleID, "Users"), "The role ID wasn't found on the reference.");
					
					Assert.IsTrue(reference2.Includes(userID, "Roles"), "The user ID wasn't found on the second reference.");
					Assert.IsTrue(reference2.Includes(role2ID, "Users"), "The role2 ID wasn't found on the second reference.");
				}
			}
		}
		
		[Test]
		public void Test_GetReferences_FullyActivated()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the retrieval of references for an entity with the entities activated as well.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				Guid roleID = role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Saver.Save(role);
				DataAccess.Data.Saver.Save(user);
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(user.GetType(), user.ID, "Roles", typeof(TestRole), true);
				
				Assert.IsNotNull(references, "The references object returned was null.");
				
				if (references != null)
				{
					Assert.AreEqual(1, references.Count, "Wrong number of references returned.");
					
					Assert.IsTrue(references[0].Includes(userID, "Roles"), "The user ID wasn't found on the reference.");
					Assert.IsTrue(references[0].Includes(roleID, "Users"), "The role ID wasn't found on the reference.");
					
					Assert.IsNotNull(references[0].SourceEntity, "The source entity wasn't activated on the reference.");
					Assert.IsNotNull(references[0].ReferenceEntity, "The source entity wasn't activated on the reference.");
				}
			}
		}

		
		[Test]
		public void Test_GetReference_Async()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetReference function with an asynchronous reference to ensure it retrieves the correct reference.", NLog.LogLevel.Debug))
			{
				EntityFive e5 = new EntityFive();
				e5.ID = Guid.NewGuid();
				e5.Name = "Test Entity 5";
				
				EntitySix e6 = new EntitySix();
				e6.ID = Guid.NewGuid();
				e6.Name = "Test Entity 6";
				
				e5.ReferencedEntities = new EntitySix[] {e6};
				
				DataAccess.Data.Saver.Save(e5);
				DataAccess.Data.Saver.Save(e6);
				
				EntityReference reference = DataAccess.Data.Referencer.GetReference(e5.GetType(),
				                                                                    e5.ID,
				                                                                    "ReferencedEntities",
				                                                                    e6.GetType(),
				                                                                    e6.ID,
				                                                                    String.Empty,
				                                                                    false);
				
				Assert.IsNotNull(reference, "The return value is null.");
				
				Assert.IsTrue(reference.Includes(e5.ID, "ReferencedEntities"), "The returned reference is invalid. (#1)");
				Assert.IsTrue(reference.Includes(e6.ID, ""), "The returned reference is invalid. (#2)");
				
				DataAccess.Data.Deleter.Delete(e5);
				DataAccess.Data.Deleter.Delete(e6);
			}
		}
		
		[Test]
		public void Test_GetReference_Sync()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetReference function with a synchronous reference to ensure it retrieves the correct reference.", NLog.LogLevel.Debug))
			{
				
				TestUtilities.CreateDummyReferences(100);
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestUser user2 = new TestUser();
				user2.ID = Guid.NewGuid();
				user2.FirstName = "Test";
				user2.LastName = "User 2";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				
				TestRole role2 = new TestRole();
				role2.ID = Guid.NewGuid();
				role2.Name = "Test Role 2";
				
				
				user.Roles = new TestRole[] {role, role2};
				
				role2.Users = new TestUser[] {user, user2};
				
				EntityReference originalReference = DataAccess.Data.Referencer.GetActiveReferences(user)[0];
				
				LogWriter.Debug("Original reference - Entity 1 ID: " + originalReference.Entity1ID.ToString());
				LogWriter.Debug("Original reference - Entity 2 ID: " + originalReference.Entity2ID.ToString());
				LogWriter.Debug("Original reference - Property 1 name: " + originalReference.Property1Name);
				LogWriter.Debug("Original reference - Property 2 name: " + originalReference.Property2Name);
				LogWriter.Debug("Original reference - Type 1 name: " + originalReference.Type1Name);
				LogWriter.Debug("Original reference - Type 2 name: " + originalReference.Type2Name);
				
				foreach (EntityIDReference r in DataAccess.Data.Referencer.GetActiveReferences(user))
					DataAccess.Data.Saver.Save(r);
				
				foreach (EntityIDReference r in DataAccess.Data.Referencer.GetActiveReferences(role2))
					DataAccess.Data.Saver.Save(r);
				
				EntityReference reference = DataAccess.Data.Referencer.GetReference(EntityState.GetType(originalReference.Type1Name),
				                                                                    originalReference.Entity1ID,
				                                                                    originalReference.Property1Name,
				                                                                    EntityState.GetType(originalReference.Type2Name),
				                                                                    originalReference.Entity2ID,
				                                                                    originalReference.Property2Name,
				                                                                    false);
				
				Assert.IsNotNull(reference, "The return value is null.");
				
				LogWriter.Debug("Found reference - Entity 1 ID: " + reference.Entity1ID.ToString());
				LogWriter.Debug("Found reference - Entity 2 ID: " + reference.Entity2ID.ToString());
				LogWriter.Debug("Found reference - Property 1 name: " + reference.Property1Name);
				LogWriter.Debug("Found reference - Property 2 name: " + reference.Property2Name);
				LogWriter.Debug("Found reference - Type 1 name: " + reference.Type1Name);
				LogWriter.Debug("Found reference - Type 2 name: " + reference.Type2Name);
				
				Assert.IsTrue(originalReference.Entity1ID.ToString() == reference.Entity1ID.ToString()
				              || originalReference.Entity2ID.ToString() == reference.Entity1ID.ToString(), "Entity 1 ID doesn't match expected.");
				
				Assert.IsTrue(originalReference.Entity2ID.ToString() == reference.Entity2ID.ToString()
				              || originalReference.Entity1ID.ToString() == reference.Entity2ID.ToString(), "Entity 2 ID doesn't match expected.");
				
				
				Assert.IsTrue(originalReference.Type1Name.ToString() == reference.Type1Name.ToString()
				              || originalReference.Type2Name.ToString() == reference.Type1Name.ToString(), "Type 1 name doesn't match expected.");
				
				Assert.IsTrue(originalReference.Type2Name.ToString() == reference.Type2Name.ToString()
				              || originalReference.Type1Name.ToString() == reference.Type2Name.ToString(), "Type 2 name doesn't match expected.");
				
				Assert.IsTrue(originalReference.Property1Name.ToString() == reference.Property1Name.ToString()
				              || originalReference.Property2Name.ToString() == reference.Property1Name.ToString(), "Property 1 name doesn't match expected.");
				
				Assert.IsTrue(originalReference.Property2Name.ToString() == reference.Property2Name.ToString()
				              || originalReference.Property1Name.ToString() == reference.Property2Name.ToString(), "Property 2 name doesn't match expected.");
				
			}
		}
		
		
		[Test]
		public void Test_GetReference_Sync_Exclude()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetReference function with a synchronous reference to ensure it retrieves the correct reference.", NLog.LogLevel.Debug))
			{
				TestUtilities.CreateDummyReferences(100);
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestUser user2 = new TestUser();
				user2.ID = Guid.NewGuid();
				user2.FirstName = "Test";
				user2.LastName = "User 2";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				
				TestRole role2 = new TestRole();
				role2.ID = Guid.NewGuid();
				role2.Name = "Test Role 2";
				
				
				user.Roles = new TestRole[] {role};
				user2.Roles = new TestRole[] { role2 };
				//role2.Users = new TestUser[] {user, user2};
				
				LogWriter.Debug("User 1 ID: " + user.ID.ToString());
				LogWriter.Debug("User 2 ID: " + user2.ID.ToString());
				LogWriter.Debug("Role 1 ID: " + role.ID.ToString());
				LogWriter.Debug("Role 2 ID: " + role2.ID.ToString());
				
				EntityReferenceCollection userReferences = DataAccess.Data.Referencer.GetActiveReferences(user);
				EntityReferenceCollection user2References = DataAccess.Data.Referencer.GetActiveReferences(user2);
				
				Assert.AreEqual(1, userReferences.Count, "userReferences.Length is incorrect");
				Assert.AreEqual(1, user2References.Count, "user2References.Length is incorrect");
				
				EntityReference originalReference1 = userReferences[0];
				EntityReference originalReference2 = user2References[0];
				
				LogWriter.Debug("Original reference - Entity 1 ID: " + originalReference1.Entity1ID.ToString());
				LogWriter.Debug("Original reference - Entity 2 ID: " + originalReference1.Entity2ID.ToString());
				LogWriter.Debug("Original reference - Property 1 name: " + originalReference1.Property1Name);
				LogWriter.Debug("Original reference - Property 2 name: " + originalReference1.Property2Name);
				LogWriter.Debug("Original reference - Type 1 name: " + originalReference1.Type1Name);
				LogWriter.Debug("Original reference - Type 2 name: " + originalReference1.Type2Name);
				
				DataAccess.Data.Saver.Save(user2);
				DataAccess.Data.Saver.Save(role2);
				DataAccess.Data.Saver.Save(user);
				DataAccess.Data.Saver.Save(role);
				
				string referenceStoreName = DataUtilities.GetDataStoreName("TestUser", "TestRole");
				
				EntityReferenceCollection referenceEntities = DataAccess.Data.Referencer.GetReferences(user.GetType().Name, role.GetType().Name);
				
				Assert.AreEqual(2, referenceEntities.Count, "Incorrect number of references found in the store after saving entities.");
				
				// Switch the references around if necessary
				if (referenceEntities[0].Entity1ID == originalReference2.Entity1ID)
				{
					EntityReference r1 = referenceEntities[1];
					EntityReference r2 = referenceEntities[0];
					
					referenceEntities = new EntityReferenceCollection();
					referenceEntities.Add(r1);
					referenceEntities.Add(r2);
				}
				
				bool firstReferenceMatches = (((referenceEntities[0].Includes(originalReference1.Entity1ID, originalReference1.Property1Name)
				                                && referenceEntities[0].Includes(originalReference1.Entity2ID, originalReference1.Property2Name))
				                               ||
				                               ((referenceEntities[0].Includes(originalReference2.Entity1ID, originalReference2.Property1Name)
				                                 && referenceEntities[0].Includes(originalReference2.Entity2ID, originalReference2.Property2Name)))));
				
				
				bool secondReferenceMatches = (((referenceEntities[1].Includes(originalReference1.Entity1ID, originalReference1.Property1Name)
				                                 && referenceEntities[1].Includes(originalReference1.Entity2ID, originalReference1.Property2Name))
				                                ||
				                                ((referenceEntities[1].Includes(originalReference2.Entity1ID, originalReference2.Property1Name)
				                                  && referenceEntities[1].Includes(originalReference2.Entity2ID, originalReference2.Property2Name)))));
				
				Assert.IsTrue(firstReferenceMatches,
				              "First reference doesn't match original references.");
				
				Assert.IsTrue(secondReferenceMatches,
				              "Second reference doesn't match original references.");
			
				EntityReference reference = DataAccess.Data.Referencer.GetReference(user.GetType(),
				                                                                    user.ID,
				                                                                    "Roles",
				                                                                    role.GetType(),
				                                                                    role2.ID,
				                                                                    "Users",
				                                                                    false);
			
				Assert.IsNull(reference, "The return value should be null.");
			}
		}
		
		
		[Test]
		public void Test_MatchReference()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the MatchReference function to ensure matches properly.", NLog.LogLevel.Debug))
			{
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				TestArticle article2 = new TestArticle();
				article2.ID = Guid.NewGuid();
				article2.Title = "Test Article 2";
				
				TestCategory category2 = new TestCategory();
				category2.ID = Guid.NewGuid();
				category2.Name = "Test Category 2";
			
				article.Categories = new TestCategory[] {category};
				
				EntityReference originalReference = DataAccess.Data.Referencer.GetActiveReferences(article)[0];
				
				LogWriter.Debug("Original reference - Entity 1 ID: " + originalReference.Entity1ID.ToString());
				LogWriter.Debug("Original reference - Entity 2 ID: " + originalReference.Entity2ID.ToString());
				LogWriter.Debug("Original reference - Property 1 name: " + originalReference.Property1Name);
				LogWriter.Debug("Original reference - Property 2 name: " + originalReference.Property2Name);
				LogWriter.Debug("Original reference - Type 1 name: " + originalReference.Type1Name);
				LogWriter.Debug("Original reference - Type 2 name: " + originalReference.Type2Name);
				
				foreach (EntityIDReference r in DataAccess.Data.Referencer.GetActiveReferences(article))
					DataAccess.Data.Saver.Save(r);

				string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(article.GetType(),
				                                                                    EntitiesUtilities.GetProperty(article.GetType(), "Categories", typeof(TestCategory[])));
					
				bool match = DataAccess.Data.Referencer.MatchReference(article.GetType(), article.ID, "Categories", category.GetType(), category.ID);
				
				Assert.IsTrue(match, "Didn't match when it should have.");
				
				DataAccess.Data.Deleter.Delete(article);
				DataAccess.Data.Deleter.Delete(category);
			}
		}
		
		
		[Test]
		public void Test_MatchReference_Opposite()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the MatchReference function to ensure matches properly.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				article.Categories = new TestCategory[] { category };
				
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(category);

				bool match = DataAccess.Data.Referencer.MatchReference(article.GetType(), article.ID, "Categories", category.GetType(), category.ID);
				bool match2 = DataAccess.Data.Referencer.MatchReference(category.GetType(), category.ID, "Articles", article.GetType(), article.ID);

				Assert.IsTrue(match, "Didn't match on standard check.");
				Assert.IsTrue(match2, "Didn't match on reverse check.");
				
				
				DataAccess.Data.Deleter.Delete(article);
				DataAccess.Data.Deleter.Delete(category);
			}
		}
		
		[Test]
		public void Test_MatchReference_Exclusion()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the MatchReference function to ensure excludes properly.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				TestArticle article2 = new TestArticle();
				article2.ID = Guid.NewGuid();
				article2.Title = "Test Article 2";
				
				TestCategory category2 = new TestCategory();
				category2.ID = Guid.NewGuid();
				category2.Name = "Test Category 2";
				
				// Must remain commented out to test exclusion
				article.Categories = new TestCategory[] { category };
				article2.Categories = new TestCategory[] { category2 };
				
				DataAccess.Data.Saver.Save(article2);
				DataAccess.Data.Saver.Save(category2);
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(category);
				
				bool match = DataAccess.Data.Referencer.MatchReference(article.GetType(), article.ID, "Categories", category2.GetType(), category2.ID);
				
				Assert.IsFalse(match, "Matched when it shouldn't have.");
				
				DataAccess.Data.Deleter.Delete(article);
				DataAccess.Data.Deleter.Delete(category);
			}
		}

		
		[Test]
		public void Test_GetActiveReferences_Multiple_Async()
		{
			TestGoal goal = new TestGoal();
			goal.ID = Guid.NewGuid();
			
			TestGoal goal2 = new TestGoal();
			goal2.ID = Guid.NewGuid();
			
			goal.Prerequisites = new TestGoal[] {goal2};
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(goal);
			
			Assert.IsNotNull(references, "The reference collection is null.");
			
			Assert.AreEqual(1, references.Count, "Incorrect number of references returned.");
			
			if (references != null)
			{
				Assert.AreEqual(goal.ID, references[0].Entity1ID, "The entity 1 ID wasn't set correctly.");
				Assert.AreEqual(goal2.ID, references[0].Entity2ID, "The entity 2 ID wasn't set correctly.");
				
				Assert.AreEqual("TestGoal", references[0].Type1Name, "The type name 1 wasn't set correctly.");
				Assert.AreEqual("TestGoal", references[0].Type2Name, "The type name 2 wasn't set correctly.");
				
				Assert.AreEqual("Prerequisites", references[0].Property1Name, "The property 1 name wasn't set correctly.");
				Assert.AreEqual(String.Empty, references[0].Property2Name, "The property 2 name wasn't set correctly.");
			}
		}
		
		[Test]
		public void Test_GetActiveReferences_Multiple_Sync()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			
			TestRole role2 = new TestRole();
			role2.ID = Guid.NewGuid();
			
			user.Roles = new TestRole[] {role, role2};
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(user);
			
			Assert.IsNotNull(references, "The reference collection is null.");
			
			Assert.AreEqual(2, references.Count, "Incorrect number of references returned.");
			
			if (references != null && references.Count == 2)
			{
				EntityReference reference = references[0];
				
				Assert.AreEqual(user.ID, references[0].Entity1ID, "The entity 1 ID wasn't set correctly.");
				Assert.AreEqual(role.ID, references[0].Entity2ID, "The entity 2 ID wasn't set correctly.");
				
				Assert.AreEqual("TestUser", references[0].Type1Name, "The type name 1 wasn't set correctly.");
				Assert.AreEqual("TestRole", references[0].Type2Name, "The type name 2 wasn't set correctly.");
				
				Assert.AreEqual("Roles", references[0].Property1Name, "The property 1 name wasn't set correctly.");
				Assert.AreEqual("Users", references[0].Property2Name, "The property 2 name wasn't set correctly.");
				
				EntityReference reference2 = references[1];
				
				Assert.AreEqual(user.ID, reference2.Entity1ID, "The entity 1 ID wasn't set correctly.");
				Assert.AreEqual(role2.ID, reference2.Entity2ID, "The entity 2 ID wasn't set correctly.");
				
				Assert.AreEqual("TestUser", reference2.Type1Name, "The type name 1 wasn't set correctly.");
				Assert.AreEqual("TestRole", reference2.Type2Name, "The type name 2 wasn't set correctly.");
				
				Assert.AreEqual("Roles", reference2.Property1Name, "The property 1 name wasn't set correctly.");
				Assert.AreEqual("Users", reference2.Property2Name, "The property 2 name wasn't set correctly.");
			}
		}
		
		[Test]
		public virtual void Test_SetCountProperties()
		{
			MockEntity entity = new MockEntity();
			entity.ID = Guid.NewGuid();
			
			MockPublicEntity publicEntity = new MockPublicEntity();
			publicEntity.ID = Guid.NewGuid();
			
			entity.PublicEntities = new MockPublicEntity[]{
				publicEntity
			};
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(entity);
			
			DataAccess.Data.Saver.Save(publicEntity);
			DataAccess.Data.Saver.Save(entity);
			
			DataAccess.Data.Referencer.SetCountProperties(references[0]);
									
			Assert.AreEqual(1, entity.TotalPublicEntities, "The TotalPublicEntities property didn't have the expected value.");
		}
	}
}
