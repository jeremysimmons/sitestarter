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
	public void Test_PreSave_IDsToIDsReference()
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

	[Test]
	public void Test_PreUpdate_IDsToIDsReference()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities IDs references.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			EntityFour e42 = new EntityFour();
			e42.ID = Guid.NewGuid();
			e42.Name = "Test E42";

			e3.ReferencedEntityIDs = new Guid[] {e4.ID};
			//e3.ReferencedEntities = new EntityFour[] {e4};
			// Only one of these needs to be set for this test. The auto preparation should take care of the other one.
            //e4.ReferencedEntities = new EntityThree[] {e3};



		
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.PropertyName = "ID";
		        filter.PropertyValue = e3.ID;
		        filter.AddType(typeof(EntityFour));



			DataAccess.Data.Stores[typeof(TestEntity)].Save(e4);
		
			DataAccess.Data.Stores[typeof(TestEntity)].Save(e3);

			DataAccess.Data.Stores[typeof(TestEntity)].Save(e42);
		
			e3.ReferencedEntityIDs = new Guid[] {e42.ID};


		        Collection<BaseEntity> toUpdate = new Collection<BaseEntity>((BaseEntity[])DataAccess.Data.Stores[typeof(EntityFour)].PreUpdate(e3));


		        //BaseEntity[] modi = (BaseEntity[])DataAccess.Data.GetEntities(filter);
		        //Collection<EntityFour> foundList = new Collection<EntityFour>(found);
	
			if (toUpdate.Count > 0)
			{
				// Ensure that the original referenced entity was modified properly
				Assert.AreEqual(true, toUpdate.Contains(e4.ID), "The original referenced entity wasn't properly modified.");

				EntityFour foundOriginal = (EntityFour)toUpdate[e4.ID];

				Assert.AreEqual(0, foundOriginal.ReferencedEntityIDs.Length, "The original referenced entity's obsolete references weren't properly cleared.");

				// Ensure that the new referenced entity was modified properly
				Assert.AreEqual(true, toUpdate.Contains(e42.ID), "The new referenced entity wasn't properly modified.");

				EntityFour foundNew = (EntityFour)toUpdate[e42.ID];

				Assert.AreEqual(1, foundNew.ReferencedEntityIDs.Length, "The new referenced entity's references weren't properly added.");


				/*Assert.Greater(found.Length, 0, "No results found.");

				EntityFour entity = (EntityFour)found[0];
				Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
				if (entity.ReferencedEntityIDs != null)
				{
					AppLogger.Debug("entity.ReferencedEntityIDs != null");

					Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");

					Assert.AreEqual(e42.ID, entity.ReferencedEntityIDs[0], "New reference ID is invalid.");
				}
				else
				{
					AppLogger.Debug("entity.ReferencedEntityIDs == null");
				}*/
			}
			else
				Assert.Fail("No entity found. The save must have failed.");
	
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e3);
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e4);
	       		DataAccess.Data.Stores[typeof(TestEntity)].Delete(e42);
		}
	}

	[Test]
	public void Test_PreUpdate_IDsToIDReference()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities IDs references.", NLog.LogLevel.Debug))
		{
	        	TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";

			TestArticlePage page2 = new TestArticlePage();
			page2.ID = Guid.NewGuid();
			page2.Title = "Test Page 2";

		
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.PropertyName = "ID";
		        filter.PropertyValue = page.ID;
		        filter.AddType(typeof(TestArticlePage));



			article.PageIDs = new Guid[] {page.ID};
			//article.PageIDs = new Guid[] {page.ID};

	                DataAccess.Data.Stores[typeof(TestArticle)].Save(page);
	
	
	                DataAccess.Data.Stores[typeof(TestArticle)].Save(article);
	
	
	                DataAccess.Data.Stores[typeof(TestArticle)].Save(page2);
	
	   		//Collection<BaseEntity> f = new Collection<BaseEntity>((BaseEntity[])DataAccess.Data.GetEntities(filter));
	
			//Assert.AreEqual(1, f.Count, "The original page wasn't saved or can't be retrieved.");
		
			
			article.PageIDs = new Guid[] {page2.ID};


		        Collection<BaseEntity> toUpdate = new Collection<BaseEntity>((BaseEntity[])DataAccess.Data.Stores[typeof(TestArticle)].PreUpdate(article));

	
			if (toUpdate.Count > 0)
			{
				// Ensure that the original referenced entity was modified properly
				Assert.IsTrue(toUpdate.Contains(page.ID), "The original referenced entity wasn't properly modified, or wasn't found in the toUpdate collection.");

				TestArticlePage foundOriginal = (TestArticlePage)toUpdate[page.ID];

				Assert.IsTrue(foundOriginal.ArticleID == null || foundOriginal.ArticleID == Guid.Empty, "The original referenced entity's obsolete references weren't properly cleared.");

				// Ensure that the new referenced entity was modified properly
				Assert.IsTrue(toUpdate.Contains(page2.ID), "The new referenced entity wasn't properly modified.");

				TestArticlePage foundNew = (TestArticlePage)toUpdate[page2.ID];

				Assert.AreEqual(article.ID, foundNew.ArticleID, "The new referenced entity's references weren't properly added.");


				/*Assert.Greater(found.Length, 0, "No results found.");

				EntityFour entity = (EntityFour)found[0];
				Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
				if (entity.ReferencedEntityIDs != null)
				{
					AppLogger.Debug("entity.ReferencedEntityIDs != null");

					Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");

					Assert.AreEqual(e42.ID, entity.ReferencedEntityIDs[0], "New reference ID is invalid.");
				}
				else
				{
					AppLogger.Debug("entity.ReferencedEntityIDs == null");
				}*/
			}
			else
				Assert.Fail("No entity found. The save must have failed.");
	
	       		//DataAccess.Data.Stores[typeof(TestEntity)].Delete(article);
	       		//DataAccess.Data.Stores[typeof(TestEntity)].Delete(page);
	       		//DataAccess.Data.Stores[typeof(TestEntity)].Delete(page2);
		}
	}

	[Test]
	public void Test_PreUpdate_IDToIDsReference()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entity ID references.", NLog.LogLevel.Debug))
		{
	        	TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test article";
			

			TestArticlePage page = new TestArticlePage();
			page.ID = Guid.NewGuid();
			page.Title = "Test Page";


	        	TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Test article 2";
		
			//PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			//filter.PropertyName = "ID";
		       // filter.PropertyValue = page.ID;
		        //filter.AddType(typeof(TestArticlePage));



			page.ArticleID = article.ID;
			//article.PageIDs = new Guid[] {page.ID};

	
	
	                DataAccess.Data.Stores[typeof(TestArticle)].Save(article);
	
	                DataAccess.Data.Stores[typeof(TestArticle)].Save(page);
	
	                DataAccess.Data.Stores[typeof(TestArticle)].Save(article2);
	
	   		//Collection<BaseEntity> f = new Collection<BaseEntity>((BaseEntity[])DataAccess.Data.GetEntities(filter));
	
			//Assert.AreEqual(1, f.Count, "The original page wasn't saved or can't be retrieved.");
		
			
			page.ArticleID = article2.ID;


		        Collection<BaseEntity> toUpdate = new Collection<BaseEntity>((BaseEntity[])DataAccess.Data.Stores[typeof(TestArticle)].PreUpdate(page));

			if (toUpdate != null && toUpdate.Count > 0)
			{
				AppLogger.Debug("# of items to update: " + toUpdate.Count.ToString());	

				// Ensure that the original referenced entity was modified properly
				Assert.AreEqual(true, toUpdate.Contains(article.ID), "The original referenced entity wasn't properly modified.");

				Assert.IsTrue(toUpdate.Contains(article.ID), "The original referenced entity wasn't found in the toUpdate list.");
				if (toUpdate.Contains(article.ID))
				{
					AppLogger.Debug("Old reference found in toUpdate list. As intended.");

					TestArticle foundOriginal = (TestArticle)toUpdate[article.ID];

					Assert.IsTrue(foundOriginal.PageIDs == null || foundOriginal.PageIDs.Length == 0, "The original referenced entity's obsolete references weren't properly cleared.");

					// Ensure that the new referenced entity was modified properly
					Assert.IsTrue(Array.IndexOf(foundOriginal.PageIDs, page.ID) == -1, "The old referenced entity wasn't properly modified.");
				}
				else
					AppLogger.Debug("The old referenced entity wasn't found in the toUpdate list.");


				Assert.IsTrue(toUpdate.Contains(article2.ID), "The new referenced entity wasn't found in the toUpdate list.");

				if (toUpdate.Contains(article2.ID))
				{
					AppLogger.Debug("New reference found in toUpdate list. As intended.");

					TestArticle foundNew = (TestArticle)toUpdate[article2.ID];
					Assert.IsTrue(Array.IndexOf(foundNew.PageIDs, page.ID) > -1, "The new referenced entity wasn't properly added.");
				}
				else
					AppLogger.Debug("The new referenced entity wasn't found in the toUpdate list.");


				/*Assert.Greater(found.Length, 0, "No results found.");

				EntityFour entity = (EntityFour)found[0];
				Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
				if (entity.ReferencedEntityIDs != null)
				{
					AppLogger.Debug("entity.ReferencedEntityIDs != null");

					Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");

					Assert.AreEqual(e42.ID, entity.ReferencedEntityIDs[0], "New reference ID is invalid.");
				}
				else
				{
					AppLogger.Debug("entity.ReferencedEntityIDs == null");
				}*/
			}
			else
				Assert.Fail("No entity found. The save must have failed.");
	
	       		//DataAccess.Data.Stores[typeof(TestEntity)].Delete(article);
	       		//DataAccess.Data.Stores[typeof(TestEntity)].Delete(page);
	       		//DataAccess.Data.Stores[typeof(TestEntity)].Delete(article2);
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

	[Test]
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
	
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.Operator = FilterOperator.Equal;
            filter.PropertyName = "name";
            filter.PropertyValue = "Another Name";

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntityIDs");
	
			DataAccess.Data.Stores[typeof(EntityOne)].Save(e4);

			DataAccess.Data.Stores[typeof(EntityOne)].Save(e3);

			BaseEntity[] found = (BaseEntity[])DataAccess.Data.Stores[typeof(EntityThree)].GetEntitiesContainingReverseReferences(e4, property);
			Collection<BaseEntity> foundList = found != null ? new Collection<BaseEntity>(found) : new Collection<BaseEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Entities weren't retrieved correctly.");
			Assert.AreEqual(true, foundList.Contains(e3.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Stores[typeof(EntityThree)].Delete(e3);
			DataAccess.Data.Stores[typeof(EntityThree)].Delete(e4);
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

			DataAccess.Data.Stores[typeof(EntityOne)].Save(page);

			DataAccess.Data.Stores[typeof(EntityOne)].Save(article);

			BaseEntity[] found = (BaseEntity[])DataAccess.Data.Stores[typeof(EntityThree)].GetEntitiesContainingReverseReferences(page, property);
			Collection<BaseEntity> foundList = found != null ? new Collection<BaseEntity>(found) : new Collection<BaseEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Entities weren't retrieved correctly.");
			Assert.AreEqual(true, foundList.Contains(article.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Stores[typeof(EntityThree)].Delete(page);
			DataAccess.Data.Stores[typeof(EntityThree)].Delete(article);
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
	

			DataAccess.Data.Stores[typeof(EntityOne)].Save(article);

			DataAccess.Data.Stores[typeof(EntityOne)].Save(page);

			BaseEntity[] found = (BaseEntity[])DataAccess.Data.Stores[typeof(EntityThree)].GetEntitiesContainingReverseReferences(article, property);
			Collection<BaseEntity> foundList = found != null ? new Collection<BaseEntity>(found) : new Collection<BaseEntity>();
	
			Assert.AreEqual(1, foundList.Count, "Entities weren't retrieved correctly.");
			Assert.AreEqual(true, foundList.Contains(page.ID), "The wrong entity was found. IDs don't match.");
	
			DataAccess.Data.Stores[typeof(EntityThree)].Delete(page);
			DataAccess.Data.Stores[typeof(EntityThree)].Delete(article);
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
