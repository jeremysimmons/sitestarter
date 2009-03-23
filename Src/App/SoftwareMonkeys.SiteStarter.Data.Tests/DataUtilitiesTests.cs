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
	public void Test_AddReferences()
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
			

			PropertyInfo property = e4.GetType().GetProperty("ReferencedEntityIDs");

			BaseEntity[] toUpdate = DataUtilities.AddReferences(e4, e3, property, DataUtilities.GetReferenceAttribute(property));

			Assert.IsNotNull(toUpdate, "The toUpdate variable was returned null.");
			if (toUpdate != null)
				Assert.AreEqual(1, toUpdate.Length, "The modified entity isn't in the 'to update' list.");
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
	
    }
}
