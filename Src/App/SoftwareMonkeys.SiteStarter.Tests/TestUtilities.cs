using System;
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
		static public string GetTestingPath(BaseTestFixture executingFixture)
		{
			if (executingFixture == null)
				throw new ArgumentNullException("executingFixture");
			
			Assembly assembly = Assembly.GetExecutingAssembly();
			string location = assembly.Location;
			
			string[] parts = location.Split(Path.DirectorySeparatorChar);
			
			string path = parts[0] + Path.DirectorySeparatorChar + "_testing";
			
			path = path + Path.DirectorySeparatorChar + executingFixture.TestID.ToString();

			return path;
		}
		
		static public string GetTestApplicationPath(BaseTestFixture executingTestFixture, string applicationName)
		{
			if (executingTestFixture == null)
				throw new ArgumentNullException("executingTestFixture");
			
			string path = GetTestingPath(executingTestFixture);
			                             
			return path + Path.DirectorySeparatorChar + applicationName;
		}
		
		static public string GetTestDataPath(BaseTestFixture executingTestFixture, string applicationName)
		{
			return GetTestApplicationPath(executingTestFixture, applicationName) + Path.DirectorySeparatorChar + "App_Data";
		}
		

		static public void ClearTestEntities(BaseTestFixture executingTestFixture)
		{
			// The whole testing directory gets deleted so this is skipped
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
		static public void ClearTestingDirectory(BaseTestFixture executingTestFixture)
		{
			ClearDirectory(GetTestingPath(executingTestFixture));
		}
		
	}
}
