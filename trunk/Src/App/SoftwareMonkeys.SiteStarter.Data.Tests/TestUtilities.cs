using System;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.IO;
using SoftwareMonkeys.SiteStarter.Data.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Entities;

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
		


		static public void ClearTestEntities()
		{
			TestArticle.RegisterType();
			TestArticlePage.RegisterType();
			TestCategory.RegisterType();
			TestUser.RegisterType();
			TestRole.RegisterType();
			TestEntity.RegisterType();
			EntityOne.RegisterType();
			EntityTwo.RegisterType();
			EntityThree.RegisterType();
			EntityFour.RegisterType();
			TestSample.RegisterType();
			
			Type[] types = new Type[] {
				typeof(TestArticle),
				typeof(TestArticlePage),
				typeof(TestCategory),
				typeof(TestUser),
				typeof(TestRole),
				typeof(TestEntity),
				typeof(EntityOne),
				typeof(EntityTwo),
				typeof(EntityThree),
				typeof(EntityFour),
				typeof(TestSample)
			};

			Collection<IEntity> entities = new Collection<IEntity>();
			foreach (Type type in types)
				entities.Add(DataAccess.Data.Indexer.GetEntities(type));

			foreach (IEntity entity in entities)
			{
				DataAccess.Data.Deleter.Delete(entity);
			}
			
			foreach (EntityIDReference r in DataAccess.Data.Referencer.GetReferences("TestUser", "TestRole"))
			{
				DataAccess.Data.Deleter.Delete(r);
			}
			
			foreach (EntityIDReference r in DataAccess.Data.Referencer.GetReferences("TestArticle", "TestArticlePage"))
			{
				DataAccess.Data.Deleter.Delete(r);
			}
			foreach (EntityIDReference r in DataAccess.Data.Referencer.GetReferences("TestArticle", "TestCategory"))
			{
				DataAccess.Data.Deleter.Delete(r);
			}
		}
		
		
		static public void CreateDummyReferences(int count)
		{
			EntityReference reference = new EntityReference();
			reference.Entity1ID = Guid.NewGuid();
			reference.Entity2ID = Guid.NewGuid();
			reference.Property1Name = "TestProperty1-" + Guid.NewGuid().ToString().Substring(0, 5);
			reference.Property2Name = "TestProperty2-" + Guid.NewGuid().ToString().Substring(0, 5);
			reference.Type1Name = "TestUser";
			reference.Type2Name = "TestRole";
		}

		
	}
}
