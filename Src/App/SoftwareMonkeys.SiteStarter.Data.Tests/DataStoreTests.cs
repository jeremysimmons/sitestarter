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
	public class DataStoreTests
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

		public DataStoreTests()
		{
			// Config.Initialize(ApplicationPath, "");
			
			TestUtilities.RegisterTestEntities();
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



				DataAccess.Data.Save(e4);
				
				DataAccess.Data.Save(e3);
				


				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities<EntityFour>("ID", e4.ID);
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
				
				DataAccess.Data.Delete(e3);
				DataAccess.Data.Delete(e4);
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
				


				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities<TestArticlePage>("ID", page.ID);
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

				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities<TestArticle>("ID", article.ID);
				
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



				DataAccess.Data.Save(e4);
				
				DataAccess.Data.Save(e3);

				DataAccess.Data.Save(e42);
				
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
				
				DataAccess.Data.Delete(e3);
				DataAccess.Data.Delete(e4);
				DataAccess.Data.Delete(e42);
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



			DataAccess.Data.Save(e4);
		
			DataAccess.Data.Save(e3);

			DataAccess.Data.Save(e42);
		
			e3.ReferencedEntityIDs = new Guid[] {e42.ID};


		        Collection<IEntity> toUpdate = new Collection<IEntity>((IEntity[])DataAccess.Data.Stores[typeof(EntityThree)].PreUpdate(e3));


		        //IEntity[] modi = (IEntity[])DataAccess.Data.GetEntities(filter);
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
	
	       		DataAccess.Data.Delete(e3);
	       		DataAccess.Data.Delete(e4);
	       		DataAccess.Data.Delete(e42);
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
				
				//Collection<IEntity> f = new Collection<IEntity>((IEntity[])DataAccess.Data.GetEntities(filter));
				
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
				
				//Collection<IEntity> f = new Collection<IEntity>((IEntity[])DataAccess.Data.GetEntities(filter));
				
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



                IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filter);
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



                IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filter);
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



				DataAccess.Data.Save(e4);

				DataAccess.Data.Save(e3);



				IEntity[] found = (IEntity[])DataAccess.Data.GetEntities(filter);
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

				DataAccess.Data.Delete(e3);
				DataAccess.Data.Delete(e4);
			}
		}*/

		
		#endregion

		

		[Test]
		public void Test_GetByPropertyValue()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing data retrieval with the GetEntities by property value function.", NLog.LogLevel.Debug))
			{
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Stores[typeof(EntityOne)].Save(e1);

				IEntity[] found = DataAccess.Data.GetEntities<EntityOne>("Name", e1.Name);
				
				Assert.IsNotNull(found, "Null array returned.");
				
				if (found != null)
					Assert.AreEqual(1, found.Length, "No results found.");
				
				
				DataAccess.Data.Delete(e1);
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
				filter.PropertyName = "Name";
				filter.PropertyValue = "Another Name";
				
				DataAccess.Data.Stores[typeof(EntityOne)].Save(e1);

				IEntity[] found = DataAccess.Data.GetEntities<EntityOne>("Name", "Another Name");
				
				Assert.IsNotNull(found, "Null array returned.");
				
				if (found != null)
					Assert.AreEqual(0, found.Length, "Entities weren't properly excluded.");
				
				
				DataAccess.Data.Delete(e1);
			}
		}


		[Test]
		public void Test_GetEntitiesByParameterDictionary()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntities<T>(IDictionary<string, object>) function.", NLog.LogLevel.Debug))
			{
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Save(e1);
				
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add("Name", "Test E1");

				EntityOne[] found = (EntityOne[])DataAccess.Data.Stores[typeof(EntityOne)].GetEntities<EntityOne>(parameters);
				
				Assert.IsNotNull(found, "The return value is null.");
				
				if (found != null)
					Assert.AreEqual(1, found.Length, "Entities weren't retrieved properly.");
				
				
				DataAccess.Data.Delete(e1);
			}
		}
		
		
		[Test]
		public void Test_GetEntitiesByParameterDictionary_Exclude()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntities<T>(IDictionary<string, object>) function to ensure it excludes entities properly.", NLog.LogLevel.Debug))
			{
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Save(e1);
				
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add("Name", "Test E2");

				EntityOne[] found = (EntityOne[])DataAccess.Data.Stores[typeof(EntityOne)].GetEntities<EntityOne>(parameters);
				
				Assert.IsNotNull(found, "The return value is null.");
				
				if (found != null)
					Assert.AreEqual(0, found.Length, "Entities weren't properly excluded.");
				
				
				DataAccess.Data.Delete(e1);
			}
		}
		
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
			
			
			IEntity[] toUpdate = null;
			IEntity[] toDelete = null;
			
			DataAccess.Data.Stores[typeof(TestUser)].PreSave(user, out toUpdate, out toDelete);
			
			IEntity user2 = DataAccess.Data.GetEntity(typeof(TestUser), "ID", user.ID);
			
			Assert.AreEqual(1, toUpdate.Length, "Incorrect number of related entities modified.");
			
		}
		
		[Test]
		public void Test_PreUpdate()
		{
			// Register the types
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			// Create the dummy objects
			TestUser user = new TestUser();
			Guid userID = user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "User";
			
			TestRole role = new TestRole();
			Guid roleID = role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			// Add the role to the User.Roles collection
			user.Roles = Collection<TestRole>.Add(user.Roles, role);
			
			// Save both objects
			DataAccess.Data.Save(role);
			DataAccess.Data.Save(user);
			
			// Load the user to another variable
			TestUser user2 = (TestUser)DataAccess.Data.GetEntity(typeof(TestUser), "ID", user.ID);
			
			// Activate the loaded user object
			DataAccess.Data.Activate(user2);
			
			// Remove the role from the list
			user2.Roles = Collection<TestRole>.RemoveAt(user2.Roles, 0);
			
			// Create the toUpdate and toDelete arrays
			IEntity[] toUpdate = null;
			IEntity[] toDelete = null;
			
			// Run the DataStore.PreUpdate function
			DataAccess.Data.Stores[typeof(TestUser)].PreUpdate(user2, out toUpdate, out toDelete);
			
			// Load the user again to a new variable, which should now reflect the changes
			TestUser user3 = (TestUser)DataAccess.Data.GetEntity(typeof(TestUser), "ID", user2.ID);
			
			// Check the roles list on the newly loaded user object
			// Should be Length == 0
			Assert.AreEqual(0, user3.Roles.Length, "Incorrect number of roles found on retrieved user entity.");
			
			// Check the toDelete list (containing obsolete reference)
			Assert.IsNotNull(toDelete, "The toDelete list is null.");
			
			if (toDelete != null)
			{
				// Check the length of the toDelete list
				// Should be Length == 1
				Assert.AreEqual(1, toDelete.Length, "Incorrect number of entities in toDelete list. Expecting the obsolete reference to be in the list.");
			}
			
		}
		
		[Test]
		public void Test_Update()
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
			
			DataAccess.Data.Save(role);
			
			DataAccess.Data.Save(user);
			
			TestUser user2 = (TestUser)DataAccess.Data.GetEntity(typeof(TestUser), "ID", user.ID);
			
			DataAccess.Data.Activate(user2);
			
			user2.FirstName = "Test-Updated";
			
			//user2.Roles.RemoveAt(0);
			
			
			DataAccess.Data.Update(user2);
			
			
			TestUser user3 = (TestUser)DataAccess.Data.GetEntity(typeof(TestUser), "ID", user2.ID);
			
			Assert.AreEqual("Test-Updated", user3.FirstName, "The name doesn't appear to have been updated.");
			
			//Assert.IsNotNull(toDelete, "The toDelete list is null.");
			//if (toDelete != null)
			//	Assert.AreEqual(1, toDelete.Length, "Incorrect number of entities in toDelete list. Expecting the obsolete reference to be in the list.");
			
			DataAccess.Data.Delete(user);
		}
		
		[Test]
		public void Test_GetEntitiesPage_Page1_SortAscending()
		{
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			ClearTestEntities();
			
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
			
			DataAccess.Data.Save(user);
			DataAccess.Data.Save(user2);
			DataAccess.Data.Save(user3);
			
			PagingLocation pagingLocation = new PagingLocation(0, 10);
			
			string sortExpression = "FirstNameAscending";
			
			TestUser[] entities = DataAccess.Data.GetEntitiesPage<TestUser>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			Assert.AreEqual(3, entities.Length, "Invalid number found.");
			
			Assert.AreEqual("A", entities[0].FirstName, "Sorting failed #1.");
			Assert.AreEqual("B", entities[1].FirstName, "Sorting failed #2.");
			Assert.AreEqual("C", entities[2].FirstName, "Sorting failed #3.");
			
		ClearTestEntities();
		}
		
		
		
		[Test]
		public void Test_GetEntitiesPage_Page1_SortDescending()
		{
			TestUser.RegisterType();
			TestRole.RegisterType();
			
			ClearTestEntities();
			
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
			
			DataAccess.Data.Save(user);
			DataAccess.Data.Save(user2);
			DataAccess.Data.Save(user3);
			
			PagingLocation pagingLocation = new PagingLocation(0, 10);
			
			string sortExpression = "FirstNameDescending";
			
			TestUser[] entities = DataAccess.Data.GetEntitiesPage<TestUser>(pagingLocation, sortExpression);
			
			Assert.IsNotNull(entities);
			
			Assert.AreEqual(3, entities.Length, "Invalid number found.");
			
			Assert.AreEqual("C", entities[0].FirstName, "Sorting failed #1.");
			Assert.AreEqual("B", entities[1].FirstName, "Sorting failed #2.");
			Assert.AreEqual("A", entities[2].FirstName, "Sorting failed #3.");
			
			ClearTestEntities();
		}
		
		/*			[Test]
		public void Test_Save_Reference()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetEntities<T>(IDictionary<string, object>) function to ensure it excludes entities properly.", NLog.LogLevel.Debug))
			{
				EntityOne e1 = new EntityOne();
				e1.Name = "Test E1";
				
				DataAccess.Data.Save(e1);
				
				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add("Name", "Test E2");

				EntityOne[] found = (EntityOne[])DataAccess.Data.Stores[typeof(EntityOne)].GetEntities<EntityOne>(parameters);
				
				Assert.IsNotNull(found, "The return value is null.");
				
				if (found != null)
					Assert.AreEqual(0, found.Length, "Entities weren't properly excluded.");
				
				
				DataAccess.Data.Delete(e1);
			}
		}*/



		private void ClearTestEntities()
		{
			Type[] types = new Type[] {
				typeof(TestUser),
				typeof(TestRole),
				typeof(TestEntity),
				typeof(EntityOne),
				typeof(EntityTwo),
				typeof(EntityThree),
				typeof(EntityFour) };

			Collection<IEntity> entities = new Collection<IEntity>();
			foreach (Type type in types)
				entities.Add(DataAccess.Data.GetEntities(type));

			foreach (IEntity entity in entities)
			{
				DataAccess.Data.Delete(entity);
			}
		}


	}
}
