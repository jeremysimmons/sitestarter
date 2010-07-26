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


		// Commented out because it was causing errors with entity deletion
		/*[TearDown]
		public void CleanUp()
		{
			TestUtilities.ClearTestEntities();

		}*/

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

				DataAccess.Data.Saver.Save(article);
				DataAccess.Data.Saver.Save(category);

				EntityReferenceCollection collection = DataAccess.Data.Referencer.GetReferences(article.GetType(), article.ID, "Categories", category.GetType(), false);

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

				
			}
		}

				
		[Test]
		public void Test_IsStored_Reference_NotStored()
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

				//DataAccess.Data.Saver.Save(article);
				//DataAccess.Data.Saver.Save(category);

				EntityReferenceCollection collection = EntitiesUtilities.GetReferences(article);
				//.Data.GetReferences(article.GetType(), article.ID, "Categories", category.GetType(), false);

				Assert.IsNotNull(collection, "Reference collection is null.");

				if (collection != null)
				{
					Assert.AreEqual(1, collection.Count, "Incorrect number of references found.");
				}

				foreach (EntityReference reference in collection)
				{
					bool match = DataAccess.Data.IsStored(reference);

					Assert.AreEqual(false, match, "Reference matched when it shouldn't have.");
				}
			}
		}

		
		#endregion
		
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
				
				DataAccess.Data.Saver.Save(user);
				
				TestUser user2 = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
				
				
		
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
	
			DataAccess.Data.Saver.Save(e4);

			DataAccess.Data.Saver.Save(e3);

			IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntitiesContainingReverseReferences(e4, property);
			
			Assert.IsNotNull(found, "Null array returned.");
			
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(e3.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Deleter.Delete(e3);
			DataAccess.Data.Deleter.Delete(e4);
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

			DataAccess.Data.Saver.Save(page);

			DataAccess.Data.Saver.Save(article);

			IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntitiesContainingReverseReferences(page, property);
			
			Assert.IsNotNull(found, "Null array returned.");
			
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(article.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Deleter.Delete(page);
			DataAccess.Data.Deleter.Delete(article);
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
	

			DataAccess.Data.Saver.Save(article);

			DataAccess.Data.Saver.Save(page);

			IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntitiesContainingReverseReferences(article, property);
			
			Assert.IsNotNull(found, "Null array returned.");
			
			Collection<IEntity> foundList = found != null ? new Collection<IEntity>(found) : new Collection<IEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Wrong number of entities found.");
			Assert.AreEqual(true, foundList.Contains(page.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Deleter.Delete(page);
			DataAccess.Data.Deleter.Delete(article);
		}
	}*/

		
	}
}
