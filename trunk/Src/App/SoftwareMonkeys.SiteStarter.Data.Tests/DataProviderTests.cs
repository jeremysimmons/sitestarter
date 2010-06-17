using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataProviderTests
	{
		public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
		}
		
		public DataProviderTests()
		{
			
			TestUtilities.RegisterTestEntities();
		}

		#region Singleton tests
		[Test]
		public void Test_Data_EnsureInitialized()
		{
			DataProvider provider = DataAccess.Data;

			Assert.IsNotNull((object)provider);
		}
		#endregion


		[TearDown]
		public void CleanUp()
		{
			ClearTestEntities();

		}

		#region Tests
		
		[Test]
		public void Test_IsStored_Reference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the IsStored function on a reference object", NLog.LogLevel.Debug))
			{
				TestUser.RegisterType();
				TestRole.RegisterType();

				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";

				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";

				article.Categories = new TestCategory[] { category };

				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);

				EntityReferenceCollection collection = DataAccess.Data.GetReferences(article.GetType(), article.ID, "Categories", category.GetType(), false);

				Assert.IsNotNull(collection, "Reference collection is null.");

				if (collection != null)
				{
					Assert.AreEqual(1, collection.Count, "Incorrect number of references found.");
				}

				foreach (EntityReference reference in collection)
				{
					bool match = DataAccess.Data.IsStored(reference);

					Assert.AreEqual(true, match, "Reference wasn't detected.");
				}

				
				/*EntityReference reference = new EntityReference();
				EntityReference reference2 = new EntityReference();
				reference.ID =  Guid.NewGuid();
				reference2.ID = Guid.NewGuid();
				reference.Type1Name = reference2.Type2Name = "TestUser";
				reference2.Type1Name = reference.Type2Name = "TestRole";
				reference.Property1Name = reference2.Property2Name = "Roles";
				reference2.Property1Name = reference.Property2Name = "Users";
				reference.Entity1ID = reference2.Entity2ID = Guid.NewGuid();
				reference2.Entity1ID = reference.Entity2ID = Guid.NewGuid();
				 */
				/*TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";*/
				
				//TestRole role = new TestRole();
				//Guid roleID = role.ID = Guid.NewGuid();
				//role.Name = "Test Role";
				
				
				//user.Roles.Add(role);
				
				/*DataAccess.Data.Save(reference);
				
				bool isStored = DataAccess.Data.IsStored(reference2);
				
				//DataAccess.Data.Save(role);
				
				
				
				
				Assert.IsTrue(isStored, "The reference wasn't detected.");*/
				
			}
		}
		
		[Test]
		public void Test_GetEntity_Generic_ByPropertyValue()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser.RegisterType();
				TestRole.RegisterType();
				
				
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				//TestRole role = new TestRole();
				//Guid roleID = role.ID = Guid.NewGuid();
				//role.Name = "Test Role";
				
				
				//user.Roles.Add(role);
				
				DataAccess.Data.Save(user);
				
				//DataAccess.Data.Save(role);
				
				user = null;
				//role = null;
				
				
				TestUser user2 = DataAccess.Data.GetEntity<TestUser>("ID", userID);
				
				
				Assert.IsNotNull(user2, "The user object is null.");
				
			}
		}
		
		
		[Test]
		public void Test_Activate()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
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
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				user = null;
				role = null;
				
				
				TestUser user2 = DataAccess.Data.GetEntity<TestUser>("ID", userID);
				
				DataAccess.Data.Activate(user2);
				
				
				
				Assert.IsNotNull(user2.Roles, "The user.Roles property is null.");
				
				if (user2.Roles != null)
				{
					Assert.AreEqual(1, user2.Roles.Length, "Wrong number of roles found.");
					
					Assert.AreEqual(roleID, user2.Roles[0].ID, "ID of referenced role after activating doesn't match the original.");
				}
			}
		}
		
		[Test]
		public void Test_Activate_SingleReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestArticle.RegisterType();
				TestArticlePage.RegisterType();
				
				
				TestArticle article = new TestArticle();
				Guid articleID = article.ID = Guid.NewGuid();
				article.Title = "Test";
				
				TestArticlePage page = new TestArticlePage();
				Guid pageID = page.ID = Guid.NewGuid();
				page.Title = "Test Page";
				
				article.Pages = new TestArticlePage[] {page};
				//user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Save(page);
				
				DataAccess.Data.Save(article);
				
				page = null;
				article = null;
				
				
				TestArticlePage page2 = DataAccess.Data.GetEntity<TestArticlePage>("ID", pageID);
				
				DataAccess.Data.Activate(page2, "Article");
				
				
				
				Assert.IsNotNull(page2.Article, "The page2.Article property is null.");
				
				if (page2.Article != null)
				{
					
					Assert.AreEqual(articleID, page2.Article.ID, "ID of referenced article after activating doesn't match the original.");
				}
			}
		}

		[Test]
		public void Test_GetReferences_Basic()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the retrieval of references for an entity.", NLog.LogLevel.Debug))
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
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				
				
				EntityReferenceCollection references = DataAccess.Data.GetReferences(user.GetType(), user.ID, "Roles", typeof(TestRole), false);
				
				
				
				
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
		public void Test_GetReferences_FullyActivated()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the retrieval of references for an entity with the entities activated as well.", NLog.LogLevel.Debug))
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
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				
				
				EntityReferenceCollection references = DataAccess.Data.GetReferences(user.GetType(), user.ID, "Roles", typeof(TestRole), true);
				
				
				
				
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
		public void Test_Activate_ReverseReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser.RegisterType();
				TestRole.RegisterType();
				
				
				TestUser user = new TestUser();
				Guid userID = user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				Guid roleID = role.ID;
				role.Name = "Test Role";
				
				
				user.Roles = Collection<TestRole>.Add(user.Roles, role);
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				user = null;
				role = null;
				
				
				TestRole role2 = DataAccess.Data.GetEntity<TestRole>("ID", roleID);
				
				DataAccess.Data.Activate(role2);
				
				
				
				Assert.IsNotNull(role2.Users, "The role2.Users property is null.");
				
				if (role2.Users != null)
				{
					Assert.AreEqual(1, role2.Users.Length, "Wrong number of users found.");
					
					Assert.AreEqual(userID, role2.Users[0].ID, "ID of referenced user after activating doesn't match the original.");
				}
			}
		}
		
		[Test]
		public void Test_GetReference_Async()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReference function with an asynchronous reference to ensure it retrieves the correct reference.", NLog.LogLevel.Debug))
			{
				TestAction.RegisterType();
				TestCategory.RegisterType();
				
				TestAction action = new TestAction();
				action.ID = Guid.NewGuid();
				action.Title = "Test Action";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				action.Categories = new TestCategory[] {category};
				
				DataAccess.Data.Save(action);
				DataAccess.Data.Save(category);
				
				EntityReference reference = DataAccess.Data.GetReference(action.GetType(),
				                                                         action.ID,
				                                                         "Categories",
				                                                         category.GetType(),
				                                                         category.ID,
				                                                         String.Empty,
				                                                         false);
				
				
				Assert.IsNotNull(reference, "The return value is null.");
				
				Assert.IsTrue(reference.Includes(action.ID, "Categories"), "The returned reference is invalid. (#1)");
				Assert.IsTrue(reference.Includes(category.ID, ""), "The returned reference is invalid. (#2)");
				
				DataAccess.Data.Delete(action);
				DataAccess.Data.Delete(category);
			}
		}
		
		[Test]
		public void Test_GetReference_Sync()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReference function with a synchronous reference to ensure it retrieves the correct reference.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				CreateDummyReferences(100);
				
				TestUser.RegisterType();
				TestRole.RegisterType();
				
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
				
				EntityReference originalReference = EntitiesUtilities.GetReferences(user)[0];
				
				AppLogger.Debug("Original reference - Entity 1 ID: " + originalReference.Entity1ID.ToString());
				AppLogger.Debug("Original reference - Entity 2 ID: " + originalReference.Entity2ID.ToString());
				AppLogger.Debug("Original reference - Property 1 name: " + originalReference.Property1Name);
				AppLogger.Debug("Original reference - Property 2 name: " + originalReference.Property2Name);
				AppLogger.Debug("Original reference - Type 1 name: " + originalReference.Type1Name);
				AppLogger.Debug("Original reference - Type 2 name: " + originalReference.Type2Name);
				
				foreach (EntityIDReference r in EntitiesUtilities.GetReferences(user))
					DataAccess.Data.Save(r);
				
				foreach (EntityIDReference r in EntitiesUtilities.GetReferences(role2))
					DataAccess.Data.Save(r);
				
				//DataAccess.Data.Save(user);
				//DataAccess.Data.Save(role);
				
				EntityReference reference = DataAccess.Data.GetReference(EntitiesUtilities.GetType(originalReference.Type1Name),
				                                                         originalReference.Entity1ID,
				                                                         originalReference.Property1Name,
				                                                         EntitiesUtilities.GetType(originalReference.Type2Name),
				                                                         originalReference.Entity2ID,
				                                                         originalReference.Property2Name,
				                                                         false);
				
				
				Assert.IsNotNull(reference, "The return value is null.");
				
				AppLogger.Debug("Found reference - Entity 1 ID: " + reference.Entity1ID.ToString());
				AppLogger.Debug("Found reference - Entity 2 ID: " + reference.Entity2ID.ToString());
				AppLogger.Debug("Found reference - Property 1 name: " + reference.Property1Name);
				AppLogger.Debug("Found reference - Property 2 name: " + reference.Property2Name);
				AppLogger.Debug("Found reference - Type 1 name: " + reference.Type1Name);
				AppLogger.Debug("Found reference - Type 2 name: " + reference.Type2Name);
				
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
				
				//IsTrue(reference.Includes(user.ID, "Roles"), "The returned reference is invalid. (#1)");
				
