using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
 
namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
    [TestFixture]
    public class DataUtilitiesTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
         //   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
       //     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }

	public DataUtilitiesTests()
	{
       // if (!SoftwareMonkeys.SiteStarter.Configuration.Config.IsInitialized == null)
       // {
       //     string path = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().ToCharArray().Length - 3);
       //     SoftwareMonkeys.SiteStarter.Configuration.Config.Initialize(path);
       // }
	}

        #region Singleton tests
        [Test]
        public void Test_Data_EnsureInitialized()
        {
            DataProvider provider = DataAccess.Data;

            Assert.IsNotNull((object)provider);
        }
        #endregion

	[Test]
	public void Test_AddReferences_IDsToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the AddReferences function for an IDs to IDs reference.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntityIDs = new Guid[] {e4.ID};
			
			IEntity[] toUpdate = DataUtilities.AddReferences(e4, e3, e4.GetType().GetProperty("ReferencedEntityIDs"));

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
		}
	}

	/*[Test]
	public void Test_AddReferences_EntitiesToEntities()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the AddReferences function for an entities to entities reference.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntities = new EntityFour[] {e4};

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntities");

			IEntity[] toUpdate = DataUtilities.AddReferences(e4, e3, property);

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
		}
	}*/

	[Test]
	public void Test_AddReferences_IDToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the AddReferences function for an ID to IDs reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e1.PageIDs = new Guid[] { e2.ID };
			
			IEntity[] toUpdate = DataUtilities.AddReferences(e2, e1, e2.GetType().GetProperty("ArticleID"));

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
		}
	}

	[Test]
	public void Test_AddReferences_IDsToID()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the AddReferences function for an IDs to ID reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e2.ArticleID = e1.ID;
			


			IEntity[] toUpdate = DataUtilities.AddReferences(e1, e2, e1.GetType().GetProperty("PageIDs"));

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
		}
	}

	[Test]
	public void Test_RemoveReferences_IDsToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the RemoveReferences function with and IDs to IDs reference.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntityIDs = new Guid[] {e4.ID};
			e4.ReferencedEntityIDs = new Guid[] {e3.ID};
			
			//DataAccess.Data.Stores[typeof(EntityThree)].Save(e4);
			//DataAccess.Data.Stores[typeof(EntityThree)].Save(e3);

			IEntity[] toUpdate = DataUtilities.RemoveReferences(e4, e3, e4.GetType().GetProperty("ReferencedEntityIDs"));

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
			{
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
				Assert.AreEqual(e4.ID, toUpdate[0].ID, "The wrong entity was modified.");
				Assert.AreEqual(0, ((EntityFour)toUpdate[0]).ReferencedEntityIDs.Length, "The original referenced entity's now obsolete references weren't removed properly.");
			}
		}
	}

	[Test]
	public void Test_RemoveReferences_IDToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the RemoveReferences function with an ID to IDs reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e1.PageIDs = new Guid[] { e2.ID };
			
			//DataAccess.Data.Stores[typeof(EntityThree)].Save(e1);
			//DataAccess.Data.Stores[typeof(EntityThree)].Save(e2);

			TestArticle[] toUpdate = Collection<TestArticle>.ConvertAll(DataUtilities.RemoveReferences(e1, e2, e1.GetType().GetProperty("PageIDs")));

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
			{
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
				Assert.AreEqual(e1.ID, toUpdate[0].ID, "The wrong entity was modified.");
				Assert.IsTrue(Array.IndexOf(toUpdate[0].PageIDs, e2.ID) == -1, "The obsolete reference wasn't cleared properly.");
				//Assert.IsTrue(toUpdate[0].ArticleID == null || toUpdate[0].ArticleID == Guid.Empty, "The obsolete reference wasn't cleared properly.");
			}
		}
	}

	[Test]
	public void Test_RemoveReferences_IDsToID()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the RemoveReferences function with an IDs to ID reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e2.ArticleID = e1.ID;
			
			//DataAccess.Data.Stores[typeof(EntityThree)].Save(e1);
			//DataAccess.Data.Stores[typeof(EntityThree)].Save(e2);

			TestArticlePage[] toUpdate = Collection<TestArticlePage>.ConvertAll(DataUtilities.RemoveReferences(e2, e1, e2.GetType().GetProperty("ArticleID")));

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
			{
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
				Assert.AreEqual(e2.ID, toUpdate[0].ID, "The wrong entity was modified.");//
				Assert.IsTrue(toUpdate[0].ArticleID == null || toUpdate[0].ArticleID == Guid.Empty, "The obsolete reference wasn't cleared properly.");
				//Assert.IsTrue(toUpdate[0].PageIDs == null || toUpdate[0].PageIDs == new Guid[] {}, "The obsolete reference wasn't cleared properly.");
			}
		}
	}

	[Test]
	public void Test_GetReferenceType_EntityIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReferenceType function.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			

			PropertyInfo property = e3.GetType().GetProperty("ReferencedEntityIDs");

			Type type = DataUtilities.GetReferenceType(e3, property);

            Assert.AreEqual(e4.GetType().ToString(), type.ToString(), "The wrong type was retrieved.");
		}
	}

	/*[Test]
	public void Test_GetReferenceType_EntityID()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReferenceType function.", NLog.LogLevel.Debug))
		{
	        	TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
            article.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";



            PropertyInfo property = article.GetType().GetProperty("ReferencedEntityIDs");

			Type type = DataUtilities.GetReferenceType(property, DataUtilities.GetReferenceAttribute(property));

			Assert.AreEqual(e4.GetType(), type, "The wrong type was retrieved.");
		}
	}*/

	[Test]
	public void Test_GetReferenceType_Entities()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReferenceType function.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			

			PropertyInfo property = e3.GetType().GetProperty("ReferencedEntities");

			Type type = DataUtilities.GetReferenceType(e3, property);

			Assert.AreEqual(e4.GetType(), type, "The wrong type was retrieved.");
		}
	}

	[Test]
	public void Test_GetMirrorProperty_IDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetMirrorProperty function.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			

			PropertyInfo property = e3.GetType().GetProperty("ReferencedEntityIDs");

			PropertyInfo mirrorProperty = e4.GetType().GetProperty(DataUtilities.GetMirrorPropertyName(property, DataUtilities.GetReferenceAttribute(property)));

			Assert.AreEqual("ReferencedEntityIDs", mirrorProperty.Name, "The names don't match");

			Assert.AreEqual(e4.GetType(), mirrorProperty.DeclaringType, "The types don't match.");
		}
	}

	[Test]
	public void Test_GetMirrorProperty_Entities()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetMirrorProperty function.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			

			PropertyInfo property = e3.GetType().GetProperty("ReferencedEntities");

			PropertyInfo mirrorProperty = e4.GetType().GetProperty(DataUtilities.GetMirrorPropertyName(property, DataUtilities.GetReferenceAttribute(property)));

			Assert.AreEqual("ReferencedEntities", mirrorProperty.Name, "The names don't match");

			Assert.AreEqual(e4.GetType(), mirrorProperty.DeclaringType, "The types don't match.");
		}
	}

	[Test]
	public void Test_TransferReferenceIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the TransferReferenceIDs function.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntities = new EntityFour[] { e4 };

			
			DataUtilities.TransferReferenceIDs(e3);

			Assert.AreEqual(new Guid[] {e4.ID}, e3.ReferencedEntityIDs, "The IDs weren't transferred properly.");


		}
	}


	[Test]
	public void Test_ApplyExclusions()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the ApplyExclusions function.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntities = new EntityFour[] { e4 };

			
			DataUtilities.ApplyExclusions(e3);

			Assert.IsNull(e3.ReferencedEntities, "The property wasn't cleared properly.");


		}
	}

        [Test]
        public void Test_SynchroniseReverseReferences_IDsToIDs()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities references.", NLog.LogLevel.Debug))
            {
                EntityThree e3 = new EntityThree();
                e3.ID = Guid.NewGuid();
                e3.Name = "Test E3";


                EntityFour e4 = new EntityFour();
                e4.ID = Guid.NewGuid();
                e4.Name = "Test E4";

                e3.ReferencedEntityIDs = new Guid[] { e4.ID };
                //e3.ReferencedEntities = new EntityFour[] {e4};
                // Only one of these needs to be set for this test. The auto preparation should take care of the other one.
                //e4.ReferencedEntities = new EntityThree[] {e3};
                



                                /*PropertyFilter filter = (PropertyFilter)DataAccess.Data.CreateFilter(typeof(PropertyFilter));
                                filter.PropertyName = "ID";
                                filter.PropertyValue = e4.ID;
                                filter.AddType(typeof(EntityFour));*/



                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e4);

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e3);

                                PropertyInfo property = e3.GetType().GetProperty("ReferencedEntityIDs");

                                IEntity[] updated = (IEntity[])DataUtilities.SynchroniseReverseReferences(e3, property, new IEntity[] {e4}, new IEntity[] {e4});

                                if (updated != null)
                                {
                                    Assert.Greater(updated.Length, 0, "No entities were updated.");

                                    EntityFour entity = (EntityFour)updated[0];
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

                                //DataAccess.Data.Stores[typeof(TestEntity)].Delete(e3);
                                //DataAccess.Data.Stores[typeof(TestEntity)].Delete(e4);
            }
        }

       [Test]
        public void Test_SynchroniseReverseReferences_IDsToID()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities references.", NLog.LogLevel.Debug))
            {
                	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e2.ArticleID = e1.ID;
            //e1.PageIDs = new Guid[] { e2.ID };

                                //DataAccess.Data.Stores[typeof(TestArticle)].Save(e1);

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e2);

                                PropertyInfo property = e2.GetType().GetProperty("ArticleID");

                                IEntity[] updated = (IEntity[])DataUtilities.SynchroniseReverseReferences(e2, property, new IEntity[] { e1 }, new IEntity[] { e1 });

                                if (updated != null)
                                {
                                    Assert.Greater(updated.Length, 0, "No entities were updated.");

                                    TestArticle entity = (TestArticle)updated[0];
                                    Assert.IsNotNull(entity.PageIDs, "The mirror entity ID references property has not beed set. The automatic preparation failed.");
                                    if (entity.PageIDs != null)
                                    {
                                        AppLogger.Debug("entity.PageIDs != null");

                                        Assert.AreEqual(1, entity.PageIDs.Length, "Incorrect number of reference entity IDs.");
                                    }
                                    else
                                    {
                                        AppLogger.Debug("entity.PageIDs == null");
                                    }
                                }
                                else
                                    Assert.Fail("No entity found. The save must have failed.");

                                //DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
                                //DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
            }
        }

       [Test]
        public void Test_SynchroniseReverseReferences_IDToIDs()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the preparation for saving entities references.", NLog.LogLevel.Debug))
            {
                	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e1.PageIDs = new Guid[] { e2.ID };



                                PropertyInfo property = e1.GetType().GetProperty("PageIDs");

                                IEntity[] updated = (IEntity[])DataUtilities.SynchroniseReverseReferences(e1, property, new IEntity[] { e2 }, new IEntity[] { });

                                if (updated != null)
                                {
                                    Assert.Greater(updated.Length, 0, "No entities were updated.");

                                    TestArticlePage foundPage = (TestArticlePage)updated[0];
                                    Assert.IsNotNull(foundPage);//, "The mirror entity ID references property has not beed set. The automatic preparation failed.");

                                    Assert.AreEqual(e2.ID, foundPage.ID, "The wrong entity was modified or returned.");
                                    Assert.IsTrue(foundPage.ArticleID == e1.ID, "The obsolete reference wasn't cleared properly.");
					//Assert.IsTrue(entity.ArticleID == null || entity.ArticleID == Guid.Empty, "The obsolete reference wasn't cleared properly.");
                                    /*if (entity.ReferencedEntityIDs != null)
                                    {
                                        AppLogger.Debug("entity.ReferencedEntityIDs != null");

                                        Assert.AreEqual(1, entity.ReferencedEntityIDs.Length, "Incorrect number of reference entity IDs.");
                                    }
                                    else
                                    {
                                        AppLogger.Debug("entity.ReferencedEntities == null");
                                    }*/
                                }
                                else
                                    Assert.Fail("No entity found. The save must have failed.");

                                //DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                                //DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

       [Test]
        public void Test_GetReferenceIDs_Many()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the retrieval of reference IDs from a property.", NLog.LogLevel.Debug))
            {
                	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e1.PageIDs = new Guid[] { e2.ID };
                

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e2);

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);

			PropertyInfo property = e1.GetType().GetProperty("PageIDs");

                         Guid[] found = DataUtilities.GetReferenceIDs(e1, property);

			Assert.AreEqual(1, found.Length, "The reference wasn't retrieved properly.");
			Assert.AreEqual(e2.ID, found[0], "The wrong ID was retrieved.");

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }
	
	[Test]
        public void Test_GetReferenceIDs_Single()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the retrieval of reference IDs from a property.", NLog.LogLevel.Debug))
            {
                	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e2.ArticleID = e1.ID;
                

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e2);

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);

			PropertyInfo property = e2.GetType().GetProperty("ArticleID");

                         Guid[] found = DataUtilities.GetReferenceIDs(e2, property);

			Assert.AreEqual(1, found.Length, "The reference wasn't retrieved properly.");
			Assert.AreEqual(e1.ID, found[0], "The wrong ID was retrieved.");

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

	[Test]
        public void Test_RemoveObsoleteReverseReferences_IDsToIDs()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the retrieval of reference IDs from a property.", NLog.LogLevel.Debug))
            {
                	EntityThree e3 = new EntityThree();
	                e3.ID = Guid.NewGuid();
	                e3.Name = "Test E3";
	
	
	                EntityFour e4 = new EntityFour();
	                e4.ID = Guid.NewGuid();
	                e4.Name = "Test E4";


                	EntityThree e32 = new EntityThree();
	                e32.ID = Guid.NewGuid();
	                e32.Name = "Test E32";
	
			// Create the obsolete reference
	                e3.ReferencedEntityIDs = new Guid[] { e4.ID };

			// Create the new reference
	                e4.ReferencedEntityIDs = new Guid[] { e32.ID };
	                e32.ReferencedEntityIDs = new Guid[] { e4.ID };

			//page1.ArticleID = article1.ID;
			//article1.PageIDs = new Guid[] {page1.ID};
                

                        //        DataAccess.Data.Stores[typeof(TestEntity)].Save(article1);


			//page1.ArticleID = article2.ID;
			//article2.PageIDs = new Guid[] {page1.ID};

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntityIDs");

			IEntity[] oldReverseReferences = new IEntity[] {e3};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntitiesContainingReverseReferences(article1, property);
			IEntity[] newReferences =  new IEntity[] {e32};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntities(article1.PageIDs);

                         EntityThree[] found = Collection<EntityThree>.ConvertAll(DataUtilities.RemoveObsoleteReverseReferences(e4, property, newReferences, oldReverseReferences));

			Assert.AreEqual(1, found.Length, "The wrong number of references were returned.");
			if (found.Length > 0)
			{
				Assert.AreEqual(e3.ID, found[0].ID, "The wrong entity was retrieved.");

				Assert.IsTrue(Array.IndexOf(found[0].ReferencedEntityIDs, e3.ID) == -1, "The obsolete reference wasn't cleared properly.");
			}

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

	[Test]
        public void Test_RemoveObsoleteReverseReferences_IDToIDs()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the retrieval of reference IDs from a property.", NLog.LogLevel.Debug))
            {
                	TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article 1";

			TestArticlePage page1 = new TestArticlePage();
			page1.ID = Guid.NewGuid();
			page1.Title = "Page 1";

			TestArticlePage page2 = new TestArticlePage();
			page2.ID = Guid.NewGuid();
			page2.Title = "Page 2";

			article1.PageIDs = new Guid[] { page1.ID };
			page1.ArticleID = article1.ID;
                

                        //        DataAccess.Data.Stores[typeof(TestEntity)].Save(article1);


			article1.PageIDs = new Guid[] { page2.ID };

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);

			PropertyInfo property = article1.GetType().GetProperty("PageIDs");

			IEntity[] oldReverseReferences = new IEntity[] {page1};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntitiesContainingReverseReferences(article1, property);
			IEntity[] newReferences =  new IEntity[] {page2};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntities(article1.PageIDs);

                         TestArticlePage[] found = Collection<TestArticlePage>.ConvertAll(DataUtilities.RemoveObsoleteReverseReferences(article1, property, newReferences, oldReverseReferences));

			Assert.AreEqual(1, found.Length, "The wrong number of references were returned.");
			if (found.Length > 0)
			{
				Assert.AreEqual(page1.ID, found[0].ID, "The wrong ID was retrieved.");

				Assert.IsTrue(found[0].ArticleID == null || found[0].ArticleID == Guid.Empty, "The obsolete reference wasn't cleared properly.");
			}

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

	[Test]
        public void Test_RemoveObsoleteReverseReferences_IDsToID()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the retrieval of reference IDs from a property.", NLog.LogLevel.Debug))
            {
                	TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article 1";

                	TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Article 2";

			TestArticlePage page1 = new TestArticlePage();
			page1.ID = Guid.NewGuid();
			page1.Title = "Page 1";

			page1.ArticleID = article1.ID;
			article1.PageIDs = new Guid[] {page1.ID};
                

                        //        DataAccess.Data.Stores[typeof(TestEntity)].Save(article1);


			page1.ArticleID = article2.ID;
			article2.PageIDs = new Guid[] {page1.ID};

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);

			PropertyInfo property = page1.GetType().GetProperty("ArticleID");

			IEntity[] oldReverseReferences = new IEntity[] {article1};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntitiesContainingReverseReferences(article1, property);
			IEntity[] newReferences =  new IEntity[] {article2};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntities(article1.PageIDs);

                         TestArticle[] found = Collection<TestArticle>.ConvertAll(DataUtilities.RemoveObsoleteReverseReferences(page1, property, newReferences, oldReverseReferences));

			Assert.AreEqual(1, found.Length, "The wrong number of references were returned.");
			if (found.Length > 0)
			{
				Assert.AreEqual(article1.ID, found[0].ID, "The wrong ID was retrieved.");

				Assert.IsTrue(Array.IndexOf(found[0].PageIDs, page1.ID) == -1, "The obsolete reference wasn't cleared properly.");
			}

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

	[Test]
        public void Test_AddNewReverseReferences_IDsToIDs()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a AddNewReverseReferences function.", NLog.LogLevel.Debug))
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
	
			// Create the obsolete reference
	                e3.ReferencedEntityIDs = new Guid[] { e4.ID };
	                //e4.ReferencedEntityIDs = new Guid[] { e3.ID };

			// Create the new reference
	             //   e3.ReferencedEntityIDs = new Guid[] { e42.ID };
	                //e32.ReferencedEntityIDs = new Guid[] { e4.ID };
                

                
			PropertyInfo property = e3.GetType().GetProperty("ReferencedEntityIDs");

			//IEntity[] oldReverseReferences = new IEntity[] {e3};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntitiesContainingReverseReferences(article1, property);
			IEntity[] newReferences =  new IEntity[] {e4};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntities(article1.PageIDs);

                         EntityFour[] found = Collection<EntityFour>.ConvertAll(DataUtilities.AddNewReverseReferences(e3, property, newReferences));

			Assert.AreEqual(1, found.Length, "The wrong number of references were returned.");
			if (found.Length > 0)
			{
				Assert.AreEqual(e4.ID, found[0].ID, "The wrong entity was retrieved.");

				Assert.IsTrue(Array.IndexOf(found[0].ReferencedEntityIDs, e3.ID) > -1, "The new reference wasn't added properly.");
			}

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

	[Test]
        public void Test_AddNewReverseReferences_IDToIDs()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the retrieval of reference IDs from a property.", NLog.LogLevel.Debug))
            {
                	TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article 1";

			TestArticlePage page1 = new TestArticlePage();
			page1.ID = Guid.NewGuid();
			page1.Title = "Page 1";

			TestArticlePage page2 = new TestArticlePage();
			page2.ID = Guid.NewGuid();
			page2.Title = "Page 2";

			article1.PageIDs = new Guid[] { page1.ID };
			page1.ArticleID = article1.ID;
                

                        //        DataAccess.Data.Stores[typeof(TestEntity)].Save(article1);


			article1.PageIDs = new Guid[] { page2.ID };

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);

			PropertyInfo property = article1.GetType().GetProperty("PageIDs");

			IEntity[] oldReverseReferences = new IEntity[] {page1};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntitiesContainingReverseReferences(article1, property);
			IEntity[] newReferences =  new IEntity[] {page2};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntities(article1.PageIDs);

                         TestArticlePage[] found = Collection<TestArticlePage>.ConvertAll(DataUtilities.AddNewReverseReferences(article1, property, newReferences));

			Assert.AreEqual(1, found.Length, "The wrong number of references were returned.");
			if (found.Length > 0)
			{
				Assert.AreEqual(page2.ID, found[0].ID, "The wrong ID was retrieved.");

				Assert.IsTrue(found[0].ArticleID == article1.ID, "The new reference wasn't added properly.");
			}

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

	[Test]
        public void Test_AddNewReverseReferences_IDsToID()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing a the retrieval of reference IDs from a property.", NLog.LogLevel.Debug))
            {
                	TestArticle article1 = new TestArticle();
			article1.ID = Guid.NewGuid();
			article1.Title = "Article 1";

                	TestArticle article2 = new TestArticle();
			article2.ID = Guid.NewGuid();
			article2.Title = "Article 2";

			TestArticlePage page1 = new TestArticlePage();
			page1.ID = Guid.NewGuid();
			page1.Title = "Page 1";

			page1.ArticleID = article1.ID;
                

                        //        DataAccess.Data.Stores[typeof(TestEntity)].Save(article1);


			page1.ArticleID = article2.ID;

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e1);

			PropertyInfo property = page1.GetType().GetProperty("ArticleID");

			IEntity[] oldReverseReferences = new IEntity[] {article1};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntitiesContainingReverseReferences(article1, property);
			IEntity[] newReferences =  new IEntity[] {article2};//DataAccess.Data.Stores[typeof(TestEntity)].GetEntities(article1.PageIDs);

                         TestArticle[] found = Collection<TestArticle>.ConvertAll(DataUtilities.AddNewReverseReferences(page1, property, newReferences));

			Assert.AreEqual(1, found.Length, "The wrong number of references were returned.");
			if (found.Length > 0)
			{
				Assert.AreEqual(article2.ID, found[0].ID, "The wrong entity was retrieved.");

				Assert.AreEqual(new Guid[] {page1.ID}, found[0].PageIDs, "The new reference wasn't added properly.");
			}

                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e2);
                             //   DataAccess.Data.Stores[typeof(TestEntity)].Delete(e1);
            }
        }

	[Test]
	public void Test_GetDataStoreNameForReference_Multiple_Null()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetDataStoreNameForReference function for a multiple reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			//e1.PageIDs = new Guid[] { e2.ID };
			
			string name = DataUtilities.GetDataStoreNameForReference(e1, e1.GetType().GetProperty("PageIDs"));

				Assert.AreEqual("Testing", name, "The wrong data store name was returned.");
		}
	}
	
		[Test]
	public void Test_GetDataStoreNameForReference_Multiple_NotNull()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetDataStoreNameForReference function for a multiple reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";

			e1.PageIDs = new Guid[] { e2.ID };
			
			string name = DataUtilities.GetDataStoreNameForReference(e1, e1.GetType().GetProperty("PageIDs"));

				Assert.AreEqual("Testing", name, "The wrong data store name was returned.");
		}
	}
	
	[Test]
	public void Test_GetDataStoreNameForReference_Many_Null()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetDataStoreNameForReference function for a multiple reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";
			
			//e2.ArticleID = e1.ID;
			
			string name = DataUtilities.GetDataStoreNameForReference(e2, e2.GetType().GetProperty("ArticleID"));

			
			Assert.AreEqual("Testing", name, "The wrong data store name was returned.");
		}
	}
	
		[Test]
	public void Test_GetDataStoreNameForReference_Many_NotNull()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetDataStoreNameForReference function for a multiple reference.", NLog.LogLevel.Debug))
		{
	        	TestArticle e1 = new TestArticle();
			e1.ID = Guid.NewGuid();
			e1.Title = "Test 1";
			

			TestArticlePage e2 = new TestArticlePage();
			e2.ID = Guid.NewGuid();
			e2.Title = "Test 2";
			
			e2.ArticleID = e1.ID;
			
			string name = DataUtilities.GetDataStoreNameForReference(e2, e2.GetType().GetProperty("ArticleID"));

			
			Assert.AreEqual("Testing", name, "The wrong data store name was returned.");
		}
	}
	
    }
}
