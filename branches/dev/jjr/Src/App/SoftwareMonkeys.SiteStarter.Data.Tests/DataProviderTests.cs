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


        /*[TearDown]
        public void CleanUp()
        {
            ClearTestEntities();

        }*/

	#region Tests
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
				
				
				user.Roles.Add(role);
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				user = null;
				role = null;
				
				
				TestUser user2 = DataAccess.Data.GetEntity<TestUser>("ID", userID);
				
				DataAccess.Data.Activate(user2);
				
				
				
				Assert.IsNotNull(user2.Roles, "The user.Roles property is null.");
				
				if (user2.Roles != null)
				{
					Assert.AreEqual(1, user2.Roles.Count, "Wrong number of roles found.");
					
					Assert.AreEqual(roleID, user2.Roles[0].ID, "ID of referenced role after activating doesn't match the original.");
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
				
				
				user.Roles.Add(role);
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				
				
				EntityReferenceCollection references = DataAccess.Data.GetReferences(user, typeof(TestRole), false);
				
				
				
				
				Assert.IsNotNull(references, "The references object returned was null.");
				
				if (references != null)
				{
					Assert.AreEqual(1, references.Count, "Wrong number of references returned.");
					
					Assert.IsTrue(references[0].Includes(userID), "The user ID wasn't found on the reference.");
					Assert.IsTrue(references[0].Includes(roleID), "The role ID wasn't found on the reference.");
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
				
				
				user.Roles.Add(role);
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				
				
				EntityReferenceCollection references = DataAccess.Data.GetReferences(user, typeof(TestRole), true);
				
				
				
				
				Assert.IsNotNull(references, "The references object returned was null.");
				
				if (references != null)
				{
					Assert.AreEqual(1, references.Count, "Wrong number of references returned.");
					
					Assert.IsTrue(references[0].Includes(userID), "The user ID wasn't found on the reference.");
					Assert.IsTrue(references[0].Includes(roleID), "The role ID wasn't found on the reference.");
					
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
				
				
				user.Roles.Add(role);
				
				DataAccess.Data.Save(user);
				
				DataAccess.Data.Save(role);
				
				user = null;
				role = null;
				
				
				TestRole role2 = DataAccess.Data.GetEntity<TestRole>("ID", roleID);
				
				DataAccess.Data.Activate(role2);
				
				
				
				Assert.IsNotNull(role2.Users, "The role2.Users property is null.");
				
				if (role2.Users != null)
				{
					Assert.AreEqual(1, role2.Users.Count, "Wrong number of users found.");
					
					Assert.AreEqual(userID, role2.Users[0].ID, "ID of referenced user after activating doesn't match the original.");
				}
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
				
				
				user.Roles.Add(role);
				
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
            Type[] types = new Type[] { typeof(TestEntity), typeof(EntityThree), typeof(EntityFour) };

            Collection<IEntity> entities = new Collection<IEntity>();
            foreach (Type type in types)
                entities.Add((IEntity[])DataAccess.Data.GetEntities(type));

            foreach (IEntity entity in entities)
            {
                DataAccess.Data.Stores[entity.GetType()].Delete(entity);
            }
        }


    }
}
