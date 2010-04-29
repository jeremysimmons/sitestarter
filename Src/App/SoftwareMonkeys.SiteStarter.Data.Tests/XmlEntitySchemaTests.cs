 using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
//using SoftwareMonkeys.SiteStarter.Modules;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Xml;
using System.Collections.Specialized;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
    [TestFixture]
    public class XmlEntitySchemaTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
            //   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
            //     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }

        [Test]
        public void Test_ConvertFileReferences_Files()
        {
            string xml = CreateDefaultXml();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string sourceDir = Path.Combine(ApplicationPath, "App_Data/Testing/Legacy");
            string convertedDir = Path.Combine(ApplicationPath, "App_Data/Testing/Converted");
            string importedDir = Path.Combine(ApplicationPath, "App_Data/Testing/Imported");

            NameValueCollection referenceMappings = new NameValueCollection();
            referenceMappings.Add("Feature.ProjectID", "Feature.Project-Project");

            XmlEntitySchema schema = new XmlEntitySchema(sourceDir, convertedDir, importedDir);

            Dictionary<string, XmlDocument> data = schema.ConvertFileReferences(doc, convertedDir, referenceMappings);

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
        }

    	[Test]
        public void Test_ConvertFileReferences_Doc()
        {
            string xml = CreateDefaultXml();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string sourceDir = Path.Combine(ApplicationPath, "App_Data/Testing/Legacy");
            string convertedDir = Path.Combine(ApplicationPath, "App_Data/Testing/Converted");
            string importedDir = Path.Combine(ApplicationPath, "App_Data/Testing/Imported");

            NameValueCollection referenceMappings = new NameValueCollection();
            referenceMappings.Add("Feature.ProjectID", "Feature.Project-Project");

            XmlEntitySchema schema = new XmlEntitySchema(sourceDir, convertedDir, importedDir);

            Dictionary<string, XmlDocument> data = schema.ConvertFileReferences(doc, convertedDir, referenceMappings);

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
        }

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
