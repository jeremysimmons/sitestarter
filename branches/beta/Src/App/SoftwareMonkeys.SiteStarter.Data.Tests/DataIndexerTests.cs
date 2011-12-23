using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataIndexerTests : BaseDataTestFixture
	{
		[Test]
		public void Test_GetEntitiesByIDArray()
		{
			TestEntity e1 = new TestEntity();
			e1.Name = "Test E1";
			
			DataAccess.Data.Saver.Save(e1);
			
			TestEntity[] entities = DataAccess.Data.Indexer.GetEntities<TestEntity>(new Guid[] {e1.ID});
			
			Assert.IsNotNull(entities, "Entities array was null.");
			
			Assert.AreEqual(1, entities.Length, "Invalid number of entities found");
		}
		
		[Test]
		public void Test_GetEntitiesByPropertyFilter()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				
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

			}
		}
		
		[Test]
		public void Test_GetEntitiesByReferenceFilter()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Saver.Save(role);
				DataAccess.Data.Saver.Save(user);
				
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
						
						Assert.IsNotNull(foundUser, "foundUser == null");
						
						Assert.AreEqual(user.ID, foundUser.ID, "The IDs don't match.");
					}
				}
			}
		}
		
		[Test]
		public void Test_GetEntitiesByReferenceFilter_Exclusion()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{

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

			}
		}
		
		
		[Test]
		public void Test_GetEntitiesByFilterGroup_ReferenceFilterAndPropertyFilter()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				user.Username = "Username";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Saver.Save(role);
				DataAccess.Data.Saver.Save(user);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateReferenceFilter();
				filter.PropertyName = "Roles";
				filter.ReferencedEntityID = role.ID;
				filter.ReferenceType = role.GetType();
				filter.AddType(typeof(TestUser));
				
				PropertyFilter filter2 = (PropertyFilter)DataAccess.Data.CreatePropertyFilter();
				filter2.PropertyName = "Username";
				filter2.PropertyValue = user.Username;
				filter2.AddType(typeof(TestUser));
				
				FilterGroup filterGroup = new FilterGroup();
				filterGroup.Add(filter);
				filterGroup.Add(filter2);
				
				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filterGroup);
				
				Assert.IsNotNull(found, "Null value returned");
				
				if (found != null)
				{
					Assert.IsTrue(found.Length > 0, "No results found.");
					
					if (found.Length > 0)
					{
						TestUser foundUser = (TestUser)found[0];
						
						Assert.IsNotNull(foundUser, "foundUser == null");
						
						Assert.AreEqual(user.ID, foundUser.ID, "The IDs don't match.");
					}
				}
			}
		}
		
		[Test]
		[ExpectedException("System.ArgumentException")]
		public void Test_GetEntitiesByFilterGroup_EmptyGroup()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing a simple query with the PropertyFilter.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				user.Username = "Username";
				
				TestRole role = new TestRole();
				role.ID = Guid.NewGuid();
				role.Name = "Test role";
				
				user.Roles = new TestRole[]{role};
				
				DataAccess.Data.Saver.Save(role);
				DataAccess.Data.Saver.Save(user);
				
				ReferenceFilter filter = (ReferenceFilter)DataAccess.Data.CreateReferenceFilter();
				filter.PropertyName = "Roles";
				filter.ReferencedEntityID = role.ID;
				filter.ReferenceType = role.GetType();
				filter.AddType(typeof(TestUser));
				
				PropertyFilter filter2 = (PropertyFilter)DataAccess.Data.CreatePropertyFilter();
				filter2.PropertyName = "Username";
				filter2.PropertyValue = user.Username;
				filter2.AddType(typeof(TestUser));
				
				FilterGroup filterGroup = new FilterGroup();
				// Leave empty by not adding filters
				//filterGroup.Add(filter);
				//filterGroup.Add(filter2);
				
				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filterGroup);
				
				Assert.IsNotNull(found, "Null value returned");
				
				if (found != null)
				{
					Assert.IsTrue(found.Length > 0, "No results found.");
					
					if (found.Length > 0)
					{
						TestUser foundUser = (TestUser)found[0];
						
						Assert.IsNotNull(foundUser, "foundUser == null");
						
						Assert.AreEqual(user.ID, foundUser.ID, "The IDs don't match.");
					}
				}
			}
		}
		
		[Test]
		public void Test_GetEntitiesMatchReference()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetEntitiesMatchReference function to ensure it finds entities properly.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				article.Categories = new TestCategory[] { category };
				
				DataAccess.Data.Saver.Save(category);
				DataAccess.Data.Saver.Save(article);
				
				IEntity[] results = DataAccess.Data.Indexer.GetEntitiesWithReference<TestArticle>("Categories", typeof(TestCategory), category.ID);
				
				Assert.IsNotNull(results, "The results were null.");
				
				if (results != null)
				{
					Assert.AreEqual(1, results.Length, "Incorrect number of results found.");
					
					IEntity entity = results[0];
					
					Assert.AreEqual(article.GetType().FullName, entity.GetType().FullName, "The types don't match.");
				}
			}
		}
		
		[Test]
		public void Test_GetEntitiesMatchReference_MultipleReferencedEntities()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetEntitiesMatchReference function and passing multiple referenced entities to ensure it finds entities properly.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestArticle article2 = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article 2";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				TestCategory category2 = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category 2";
				
				article.Categories = new TestCategory[] { category2 };
				article2.Categories = new TestCategory[] { category };
				
				DataAccess.Data.Saver.Save(category);
				DataAccess.Data.Saver.Save(category2);
				
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(article2);
				
				IEntity[] categories = new TestCategory[] { category, category2 };
				
				TestArticle[] results = DataAccess.Data.Indexer.GetEntitiesWithReference<TestArticle>("Categories", categories);
				
				Assert.IsNotNull(results, "The results were null.");
				
				
				if (results != null)
				{
					Assert.AreEqual(2, results.Length, "Incorrect number of results found.");
					
					IEntity entity = results[0];
					
					Assert.AreEqual(article.GetType().FullName, entity.GetType().FullName, "The types don't match.");
				}
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesMatchReference_MultipleReferencedEntities_NoMatch()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetEntitiesMatchReference function and passing multiple referenced entities to ensure it finds entities properly.", NLog.LogLevel.Debug))
			{
				TestArticle article = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article";
				
				TestArticle article2 = new TestArticle();
				article.ID = Guid.NewGuid();
				article.Title = "Test Article 2";
				
				TestCategory category = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category";
				
				TestCategory category2 = new TestCategory();
				category.ID = Guid.NewGuid();
				category.Name = "Test Category 2";
				
				
				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(article2);
				DataAccess.Data.Saver.Save(category);
				DataAccess.Data.Saver.Save(category2);
				
				
				IEntity[] categories = new TestCategory[] { category, category2 };
				
				TestArticle[] results = DataAccess.Data.Indexer.GetEntitiesWithReference<TestArticle>("Categories", categories);
				
				Assert.IsNotNull(results, "The results were null.");
				
				
				if (results != null)
				{
					Assert.AreEqual(0, results.Length, "Incorrect number of results found.");
				}
			}
		}
		

		[Test]
		public virtual void Test_GetEntitiesPageMatchReference()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetEntitiesPageMatchReference function to ensure it finds entities properly.", NLog.LogLevel.Debug))
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
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesByPropertyValue()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing data retrieval with the GetEntities by property value function.", NLog.LogLevel.Debug))
			{
				
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Saver.Save(e1);

				IEntity[] found = DataAccess.Data.Indexer.GetEntities<EntityOne>("Name", e1.Name);
				
				Assert.IsNotNull(found, "Null array returned.");
				
				if (found != null)
					Assert.AreEqual(1, found.Length, "No results found.");
				
				
			}
		}


		[Test]
		public void Test_GetEntitiesByPropertyValue_Exclusion()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
			{
				
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
				
			}
		}


		[Test]
		public void Test_GetEntitiesByParameterDictionary()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetEntities<T>(IDictionary<string, object>) function.", NLog.LogLevel.Debug))
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
				
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesByParameterDictionary_Exclude()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the GetEntities<T>(IDictionary<string, object>) function to ensure it excludes entities properly.", NLog.LogLevel.Debug))
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
		public void Test_GetEntitiesPage_Page1_PageSize2_SortAscending()
		{
			TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article C";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Article B";
			
			TestArticle article3 = new TestArticle();
			article3.ID = Guid.NewGuid();
			article3.Title = "Article A";
			
			DataAccess.Data.Saver.Save(article3);
			DataAccess.Data.Saver.Save(article2);
			DataAccess.Data.Saver.Save(article1);
			
			string[] titles = new String[]
			{
				article1.Title,
				article2.Title,
				article3.Title
			};
			
			PagingLocation pagingLocation = new PagingLocation(0, 2);
			
			string sortExpression = "TitleAscending";
			
			TestArticle[] entities = DataAccess.Data.Indexer.GetPageOfEntities<TestArticle>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			foreach (TestArticle a in entities)
			{
				Assert.Greater(Array.IndexOf(titles, a.Title), -1, "The title of one of the retrieved entities doesn't match any of those expected.");
			}
			
			Assert.AreEqual(2, entities.Length, "Invalid number found.");
			
			Assert.AreEqual(article3.Title, entities[0].Title, "Sorting failed #1.");
			Assert.AreEqual(article2.Title, entities[1].Title, "Sorting failed #2.");
			//Assert.AreEqual(article1.Title, entities[1].Title, "Sorting failed #3.");
			
			Assert.AreEqual(3, pagingLocation.AbsoluteTotal, "Invalid total");
			
		}
		
		
		
		
		[Test]
		public void Test_GetEntitiesPage_Page1_PageSize2_SortDescending()
		{
			TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article C";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Article B";
			
			TestArticle article3 = new TestArticle();
			article3.ID = Guid.NewGuid();
			article3.Title = "Article A";
			
			List<string> titles = new List<string>();
			titles.Add(article1.Title);
			titles.Add(article2.Title);
			titles.Add(article3.Title);
			
			List<Guid> ids = new List<Guid>();
			ids.Add(article1.ID);
			ids.Add(article2.ID);
			ids.Add(article3.ID);
			
			DataAccess.Data.Saver.Save(article1);
			DataAccess.Data.Saver.Save(article2);
			DataAccess.Data.Saver.Save(article3);
			
			PagingLocation pagingLocation = new PagingLocation(0, 2);
			
			string sortExpression = "TitleDescending";
			
			TestArticle[] entities = DataAccess.Data.Indexer.GetPageOfEntities<TestArticle>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			List<string> titlesFound = new List<string>();
			List<Guid> idsFound = new List<Guid>();
			
			foreach (TestArticle a in entities)
			{
				Assert.IsFalse(idsFound.Contains(a.ID), "The returned entities list includes more than one entity with the same ID'.");
				
				Assert.IsFalse(titlesFound.Contains(a.Title), "The returned entities list includes more than one entity with the same title: '" + a.Title + "'.");
				
				Assert.IsTrue(titles.Contains(a.Title), "The title of one of the retrieved entities doesn't match any of those expected.");
				
				Assert.IsTrue(ids.Contains(a.ID), "The ID of one of the retrieved entities doesn't match any of those expected.");
				
				titlesFound.Add(a.Title);
				idsFound.Add(a.ID);
			}
			
			Assert.AreEqual(2, entities.Length, "Invalid number found.");
			
			Assert.AreEqual(article1.Title, entities[0].Title, "Sorting failed #1.");
			Assert.AreEqual(article2.Title, entities[1].Title, "Sorting failed #2.");
		}
		
		
		[Test]
		public void Test_GetPageOfEntitiesWithReference_Page1_PageSize1_SortAscending()
		{
			TestCategory category = new TestCategory();
			category.ID = Guid.NewGuid();
			category.Name = "Test Category";
			
			TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article C";
			article1.Categories = new TestCategory[]{category};
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Article B";
			article2.Categories = new TestCategory[]{category};
			
			TestArticle article3 = new TestArticle();
			article3.ID = Guid.NewGuid();
			article3.Title = "Article A";
			article3.Categories = new TestCategory[]{category};
			
			DataAccess.Data.Saver.Save(category);
			DataAccess.Data.Saver.Save(article3);
			DataAccess.Data.Saver.Save(article2);
			DataAccess.Data.Saver.Save(article1);
			
			
			string[] titles = new String[]
			{
				article1.Title,
				article2.Title,
				article3.Title
			};
			
			PagingLocation pagingLocation = new PagingLocation(0, 1);
			
			string sortExpression = "TitleAscending";
			
			TestArticle[] entities = DataAccess.Data.Indexer.GetPageOfEntitiesWithReference<TestArticle>("Categories", typeof(TestCategory), category.ID, pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			foreach (TestArticle a in entities)
			{
				Assert.Greater(Array.IndexOf(titles, a.Title), -1, "The title of one of the retrieved entities doesn't match any of those expected.");
			}
			
			Assert.AreEqual(1, entities.Length, "Invalid number found.");
			
			Assert.AreEqual(article3.Title, entities[0].Title, "Sorting failed #1.");
			//Assert.AreEqual(article2.Title, entities[1].Title, "Sorting failed #2.");
			//Assert.AreEqual(article1.Title, entities[1].Title, "Sorting failed #3.");
			
			Assert.AreEqual(3, pagingLocation.AbsoluteTotal, "Invalid total");
			
		}
		
		[Test]
		public void Test_GetEntitiesPage_Page1_PageSize1_SortAscending()
		{
			TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article C";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Article B";
			
			TestArticle article3 = new TestArticle();
			article3.ID = Guid.NewGuid();
			article3.Title = "Article A";
			
			DataAccess.Data.Saver.Save(article3);
			DataAccess.Data.Saver.Save(article2);
			DataAccess.Data.Saver.Save(article1);
			
			string[] titles = new String[]
			{
				article1.Title,
				article2.Title,
				article3.Title
			};
			
			PagingLocation pagingLocation = new PagingLocation(0, 1);
			
			string sortExpression = "TitleAscending";
			
			TestArticle[] entities = DataAccess.Data.Indexer.GetPageOfEntities<TestArticle>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			foreach (TestArticle a in entities)
			{
				Assert.Greater(Array.IndexOf(titles, a.Title), -1, "The title of one of the retrieved entities doesn't match any of those expected.");
			}
			
			Assert.AreEqual(1, entities.Length, "Invalid number found.");
			
			Assert.AreEqual(article3.Title, entities[0].Title, "Sorting failed #1.");
			//Assert.AreEqual(article2.Title, entities[1].Title, "Sorting failed #2.");
			//Assert.AreEqual(article1.Title, entities[1].Title, "Sorting failed #3.");
			
			Assert.AreEqual(3, pagingLocation.AbsoluteTotal, "Invalid total");
			
		}
		
		
		
		
		[Test]
		public void Test_GetEntitiesPage_Page1_PageSize1_SortDescending()
		{
			TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article C";
			
			TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Article B";
			
			TestArticle article3 = new TestArticle();
			article3.ID = Guid.NewGuid();
			article3.Title = "article A";
			
			List<string> titles = new List<string>();
			titles.Add(article1.Title);
			titles.Add(article2.Title);
			titles.Add(article3.Title);
			
			List<Guid> ids = new List<Guid>();
			ids.Add(article1.ID);
			ids.Add(article2.ID);
			ids.Add(article3.ID);
			
			DataAccess.Data.Saver.Save(article2);
			DataAccess.Data.Saver.Save(article3);
			DataAccess.Data.Saver.Save(article1);
			
			PagingLocation pagingLocation = new PagingLocation(0, 1);
			
			string sortExpression = "TitleDescending";
			
			TestArticle[] entities = DataAccess.Data.Indexer.GetPageOfEntities<TestArticle>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			List<string> titlesFound = new List<string>();
			List<Guid> idsFound = new List<Guid>();
			
			foreach (TestArticle a in entities)
			{
				Assert.IsFalse(idsFound.Contains(a.ID), "The returned entities list includes more than one entity with the same ID'.");
				
				Assert.IsFalse(titlesFound.Contains(a.Title), "The returned entities list includes more than one entity with the same title: '" + a.Title + "'.");
				
				Assert.IsTrue(titles.Contains(a.Title), "The title of one of the retrieved entities doesn't match any of those expected.");
				
				Assert.IsTrue(ids.Contains(a.ID), "The ID of one of the retrieved entities doesn't match any of those expected.");
				
				titlesFound.Add(a.Title);
				idsFound.Add(a.ID);
			}
			
			Assert.AreEqual(1, entities.Length, "Invalid number found.");
			
			Assert.AreEqual(article1.Title, entities[0].Title, "Sorting failed #1.");
			//Assert.AreEqual(article2.Title, entities[1].Title, "Sorting failed #2.");
			//Assert.AreEqual(article3.Title, entities[2].Title, "Sorting failed #3.");
		}
		
		
		[Test]
		public void Test_GetPageOfEntitiesWithReference_EmptyReferencedEntityID_Found()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the index retrieval of entities that don't have any references on a particular property.", NLog.LogLevel.Debug))
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
				
				
				PagingLocation location = new PagingLocation(0, 10);
				string sortExpression = "UsernameAscending";
				
				TestRole[] foundRoles = DataAccess.Data.Indexer.GetPageOfEntitiesWithReference<TestRole>("Users", typeof(TestUser), Guid.Empty, location, sortExpression);
				
				
				
				
				Assert.IsNotNull(foundRoles, "The found roles object returned was null.");
				
				Assert.AreEqual(1, foundRoles.Length, "Invalid number of roles found.");
				
				
			}
		}
		
		
		[Test]
		public void Test_GetPageOfEntitiesWithReference_EmptyReferencedEntityID_NotFound()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing the index retrieval of entities that don't have any references on a particular property.", NLog.LogLevel.Debug))
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
				
				
				PagingLocation location = new PagingLocation(0, 10);
				string sortExpression = "UsernameAscending";
				
				TestRole[] foundRoles = DataAccess.Data.Indexer.GetPageOfEntitiesWithReference<TestRole>("Users", typeof(TestUser), Guid.Empty, location, sortExpression);
				
				
				
				
				Assert.IsNotNull(foundRoles, "The found roles object returned was null.");
				
				Assert.AreEqual(0, foundRoles.Length, "Invalid number of roles found.");
				
				
			}
		}
		
		[Test]
		public virtual void Test_GetEntitiesByType()
		{
			using (LogGroup logGroup = LogGroup.Start("Testing a query of all entities of a specific type.", NLog.LogLevel.Debug))
			{
				TestUser user = new TestUser();
				user.ID = Guid.NewGuid();
				user.FirstName = "Test";
				user.LastName = "User";
				user.Username = "Username";
		
				DataAccess.Data.Saver.Save(user);
				
				IEntity[] entities = DataAccess.Data.Indexer.GetEntities(user.GetType());
				
				Assert.AreEqual(1, entities.Length, "Invalid number found.");
	}
		}
	}
}
