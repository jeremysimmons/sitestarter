using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Web;
using System.IO;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataExporterTests : BaseDataTestFixture
	{
		
		/// <summary>
		/// Tests the DataExporter.ExportToXml function while exporting all data.
		/// </summary>
		[Test]
		public void Test_ExportToXml()
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
			
			string outputDirectory = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Exported";
			
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
			
		}
	}
}
