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
			
			// IMPORTANT: Export the references before the entities, because the references are stripped from the entities upon export
			// Alternative is to reactivate the entities and their references using DataAccess.Data.Activator.
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(user);
			
			if (references.Count <= 0)
				Assert.Fail("No active references found.");
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Exported";
			
			exporter.ExportEntity(references[0]);
			
			exporter.ExportEntity(user);
			exporter.ExportEntity(role);
			exporter.ExportEntity(article);
			exporter.ExportEntity(category);
			
			
			
			DataAccess.Data.Schema.LegacyVersion = legacyVersion;
			DataAccess.Data.Schema.ApplicationVersion = currentVersion;
			
			
			DataImporter importer = (DataImporter)DataAccess.Data.InitializeDataImporter();
			
			importer.ImportableDirectoryPath = exporter.ExportDirectoryPath;
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Imported";
			
			importer.ImportFromXml();
			
			TestUser foundUser = DataAccess.Data.Reader.GetEntity<TestUser>("ID", user.ID);
			
			Assert.IsNotNull(foundUser, "foundUser == null");
			
			Assert.AreEqual(user.ID.ToString(), foundUser.ID.ToString(), "The ID of the found user doesn't match the ID of the original user.");
			
			DataAccess.Data.Activator.Activate(foundUser, "Roles");
			
			Assert.IsNotNull(foundUser.Roles, "user.Roles == null");
			
			Assert.AreEqual(1, foundUser.Roles.Length, "Invalid number of roles found.");
			
			
			
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
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Exported";
			
			// IMPORTANT: Export the references before the entities, because the references are stripped from the entities upon export
			// Alternative is to reactivate the entities and their references using DataAccess.Data.Activator.
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(user);
			
			exporter.ExportEntity(references[0]);
			
			exporter.ExportEntity(user);
			exporter.ExportEntity(role);
			
			
			DataSchema schema = (DataSchema)DataAccess.Data.Schema;
			
			schema.LegacyVersion = legacyVersion;
			schema.ApplicationVersion = currentVersion;
			
			schema.SchemaCommandDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Schema";
			
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
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Imported";
			
			importer.ImportFromXml();
			
			
			
			TestAccount foundAccount = DataAccess.Data.Reader.GetEntity<TestAccount>("ID", user.ID);
			
			Assert.IsNotNull(foundAccount, "foundAccount == null");
			
			Assert.AreEqual(user.ID.ToString(), foundAccount.ID.ToString(), "The ID of the found account doesn't match the ID of the original user.");
			
			DataAccess.Data.Activator.Activate(foundAccount, "GrantedRoles");
			
			Assert.IsNotNull(foundAccount.GrantedRoles, "user.GrantedRoles == null");
			
			Assert.AreEqual(1, foundAccount.GrantedRoles.Length, "Invalid number of roles found.");
			
			
			
			TestRole foundRole = DataAccess.Data.Reader.GetEntity<TestRole>("ID", role.ID);
			
			Assert.IsNotNull(foundRole, "foundRole == null");
			
			Assert.AreEqual(role.ID.ToString(), foundRole.ID.ToString(), "The ID of the found role doesn't match the ID of the original role.");
			
			
			
		}
		
		[Test]
		public void Test_IsValid_ReferenceParameter_False()
		{
			EntityIDReference reference = new EntityIDReference();
			
			bool isValid = DataAccess.Data.Importer.IsValid(reference);
			
			Assert.IsFalse(isValid, "Should have returned false.");
		}
		
		[Test]
		public void Test_IsValid_ReferenceParameter_True()
		{
			EntityIDReference reference = new EntityIDReference();
			reference.Entity1ID = Guid.NewGuid();
			reference.Entity2ID = Guid.NewGuid();
			reference.Type2Name = "TestArticle";
			reference.Type1Name = "TestCategory";
			reference.Property1Name = "Title";
			reference.Property2Name = "Name";
			
			bool isValid = DataAccess.Data.Importer.IsValid(reference);
			
			Assert.IsTrue(isValid, "Should have returned true.");
		}
		
		[Test]
		public void Test_IsValid_EntityParameter_False()
		{
			EntityIDReference reference = new EntityIDReference();
			
			bool isValid = DataAccess.Data.Importer.IsValid(reference);
			
			Assert.IsFalse(isValid, "Should have returned false.");
		}
		
		[Test]
		public void Test_IsValid_EntityParameter_True()
		{
			TestArticle article = new TestArticle();
			article.ID = Guid.NewGuid();
			article.Title = "Test title";
			
			bool isValid = DataAccess.Data.Importer.IsValid(article);
			
			Assert.IsTrue(isValid, "Should have returned true.");
		}
		
		public void Test_GetPreviousVersion_Legacy()
		{
			throw new NotImplementedException();
			
			Version version = DataAccess.Data.Importer.GetPreviousVersion();
			
			Version expected = new Version(1,2,3,4);
			
			Assert.AreEqual(expected.ToString(), version.ToString());
		}
		
		public void Test_GetPreviousVersion_Import()
		{
			throw new NotImplementedException();
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
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Exported";
			
			// IMPORTANT: Export the references before the entities, because the references are stripped from the entities upon export
			// Alternative is to reactivate the entities and their references using DataAccess.Data.Activator.
			EntityReferenceCollection references = DataAccess.Data.Referencer.GetActiveReferences(user);
			
			exporter.ExportEntity(references[0]);
			
			exporter.ExportEntity(user);
			exporter.ExportEntity(role);
			exporter.ExportEntity(article);
			exporter.ExportEntity(category);
			
			
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
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Exported";
			
			// Export the user to XML
			exporter.ExportEntity(user);
			
			DataImporter importer = (DataImporter)DataAccess.Data.InitializeDataImporter();
			
			importer.ImportableDirectoryPath = exporter.ExportDirectoryPath;
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Imported";
			
			// Test the move to imported function
			importer.MoveToImported(user, importer.CreateImportableEntityPath(user));
			
			string importedPath = importer.CreateImportedEntityPath(user);
			
			int importableCount = 0;
			
			foreach (string dir in Directory.GetDirectories(importer.ImportableDirectoryPath))
			{
				importableCount += Directory.GetFiles(dir, "*.xml").Length;
			}
			
			Assert.AreEqual(0, importableCount, "Incorrect number of files found in importable folder.");
			
			bool foundImported = File.Exists(importedPath);
			
			Assert.IsTrue(foundImported, "Imported file not found.");
		}
		
		
		[Test]
		public void Test_LoadEntityFromFile()
		{
			
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "Test";
			user.LastName = "Test";
			
			DataExporter exporter = (DataExporter)DataAccess.Data.InitializeDataExporter();
			
			exporter.ExportDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Exported";
			
			// Export the user to XML
			exporter.ExportEntity(user);
			
			string filePath = exporter.CreateEntityPath(user);
			
			DataImporter importer = (DataImporter)DataAccess.Data.InitializeDataImporter();
			
			importer.ImportedDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "Imported";
			
			TestUser foundUser = (TestUser)importer.LoadEntityFromFile(filePath);
			
			Assert.IsNotNull(foundUser, "foundUser == null");
			
			Assert.AreEqual(user.ID.ToString(), foundUser.ID.ToString(), "The ID of the found user doesn't have the same ID as the original.");
		}
	}
}
