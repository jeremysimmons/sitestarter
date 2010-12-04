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
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataStoreTests : BaseDataTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(this); }
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

		public DataStoreTests()
		{
			// Config.Initialize(ApplicationPath, "");
			
		}
		


		#region Tests
		/*[Test]
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



				
//				PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
//				filter.PropertyName = "ID";
//				filter.PropertyValue = e4.ID;
//				filter.AddType(typeof(EntityFour));



				DataAccess.Data.Saver.Save(e4);
				
				DataAccess.Data.Saver.Save(e3);
				


				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities<EntityFour>("ID", e4.ID);
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
				
				DataAccess.Data.Deleter.Delete(e3);
				DataAccess.Data.Deleter.Delete(e4);
			}
		}*/

		/*	[Test]
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






				DataAccess.Data.Stores[typeof(TestArticle)].Save(page);

				DataAccess.Data.Stores[typeof(TestArticle)].Save(article);
				


				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities<TestArticlePage>("ID", page.ID);
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
		}*/

		/*[Test]
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


				DataAccess.Data.Stores[typeof(TestArticle)].Save(article);
				
				
				DataAccess.Data.Stores[typeof(TestArticle)].Save(page);

				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities<TestArticle>("ID", article.ID);
				
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
		}*/

		/*
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



				DataAccess.Data.Saver.Save(e4);
				
				DataAccess.Data.Saver.Save(e3);

				DataAccess.Data.Saver.Save(e42);
				
				e3.ReferencedEntityIDs = new Guid[] {e42.ID};


				Collection<IEntity> toUpdate = new Collection<IEntity>((IEntity[])DataAccess.Data.Stores[typeof(EntityThree)].PreUpdate(e3));

				
				if (toUpdate.Count > 0)
				{
					// Ensure that the original referenced entity was modified properly
					Assert.AreEqual(true, toUpdate.Contains(e4.ID), "The original referenced entity wasn't found in the modified list.");

					EntityFour foundOriginal = (EntityFour)toUpdate[e4.ID];

					Assert.AreEqual(0, foundOriginal.ReferencedEntityIDs.Length, "The original referenced entity's obsolete references weren't properly cleared.");

					// Ensure that the new referenced entity was modified properly
					Assert.AreEqual(true, toUpdate.Contains(e42.ID), "The new referenced entity wasn't found in the modified list.");

					EntityFour foundNew = (EntityFour)toUpdate[e42.ID];

					Assert.AreEqual(1, foundNew.ReferencedEntityIDs.Length, "The new referenced entity's references weren't properly added.");


//					/*Assert.Greater(found.Length, 0, "No results found.");
//
//				EntityFour entity = (EntityFour)found[0];
//				Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
//				if (entity.ReferencedEntityIDs != null)
//				{
//					AppLogger.Debug("entity.ReferencedEntityIDs != null");
//
//					Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");
//
//					Assert.AreEqual(e42.ID, entity.ReferencedEntityIDs[0], "New reference ID is invalid.");
//				}
//				else
//				{
//					AppLogger.Debug("entity.ReferencedEntityIDs == null");
//				}
				}
				else
					Assert.Fail("No entity found. The save must have failed.");
				
				DataAccess.Data.Deleter.Delete(e3);
				DataAccess.Data.Deleter.Delete(e4);
				DataAccess.Data.Deleter.Delete(e42);
			}
		}
		 */

		/*[Test]
	public void Test_PreUpdate_EntitiesToEntitiesReference()
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

			e3.ReferencedEntities = new EntityFour[] {e4.ID};
			//e3.ReferencedEntities = new EntityFour[] {e4};
			// Only one of these needs to be set for this test. The auto preparation should take care of the other one.
            //e4.ReferencedEntities = new EntityThree[] {e3};



		
			PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
			filter.PropertyName = "ID";
		        filter.PropertyValue = e3.ID;
		        filter.AddType(typeof(EntityFour));



			DataAccess.Data.Saver.Save(e4);
		
			DataAccess.Data.Saver.Save(e3);

			DataAccess.Data.Saver.Save(e42);
		
			e3.ReferencedEntityIDs = new Guid[] {e42.ID};


		        Collection<IEntity> toUpdate = new Collection<IEntity>((IEntity[])DataAccess.Data.Stores[typeof(EntityThree)].PreUpdate(e3));


		        //IEntity[] modi = (IEntity[])DataAccess.Data.Indexer.GetEntities(filter);
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


	//			Assert.Greater(found.Length, 0, "No results found.");
///
	//			EntityFour entity = (EntityFour)found[0];
	//			Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
	//			if (entity.ReferencedEntityIDs != null)
	//			{
	//				AppLogger.Debug("entity.ReferencedEntityIDs != null");
//
	//				Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");
//
	//				Assert.AreEqual(e42.ID, entity.ReferencedEntityIDs[0], "New reference ID is invalid.");
	//			}
	//			else
	//			{
	//				AppLogger.Debug("entity.ReferencedEntityIDs == null");
	//			}
			}
			else
				Assert.Fail("No entity found. The save must have failed.");
	
	       		DataAccess.Data.Deleter.Delete(e3);
	       		DataAccess.Data.Deleter.Delete(e4);
	       		DataAccess.Data.Deleter.Delete(e42);
		}
	}*/

		/*[Test]
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
				
				//Collection<IEntity> f = new Collection<IEntity>((IEntity[])DataAccess.Data.Indexer.GetEntities(filter));
				
				//Assert.AreEqual(1, f.Count, "The original page wasn't saved or can't be retrieved.");
				
				
				article.PageIDs = new Guid[] {page2.ID};


				Collection<IEntity> toUpdate = new Collection<IEntity>((IEntity[])DataAccess.Data.Stores[typeof(TestArticle)].PreUpdate(article));

				
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


//					Assert.Greater(found.Length, 0, "No results found.");

//				EntityFour entity = (EntityFour)found[0];
//				Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
//				if (entity.ReferencedEntityIDs != null)
//				{
//					AppLogger.Debug("entity.ReferencedEntityIDs != null");
//
//					Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");
//
//					Assert.AreEqual(e42.ID, entity.ReferencedEntityIDs[0], "New reference ID is invalid.");
//				}
//				else
//				{
//					AppLogger.Debug("entity.ReferencedEntityIDs == null");
//				}
				}
				else
					Assert.Fail("No entity found. The save must have failed.");
				
				//DataAccess.Data.Stores[typeof(TestEntity)].Delete(article);
				//DataAccess.Data.Stores[typeof(TestEntity)].Delete(page);
				//DataAccess.Data.Stores[typeof(TestEntity)].Delete(page2);
			}
		}*/

		/*[Test]
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
				
				//Collection<IEntity> f = new Collection<IEntity>((IEntity[])DataAccess.Data.Indexer.GetEntities(filter));
				
				//Assert.AreEqual(1, f.Count, "The original page wasn't saved or can't be retrieved.");
				
				
				page.ArticleID = article2.ID;


				Collection<IEntity> toUpdate = new Collection<IEntity>((IEntity[])DataAccess.Data.Stores[typeof(TestArticle)].PreUpdate(page));

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


//					Assert.Greater(found.Length, 0, "No results found.");

//				EntityFour entity = (EntityFour)found[0];
//				Assert.IsNotNull(entity.ReferencedEntityIDs, "The mirror entity ID references property has not been set. The automatic preparation failed.");
//				if (entity.ReferencedEntityIDs != null)
//				{
//					AppLogger.Debug("entity.ReferencedEntityIDs != null");
//
//					Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");
//
//					Assert.AreEqual(e42.ID, entity.ReferencedEntityIDs[0], "New reference ID is invalid.");
//				}
//				else
//				{
//					AppLogger.Debug("entity.ReferencedEntityIDs == null");
//				}
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



                IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filter);
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



                IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filter);
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

		/*[Test]
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



				DataAccess.Data.Saver.Save(e4);

				DataAccess.Data.Saver.Save(e3);



				IEntity[] found = (IEntity[])DataAccess.Data.Indexer.GetEntities(filter);
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

				DataAccess.Data.Deleter.Delete(e3);
				DataAccess.Data.Deleter.Delete(e4);
			}
		}*/

		
		#endregion

		

	}
}
