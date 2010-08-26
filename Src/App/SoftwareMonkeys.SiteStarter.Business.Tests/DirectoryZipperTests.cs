using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using SoftwareMonkeys.SiteStarter.Data;
using System.IO;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Tests the DirectoryZipper component.
	/// </summary>
	[TestFixture]
	public class DirectoryZipperTests : BaseBusinessTestFixture
	{
		/// <summary>
		/// Tests the ZipToFile function.
		/// </summary>
		[Test]
		public void Test_ZipToFile()
		{
			
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
			
			string outputDirectory = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Exported";
			
			int expectedCount = 0;
			
			foreach (string dataStoreName in DataAccess.Data.GetDataStoreNames())
			{
				foreach (IEntity entity in DataAccess.Data.Stores[dataStoreName].Indexer.GetEntities())
					expectedCount ++;
			}
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			exporter.ExportDirectoryPath = outputDirectory;
						
			exporter.ExportToXml();
			
			int fileCount = 0;
			foreach (string directory in Directory.GetDirectories(outputDirectory))
			{
				foreach (String file in Directory.GetFiles(directory))
				{
					fileCount++;
				}
			}
			
			Assert.AreEqual(expectedCount, fileCount, "Incorrect number of files found.");
			
			
			DirectoryZipper zipper = new DirectoryZipper(exporter.ExportDirectoryPath);
			
			string zipFileName = @"Backup--" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "--" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".zip";
			string zipFilePath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Backups" + Path.DirectorySeparatorChar + zipFileName;
			//zipShortPath;//Config.Application.PhysicalApplicationPath + zipShortPath;


			
			zipper.ZipToFile(zipFilePath);
			
			Assert.IsTrue(File.Exists(zipFilePath), "The zip file wasn't created.");

			
			// TODO: Open the zip file and check the contents
			
		}
	}
}
