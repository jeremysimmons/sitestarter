using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	[TestFixture]
	public class XmlEntityManagerTests
	{
		public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath.Trim('\\'); }
		}

		public XmlEntityManagerTests()
		{

			// TODO: Check if needed for testing the XmlEntityManager class
			//   Config.Initialize(ApplicationPath, "Testing");
			
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

        [SetUp]
        public void Initialize()
        {
            DeleteTestingDirectory();
        }

        [TearDown]
        public void Dispose()
        {
            DeleteTestingDirectory();
        }


        [Test]
        public void Test_ImportTypeFromDirectory()
        {
            TestArticle.RegisterType();
            TestArticlePage.RegisterType();

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));

            // Create default objects
            TestArticle article = new TestArticle();
            article.ID = Guid.NewGuid();
            article.Title = "Test article";

            TestArticlePage page = new TestArticlePage();
            page.ID = Guid.NewGuid();
            page.Title = "Test page";

            // Create paths
            string importablesPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Importables";
            string importedPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Imported";

            string articlesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string pagesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();


            // Create directories
            if (!Directory.Exists(articlesPath))
                Directory.CreateDirectory(articlesPath);
            if (!Directory.Exists(pagesPath))
                Directory.CreateDirectory(pagesPath);

            // Serialize objects to files in test folder
            XmlEntityManager.SaveToFile(article, articlesPath);
            //XmlEntityManager.SaveToFile(page, pagesPath);

            // Import all objects from test folder
            XmlEntityManager.ImportTypeFromDirectory(article.GetType().FullName, importablesPath, importedPath);

            // Load the imported objects
            IEntity[] entities = DataAccess.Data.GetEntities<TestArticle>();

            Assert.AreEqual(1, entities.Length, "Incorrect number of imported objects found.");

            Assert.AreEqual(0, Directory.GetFiles(articlesPath).Length, "Xml entity file hasn't been moved after being imported.");

            DataAccess.Data.Delete(article);
            DataAccess.Data.Delete(page);

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));
        }



        [Test]
        public void Test_ImportTypeFromDirectory_Reference()
        {
            TestArticle.RegisterType();
            TestArticlePage.RegisterType();

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));

            // Create default objects
            TestArticle article = new TestArticle();
            article.ID = Guid.NewGuid();
            article.Title = "Test article";

            TestArticlePage page = new TestArticlePage();
            page.ID = Guid.NewGuid();
            page.Title = "Test page";

            article.Pages = new TestArticlePage[] { page };

            // Create paths
            string importablesPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Importables";
            string importedPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Imported";

            string articlesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().FullName;
            string referencesPath = Path.GetDirectoryName(XmlEntitySchema.CreateReferenceFilePath(importablesPath, EntitiesUtilities.GetReferences(article)[0]));
            string referencesImportedPath = Path.GetDirectoryName(XmlEntitySchema.CreateReferenceFilePath(importedPath, EntitiesUtilities.GetReferences(article)[0]));


            // Serialize objects to files in test folder
            XmlEntityManager.SaveToFile(article, articlesPath);
            //XmlEntityManager.SaveToFile(page, pagesPath);

            // Import all objects from test folder

            string type = Path.GetFileName(referencesPath);
           
            XmlEntityManager.ImportTypeFromDirectory(type, importablesPath, importedPath);

            // Load the imported objects
            EntityReferenceCollection references = DataAccess.Data.GetReferences(article.GetType(), article.ID, "Pages", page.GetType(), false);

            Assert.AreEqual(1, references.Count, "Incorrect number of imported references found.");

            Assert.AreEqual(0, Directory.GetFiles(referencesPath).Length, "Xml reference file hasn't been moved after being imported.");

            DataAccess.Data.Delete(article);
            DataAccess.Data.Delete(page);

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));
        }


        [Test]
        public void Test_ImportAllFromDirectory()
        {

            TestArticle.RegisterType();
            TestArticlePage.RegisterType();

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));

            DeleteTestingDirectory();

            // Create default objects
            TestArticle article = new TestArticle();
            article.ID = Guid.NewGuid();
            article.Title = "Test article";

            TestArticlePage page = new TestArticlePage();
            page.ID = Guid.NewGuid();
            page.Title = "Test page";

            article.Pages = new TestArticlePage[] { page };

            // Create paths
            string importablesPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Importables";
            string importedPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Imported";

            string articlesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string articlesImportedPath = importedPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string pagesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string referencesPath = Path.GetDirectoryName(XmlEntitySchema.CreateReferenceFilePath(importablesPath, EntitiesUtilities.GetReferences(article)[0]));
            string referencesImportedPath = Path.GetDirectoryName(XmlEntitySchema.CreateReferenceFilePath(importedPath, EntitiesUtilities.GetReferences(article)[0]));
            //string referencesPath = importablesPath + Path.DirectorySeparatorChar + DataUtilities.GetDataStoreName(new string[] { "TestArticle", "TestArticlePage" });
            
            //article.GetType().ToString();


            // Create directories
            if (!Directory.Exists(articlesPath))
                Directory.CreateDirectory(articlesPath);
            if (!Directory.Exists(pagesPath))
                Directory.CreateDirectory(pagesPath);

            // Serialize objects to files in test folder
            XmlEntityManager.SaveToFile(article, articlesPath);
            //XmlEntityManager.SaveToFile(page, pagesPath);

            // Import all objects from test folder
            XmlEntityManager.ImportAllFromDirectory(importablesPath, importedPath);

            
            // Load the imported objects
            IEntity[] articles = DataAccess.Data.GetEntities<TestArticle>();

            Assert.AreEqual(1, articles.Length, "Incorrect number of imported objects found.");

            Assert.AreEqual(0, Directory.GetFiles(articlesPath).Length, "Xml entity file hasn't been moved after being imported.");


            // Load the references
            EntityReferenceCollection references = DataAccess.Data.GetReferences(article.GetType(), article.ID, "Pages", page.GetType(), false);
           

            Assert.AreEqual(1, references.Count, "Incorrect number of references found.");

            Assert.AreEqual(0, Directory.GetFiles(referencesPath).Length, "Xml reference hasn't been moved after being imported.");



            string[] files = Directory.GetFiles(referencesPath);

            Assert.AreEqual(0, files.Length, "The references file still found in converted directory.");


            string[] articlesFiles = Directory.GetFiles(articlesPath);

            Assert.AreEqual(0, articlesFiles.Length, "The articles file still found in converted directory.");




            string[] importedFiles = Directory.GetFiles(referencesImportedPath);

            Assert.AreEqual(1, importedFiles.Length, "The references weren't found in imported directory.");


            string[] importedArticles = Directory.GetFiles(articlesImportedPath);

            Assert.AreEqual(1, importedArticles.Length, "The articles weren't found in imported directory.");



            DataAccess.Data.Delete(article);

            DataAccess.Data.Delete(page);

            foreach (EntityReference reference in references)
            {
                DataAccess.Data.Delete(reference);
            }

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));
        }

        [Test]
        public void Test_MarkAsImported()
        {

            DeleteTestingDirectory();

            // Create default objects
            TestArticle article = new TestArticle();
            article.ID = Guid.NewGuid();
            article.Title = "Test article";

            TestArticlePage page = new TestArticlePage();
            page.ID = Guid.NewGuid();
            page.Title = "Test page";

            article.Pages = new TestArticlePage[] { page };

            // Create paths
            string importablesPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Importables";
            string importedPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Imported";

            string articlesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string articlesImportedPath = importedPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string pagesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string referencesPath = Path.GetDirectoryName(XmlEntitySchema.CreateReferenceFilePath(importablesPath, EntitiesUtilities.GetReferences(article)[0]));
            string referencesImportedPath = Path.GetDirectoryName(XmlEntitySchema.CreateReferenceFilePath(importedPath, EntitiesUtilities.GetReferences(article)[0]));
            //string referencesPath = importablesPath + Path.DirectorySeparatorChar + DataUtilities.GetDataStoreName(new string[] { "TestArticle", "TestArticlePage" });

            //article.GetType().ToString();


            // Create directories
            if (!Directory.Exists(articlesPath))
                Directory.CreateDirectory(articlesPath);
            if (!Directory.Exists(pagesPath))
                Directory.CreateDirectory(pagesPath);
            if (!Directory.Exists(referencesImportedPath))
                Directory.CreateDirectory(referencesImportedPath);
            if (!Directory.Exists(articlesImportedPath))
                Directory.CreateDirectory(articlesImportedPath);

            // Serialize objects to files in test folder
            XmlEntityManager.SaveToFile(article, articlesPath);
            //XmlEntityManager.SaveToFile(page, pagesPath);


            XmlEntityManager manager = new XmlEntityManager(articlesPath, articlesImportedPath);
            manager.MarkAsImported(article);
            XmlEntityManager manager2 = new XmlEntityManager(referencesPath, referencesImportedPath);
            manager.MarkAsImported(EntitiesUtilities.GetReferences(article)[0]);



            string[] files = Directory.GetFiles(referencesPath);

            Assert.AreEqual(0, files.Length, "The references file still found in converted directory.");


            string[] articlesFiles = Directory.GetFiles(articlesPath);

            Assert.AreEqual(0, articlesFiles.Length, "The articles file still found in converted directory.");




            string[] importedFiles = Directory.GetFiles(referencesImportedPath);

            Assert.AreEqual(1, importedFiles.Length, "The references weren't found in imported directory.");


            string[] importedArticles = Directory.GetFiles(articlesImportedPath);

            Assert.AreEqual(1, importedArticles.Length, "The articles weren't found in imported directory.");
        }


        [Test]
        public void Test_ImportAllFromDirectory_References()
        {

            TestArticle.RegisterType();
            TestArticlePage.RegisterType();

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));

            // Create default objects
            TestArticle article = new TestArticle();
            article.ID = Guid.NewGuid();
            article.Title = "Test article";

            TestArticlePage page = new TestArticlePage();
            page.ID = Guid.NewGuid();
            page.Title = "Test page";

            // Create paths
            string importablesPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Importables";
            string importedPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing"
                + Path.DirectorySeparatorChar + "Imported";

            string articlesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
            string pagesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();


            // Create directories
            if (!Directory.Exists(articlesPath))
                Directory.CreateDirectory(articlesPath);
            if (!Directory.Exists(pagesPath))
                Directory.CreateDirectory(pagesPath);

            // Serialize objects to files in test folder
            XmlEntityManager.SaveToFile(article, articlesPath);
            //XmlEntityManager.SaveToFile(page, pagesPath);

            // Import all objects from test folder
            XmlEntityManager.ImportAllFromDirectory(importablesPath, importedPath);

            // Load the imported objects
            IEntity[] entities = DataAccess.Data.GetEntities<TestArticle>();

            Assert.AreEqual(1, entities.Length, "Incorrect number of imported objects found.");

            Assert.AreEqual(0, Directory.GetFiles(articlesPath).Length, "Xml entity file hasn't been moved after being imported.");

            DataAccess.Data.Delete(article);
            DataAccess.Data.Delete(page);

            ClearAll(typeof(TestArticle));
            ClearAll(typeof(TestArticlePage));
        }


        [Test]
        public void Test_SaveToFile()
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Testing the SaveToFile function.", NLog.LogLevel.Debug))
            {
                ClearAll(typeof(TestArticle));
                ClearAll(typeof(TestArticlePage));
                //DeleteTestingDirectory();

                TestArticle.RegisterType();
                TestArticlePage.RegisterType();

                // Create default objects
                TestArticle article = new TestArticle();
                article.ID = Guid.NewGuid();
                article.Title = "Test article";

                TestArticlePage page = new TestArticlePage();
                page.ID = Guid.NewGuid();
                page.Title = "Test page";

                article.Pages = new TestArticlePage[] { page };

                // Create paths
                string importablesPath = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                    + Path.DirectorySeparatorChar + "Testing"
                    + Path.DirectorySeparatorChar + "Importables";

                string articlesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
                string pagesPath = importablesPath + Path.DirectorySeparatorChar + article.GetType().ToString();
                string referencesPath = Path.GetDirectoryName(XmlEntitySchema.CreateReferenceFilePath(importablesPath, EntitiesUtilities.GetReferences(article)[0]));

                // Create test directory
                if (!Directory.Exists(articlesPath))
                    Directory.CreateDirectory(articlesPath);

                // Serialize objects to files in test folder
                XmlEntityManager.SaveToFile(article, articlesPath);

                // Import all objects from test folder
                string[] referenceFiles = Directory.GetFiles(referencesPath);
                int referenceCount = referenceFiles.Length;
                AppLogger.Debug("Checking whether the reference file was saved.");
                AppLogger.Debug("References path: " + referencesPath);
                AppLogger.Debug("# references: " + referenceCount);
                Assert.AreEqual(1, referenceCount, "Incorrect number of reference files found.");


                string expectedReferenceFileName = XmlEntitySchema.CreateReferenceFilePath(importablesPath,
                    EntitiesUtilities.GetReferences(article)[0]);


                AppLogger.Debug("Reference file name: " + referenceFiles[0]);
                AppLogger.Debug("Expected reference file name: " + expectedReferenceFileName);

                Assert.AreEqual(
                    Path.GetFileName(expectedReferenceFileName), Path.GetFileName(referenceFiles[0]), "The reference file name is invalid.");

                Assert.AreEqual(
                    Path.GetFileName(Path.GetDirectoryName(expectedReferenceFileName)),
                    Path.GetFileName(Path.GetDirectoryName(referenceFiles[0])), "The reference folder name is invalid.");


                Assert.AreEqual(
                    expectedReferenceFileName, referenceFiles[0], "The reference path is invalid.");

                // Import all objects from test folder
                int objectCount = Directory.GetFiles(articlesPath).Length;
                AppLogger.Debug("Checking whether the reference file was saved.");
                AppLogger.Debug("Objects path: " + articlesPath);
                AppLogger.Debug("# objects: " + objectCount);
                Assert.AreEqual(1, objectCount, "Incorrect number of files found.");

                //Directory.Delete(importablesPath, true);

                ClearAll(typeof(TestArticle));
                ClearAll(typeof(TestArticlePage));
                //DeleteTestingDirectory();
            }
        }

        public void ClearAll(Type type)
        {
            foreach (IEntity entity in DataAccess.Data.GetEntities(type))
            {
                DataAccess.Data.Delete(entity);
            }
        }

        public void DeleteTestingDirectory()
        {
            
            string testingDirectory = ApplicationPath + Path.DirectorySeparatorChar + "App_Data"
                + Path.DirectorySeparatorChar + "Testing";

            if (Directory.Exists(testingDirectory))
            {
                Directory.Delete(testingDirectory, true);
            }
        }
	}
}
