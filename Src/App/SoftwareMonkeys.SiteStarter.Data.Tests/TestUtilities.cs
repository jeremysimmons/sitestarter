
using System;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data.Tests
{
	static public class TestUtilities
	{
		
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
