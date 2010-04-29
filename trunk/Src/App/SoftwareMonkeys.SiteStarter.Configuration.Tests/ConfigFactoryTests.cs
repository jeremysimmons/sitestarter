using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Configuration.Tests
{
    [TestFixture]
    public class ConfigFactoryTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
         //   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
       //     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }
        
        public ConfigFactoryTests()
        {
            //Config.Initialize(ApplicationPath, "");
        }


        #region Save tests
        [Test]
        public void Test_SaveConfig_And_LoadConfig()
        {
        	string fullPath = ApplicationPath + @"\App_Data\Testing";
        	
        	if (!Directory.Exists(fullPath))
        		Directory.CreateDirectory(fullPath);
        	
        	AppConfig config = (AppConfig)Configuration.Config.Application;
            
            string title = config.Title = "Test1";
            config.Settings["TestSetting"] = "TestValue";
            config.Settings["TestSetting2"] = true;
            
            ConfigFactory<AppConfig>.SaveConfig(fullPath, config, "");

            config = null;
            
            IAppConfig config2 = ConfigFactory<AppConfig>.LoadConfig(fullPath, "Application", "");
            
            
            Assert.IsNotNull(config2);
            
            Assert.AreEqual("Test1", config2.Title, "Titles don't match.");
            Assert.AreEqual("TestValue", config2.Settings["TestSetting"], "Test setting 1 doesn't match.");
            Assert.AreEqual(true, config2.Settings["TestSetting2"], "Test setting 2 doesn't don't match.");
        }
        #endregion

    }
}
