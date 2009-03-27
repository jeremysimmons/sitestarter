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
 
namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
    [TestFixture]
    public class DataTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
         //   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
       //     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
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
	public void Test_PreSave_IDsReference()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities IDs references.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntityIDs = new Guid[] {e4.ID};
			//e3.ReferencedEntities = new EntityFour[] {e4};
			// Only one of these needs to be set for this test. The auto preparation should take care of the other one.
            //e4.ReferencedEntities = new EntityThree[] {e3};



		
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.PropertyName = "ID";
		        filter.PropertyValue = e4.ID;
		        filter.AddType(typeof(EntityFour));



			DataAccess.Data.Stores[typeof(TestEntity)].Save(e4);
		
			DataAccess.Data.Stores[typeof(TestEntity)].Save(e3);
		


		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
		        //Collection<EntityFour> foundList = new Collection<EntityFour>(found);
	
			if (found != null)
			{
				Assert.Greater(found.Length, 0, "No results found.");

				EntityFour entity = (EntityFour)found[0];
				Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
				if (entity.ReferencedEntityIDs != null)
				{
					AppLogger.Debug("entity.ReferencedEntityIDs != null");

					Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");
				}
				else
				{
					AppLogger.Debug("entity.ReferencedEntityIDs == null");
				}
			}
			else
				Assert.Fail("No entity found. The save must have failed.");
	
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e3);
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e4);
		}
	}

	[Test]
	public void Test_PreSave_IDsToIDReference()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities IDs references.", NLog.LogLevel.Debug))
		{
	        	TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";

			article.PageIDs = new Guid[] {page.ID};
			//e3.ReferencedEntities = new EntityFour[] {e4};
			// Only one of these needs to be set for this test. The auto preparation should take care of the other one.
            //e4.ReferencedEntities = new EntityThree[] {e3};



		
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.PropertyName = "ID";
		        filter.PropertyValue = page.ID;
		        filter.AddType(typeof(TestArticlePage));



                DataAccess.Data.Stores[typeof(TestArticle)].Save(page);

                DataAccess.Data.Stores[typeof(TestArticle)].Save(article);
		


		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
		        //Collection<EntityFour> foundList = new Collection<EntityFour>(found);
	
			if (found != null)
			{
				Assert.Greater(found.Length, 0, "No results found.");

                TestArticlePage articlePageFound = (TestArticlePage)found[0];
                Assert.IsNotNull(articlePageFound.ArticleID, "The mirror article ID reference property has not been set. The automatic preparation failed.");
                if (articlePageFound.ArticleID != Guid.Empty)
				{
                    AppLogger.Debug("articlePageFound.ArticleID != Guid.Empty");

                    Assert.AreEqual(article.ID, articlePageFound.ArticleID, "The article ID wasn't set to the article page.");
				}
				else
				{
                    AppLogger.Debug("articlePageFound.ArticleID == Guid.Empty");
				}
			}
			else
				Assert.Fail("No entity found. The save must have failed.");

            DataAccess.Data.Stores[typeof(TestArticle)].Delete(page);
            DataAccess.Data.Stores[typeof(TestArticle)].Delete(article);
		}
	}

