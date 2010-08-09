﻿using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Tests
{
	static public class TestUtilities
	{
		/*static public string GetApplicationPath()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string location = assembly.Location;
			
			string[] parts = location.Split
			
			return Path.GetDirectoryName(location);
		}*/
		
		static public string GetTestingPath()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string location = assembly.Location;
			
			string[] parts = location.Split(Path.DirectorySeparatorChar);
			
			string path = parts[0] + Path.DirectorySeparatorChar + "_testing";
			
			return path;
		}
		
		static public void RegisterTestEntities()
		{
			if (Config.Mappings == null)
				Config.Mappings = ConfigFactory<MappingConfig>.NewConfig("Mappings");
			
			TestArticle.RegisterType();
			TestArticlePage.RegisterType();
			TestCategory.RegisterType();
			TestSample.RegisterType();
			TestEntity.RegisterType();
			EntityOne.RegisterType();
			EntityTwo.RegisterType();
			EntityThree.RegisterType();
			EntityFour.RegisterType();
			TestUser.RegisterType();
			TestRole.RegisterType();
			TestAccount.RegisterType();
			
		}
		

		static public void ClearTestEntities()
		{
			
		}

		/*static public void ClearTestEntities()
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
		}*/
		
		
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

		/// <summary>
		/// Deletes all the files and folders in the specified directory.
		/// </summary>
		/// <param name="directory"></param>
		static public void ClearDirectory(string directory)
		{
			if (Directory.Exists(directory))
			{
				foreach (string file in Directory.GetFiles(directory))
					File.Delete(file);
				
				foreach (string d in Directory.GetDirectories(directory))
				{
					ClearDirectory(d);
				}
				
				Directory.Delete(directory);
			}
		}
		
		/// <summary>
		/// Clears all folders and files from the testing directory.
		/// </summary>
		static public void ClearTestingDirectory()
		{
			ClearDirectory(GetTestingPath());
		}
		
	}
}
