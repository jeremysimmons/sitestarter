using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class ApplicationRestorerTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_RequiresRestore_true()
		{
			
			string applicationName = "TestApplication";
			
			string applicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			string pathVariation = "testing";
			
			string appConfigPath = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Application." + pathVariation + ".config";
			
			string legacyDirectory = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Legacy";
			
			CreateMockLegacy(legacyDirectory);
			
			ApplicationRestorer restorer = new ApplicationRestorer(new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName));
			restorer.LegacyDirectoryPath = legacyDirectory;
			//restorer.ApplicationPath = applicationName;
			//restorer.PathVariation = pathVariation;
			//restorer.FileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			bool does = restorer.RequiresRestore;
			
			Assert.IsTrue(does, "Returned false when it should have returned true.");
		}
		
		
		[Test]
		public void Test_RequiresRestore_false()
		{
			
			string applicationName = "TestApplication";
			
			string applicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			string pathVariation = "testing";
			
			string appConfigPath = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Application." + pathVariation + ".config";
			
			
			string legacyDirectory = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Legacy";
			
			//CreateMockLegacy(legacyDirectory);
			
			
			ApplicationRestorer restorer = new ApplicationRestorer(new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName));
			restorer.LegacyDirectoryPath = legacyDirectory;
			//restorer.ApplicationPath = applicationName;
			//restorer.PathVariation = pathVariation;
			//restorer.FileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			bool does = restorer.RequiresRestore;
			
			Assert.IsFalse(does, "Returned false when it should have returned false.");
		}
				
		
		public void CreateMockLegacy(string legacyDirectory)
		{
			if (!Directory.Exists(legacyDirectory))
				Directory.CreateDirectory(legacyDirectory);
			
		}
	}
}
