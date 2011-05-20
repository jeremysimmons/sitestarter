using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using System.Web;
using SoftwareMonkeys.SiteStarter.Tests;
using System.Xml;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class DataSchemaTests : BaseDataTestFixture
	{

		#region Singleton tests
		[Test]
		public void Test_Data_EnsureInitialized()
		{
			DataProvider provider = DataAccess.Data;

			Assert.IsNotNull((object)provider);
		}
		#endregion
		 
		
		[Test]
		public void Test_RenewSchema()
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "--FirstName--";
			user.LastName = "--LastName--";
			user.Email = "--Email--";
			user.Username = "--Username--";
			
			XmlDocument document = XmlUtilities.SerializeToDocument(user);
			
			RenamePropertyCommand command1 = new RenamePropertyCommand();
			command1.PropertyName = "LastName";
			command1.NewPropertyName = "Surname";
			command1.TypeName = "TestUser";
			
			DataSchemaCommandCollection commands = new DataSchemaCommandCollection();
			commands.Add(command1);
			
			DataSchema schema = (DataSchema)DataAccess.Data.InitializeDataSchema();
			
			schema.SchemaCommands = commands;
			
			schema.RenewSchema(document);
			
			TestUser user2 = (TestUser)XmlUtilities.DeserializeFromDocument(document, user.GetType());
			
			Assert.IsNotNull(user2, "user2 == null");
			
			Assert.AreEqual(user.ID.ToString(), user2.ID.ToString(), "The IDs don't match.");
			
			Assert.AreEqual(user.LastName, user2.Surname, "The value of the LastName wasn't moved to the Surname property like it should have.");
		}

		[Test]
		public void Test_CheckIsUpToDate_False()
		{
			RenamePropertyCommand command1 = new RenamePropertyCommand();
			command1.TypeName = "TestUser";
			command1.PropertyName = "LastName";
			command1.NewPropertyName = "Surname";
			
			RenameTypeCommand command2 = new RenameTypeCommand();
			command2.TypeName = "TestUser";
			command2.NewTypeName = "TestAccount";
			
			DataSchemaCommandCollection commands = new DataSchemaCommandCollection();
			commands.Add(command1);
			commands.Add(command2);
			
			Version legacyVersion = new Version(1, 0, 0, 0);
			Version commandVersion = new Version(1, 1, 0, 0);
			Version currentVersion = new Version(1, 1, 0, 0);
			
			string groupName = "TestGroup";
			
			string commandsPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + DataAccess.Data.Schema.SchemaDirectory;
			
			
			DataSchema schema = (DataSchema)DataAccess.Data.InitializeDataSchema();
			
			schema.SchemaDirectory = "Testing" + Path.DirectorySeparatorChar + "Schema";
			
			schema.LegacyVersion = legacyVersion;
			schema.ApplicationVersion = currentVersion;
			
			schema.SaveCommands(commandsPath, commands, groupName, commandVersion);
			
			bool isUpToDate = schema.CheckIsUpToDate();
		}
		
		
		[Test]
		public void Test_CheckIsUpToDate_True()
		{
			RenamePropertyCommand command1 = new RenamePropertyCommand();
			command1.TypeName = "TestUser";
			command1.PropertyName = "LastName";
			command1.NewPropertyName = "Surname";
			
			RenameTypeCommand command2 = new RenameTypeCommand();
			command2.TypeName = "TestUser";
			command2.NewTypeName = "TestAccount";
			
			DataSchemaCommandCollection commands = new DataSchemaCommandCollection();
			commands.Add(command1);
			commands.Add(command2);
			
			Version legacyVersion = new Version(1, 0, 0, 0);
			Version commandVersion = new Version(0, 1, 0, 0);
			Version currentVersion = new Version(1, 1, 0, 0);
			
			string groupName = "TestGroup";
			
			string commandsPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + DataAccess.Data.Schema.SchemaDirectory;
			
			
			DataSchema schema = (DataSchema)DataAccess.Data.InitializeDataSchema();
			
			schema.SchemaDirectory = "Testing" + Path.DirectorySeparatorChar + "Schema";
			
			schema.LegacyVersion = legacyVersion;
			schema.ApplicationVersion = currentVersion;
			
			schema.SaveCommands(commandsPath, commands, groupName, commandVersion);
			
			bool isUpToDate = schema.CheckIsUpToDate();
		}
		
		[Test]
		public void Test_CreateCommandFilePath()
		{
			string groupName = "TestGroup";
			Version version = new Version(1, 2, 3, 4);
			
			DataSchema schema = (DataSchema)DataAccess.Data.InitializeDataSchema();
			
			string schemaDirectoryPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + schema.SchemaDirectory;
			
			string path = schema.CreateCommandFilePath(schemaDirectoryPath, groupName, version);
			
			string expectedPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar +
				schema.SchemaDirectory + Path.DirectorySeparatorChar +
				groupName + "--" + version.ToString().Replace(".", "-") + ".schema";
				
			
			Assert.AreEqual(expectedPath, path, "The path doesn't match what's expected.");
		}
		
		[Test]
		public void Test_ExtractVersionFromFileName()
		{
			string groupName = "TestGroup";
			Version version = new Version(1, 2, 3, 4);
			
			string fileName = groupName + "--" + version.ToString().Replace(".", "-") + ".schema";
			
			DataSchema schema = (DataSchema)DataAccess.Data.InitializeDataSchema();
			Version extractedVersion = schema.ExtractVersionFromFileName(fileName);
			
			Assert.AreEqual(version.ToString(), extractedVersion.ToString(), "Wrong version extracted.");
		}
		
		[Test]
		public void Test_SaveCommands()
		{
			RenamePropertyCommand command1 = new RenamePropertyCommand();
			command1.TypeName = "TestUser";
			command1.PropertyName = "LastName";
			command1.NewPropertyName = "Surname";
			
			RenameTypeCommand command2 = new RenameTypeCommand();
			command2.TypeName = "TestUser";
			command2.NewTypeName = "TestAccount";
			
			DataSchemaCommandCollection commands = new DataSchemaCommandCollection();
			commands.Add(command1);
			commands.Add(command2);
			
			Version legacyVersion = new Version(1, 0, 0, 0);
			Version commandVersion = new Version(1, 1, 0, 0);
			Version currentVersion = new Version(1, 1, 0, 0);
			
			string groupName = "TestGroup";
			
			string commandsPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + DataAccess.Data.Schema.SchemaDirectory;
			
			DataAccess.Data.Schema.SaveCommands(commandsPath, commands, groupName, commandVersion);
			
			int fileCount = Directory.GetFiles(commandsPath).Length;
			
			Assert.AreEqual(1, fileCount, "Incorrect number of files created.");
		}
		
		[Test]
		public void Test_GetCommands()
		{
			RenamePropertyCommand command1 = new RenamePropertyCommand();
			command1.TypeName = "TestUser";
			command1.PropertyName = "LastName";
			command1.NewPropertyName = "Surname";
			
			RenameTypeCommand command2 = new RenameTypeCommand();
			command2.TypeName = "TestUser";
			command2.NewTypeName = "TestAccount";
			
			DataSchemaCommandCollection commands = new DataSchemaCommandCollection();
			commands.Add(command1);
			commands.Add(command2);
			
			Version legacyVersion = new Version(1, 0, 0, 0);
			Version commandVersion = new Version(1, 1, 0, 0);
			Version currentVersion = new Version(1, 1, 0, 0);
			
			string groupName = "TestGroup";
			
			string commandsPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + DataAccess.Data.Schema.SchemaDirectory;
			
			DataSchema schema = (DataSchema)DataAccess.Data.InitializeDataSchema();
			
			schema.SchemaCommandDirectoryPath = commandsPath;
			schema.LegacyVersion = legacyVersion;
			schema.ApplicationVersion = currentVersion;
			
			schema.SaveCommands(commandsPath, commands, groupName, commandVersion);
			
			DataSchemaCommandCollection foundCommands = schema.GetCommands();
			
			Assert.AreEqual(commands.Count, foundCommands.Count, "Incorrect number of commands found.");
		}
		
		public TestUser CreateTestUser(string testingDirectory)
		{
			TestUser user = new TestUser();
			user.ID = Guid.NewGuid();
			user.FirstName = "First";
			user.LastName = "Last";
			user.Username = "TestUsername";
			user.Email = "test@softwaremonkeys.net";
			
			string filePath = testingDirectory + Path.DirectorySeparatorChar
				+ user.GetType().FullName + Path.DirectorySeparatorChar
				+ user.ID.ToString() + ".xml";
			
			string dirPath = Path.GetDirectoryName(filePath);
			
			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);
			
			using (StreamWriter writer = File.CreateText(filePath))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(TestUser));
				
				serializer.Serialize(writer, user);
				
				writer.Close();
			}
			
			return user;
		}
		
		public void CreateTestRenamePropertySchema(string testingDirectory)
		{
			
			string content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SchemaUpdate xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RenameProperty TypeName=""TestUser"" OriginalName=""LastName"" NewName=""Surname"" />
</SchemaUpdate>";
			
			string filePath = testingDirectory + Path.DirectorySeparatorChar
				+ "Schema" + Path.DirectorySeparatorChar
				+ "1-0-0-0" + Path.DirectorySeparatorChar
				+ "User.xml";
			
			string dirPath = Path.GetDirectoryName(filePath);
			
			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);
			
			using (StreamWriter writer = File.CreateText(filePath))
			{
				writer.Write(content);
				
				writer.Close();
			}
		}
		
		public void CreateTestRenameTypeSchema(string sourceDirectory)
		{
			
			string content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SchemaUpdate xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RenameType OriginalName=""TestUser"" NewName=""TestAccount"" />
</SchemaUpdate>";
			
			string filePath = sourceDirectory + Path.DirectorySeparatorChar
				+ "Schema" + Path.DirectorySeparatorChar
				+ "1-0-0-0" + Path.DirectorySeparatorChar
				+ "User.xml";
			
			string dirPath = Path.GetDirectoryName(filePath);
			
			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);
			
			using (StreamWriter writer = File.CreateText(filePath))
			{
				writer.Write(content);
				
				writer.Close();
			}
		}
		
		private void ClearTestEntities()
		{
			
			Type[] types = new Type[] {
				typeof(TestArticle),
				typeof(TestArticlePage),
				typeof(TestCategory),
				typeof(TestEntity),
				typeof(TestSample),
				typeof(TestUser),
				typeof(TestRole),
				typeof(TestEntity),
				typeof(EntityOne),
				typeof(EntityTwo),
				typeof(EntityThree),
				typeof(EntityFour) };

			Collection<IEntity> entities = new Collection<IEntity>();
			foreach (Type type in types)
				entities.Add((IEntity[])DataAccess.Data.Indexer.GetEntities(type));

			foreach (IEntity entity in entities)
			{
				DataAccess.Data.Deleter.Delete(entity);
			}
		}

		private void CreateDummyReferences(int count)
		{
			EntityReference reference = new EntityReference();
			reference.Entity1ID = Guid.NewGuid();
			reference.Entity2ID = Guid.NewGuid();
			reference.Property1Name = "TestProperty1-" + Guid.NewGuid().ToString().Substring(0, 5);
			reference.Property2Name = "TestProperty2-" + Guid.NewGuid().ToString().Substring(0, 5);
			reference.Type1Name = "TestUser";
			reference.Type2Name = "TestRole";
		}

	}
}
