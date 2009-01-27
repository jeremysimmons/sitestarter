using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Web;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
          //  get { return @"f:\SoftwareMonkeys\WorkHub\Application2\Web\"; }
         //   get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }

        public string WorkingDirectory
        {
            // TODO: Path MUST NOT be hard coded
            //get { return @"f:\SoftwareMonkeys\WorkHub\Application2\WebFacadeTests\Temp"; }
            //   get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }

        #region Initialization
        [SetUp]
        public void Initialize()
        {
            Config config = CreateTestConfig();
            SaveTestConfig(config);
        }
        #endregion

        #region Dispose functions
        [TearDown]
        public void Dispose()
        {
            DeleteTestConfig();
        }
        #endregion

        #region Prelim tests
        [Test]
        public void Prelim_TestWorkingDirectory()
        {
            // Create the path for a test file
            string path = WorkingDirectory + @"\TestFile.txt";

            // Ensure the working directory exists
            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);

            // Create the test file
            StreamWriter writer = File.CreateText(path);
            writer.Write("blah");
            writer.Close();

            // Delete the test file
            File.Delete(path);
        }
        #endregion

        #region Singleton tests
        [Test]
        public void Test_Current()
        {
            Web.Config config = Web.Config.Current;

            Assert.IsNotNull(config);
        }

        [Test]
        public void Test_Save()
        {
            SoftwareMonkeys.SiteStarter.Web.Config config = new SoftwareMonkeys.SiteStarter.Web.Config();
            config.ApplicationPath = HttpContext.Current.Request.ApplicationPath;
            config.ApplicationUrl = HttpContext.Current.Request.Url.ToString().ToLower().Replace("/setup.aspx", "");
            config.PhysicalPath = HttpContext.Current.Request.PhysicalApplicationPath;
            config.BackupDirectory = "Backup";
            config.DataDirectory = "Data";
            //config.PrimaryAdministrator = user;
         
            config.Save();

            Assert.IsNotNull(Config.Current);
        }
        #endregion

        #region Utility functions
        public IConfig CreateTestConfig()
        {
            IConfig config = new AppConfig();
            config.PhysicalPath = WorkingDirectory + @"\Test.Config";
            config.ApplicationPath = "Test/";
            config.ApplicationUrl = "Test/";
            config.PhysicalPath = WorkingDirectory;
            config.BackupDirectory = "Backup";
            config.DataDirectory = "Data";
            // config.FriendlyDateFormat = "D";
            // config.HostingDirectory = "Hosted";
            // config.AttachmentDirectory = "Attachments";
            //config.PrimaryAdministrator = user;
            // config.ProjectID = new Guid("2b60aba5-db91-4af8-a5bf-e8ad948d50cd");
            // config.WorkHubID = new Guid("14bbbd37-51f6-4d82-a8f5-66fa737f4438");
            // config.WorkHubUrl = "http://www.softwaremonkeys.net/Hub";
            // config.WorkHubUsername = "WorkHubClient";
            // config.WorkHubPassword = "d8h3ns6gh";
            // config.WorkHubHostedUrl = "http://localhost/SoftwareMonkeys/WorkHub/Application/Web";

            return config;
        }

        /// <summary>
        /// Saves the provided configuration object.
        /// </summary>
        /// <param name="config">The configuration object to save.</param>
        public void SaveTestConfig(AppConfig config)
        {
            if (!Directory.Exists(WorkingDirectory))
                Directory.CreateDirectory(WorkingDirectory);

            FileStream stream = File.Create(config.PhysicalPath + @"\Test.config");
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            serializer.Serialize(stream, config);
            stream.Close();
        }

        /// <summary>
        /// Deletes the test config file.
        /// </summary>
        public void DeleteTestConfig()
        {
            string path = Config.PhysicalConfigPath;
            Config.Current = null;
            File.Delete(path);
        }
        #endregion
    }
}
