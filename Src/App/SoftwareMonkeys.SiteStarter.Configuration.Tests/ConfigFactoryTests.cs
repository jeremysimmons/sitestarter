using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Configuration.Tests
{
    [TestFixture]
    public class ConfigFactoryTests : BaseConfigurationTestFixture
    {        
        #region Save tests
        [Test]
        public void Test_LoadConfig()
        {
        	string directory = TestUtilities.GetTestingPath(this);
        	
        	if (!Directory.Exists(directory))
        		Directory.CreateDirectory(directory);
        	
        	
        	MockAppConfig config = (MockAppConfig)Configuration.Config.Application;
        	
        	Assert.IsNotNull(config, "config == null");
            
            string title = config.Title = "Test1";
            config.Settings.Add("TestSetting", "TestValue");
            config.Settings.Add("TestSetting2", true);
            
            config.Save();
            
            string configPath = config.FilePath;
            
            string pathVariation = config.PathVariation;
            
            config = null;
            
            Assert.IsTrue(File.Exists(configPath), "Config file not saved.");
            
            IAppConfig config2 = ConfigFactory<MockAppConfig>.LoadConfig(directory, "Application", pathVariation);
            
            
            Assert.IsNotNull(config2, "Configuration object wasn't re-loaded.");
            
            Assert.AreEqual("Test1", config2.Title, "Titles don't match.");
            Assert.AreEqual("TestValue", config2.Settings["TestSetting"], "Test setting 1 doesn't match.");
            Assert.AreEqual(true, config2.Settings["TestSetting2"], "Test setting 2 doesn't don't match.");
        }
        #endregion
        
        #region Path tests
        
        [Test]
        public void CreateConfigPath_Variation()
        {
        	string directory = TestUtilities.GetTestingPath(this);
        	
        	string variation = "local";
        	
        	string configName = "Application";
        	
        	string path = ConfigFactory<MockAppConfig>.CreateConfigPath(directory, configName, variation);
        	
        	string expected = directory + Path.DirectorySeparatorChar + configName + "." + variation + ".config";
        	
        	Assert.AreEqual(expected, path, "The paths don't match.");
        }
        
        
        [Test]
        public void CreateConfigPath_NoVariation()
        {
        	string directory = TestUtilities.GetTestingPath(this);
        	
        	string configName = "Application";
        	string variation = String.Empty;
        	
        	string path = ConfigFactory<MockAppConfig>.CreateConfigPath(directory, configName, variation);
        	
        	string expected = directory + Path.DirectorySeparatorChar + configName + ".config";
        	
        	Assert.AreEqual(expected, path, "The paths don't match.");
        }
        #endregion
    }
}
