using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategySaverTests : BaseBusinessTestFixture
	{
		string MockApplicationName = "TestApplication";
		
		[Test]
		public void Test_SaveToFile()
		{
			StrategyInfo strategy = CreateMockStrategy();
			
			StrategySaver saver = new StrategySaver();
			saver.FileNamer.StrategiesDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			saver.SaveToFile(strategy);
			
			StrategyInfo foundStrategy = LoadMockStrategy(strategy);
			
			Assert.IsNotNull(foundStrategy, "The strategy wasn't saved or couldn't be loaded.");
		}
		
		public StrategyInfo LoadMockStrategy(StrategyInfo strategy)
		{
			StrategyFileNamer namer = new StrategyFileNamer();
			namer.StrategiesDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			string fullPath = namer.CreateFilePath(strategy);
			
			if (!File.Exists(fullPath))
				Assert.Fail("Mock strategy not found. It must not have been saved properly or must have used wrong file name.");
			
			StrategyInfo foundStrategy = null;
			
			using (StreamReader reader = new StreamReader(File.OpenRead(fullPath)))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(StrategyInfo));
				foundStrategy = (StrategyInfo)serializer.Deserialize(reader);
				reader.Close();
			}
			
			return foundStrategy;
		}
		
		public StrategyInfo CreateMockStrategy()
		{
			IStrategy mockStrategy = new MockRetrieveStrategy();
			
			return StrategyInfo.ExtractInfo(typeof(MockRetrieveStrategy))[0];
		}
		
		public string GetMockStrategiesDirectoryPath(string applicationName)
		{
			
			return TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Strategies";
		}
	}
}
