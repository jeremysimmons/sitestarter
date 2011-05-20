using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Tests the ZipAnalyzer component.
	/// </summary>
	[TestFixture]
	public class ZipAnalyzerTests : BaseBusinessTestFixture
	{	
		[Test]
		public void Test_CountFiles()
		{
			
			string applicationName = "TestApplication";
			
			
			string applicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
			
			string dataDirectoryPath = TestUtilities.GetTestDataPath(this, applicationName);
			
			string backupDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Backup";
			
			string exportDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Export";
			
			string legacyDirectoryPath = dataDirectoryPath + Path.DirectorySeparatorChar + "Legacy";
			
			
			VersionTestUtilities.CreateDummyVersion(dataDirectoryPath, "testing");
			
			
			CreateDummyFiles(dataDirectoryPath);
			
			
			
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "Test";
			
			DataAccess.Data.Saver.Save(user);
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			DataAccess.Data.Saver.Save(role);
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test";
			
			DataAccess.Data.Saver.Save(article);
			
			TestCategory category = new TestCategory();
			category.Name = "Test";
			
			DataAccess.Data.Saver.Save(category);
		
			
			// Export data
			ApplicationBackup backup = new ApplicationBackup();
			backup.BackupDirectoryPath = backupDirectoryPath;
			backup.ExportDirectoryPath = exportDirectoryPath;
			backup.DataDirectoryPath = dataDirectoryPath;
			
			string zipFilePath = backup.Backup();
			
			Assert.IsTrue(File.Exists(zipFilePath), "The zip file wasn't created.");

			long total = 0;
			using (ZipAnalyzer analyzer = new ZipAnalyzer(zipFilePath))
			{
				total = analyzer.CountFiles();
			}
			
			Assert.AreEqual(7, total, "The total number of files in the the zip.");
			
		}
		
		private void CreateDummyFiles(string dataDirectoryPath)
		{
			if (!Directory.Exists(dataDirectoryPath))
				Directory.CreateDirectory(dataDirectoryPath);
			
			string[] fileNames = new String[]{"Version.number",
				"Mappings.local.config",
				"Application.local.config"};
			
			foreach (string file in fileNames)
			{
				string filePath = dataDirectoryPath + Path.DirectorySeparatorChar +
					file;
				
				using (StreamWriter writer = File.CreateText(filePath))
				{
					writer.Write("Dummy data");
					writer.Close();
				}
			}
		}
	}
}
