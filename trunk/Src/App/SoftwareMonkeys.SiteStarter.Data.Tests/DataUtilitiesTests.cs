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
			

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntityIDs");

			BaseEntity[] toUpdate = DataUtilities.AddReferences(e4, e3, property);

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

			BaseEntity[] toUpdate = DataUtilities.AddReferences(e4, e3, property);

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
			

			PropertyInfo property = e2.GetType().GetProperty("ArticleID");

			BaseEntity[] toUpdate = DataUtilities.AddReferences(e2, e1, property);

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
			

			PropertyInfo property = e1.GetType().GetProperty("PageIDs");

			BaseEntity[] toUpdate = DataUtilities.AddReferences(e1, e2, property);

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
		}
	}

	[Test]
	public void Test_RemoveReferences_IDsToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReferenceType function.", NLog.LogLevel.Debug))
		{
	        	EntityThree e3 = new EntityThree();
			e3.ID = Guid.NewGuid();
			e3.Name = "Test E3";
			

			EntityFour e4 = new EntityFour();
			e4.ID = Guid.NewGuid();
			e4.Name = "Test E4";

			e3.ReferencedEntityIDs = new Guid[] {e4.ID};
			e4.ReferencedEntityIDs = new Guid[] {e3.ID};
			
			DataAccess.Data.Stores[typeof(EntityThree)].Save(e4);
			DataAccess.Data.Stores[typeof(EntityThree)].Save(e3);

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntityIDs");

			BaseEntity[] toUpdate = DataUtilities.RemoveReferences(e4, e3, property);

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
			{
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
				Assert.AreEqual(e4.ID, toUpdate[0].ID, "The wrong entity was modified.");
			}
		}
	}

	[Test]
	public void Test_RemoveReferences_IDToIDs()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReferenceType function.", NLog.LogLevel.Debug))
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

			PropertyInfo property = e1.GetType().GetProperty("PageIDs");

			BaseEntity[] toUpdate = DataUtilities.RemoveReferences(e1, e2, property);

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
			{
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
				Assert.AreEqual(e1.ID, toUpdate[0].ID, "The wrong entity was modified.");
			}
		}
	}

	[Test]
	public void Test_RemoveReferences_IDsToID()
	{
		using (LogGroup logGroup = AppLogger.StartGroup("Testing the GetReferenceType function.", NLog.LogLevel.Debug))
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

			PropertyInfo property = e2.GetType().GetProperty("ArticleID");

			BaseEntity[] toUpdate = DataUtilities.RemoveReferences(e2, e1, property);

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
			{
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
				Assert.AreEqual(e2.ID, toUpdate[0].ID, "The wrong entity was modified.");
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

			Type type = DataUtilities.GetReferenceType(property, DataUtilities.GetReferenceAttribute(property));

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

			PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property, DataUtilities.GetReferenceAttribute(property));

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

			PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property, DataUtilities.GetReferenceAttribute(property));

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
        public void Test_SynchroniseReverseReferences()
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



                                DataAccess.Data.Stores[typeof(TestEntity)].Save(e4);

                                //DataAccess.Data.Stores[typeof(TestEntity)].Save(e3);

                                PropertyInfo property = e3.GetType().GetProperty("ReferencedEntityIDs");

                                BaseEntity[] updated = (BaseEntity[])DataUtilities.SynchroniseReverseReferences(e3, property, DataAccess.Data.Stores[e4.GetType()].GetEntitiesContainingReverseReferences(e3, property));

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
                                DataAccess.Data.Stores[typeof(TestEntity)].Delete(e4);
            }
        }
	
    }
}