//				Assert.IsTrue(reference.Includes(user.ID, "Roles"), "The returned reference is invalid. (#1)");
//				Assert.IsTrue(reference.Includes(role.ID, "Users"), "The returned reference is invalid. (#2)");
				
				DataAccess.Data.Delete(user);
				DataAccess.Data.Delete(role);
			}
		}
		
		
		
		[Test]
		public void Test_GetEntitiesPageMatchReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntitiesPageMatchReference function to ensure it finds entities properly.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				
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
				
				article.Categories = new TestCategory[] { category, category2 };
				
				category.Articles = new TestArticle[] { article, article2 };
				
				DataAccess.Data.Save(article2);
				DataAccess.Data.Save(category2);
				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);
				
				
				PagingLocation location = new PagingLocation(0, 10);
				
				IEntity[] results = DataAccess.Data.GetEntitiesPageMatchReference<TestArticle>("Categories", typeof(TestCategory), category.ID, location, "NameAscending");
				
				Assert.IsNotNull(results, "The results were null.");
				
				Assert.AreEqual(1, location.AbsoluteTotal, "The absolute total count is incorrect.");
				
				if (results != null)
				{
					Assert.AreEqual(1, results.Length, "Incorrect number of results found.");
					
					IEntity entity = results[0];
					
					Assert.AreEqual(article.GetType().FullName, entity.GetType().FullName, "The types don't match.");
				}
				
				DataAccess.Data.Delete(article);
				DataAccess.Data.Delete(category);
				DataAccess.Data.Delete(article2);
				DataAccess.Data.Delete(category2);
			}
		}
		
		[Test]
		public void Test_GetEntitiesMatchReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntitiesMatchReference function to ensure it finds entities properly.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				article.Categories = new TestCategory[] { category };
				
				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);
				
				
				
				IEntity[] results = DataAccess.Data.GetEntitiesMatchReference<TestArticle>("Categories", typeof(TestCategory), category.ID);
				
				Assert.IsNotNull(results, "The results were null.");
				
				
				if (results != null)
				{
					Assert.AreEqual(1, results.Length, "Incorrect number of results found.");
					
					IEntity entity = results[0];
					
					Assert.AreEqual(article.GetType().FullName, entity.GetType().FullName, "The types don't match.");
				}
				
				DataAccess.Data.Delete(article);
				DataAccess.Data.Delete(category);
			}
		}
		
		[Test]
		public void Test_MatchReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the MatchReference function to ensure matches properly.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
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
				
				//article.Categories = new TestCategory[] { category };
				
				
				
				article.Categories = new TestCategory[] {category};//, category2};
				
				//category2.Articles = new TestArticle[] {article, article2};
				
				EntityReference originalReference = EntitiesUtilities.GetReferences(article)[0];
				
				AppLogger.Debug("Original reference - Entity 1 ID: " + originalReference.Entity1ID.ToString());
				AppLogger.Debug("Original reference - Entity 2 ID: " + originalReference.Entity2ID.ToString());
				AppLogger.Debug("Original reference - Property 1 name: " + originalReference.Property1Name);
				AppLogger.Debug("Original reference - Property 2 name: " + originalReference.Property2Name);
				AppLogger.Debug("Original reference - Type 1 name: " + originalReference.Type1Name);
				AppLogger.Debug("Original reference - Type 2 name: " + originalReference.Type2Name);
				
				foreach (EntityIDReference r in EntitiesUtilities.GetReferences(article))
					DataAccess.Data.Save(r);
				
				//foreach (EntityIDReference r in EntitiesUtilities.GetReferences(category2))
				//	DataAccess.Data.Save(r);
				
				
				/*
				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);*/
				
				
				string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(article.GetType(),
				                                                                    EntitiesUtilities.GetProperty(article.GetType(), "Categories", typeof(TestCategory[])));
				
				
				bool match = DataAccess.Data.MatchReference(article.GetType(), article.ID, "Categories", category.GetType(), category.ID);
				
				Assert.IsTrue(match, "Didn't match when it should have.");
				
				
				DataAccess.Data.Delete(article);
				DataAccess.Data.Delete(category);
			}
		}
		
		
		[Test]
		public void Test_MatchReference_Opposite()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the MatchReference function to ensure matches properly.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				article.Categories = new TestCategory[] { category };
				
				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);
				
				
				//string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(category.GetType(),
				//                                                                    EntitiesUtilities.GetProperty(category.GetType(), "Articles", typeof(TestArticle[])));

				
				bool match = DataAccess.Data.MatchReference(article.GetType(), article.ID, "Categories", category.GetType(), category.ID);
				bool match2 = DataAccess.Data.MatchReference(category.GetType(), category.ID, "Articles", article.GetType(), article.ID);

				Assert.IsTrue(match, "Didn't match on standard check.");
				Assert.IsTrue(match2, "Didn't match on reverse check.");
				
				
				DataAccess.Data.Delete(article);
				DataAccess.Data.Delete(category);
			}
		}
		
		[Test]
		public void Test_MatchReference_Exclusion()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the MatchReference function to ensure excludes properly.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				// Must remain commented out to test exclusion
				//article.Categories = new TestCategory[] { category };
				
				DataAccess.Data.Save(article);
				DataAccess.Data.Save(category);
				
				
				//string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(article.GetType(),
				//                                                                    EntitiesUtilities.GetProperty(article.GetType(), "Categories", typeof(TestCategory[])));

				
				bool match = DataAccess.Data.MatchReference(article.GetType(), article.ID, "Categories", category.GetType(), category.ID);
				
				Assert.IsFalse(match, "Matched when it shouldn't have.");
				
				
				DataAccess.Data.Delete(article);
				DataAccess.Data.Delete(category);
			}
		}
		
		
		
		[Test]
		public void Test_GetEntity_ByTypeAndPropertyValue()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
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
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				user = null;
				role = null;
				
				
				TestRole role2 = (TestRole)DataAccess.Data.GetEntity(typeof(TestRole), "ID", roleID);
				
				
				
				Assert.IsNotNull(role2, "role2 == null");
				
				if (role2 != null)
				{
					
					Assert.AreEqual(roleID, role2.ID, "ID of referenced user after activating doesn't match the original.");
				}
			}
		}
		
		#region Filter tests
		[Test]
		public void Test_GetEntitiesByPropertyFilter()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();

				TestEntity e1 = new TestEntity();
				e1.Name = "Test E1";
				
				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter.PropertyName = "Name";
				filter.PropertyValue = e1.Name;
				filter.AddType(typeof(TestEntity));
				
				DataAccess.Data.Save(e1);
				
				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filter);
				
				Assert.IsNotNull(found, "Null value returned");
				
				if (found != null)
					Assert.IsTrue(found.Length > 0, "No results found.");

				ClearTestEntities();
			}
		}
		
		[Test]
		public void Test_GetEntitiesByReferenceFilter()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();
				
				TestUser.RegisterType();
				TestRole.RegisterType();

				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Save(user);
				DataAccess.Data.Save(role);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				filter.PropertyName = "Roles";
				filter.ReferencedEntityID = role.ID;
				filter.ReferenceType = role.GetType();
				filter.AddType(typeof(TestUser));
				
				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filter);
				
				Assert.IsNotNull(found, "Null value returned");
				
				if (found != null)
				{
					Assert.IsTrue(found.Length > 0, "No results found.");
					
					if (found.Length > 0)
					{
						TestUser foundUser = (TestUser)found[0];
						
						Assert.AreEqual(user.ID, foundUser.ID, "The IDs don't match.");
					}
				}
				
				
				//TestUser.DeregisterType();
				//TestRole.DeregisterType();

				ClearTestEntities();
			}
		}
		
		[Test]
		public void Test_GetEntitiesByReferenceFilter_Exclusion()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				ClearTestEntities();

				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				// This should remain commented out to check for exclusion
				//user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Save(user);
				DataAccess.Data.Save(role);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				filter.PropertyName = "Roles";
				filter.ReferencedEntityID = role.ID;
				//filter.ReferenceType = role.GetType();
				
				filter.AddType(typeof(TestUser));
				
				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filter);
				
				Assert.IsNotNull(found, "Null value returned");
				
				if (found != null)
				{
					Assert.IsTrue(found.Length == 0, "Results returned when none should have been returned.");
					
					/*if (found.Length > 0)
					{
						TestUser foundUser = (TestUser)found[0];
						
						Assert.AreEqual(user.ID, foundUser.ID, "The IDs don't match.");
					}*/
				}

				ClearTestEntities();
			}
		}

		[Test]
		public void Test_FilterGroup_Or()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a filter group query with the OR operator.", NLog.LogLevel.Debug))
			{
				TestEntity e1 = new TestEntity();
				e1.Name = "Test E1";
				
				FilterGroup filterGroup = new FilterGroup();
				filterGroup.Operator = FilterOperator.Or;
				
				PropertyFilter filter1 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter1.PropertyName = "Name";
				filter1.PropertyValue = e1.Name;
				filter1.AddType(typeof(TestEntity));
				
				PropertyFilter filter2 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter2.PropertyName = "Name";
				filter2.PropertyValue = "Another value";
				filter2.AddType(typeof(TestEntity));
				
				filterGroup.Filters = new BaseFilter[] {filter1, filter2};
				
				DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
				
				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filterGroup);
				
				Assert.IsNotNull(found, "Null value returned.");
				
				if (found != null)
					Assert.IsTrue(found.Length > 0, "No results found.");
				
				
				DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
			}
		}

		[Test]
		public void Test_FilterGroup_And_Exclusion()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a filter group query with the AND operator.", NLog.LogLevel.Debug))
			{
				TestEntity e1 = new TestEntity();
				e1.Name = "Test E1";
				
				FilterGroup filterGroup = new FilterGroup();
				filterGroup.Operator = FilterOperator.And;
				
				PropertyFilter filter1 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter1.PropertyName = "Name";
				filter1.PropertyValue = e1.Name;
				filter1.AddType(typeof(TestEntity));
				
				PropertyFilter filter2 = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter2.PropertyName = "Name";
				filter2.PropertyValue = "Another value";
				filter2.AddType(typeof(TestEntity));
				
				filterGroup.Filters = new BaseFilter[] {filter1, filter2};
				
				DataAccess.Data.Save(e1);
				
				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filterGroup);
				
				Assert.IsNotNull(found, "Null array returned.");
				
				Collection<TestEntity> foundList = new Collection<TestEntity>(found);
				
				if (found != null)
					Assert.AreEqual(0, foundList.Count, "Entity retrieved when it shouldn't have matched.");
				
				
				DataAccess.Data.Delete(e1);
			}
		}

		[Test]
		public void Test_GetEntitiesByPropertyFilter_Exclusion()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestEntity e1 = new TestEntity();
				e1.ID = Guid.NewGuid();
				e1.Name = "Test E1";

				AppLogger.Debug("Entity ID: " + e1.ID);
				AppLogger.Debug("Entity name: " + e1.Name);
				
				//FilterGroup filterGroup = new FilterGroup();
				//filterGroup.Operator
				
				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter.Operator = FilterOperator.Equal;
				filter.PropertyName = "Name";
				filter.PropertyValue = "Another Name";
				filter.AddType(typeof(TestEntity));
				
				
				AppLogger.Debug("Filter.FieldName: " + filter.PropertyName);
				AppLogger.Debug("Filter.FieldValue: " + filter.PropertyValue);
				
				DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
				
				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filter);
				
				Assert.IsNotNull(found, "Null array returned.");
				
				Collection<TestEntity> foundList = new Collection<TestEntity>(found);
				
				if (found != null)
					Assert.AreEqual(0, foundList.Count, "Entities weren't properly excluded.");


				DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
			}
		}
		#endregion
		
		[Test]
		public void Test_Save_EntityIDReference()
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Testing saving of an EntityIDReference.", NLog.LogLevel.Debug))
			{
				TestArticle.RegisterType();
				TestArticlePage.RegisterType();
				
				EntityIDReference reference = new EntityIDReference();
				reference.ID = Guid.NewGuid();
				reference.Type1Name = "TestArticle";
				reference.Type2Name = "TestArticlePage";
				reference.Entity1ID = Guid.NewGuid();
				reference.Entity2ID = Guid.NewGuid();
				reference.Property1Name = "Pages";
				reference.Property2Name = "Article";
				
				DataAccess.Data.Save(reference);
				
				IDataStore store = DataAccess.Data.Stores["Testing_Articles-Testing_Articles"];
				
				Assert.IsNotNull(store, "The data store wasn't created/initialized.");
			}
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
				
				DataAccess.Data.Save(user);
				DataAccess.Data.Save(role);
				
				
				EntityIDReference reference = new EntityIDReference();
				reference.ID = Guid.NewGuid();
				reference.Type1Name = "TestUser";
				reference.Type2Name = "TestRole";
				reference.Entity1ID = user.ID;
				reference.Entity2ID = role.ID;
				reference.Property1Name = "Roles";
				reference.Property2Name = "Users";
				
				TestUser user2 = DataAccess.Data.GetEntity<TestUser>("ID", user.ID);
				
				DataAccess.Data.Activate(user2);
				
				string newFirstName =  "Something else";
				user2.FirstName  = newFirstName;
				
				DataAccess.Data.Update(user2);
				
				TestUser user3 = DataAccess.Data.GetEntity<TestUser>("ID", user.ID);
				
				DataAccess.Data.Activate(user3);
				
				Assert.IsNotNull(user3.Roles);
				
				if (user3.Roles != null)
					Assert.AreEqual(1, user3.Roles.Length, "Incorrect number of roles.");
				Assert.AreEqual(newFirstName, user3.FirstName, "First name mismatch.");
				
				//IDataStore store = DataAccess.Data.Stores["Testing_Articles-Testing_Articles"];
				
				//Assert.IsNotNull(store, "The data store wasn't created/initialized.");
			}
		}
		
		[Test]
		public void Test_Delete_RemoveReferences()
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
				
				DataAccess.Data.Save(user);
				DataAccess.Data.Save(role);
				
				DataAccess.Data.Delete(role);
				
				TestUser user2 = DataAccess.Data.GetEntity<TestUser>("ID", user.ID);
				
				DataAccess.Data.Activate(user2);
				
				Assert.IsNotNull(user2.Roles);
				
				if (user2.Roles != null)
					Assert.AreEqual(0, user2.Roles.Length, "Incorrect number of roles. The role should have been removed.");
				//Assert.AreEqual(newFirstName, user3.FirstName, "First name mismatch.");
				
				//IDataStore store = DataAccess.Data.Stores["Testing_Articles-Testing_Articles"];
				
				//Assert.IsNotNull(store, "The data store wasn't created/initialized.");
			}
		}
		
		/*[Test]
		public void Test_GetEntity_Generic_SingleParameter()
		{
				TestUser.RegisterType();
				TestRole.RegisterType();
				
				
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test Role";
				
				
				user.Roles.Add(role);
				
				DataAccess.Data.Save(user);
				
				TestUser user2 = DataAccess.Data.GetEntity<TestUser>("ID", user.ID);
				
				
		
		}*/
		
		
		
		/*[Test]
	public void Test_GetEntitiesContainingReverseReferences_IDsToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntityIDs = new Guid[] {e4.ID};
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntityIDs");
	
			DataAccess.Data.Save(e4);

			DataAccess.Data.Save(e3);

			IEntity[] found = (IEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(e4, property);
			
			Assert.IsNotNull(found, "Null array returned.");
			
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(e3.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Delete(e3);
			DataAccess.Data.Delete(e4);
		}
	}

	[Test]
	public void Test_GetEntitiesContainingReverseReferences_IDsToID()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";

			article.PageIDs = new Guid[] {page.ID};
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
	//		PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
	//		filter.Operator = FilterOperator.Equal;
        //    filter.PropertyName = "name";
        //    filter.PropertyValue = "Another Name";

			PropertyInfo property = page.GetType().GetProperty("ArticleID");

			DataAccess.Data.Save(page);

			DataAccess.Data.Save(article);

			IEntity[] found = (IEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(page, property);
			
			Assert.IsNotNull(found, "Null array returned.");
			
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(article.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Delete(page);
			DataAccess.Data.Delete(article);
		}
	}

	[Test]
	public void Test_GetEntitiesContainingReverseReferences_IDToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";

			page.ArticleID = article.ID;
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
	//		PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
	//		filter.Operator = FilterOperator.Equal;
        //    filter.PropertyName = "name";
         //   filter.PropertyValue = "Another Name";

			PropertyInfo property = article.GetType().GetProperty("PageIDs");
	

			DataAccess.Data.Save(article);

			DataAccess.Data.Save(page);

			IEntity[] found = (IEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(article, property);
			
			Assert.IsNotNull(found, "Null array returned.");
			
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(page.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Delete(page);
			DataAccess.Data.Delete(article);
		}
	}*/
		#endregion

		private void ClearTestEntities()
		{
			
			TestArticle.RegisterType();
			TestCategory.RegisterType();
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			Type[] types = new Type[] {
				typeof(TestArticle),
				typeof(TestArticlePage),
				typeof(TestCategory),
				typeof(TestEntity),
				typeof(TestSample),
				typeof(TestUser),
				typeof(TestRole),
				typeof(TestEntity),
				typeof(EntityOne),
				typeof(EntityTwo),
				typeof(EntityThree),
				typeof(EntityFour) };

			Collection<IEntity> entities = new Collection<IEntity>();
			foreach (Type type in types)
				entities.Add((IEntity[])DataAccess.Data.GetEntities(type));

			foreach (IEntity entity in entities)
			{
				DataAccess.Data.Stores[entity.GetType()].Delete(entity);
			}
		}

		private void CreateDummyReferences(int count)
		{
			EntityReference reference = new EntityReference();
			reference.Entity1ID = Guid.NewGuid();
			reference.Entity2ID = Guid.NewGuid();
			reference.Property1Name = "TestProperty1-" + Guid.NewGuid().ToString().Substring(0, 5);
			reference.Property2Name = "TestProperty2-" + Guid.NewGuid().ToString().Substring(0, 5);
			reference.Type1Name = "TestUser";
			reference.Type2Name = "TestRole";
		}

	}
}
