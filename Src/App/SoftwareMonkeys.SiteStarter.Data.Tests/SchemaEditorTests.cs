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
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class SchemaEditorTests
	{
		public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
		}
		
		public SchemaEditorTests()
		{
			
			TestUtilities.RegisterTestEntities();
		}

		#region Singleton tests
		[Test]
		public void Test_Data_EnsureInitialized()
		{
			DataProvider provider = DataAccess.Data;

			Assert.IsNotNull((object)provider);
		}
		#endregion


		[TearDown]
		public void CleanUp()
		{
			ClearTestEntities();

		}

		[Test]
		public void Test_ApplySchema_RenameType()
		{
			TestUser.RegisterType();
			TestAccount.RegisterType();
			
			ClearTestEntities();
			
			CreateTestRenameTypeSchema(TestUtilities.GetTestingPath());
			TestUser user = CreateTestUser(TestUtilities.GetTestingPath());
			
			DataAccess.Data.Save(user);
			
			DataAccess.Data.Schema.ApplySchema(TestUtilities.GetTestingPath());
			
			TestAccount account = DataAccess.Data.GetEntity<TestAccount>("ID", user.ID);
			
			Assert.IsNotNull(account, "account not loaded");
			
			Assert.AreEqual(user.ID.ToString(), account.ID.ToString(), "IDs don't match.");
			
			ClearTestEntities();
		}
		
		
		[Test]
		public void Test_RenameProperty()
		{
			TestUser.RegisterType();
			//TestAccount.RegisterType();
			
			ClearTestEntities();
			
			CreateTestRenameTypeSchema(TestUtilities.GetTestingPath());
			TestUser user = CreateTestUser(TestUtilities.GetTestingPath());
			
			DataAccess.Data.Save(user);
			
			DataAccess.Data.Schema.Suspend();
			
			//DataAccess.Data.Schema.ApplySchema(TestUtilities.GetTestingPath());
			DataAccess.Data.Schema.RenameProperty("TestUser", "LastName", "Surname");
			
			DataAccess.Data.Schema.Resume();
			
			TestUser found = DataAccess.Data.GetEntity<TestUser>("ID", user.ID);
			
			Assert.IsNotNull(found, "user object not loaded");
			
			Assert.AreEqual(user.ID.ToString(), found.ID.ToString(), "IDs don't match.");
			
			Assert.AreEqual(user.LastName, found.Surname, "LastName wasn't converted to Surname");
			
			ClearTestEntities();
		}
		
		
		[Test]
		public void Test_ApplySchema_RenameProperty()
		{
			TestUser.RegisterType();
			TestAccount.RegisterType();
			
			ClearTestEntities();
			
			CreateTestRenameTypeSchema(TestUtilities.GetTestingPath());
			TestUser user = CreateTestUser(TestUtilities.GetTestingPath());
			
			DataAccess.Data.Save(user);
			
			DataAccess.Data.Schema.ApplySchema(TestUtilities.GetTestingPath());
			
			TestAccount account = DataAccess.Data.GetEntity<TestAccount>("ID", user.ID);
			
			Assert.IsNotNull(account, "account not loaded");
			
			Assert.AreEqual(user.ID.ToString(), account.ID.ToString(), "IDs don't match.");
			
			ClearTestEntities();
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
			
			TestArticle.RegisterType();
			TestCategory.RegisterType();
			TestUser.RegisterType();
			TestRole.RegisterType();
			
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
				entities.Add((IEntity[])DataAccess.Data.GetEntities(type));

			foreach (IEntity entity in entities)
			{
				DataAccess.Data.Stores[entity.GetType()].Delete(entity);
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
