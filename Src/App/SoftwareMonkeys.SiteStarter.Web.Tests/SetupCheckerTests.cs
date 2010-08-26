using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Web;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using SoftwareMonkeys.SiteStarter.IO;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	[TestFixture]
	public class SetupCheckerTests : BaseWebTestFixture
	{
		[Test]
		public void Test_SkipPage_True()
		{
			Uri url = new Uri("http://localhost/Test/Setup.aspx");
			
			string applicationName = "TestApplication";
			
			string applicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			SetupChecker checker = new SetupChecker(url, applicationPath);
			
			bool skip = checker.SkipPage();
			
			Assert.IsTrue(skip, "Returned false when it should have returned true.");
		}
		
		[Test]
		public void Test_SkipPage_False()
		{
			Uri url = new Uri("http://localhost/Test/Default.aspx");
			
			string applicationName = "TestApplication";
			
			string applicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			SetupChecker checker = new SetupChecker(url, applicationPath);
			
			bool skip = checker.SkipPage();
			
			Assert.IsFalse(skip, "Returned true when it should have returned false.");
		}
		
		
		[Test]
		public void Test_GetRedirectPath_NoRedirect()
		{
			Uri url = new Uri("http://localhost/Test/Default.aspx");
			
			string applicationName = "TestApplication";
			
			string pathVariation = "testing";
			
			string appConfigPath = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Application." + pathVariation + ".config";
			
			CreateMockConfig(appConfigPath);
			
			SetupChecker checker = new SetupChecker(url, applicationName);
			checker.Installer = new ApplicationInstaller(applicationName, pathVariation);
			checker.FileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName);
			
			
			checker.Restorer = new ApplicationRestorer(new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName));
			
			
			string redirectPath = checker.GetRedirectPath();
			
			Assert.AreEqual(String.Empty, redirectPath, "The redirect path should have returned String.Empty but it didn't.");
		}
		
		[Test]
		public void Test_GetRedirectPath_Setup()
		{
			Uri url = new Uri("http://localhost/Test/Default.aspx");
			
			string applicationName = "TestApplication";
			
			string pathVariation = "testing";
			
			string appConfigPath = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Application." + pathVariation + ".config";
			
			IFileMapper fileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName);
			
			SetupChecker checker = new SetupChecker(url, applicationName, fileMapper);
			checker.Installer = new ApplicationInstaller(applicationName, pathVariation, fileMapper);
			//checker.Installer = new ApplicationInstaller(applicationName, pathVariation);
			//checker.Installer.FileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName);
			
			//checker.Restorer = new ApplicationRestorer();
	
			
			string redirectPath = checker.GetRedirectPath();
			
			string expected = applicationName + "/Admin/Setup.aspx";
			
			Assert.AreEqual(expected.ToLower(), redirectPath.ToLower(), "The redirect path didn't return the expected value.");
		}
		
		[Test]
		public void Test_GetRedirectPath_Maintenance()
		{
			Uri url = new Uri("http://localhost/Test/Default.aspx");
			
			string applicationName = "TestApplication";
			
			string pathVariation = "testing";
			
			string appConfigPath = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Application." + pathVariation + ".config";
			
			string legacyDirectory = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Legacy";
			
			CreateMockLegacy(legacyDirectory);
			
			SetupChecker checker = new SetupChecker(url, applicationName);
			checker.Installer = new ApplicationInstaller(applicationName, pathVariation,
			                                            new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName));
		
			checker.Restorer = new ApplicationRestorer(new MockFileMapper(this,TestUtilities.GetTestingPath(this), applicationName));
			checker.Restorer.LegacyDirectoryPath = legacyDirectory;
			
			string redirectPath = checker.GetRedirectPath();
			
			string expected = applicationName + "/Maintenance.html";
			
			Assert.AreEqual(expected.ToLower(), redirectPath.ToLower(), "The redirect path didn't return the expected value.");
		}
		
		public void CreateMockConfig(string appConfigPath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(appConfigPath)))
				Directory.CreateDirectory(Path.GetDirectoryName(appConfigPath));
			
			using (StreamWriter writer = File.CreateText(appConfigPath))
			{
				writer.Write("no content needed");
				writer.Close();
			}
		}
		
		public void CreateMockLegacy(string legacyDirectory)
		{
			if (!Directory.Exists(legacyDirectory))
				Directory.CreateDirectory(legacyDirectory);
			
		}
	}
}
