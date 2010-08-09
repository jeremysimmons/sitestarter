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
	/// Tests the ApplicationBackup component.
	/// </summary>
	[TestFixture]
	public class ApplicationBackupTests : BaseBusinessTestFixture
	{
		
		[Test]
		public void Test_Backup_SkipLegacy()
		{
			string backupDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"Backup";
			
			string exportDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"Export";
			
			string legacyDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"Legacy";
			
			string dataDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"App_Data";
			
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
			backup.LegacyDirectoryPath = legacyDirectoryPath;
			
			backup.KeepLegacy = false;
			
			string zipFilePath = backup.Backup();
			
			Assert.IsTrue(File.Exists(zipFilePath), "The zip file wasn't created.");

			
			long total = 0;
			using (ZipAnalyzer analyzer = new ZipAnalyzer(zipFilePath))
			{
				total = analyzer.CountFiles();
			}
			
			Assert.AreEqual(7, total, "The total number of files in the the zip.");
			
			Assert.IsFalse(Directory.Exists(backup.ExportDirectoryPath), "The export directory is still there. It should be removed after export.");
						
			Assert.IsFalse(Directory.Exists(backup.LegacyDirectoryPath), "The legacy directory was found when it should have been skipped.");
		}
		
		
		[Test]
		public void Test_Backup_KeepLegacy()
		{
			string backupDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"Backup";
			
			string exportDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"Export";
			
			string legacyDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"Legacy";
			
			string dataDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar +
				"App_Data";
			
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
			backup.LegacyDirectoryPath = legacyDirectoryPath;
			backup.KeepLegacy = true;
			
			string zipFilePath = backup.Backup();
			
			Assert.IsTrue(File.Exists(zipFilePath), "The zip file wasn't created.");

			
			long total = 0;
			using (ZipAnalyzer analyzer = new ZipAnalyzer(zipFilePath))
			{
				total = analyzer.CountFiles();
			}
			
			Assert.AreEqual(7, total, "The total number of files in the the zip.");
			
			Assert.IsFalse(Directory.Exists(backup.ExportDirectoryPath), "The export directory is still there. It should be removed after export.");
						
			Assert.IsTrue(Directory.Exists(backup.LegacyDirectoryPath), "The legacy directory wasn't found when it should have been.");
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
