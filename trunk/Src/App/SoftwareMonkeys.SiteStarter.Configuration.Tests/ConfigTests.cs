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
    public class ConfigTests
    {
        public string ApplicationPath
        {
            // TODO: Path MUST NOT be hard coded
         //   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
       //     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
            get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
        }
        
        public ConfigTests()
        {
            //Config.Initialize(ApplicationPath, "");
        }

        #region Singleton tests
        [Test]
        public void Test_Current()
        {
            IAppConfig config = Configuration.Config.Application;

            Assert.IsNotNull(config);
        }
        #endregion

    }
}
