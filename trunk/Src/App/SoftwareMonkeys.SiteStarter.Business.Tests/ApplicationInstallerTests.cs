using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;
using SoftwareMonkeys.SiteStarter.IO;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class ApplicationInstallerTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_InitializeVersion()
		{
			string applicationName = "TestApplication";
			
			string fullApplicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			VersionTestUtilities.CreateDummyVersion(fullApplicationPath, String.Empty);
			
			ApplicationInstaller installer = new ApplicationInstaller();
			installer.ApplicationPath = applicationName;
			installer.FileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			installer.InitializeVersion();
			
			
			string versionFile = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar
				+ applicationName + Path.DirectorySeparatorChar
				+ installer.DataDirectory + Path.DirectorySeparatorChar
				+ "Version.number";
			
			Assert.IsTrue(File.Exists(versionFile), "The version file wasn't found in the data directory.");
		}
		
		[Test]
		public void Test_IsInstalled_true()
		{
			
			string applicationName = "TestApplication";
			
			string applicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			string pathVariation = "testing";
			
			string appConfigPath = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Application." + pathVariation + ".config";
			
			CreateDummyConfig(appConfigPath);
			
			ApplicationInstaller installer = new ApplicationInstaller();
			installer.ApplicationPath = applicationName;
			installer.PathVariation = pathVariation;
			
			installer.FileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			bool isInstalled = installer.IsInstalled;
			
			Assert.IsTrue(isInstalled, "Returned false when it should have returned true.");
		}
		
		
		[Test]
		public void Test_IsInstalled_false()
		{
			
			string applicationName = "TestApplication";
			
			string applicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			string pathVariation = "testing";
			
			string appConfigPath = TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Application." + pathVariation + ".config";
			
			
			ApplicationInstaller installer = new ApplicationInstaller();
			installer.ApplicationPath = applicationName;
			installer.PathVariation = pathVariation;
			installer.FileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			bool isInstalled = installer.IsInstalled;
			
			Assert.IsFalse(isInstalled, "Returned true when it should have returned false.");
		}
		
		[Test]
		public void Test_Setup()
		{
			CreateDummyVersionFile(TestUtilities.GetTestApplicationPath(this, "MockApplication"));
			
			User admin = new User();
			admin.ID = Guid.NewGuid();
			admin.Username = "admin";
			admin.Password = Crypter.EncryptPassword("pass");
			
			ApplicationInstaller installer = new ApplicationInstaller();
			installer.ApplicationPath = "/MockApplication";
			installer.Administrator = admin;
			installer.AdministratorRoleName = "Administrator";
			installer.PathVariation = "testing";
			installer.FileMapper = new MockFileMapper(this);
			installer.DataProviderInitializer = new MockDb4oDataProviderInitializer(this);
			installer.Setup();
			
			User foundAdministrator = DataAccess.Data.Reader.GetEntity<User>("ID", admin.ID);
			
			DataAccess.Data.Activator.Activate(foundAdministrator);
			
			Assert.AreEqual(1, foundAdministrator.Roles.Length, "Administrator isn't in admin role.");
		}
		
		public void CreateDummyVersionFile(string appPath)
		{
			string filePath = Path.Combine(appPath, "Version.number");
			
			string content = "1.2.3.4";
			
			using (StreamWriter writer = File.CreateText(filePath))
			{
				writer.Write(content);
			}
		}
		
		
		public void CreateDummyConfig(string appConfigPath)
		{
			if (!Directory.Exists(Path.GetDirectoryName(appConfigPath)))
				Directory.CreateDirectory(Path.GetDirectoryName(appConfigPath));
			
			using (StreamWriter writer = File.CreateText(appConfigPath))
			{
				writer.Write("no content needed");
				writer.Close();
			}
		}
	}
}
