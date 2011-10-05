using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Configuration.Tests
{
	[TestFixture]
	public class ConfigSaverTests : BaseConfigurationTestFixture
	{		
		[Test]
		public void Test_Save()
		{
			string path = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "TestConfig.xml";
			
			MockAppConfig config = new MockAppConfig();
			config.FilePath = path;
			
			ConfigSaver saver = new ConfigSaver();
			saver.Save(config);
			
			Assert.IsTrue(File.Exists(path), "The configuration file wasn't found.");
		}
	}
}