[Test]
	public void Test_PreSave_IDToIDsReference()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities IDs references.", NLog.LogLevel.Debug))
		{
	        	TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";

			page.ArticleID = article.ID;
			//e3.ReferencedEntities = new EntityFour[] {e4};
			// Only one of these needs to be set for this test. The auto preparation should take care of the other one.
            //e4.ReferencedEntities = new EntityThree[] {e3};



		
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.PropertyName = "ID";
		        filter.PropertyValue = article.ID;
		        filter.AddType(typeof(TestArticle));



	                DataAccess.Data.Stores[typeof(TestArticle)].Save(article);
			
	
	                DataAccess.Data.Stores[typeof(TestArticle)].Save(page);

		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
		        //Collection<EntityFour> foundList = new Collection<EntityFour>(found);
	
			if (found != null)
			{
				Assert.Greater(found.Length, 0, "No results found.");

		                TestArticle articleFound = (TestArticle)found[0];
		                Assert.IsNotNull(articleFound.PageIDs, "The mirror article ID references property has not been set. The automatic preparation failed.");
		                if (articleFound.PageIDs != null)
				{
	                   		 AppLogger.Debug("articleFound.PageIDs != null");
	
	                    			Assert.AreEqual(1, article.PageIDs.Length, "Incorrect number of page IDs.");
				}
				else
				{
                    			AppLogger.Debug("articleFound.PageIDs == null");
				}
			}
			else
				Assert.Fail("No entity found. The save must have failed.");
	
	            //DataAccess.Data.Stores[typeof(TestArticle)].Delete(page);
	            //DataAccess.Data.Stores[typeof(TestArticle)].Delete(article);
		}
	}

      /*  [Test]
        public void Test_PreSave_EntitiesToEntityReference()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities IDs references.", NLog.LogLevel.Debug))
            {
                TestArticle article = new TestArticle();
                article.ID = Guid.NewGuid();
                article.Title = "Test article";


                TestArticlePage page = new TestArticlePage();
                page.ID = Guid.NewGuid();
                page.Title = "Test Page";

                article.Pages = new TestArticlePage[] { page };
                //e3.ReferencedEntities = new EntityFour[] {e4};
                // Only one of these needs to be set for this test. The auto preparation should take care of the other one.
                //e4.ReferencedEntities = new EntityThree[] {e3};




                PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
                filter.PropertyName = "ID";
                filter.PropertyValue = page.ID;
                filter.AddType(typeof(TestArticlePage));



                DataAccess.Data.Stores[typeof(TestArticle)].Save(page);

                DataAccess.Data.Stores[typeof(TestArticle)].Save(article);



                BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
                //Collection<EntityFour> foundList = new Collection<EntityFour>(found);

                if (found != null)
                {
                    Assert.Greater(found.Length, 0, "No results found.");

                    TestArticlePage articlePageFound = (TestArticlePage)found[0];
                    Assert.IsNotNull(articlePageFound.ArticleID, "The mirror article ID reference property has not been set. The automatic preparation failed.");
                    if (articlePageFound.ArticleID != Guid.Empty)
                    {
                        AppLogger.Debug("articlePageFound.ArticleID != Guid.Empty");

                        Assert.AreEqual(article.ID, articlePageFound.ArticleID, "The article ID wasn't set to the article page.");
                    }
                    else
                    {
                        AppLogger.Debug("articlePageFound.ArticleID == Guid.Empty");
                    }
                }
                else
                    Assert.Fail("No entity found. The save must have failed.");

                DataAccess.Data.Stores[typeof(TestArticle)].Delete(page);
                DataAccess.Data.Stores[typeof(TestArticle)].Delete(article);
            }
        }

[Test]
        public void Test_PreSave_EntityToEntitiesReference()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities IDs references.", NLog.LogLevel.Debug))
            {
                TestArticle article = new TestArticle();
                article.ID = Guid.NewGuid();
                article.Title = "Test article";


                TestArticlePage page = new TestArticlePage();
                page.ID = Guid.NewGuid();
                page.Title = "Test Page";

                article.Pages = new TestArticlePage[] { page };
                //e3.ReferencedEntities = new EntityFour[] {e4};
                // Only one of these needs to be set for this test. The auto preparation should take care of the other one.
                //e4.ReferencedEntities = new EntityThree[] {e3};




                PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
                filter.PropertyName = "ID";
                filter.PropertyValue = article.ID;
                filter.AddType(typeof(TestArticle));



                DataAccess.Data.Stores[typeof(TestArticle)].Save(page);

                DataAccess.Data.Stores[typeof(TestArticle)].Save(article);



                BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
                //Collection<EntityFour> foundList = new Collection<EntityFour>(found);

			if (found != null)
			{
				Assert.Greater(found.Length, 0, "No results found.");

		                TestArticle articleFound = (TestArticle)found[0];
		                Assert.IsNotNull(articleFound.PageIDs, "The mirror article ID references property has not been set. The automatic preparation failed.");
		                if (articleFound.PageIDs != null)
				{
	                   		 AppLogger.Debug("articleFound.PageIDs != null");
	
	                    			Assert.AreEqual(1, article.PageIDs.Length, "Incorrect number of page IDs.");
				}
				else
				{
                    			AppLogger.Debug("articleFound.PageIDs == null");
				}
			}
			else
				Assert.Fail("No entity found. The save must have failed.");

                DataAccess.Data.Stores[typeof(TestArticle)].Delete(page);
                DataAccess.Data.Stores[typeof(TestArticle)].Delete(article);
            }
        }*/

        [Test]
        public void Test_PreSave_EntitiesReference()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities references.", NLog.LogLevel.Debug))
            {
                EntityThree e3 = new EntityThree();
                e3.ID = Guid.NewGuid();
                e3.Name = "Test E3";


                EntityFour e4 = new EntityFour();
                e4.ID = Guid.NewGuid();
                e4.Name = "Test E4";

                e3.ReferencedEntities = new EntityFour[] { e4 };
                //e3.ReferencedEntities = new EntityFour[] {e4};
                // Only one of these needs to be set for this test. The auto preparation should take care of the other one.
                //e4.ReferencedEntities = new EntityThree[] {e3};




                PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
                filter.PropertyName = "ID";
                filter.PropertyValue = e4.ID;
                filter.AddType(typeof(EntityFour));



                DataAccess.Data.Stores[typeof(TestEntity)].Save(e4);

                DataAccess.Data.Stores[typeof(TestEntity)].Save(e3);



                BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
                Collection<EntityFour> foundList = new Collection<EntityFour>(found);

                if (found != null)
                {
                    Assert.Greater(foundList.Count, 0, "No results found.");

                    EntityFour entity = (EntityFour)found[0];
                    Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not beed set. The automatic preparation failed.");
                    if (entity.ReferencedEntityIDs != null)
                    {
                        AppLogger.Debug("entity.ReferencedEntityIDs != null");

                        Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");
                    }
                    else
                    {
                        AppLogger.Debug("entity.ReferencedEntities == null");
                    }
                }
                else
                    Assert.Fail("No entity found. The save must have failed.");

                DataAccess.Data.Stores[typeof(TestEntity)].Delete(e3);
                DataAccess.Data.Stores[typeof(TestEntity)].Delete(e4);
            }
        }

    
	#endregion

	#region Filter tests
	[Test]
	public void Test_PropertyFilter()
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
		
			DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
		
		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
	
			if (found != null)
                Assert.Greater(found.Length, 0, "No results found.");

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
		
		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filterGroup);
	
			if (found != null)
				Assert.Greater(found.Length, 0, "No results found.");
	
	
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
		}
	}

	[Test]
	public void Test_FilterGroup_And()
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
		
				DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);
		
		        BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filterGroup);
		        Collection<TestEntity> foundList = new Collection<TestEntity>(found);
	
			if (found != null)
				Assert.AreEqual(foundList.Count, 0, "Matches found when they shouldn't be.");
	
	
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
		}
	}

	[Test]
	public void Test_PropertyFilter_Exclusion()
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
		
		            BaseEntity[] found = (BaseEntity[])DataAccess.Data.GetEntities(filter);
		            Collection<TestEntity> foundList = new Collection<TestEntity>(found);
	
			if (found != null)
				Assert.AreEqual(0, foundList.Count, "Entities weren't properly excluded.");


            		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
		}
	}

	[Test]
	public void Test_GetByPropertyValue()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing data retrieval with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			EntityOne e1 = new EntityOne();
			e1.Name = "Test E1";
	
			DataAccess.Data.Stores[typeof(EntityOne)].Save(e1);

			BaseEntity[] found = (BaseEntity[])DataAccess.Data.Stores[typeof(EntityOne)].GetEntities(typeof(EntityOne), "name", e1.Name);
			Collection<EntityOne> foundList = new Collection<EntityOne>(found);
	
			if (found != null)
				Assert.AreEqual(0, foundList.Count, "No results found.");
	
	
			DataAccess.Data.Stores[typeof(EntityOne)].Delete(e1);
		}
	}


	[Test]
	public void Test_GetByPropertyValue_Exclusion()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing exclusion with the GetEntities by property value function.", NLog.LogLevel.Debug))
		{
			EntityOne e1 = new EntityOne();
			e1.Name = "Test E1";
	
			//FilterGroup filterGroup = new FilterGroup();
			//filterGroup.Operator
	
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.Operator = FilterOperator.Equal;
            filter.PropertyName = "name";
            filter.PropertyValue = "Another Name";
	
			DataAccess.Data.Stores[typeof(EntityOne)].Save(e1);

			BaseEntity[] found = (BaseEntity[])DataAccess.Data.Stores[typeof(EntityOne)].GetEntities(typeof(EntityOne), "name", "Another Name");
			Collection<EntityOne> foundList = new Collection<EntityOne>(found);
	
			if (found != null)
				Assert.AreEqual(0, foundList.Count, "Entities weren't properly excluded.");
	
	
			DataAccess.Data.Stores[typeof(EntityOne)].Delete(e1);
		}
	}
	#endregion

        private void ClearTestEntities()
        {
            Type[] types = new Type[] { typeof(TestEntity), typeof(EntityThree), typeof(EntityFour) };

            Collection<BaseEntity> entities = new Collection<BaseEntity>();
            foreach (Type type in types)
                entities.Add((BaseEntity[])DataAccess.Data.Stores[type].GetEntities(type));

            foreach (BaseEntity entity in entities)
            {
                DataAccess.Data.Stores[entity.GetType()].Delete(entity);
            }
        }


    }
}
