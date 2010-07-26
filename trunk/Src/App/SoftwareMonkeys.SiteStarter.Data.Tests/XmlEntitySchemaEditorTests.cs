 using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
//using SoftwareMonkeys.SiteStarter.Modules;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
    [TestFixture]
    public class XmlEntitySchemaEditorTests
    {
		public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return TestUtilities.GetApplicationPath(); }
		}

		[Test]
        public void Test_Execute_RenameProperty()
        {
        	TestUser.RegisterType();
        	TestRole.RegisterType();
        	TestAccount.RegisterType();
        	
        	string originalName = "LastName";
        	string newName = "Surname";
        	
        	XmlEntitySchemaEditor editor = new XmlEntitySchemaEditor(TestUtilities.GetTestingPath());
        	
        	ClearDirectories(editor);
        	
        	TestUser user = CreateTestUser(editor.SourceDirectory);
        	CreateTestRenamePropertySchema(TestUtilities.GetTestingPath());
        	
        	editor.Execute();
        	
        	string convertedFilePath = editor.ConvertedDirectory + Path.DirectorySeparatorChar
        		+ user.GetType().FullName + Path.DirectorySeparatorChar 
        		+ user.ID.ToString() + ".xml";
        	
        	Assert.IsTrue(
        		File.Exists(convertedFilePath),
        		"Converted file not found: " + convertedFilePath);
        	
        	XmlDocument doc = new XmlDocument();
        	doc.Load(convertedFilePath);
        	
        	bool originalPropertyFound = doc.DocumentElement.GetElementsByTagName(originalName).Count > 0;
        	
        	Assert.IsFalse(originalPropertyFound);
        	
        	bool newPropertyFound = doc.DocumentElement.GetElementsByTagName(newName).Count > 0;
        	
        	Assert.IsTrue(newPropertyFound);
        	
        	ClearDirectories(editor);
        }
        
        
		[Test]
        public void Test_Execute_RenameType()
        {
        	TestUser.RegisterType();
        	TestRole.RegisterType();
        	TestAccount.RegisterType();
        	
        	//string originalName = "TestUser";
        	string newName = "TestAccount";
        	
        	XmlEntitySchemaEditor editor = new XmlEntitySchemaEditor(TestUtilities.GetTestingPath());
        	
        	ClearDirectories(editor);
        	
        	TestUser user = CreateTestUser(editor.SourceDirectory);
        	CreateTestRenameTypeSchema(TestUtilities.GetTestingPath());
        	
        	editor.Execute();
        	
        	Type type = EntitiesUtilities.GetType(newName);
        	
        	string convertedFilePath = editor.ConvertedDirectory + Path.DirectorySeparatorChar
        		+ type.ToString() + Path.DirectorySeparatorChar
        		+ user.ID.ToString() + ".xml";
        	
        	Assert.IsTrue(
        		File.Exists(convertedFilePath),
        		"Converted file not found: " + convertedFilePath);
        	
        	XmlDocument doc = new XmlDocument();
        	doc.Load(convertedFilePath);
        	
        	//bool originalPropertyFound = doc.DocumentElement.GetElementsByTagName(originalName).Count > 0;
        	
        	//Assert.IsFalse(originalPropertyFound);
        	
        	//bool newPropertyFound = doc.DocumentElement.GetElementsByTagName(newName).Count > 0;
        	
        	Assert.AreEqual(newName, doc.DocumentElement.Name, "Type name wasn't changed.");
        	
        	ClearDirectories(editor);
        }
        
        private void ClearDirectories(XmlEntitySchemaEditor editor)
        {
        	
        	if (Directory.Exists(editor.SourceDirectory))
        		Directory.Delete(editor.SourceDirectory, true);
        	
        	
        	if (Directory.Exists(editor.ConvertedDirectory))
        		Directory.Delete(editor.ConvertedDirectory, true);
        	
        	if (Directory.Exists(editor.ImportedDirectory))
        		Directory.Delete(editor.ImportedDirectory, true);
        }
        
        public TestUser CreateTestUser(string sourceDirectory)
        {
        	TestUser user = new TestUser();
        	user.ID = Guid.NewGuid();
        	user.FirstName = "First";
        	user.LastName = "Last";
        	user.Username = "TestUsername";
        	user.Email = "test@softwaremonkeys.net";
        	
        	string filePath = sourceDirectory + Path.DirectorySeparatorChar
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
        
        public void CreateTestRenamePropertySchema(string sourceDirectory)
        {
        	
        	string content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SchemaUpdate xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RenameProperty TypeName=""TestUser"" OriginalName=""LastName"" NewName=""Surname"" />
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
		
		#region Old tests
        [Test]
        public void Test_ConvertFileReferences_Files()
        {
            string xml = CreateDefaultXml();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            //string sourceDir = Path.Combine(ApplicationPath, "App_Data/Testing/Legacy");
            //string convertedDir = Path.Combine(ApplicationPath, "App_Data/Testing/Converted");
            //string importedDir = Path.Combine(ApplicationPath, "App_Data/Testing/Imported");

            NameValueCollection referenceMappings = new NameValueCollection();
            referenceMappings.Add("Feature.ProjectID", "Feature.Project-Project");

            XmlEntitySchemaEditor schema = new XmlEntitySchemaEditor(TestUtilities.GetTestingPath());

            Dictionary<string, XmlDocument> data = schema.ConvertFileReferences(doc, schema.ConvertedDirectory, referenceMappings);

            Assert.AreEqual(1, data.Keys.Count, "Invalid number of data items.");


            if (data.Keys.Count > 0)
            {
                foreach (string key in data.Keys)
                {
                    XmlDocument returnedDocument = (XmlDocument)data[key];

                    string entityType = returnedDocument.DocumentElement.Name;

                    Assert.AreEqual("EntityReference", entityType, "The entities don't match.");
                }
            }
            //XmlDocument doc, string convertedDirectory, NameValueCollection referenceMappings);
            
            
        	ClearDirectories(schema);
        }

    	[Test]
        public void Test_ConvertFileReferences_Doc()
        {
            string xml = CreateDefaultXml();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            NameValueCollection referenceMappings = new NameValueCollection();
            referenceMappings.Add("Feature.ProjectID", "Feature.Project-Project");

            XmlEntitySchemaEditor schema = new XmlEntitySchemaEditor(TestUtilities.GetTestingPath());

            Dictionary<string, XmlDocument> data = schema.ConvertFileReferences(doc, schema.ConvertedDirectory, referenceMappings);

            Assert.AreEqual(1, data.Keys.Count, "Invalid number of data items.");


            if (data.Keys.Count > 0)
            {
                foreach (string key in data.Keys)
                {
                    XmlDocument returnedDocument = (XmlDocument)data[key];

                    string entityType = returnedDocument.DocumentElement.Name;

                    Assert.AreEqual("EntityReference", entityType, "The entities don't match.");
                }
            }
            //XmlDocument doc, string convertedDirectory, NameValueCollection referenceMappings);
            
        	ClearDirectories(schema);
        }
        #endregion

        private string CreateDefaultXml()
        {
            string xml = @"<?xml version=""1.0""?>
<Feature xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ID>5b99527c-086d-4f27-baeb-d646dd141027</ID>
  <Name>Entities</Name>
  <Description />
  <ProjectID>d05fe519-0dfb-4cd9-8c5c-ceb4eaf7fa55</ProjectID>
  <ProjectVersion />
  <ActionIDs>
    <guid>e7ebb706-fe6e-4818-933c-010af639dd07</guid>
  </ActionIDs>
  <EntityIDs>
    <guid>04c926c6-5d21-4ca0-b851-0c911bbdb444</guid>
  </EntityIDs>
</Feature>";

            return xml;
        }
    }
}
