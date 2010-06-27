
using System;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	static public class TestUtilities
	{
		static public string GetApplicationPath()
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath.Trim('\\');
		}
		
		static public string GetTestingPath()
		{
			return GetApplicationPath() + Path.DirectorySeparatorChar +
				"Testing";
		}
		
		static public void RegisterTestEntities()
        {
			if (Config.Mappings == null)
				Config.Mappings = ConfigFactory<MappingConfig>.NewConfig("Mappings");
			Entities.TestArticle.RegisterType();
			Entities.TestArticlePage.RegisterType();
			Entities.TestCategory.RegisterType();
			Entities.TestSample.RegisterType();
			Entities.TestEntity.RegisterType();
			Entities.EntityOne.RegisterType();
			Entities.EntityTwo.RegisterType();
			Entities.EntityThree.RegisterType();
			Entities.EntityFour.RegisterType();
			
			
			
        }
	}
}
