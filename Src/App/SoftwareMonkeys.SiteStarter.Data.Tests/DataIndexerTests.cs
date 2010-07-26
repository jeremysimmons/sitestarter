using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Description of DataIndexerTests.
	/// </summary>
	[TestFixture]
	public class DataIndexerTests
	{
		public DataIndexerTests()
		{
		}
		
		/*[SetUp]
		public void SetUp()
		{
			
			TestUtilities.ClearTestEntities();
		}
		
		[TearDown]
		public void TearDown()
		{
			TestUtilities.ClearTestEntities();
		}*/
	
		
		[Test]
		public void Test_GetEntitiesByPropertyFilter()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUtilities.ClearTestEntities();

				TestEntity e1 = new TestEntity();
				e1.Name = "Test E1";
				
				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter.PropertyName = "Name";
				filter.PropertyValue = e1.Name;
				filter.AddType(typeof(TestEntity));
				
				DataAccess.Data.Saver.Save(e1);
				
				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filter);
				
				Assert.IsNotNull(found, "Null value returned");
				
				if (found != null)
					Assert.IsTrue(found.Length > 0, "No results found.");

				TestUtilities.ClearTestEntities();
			}
		}
		
		[Test]
		public void Test_GetEntitiesByReferenceFilter()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUtilities.ClearTestEntities();
				
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
				
				DataAccess.Data.Saver.Save(user);
				DataAccess.Data.Saver.Save(role);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				filter.PropertyName = "Roles";
				filter.ReferencedEntityID = role.ID;
				filter.ReferenceType = role.GetType();
				filter.AddType(typeof(TestUser));
				
				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filter);
				
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

				TestUtilities.ClearTestEntities();
			}
		}
		
		[Test]
		public void Test_GetEntitiesByReferenceFilter_Exclusion()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUtilities.ClearTestEntities();

				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				// This should remain commented out to check for exclusion
				//user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Saver.Save(user);
				DataAccess.Data.Saver.Save(role);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateFilter(typeof(ReferenceFilter));
				filter.PropertyName = "Roles";
				filter.ReferencedEntityID = role.ID;
				//filter.ReferenceType = role.GetType();
				
				filter.AddType(typeof(TestUser));
				
				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filter);
				
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

				TestUtilities.ClearTestEntities();
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesMatchReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntitiesMatchReference function to ensure it finds entities properly.", NLog.LogLevel.Debug))
			{
				TestUtilities.ClearTestEntities();
				
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				article.Categories = new TestCategory[] { category };
				
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(category);
				
				
				
				IEntity[] results = DataAccess.Data.Indexer.GetEntitiesWithReference<TestArticle>("Categories", typeof(TestCategory), category.ID);
				
				Assert.IsNotNull(results, "The results were null.");
				
				
				if (results != null)
				{
					Assert.AreEqual(1, results.Length, "Incorrect number of results found.");
					
					IEntity entity = results[0];
					
					Assert.AreEqual(article.GetType().FullName, entity.GetType().FullName, "The types don't match.");
				}
				
				DataAccess.Data.Deleter.Delete(article);
				DataAccess.Data.Deleter.Delete(category);
			}
		}
		

		[Test]
		public void Test_GetEntitiesPageMatchReference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntitiesPageMatchReference function to ensure it finds entities properly.", NLog.LogLevel.Debug))
			{
				TestUtilities.ClearTestEntities();
				
				
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
				
				article2.Categories = new TestCategory[] { category, category2 };
				
				DataAccess.Data.Saver.Save(category2);
				DataAccess.Data.Saver.Save(category);
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(article2);
				
				
				PagingLocation location = new PagingLocation(0, 10);
				
				IEntity[] results = DataAccess.Data.Indexer.GetPageOfEntitiesWithReference<TestArticle>("Categories", typeof(TestCategory), category.ID, location, "TitleAscending");
				
				Assert.IsNotNull(results, "The results were null.");
				
				Assert.AreEqual(2, location.AbsoluteTotal, "The absolute total count is incorrect.");
				
				if (results != null)
				{
					Assert.AreEqual(2, results.Length, "Incorrect number of results found.");
					
					IEntity entity = results[0];
					
					Assert.AreEqual(article.GetType().FullName, entity.GetType().FullName, "The types don't match.");
				}
				
				DataAccess.Data.Deleter.Delete(article);
				DataAccess.Data.Deleter.Delete(category);
				DataAccess.Data.Deleter.Delete(article2);
				DataAccess.Data.Deleter.Delete(category2);
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesByPropertyValue()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing data retrieval with the GetEntities by property value function.", NLog.LogLevel.Debug))
			{
				TestUtilities.ClearTestEntities();
				
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Saver.Save(e1);

				IEntity[] found = DataAccess.Data.Indexer.GetEntities<EntityOne>("Name", e1.Name);
				
				Assert.IsNotNull(found, "Null array returned.");
				
				if (found != null)
					Assert.AreEqual(1, found.Length, "No results found.");
				
				
				TestUtilities.ClearTestEntities();
			}
		}


		[Test]
		public void Test_GetEntitiesByPropertyValue_Exclusion()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
			{
				
				TestUtilities.ClearTestEntities();
				
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				//FilterGroup filterGroup = new FilterGroup();
				//filterGroup.Operator
				
				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
				filter.Operator = FilterOperator.Equal;
				filter.PropertyName = "Name";
				filter.PropertyValue = "Another Name";
				
				DataAccess.Data.Saver.Save(e1);

				IEntity[] found = DataAccess.Data.Indexer.GetEntities<EntityOne>("Name", "Another Name");
				
				Assert.IsNotNull(found, "Null array returned.");
				
				if (found != null)
					Assert.AreEqual(0, found.Length, "Entities weren't properly excluded.");
				
				
				TestUtilities.ClearTestEntities();
			}
		}


		[Test]
		public void Test_GetEntitiesByParameterDictionary()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntities<T>(IDictionary<string, object>) function.", NLog.LogLevel.Debug))
			{
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Saver.Save(e1);
				
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add("Name", "Test E1");

				EntityOne[] found = (EntityOne[])DataAccess.Data.Indexer.GetEntities<EntityOne>(parameters);
				
				Assert.IsNotNull(found, "The return value is null.");
				
				if (found != null)
					Assert.AreEqual(1, found.Length, "Entities weren't retrieved properly.");
				
				
				DataAccess.Data.Deleter.Delete(e1);
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesByParameterDictionary_Exclude()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntities<T>(IDictionary<string, object>) function to ensure it excludes entities properly.", NLog.LogLevel.Debug))
			{
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Saver.Save(e1);
				
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add("Name", "Test E2");

				EntityOne[] found = (EntityOne[])DataAccess.Data.Indexer.GetEntities<EntityOne>(parameters);
				
				Assert.IsNotNull(found, "The return value is null.");
				
				if (found != null)
					Assert.AreEqual(0, found.Length, "Entities weren't properly excluded.");
				
				
				DataAccess.Data.Deleter.Delete(e1);
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesPage_Page1_SortAscending()
		{
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			TestUtilities.ClearTestEntities();
			
			// Create the dummy objects
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "C";
			user.LastName = "User 1";
			
			TestUser user2 = new TestUser();
			Guid user2ID = user2.ID = Guid.NewGuid();
			user2.FirstName = "B";
			user2.LastName = "User 2";
			
			
			TestUser user3 = new TestUser();
			Guid user3ID = user3.ID = Guid.NewGuid();
			user3.FirstName = "A";
			user3.LastName = "User 3";
			
			DataAccess.Data.Saver.Save(user);
			DataAccess.Data.Saver.Save(user2);
			DataAccess.Data.Saver.Save(user3);
			
			PagingLocation pagingLocation = new PagingLocation(0, 10);
			
			string sortExpression = "FirstNameAscending";
			
			TestUser[] entities = DataAccess.Data.Indexer.GetPageOfEntities<TestUser>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			Assert.AreEqual(3, entities.Length, "Invalid number found.");
			
			Assert.AreEqual("A", entities[0].FirstName, "Sorting failed #1.");
			Assert.AreEqual("B", entities[1].FirstName, "Sorting failed #2.");
			Assert.AreEqual("C", entities[2].FirstName, "Sorting failed #3.");
			
			TestUtilities.ClearTestEntities();
		}
		
		
		
		
		[Test]
		public void Test_GetEntitiesPage_Page1_SortDescending()
		{
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			TestUtilities.ClearTestEntities();
			
			// Create the dummy objects
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "A";
			user.LastName = "User 1";
			
			TestUser user2 = new TestUser();
			Guid user2ID = user2.ID = Guid.NewGuid();
			user2.FirstName = "B";
			user2.LastName = "User 2";
			
			
			TestUser user3 = new TestUser();
			Guid user3ID = user3.ID = Guid.NewGuid();
			user3.FirstName = "C";
			user3.LastName = "User 3";
			
			DataAccess.Data.Saver.Save(user);
			DataAccess.Data.Saver.Save(user2);
			DataAccess.Data.Saver.Save(user3);
			
			PagingLocation pagingLocation = new PagingLocation(0, 10);
			
			string sortExpression = "FirstNameDescending";
			
			TestUser[] entities = DataAccess.Data.Indexer.GetPageOfEntities<TestUser>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			Assert.AreEqual(3, entities.Length, "Invalid number found.");
			
			Assert.AreEqual("C", entities[0].FirstName, "Sorting failed #1.");
			Assert.AreEqual("B", entities[1].FirstName, "Sorting failed #2.");
			Assert.AreEqual("A", entities[2].FirstName, "Sorting failed #3.");
			
			TestUtilities.ClearTestEntities();
		}
	}
}
