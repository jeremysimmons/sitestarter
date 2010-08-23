using NUnit.Framework;
using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	/// <summary>
	/// Provides a base implementation for all data importer adapters, which are used to import data from serialized XML files.
	/// </summary>
	[TestFixture]
	public class DataImporterTests : BaseDataTestFixture
	{
		[Test]
		public void Test_ImportFromXml()
		{
			
			Version currentVersion = new Version(1, 0, 0, 0);
			Version legacyVersion = new Version(0, 9, 0, 0);
			
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "Test";
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test";
			
			TestCategory category = new TestCategory();
			category.ID = Guid.NewGuid();
			category.Name = "Test";
			
			
			
			
			user.Roles = new TestRole[] {role};
			
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Exported";
			
			exporter.ExportEntity(user);
			exporter.ExportEntity(role);
			exporter.ExportEntity(article);
			exporter.ExportEntity(category);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(user);
			
			exporter.ExportEntity(references[0]);
			
			DataAccess.Data.Schema.LegacyVersion = legacyVersion;
			DataAccess.Data.Schema.ApplicationVersion = currentVersion;
			
			
			DataImporter importer = (DataImporter)DataAccess.Data.InitializeDataImporter();
			
			importer.ImportableDirectoryPath = exporter.ExportDirectoryPath;
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Imported";
			
			importer.ImportFromXml();
			
			TestUser foundUser = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
			
			Assert.IsNotNull(foundUser, "foundUser == null");
			
			Assert.AreEqual(user.ID.ToString(), foundUser.ID.ToString(), "The ID of the found user doesn't match the ID of the original user.");
			
			DataAccess.Data.Activator.Activate(foundUser, "Roles");
			
			Assert.IsNotNull(foundUser.Roles, "user.Roles == null");
			
			Assert.AreEqual(user.Roles.Length, foundUser.Roles.Length, "Invalid number of roles found.");
			
			
			
			TestRole foundRole = DataAccess.Data.Reader.GetEntity<TestRole>("ID", role.ID);
			
			Assert.IsNotNull(foundRole, "foundRole == null");
			
			Assert.AreEqual(role.ID.ToString(), foundRole.ID.ToString(), "The ID of the found role doesn't match the ID of the original role.");
			
			
			
			TestCategory foundCategory = DataAccess.Data.Reader.GetEntity<TestCategory>("ID", category.ID);
			
			Assert.IsNotNull(foundCategory, "foundCategory == null");
			
			Assert.AreEqual(category.ID.ToString(), foundCategory.ID.ToString(), "The ID of the found category doesn't match the ID of the original category.");
			
			
			
			TestArticle foundArticle = DataAccess.Data.Reader.GetEntity<TestArticle>("ID", article.ID);
			
			Assert.IsNotNull(foundArticle, "foundArticle == null");
			
			Assert.AreEqual(article.ID.ToString(), foundArticle.ID.ToString(), "The ID of the found article doesn't match the ID of the original article.");
			
		}
		
		
		[Test]
		public void Test_ImportFromXml_RenewSchema_TypeAndProperty()
		{
			Version currentVersion = new Version(1, 0, 0, 0);
			Version legacyVersion = new Version(0, 9, 0, 0);
			
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "Test";
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			
			user.Roles = new TestRole[] {role};
			
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Exported";
			
			exporter.ExportEntity(user);
			exporter.ExportEntity(role);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(user);
			
			exporter.ExportEntity(references[0]);
			
			DataSchema schema = (DataSchema)DataAccess.Data.Schema;
			
			schema.LegacyVersion = legacyVersion;
			schema.ApplicationVersion = currentVersion;
			
			schema.SchemaCommandDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Schema";
			
			RenamePropertyCommand command1 = new RenamePropertyCommand();
			command1.TypeName = user.ShortTypeName;
			command1.PropertyName = "Roles";
			command1.NewPropertyName = "GrantedRoles";
			
			RenameTypeCommand command2 = new RenameTypeCommand();
			command2.TypeName = user.ShortTypeName;
			command2.NewTypeName = typeof(TestAccount).Name;
			
			DataSchemaCommandCollection commands = new DataSchemaCommandCollection();
			commands.Add(command1);
			commands.Add(command2);
			
			schema.SchemaCommands = commands;
			
			DataImporter importer = (DataImporter)DataAccess.Data.Importer;
			
			importer.ImportableDirectoryPath = exporter.ExportDirectoryPath;
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Imported";
			
			importer.ImportFromXml();
			
			
			
			TestAccount foundAccount = DataAccess.Data.Reader.GetEntity<TestAccount>("ID", user.ID);
			
			Assert.IsNotNull(foundAccount, "foundAccount == null");
			
			Assert.AreEqual(user.ID.ToString(), foundAccount.ID.ToString(), "The ID of the found account doesn't match the ID of the original user.");
			
			DataAccess.Data.Activator.Activate(foundAccount, "GrantedRoles");
			
			Assert.IsNotNull(foundAccount.GrantedRoles, "user.GrantedRoles == null");
			
			Assert.AreEqual(user.Roles.Length, foundAccount.GrantedRoles.Length, "Invalid number of roles found.");
			
			
			
			TestRole foundRole = DataAccess.Data.Reader.GetEntity<TestRole>("ID", role.ID);
			
			Assert.IsNotNull(foundRole, "foundRole == null");
			
			Assert.AreEqual(role.ID.ToString(), foundRole.ID.ToString(), "The ID of the found role doesn't match the ID of the original role.");
			
			
			
		}
		
		
		
		[Test]
		public void Test_LoadEntitiesFileList()
		{
			
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "Test";
			
			TestRole role = new TestRole();
			role.ID = Guid.NewGuid();
			role.Name = "Test Role";
			
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test";
			
			TestCategory category = new TestCategory();
			category.ID = Guid.NewGuid();
			category.Name = "Test";
			
			
			
			
			user.Roles = new TestRole[] {role};
			
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Exported";
			
			exporter.ExportEntity(user);
			exporter.ExportEntity(role);
			exporter.ExportEntity(article);
			exporter.ExportEntity(category);
			
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(user);
			
			exporter.ExportEntity(references[0]);
			
			DataImporter importer = (DataImporter)DataAccess.Data.InitializeDataImporter();
			importer.ImportableDirectoryPath = exporter.ExportDirectoryPath;
			
			string[] fileList = importer.LoadEntitiesFileList();
			
			Assert.AreEqual(5, fileList.Length, "Incorrect number of files found.");
		}
		
		[Test]
		public void Test_MoveToImported()
		{
			
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "Test";
			
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Exported";
			
			// Export the user to XML
			exporter.ExportEntity(user);
			
			DataImporter importer = (DataImporter)DataAccess.Data.InitializeDataImporter();
			
			importer.ImportableDirectoryPath = exporter.ExportDirectoryPath;
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Imported";
			
			// Test the move to imported function
			importer.MoveToImported(user, importer.CreateImportableEntityPath(user));
			
			
			int importableCount = 0;
			
			foreach (string dir in Directory.GetDirectories(importer.ImportableDirectoryPath))
			{
				importableCount += Directory.GetFiles(dir, "*.xml").Length;
			}
			
			Assert.AreEqual(0, importableCount, "Incorrect number of files found in importable folder.");
			
			int importedCount = 0;
			foreach (string dir in Directory.GetDirectories(importer.ImportedDirectoryPath))
			{
				importedCount += Directory.GetFiles(dir, "*.xml").Length;
			}
			
			Assert.AreEqual(1, importedCount, "Incorrect number of files found in imported folder.");
		}
		
		
		[Test]
		public void Test_LoadEntityFromFile()
		{
			
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "Test";
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Exported";
			
			// Export the user to XML
			exporter.ExportEntity(user);
			
			string filePath = exporter.CreateEntityPath(user);
			
			DataImporter importer = (DataImporter)DataAccess.Data.InitializeDataImporter();
			
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath() + Path.DirectorySeparatorChar + "Imported";
			
			TestUser foundUser = (TestUser)importer.LoadEntityFromFile(filePath);
			
			Assert.IsNotNull(foundUser, "foundUser == null");
			
			Assert.AreEqual(user.ID.ToString(), foundUser.ID.ToString(), "The ID of the found user doesn't have the same ID as the original.");
		}
	}
}
