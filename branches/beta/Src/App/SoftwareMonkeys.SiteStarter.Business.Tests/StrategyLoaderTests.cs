using System;
using NUnit.Framework;
using System.IO;
using System.Xml.Serialization;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyLoaderTests : BaseBusinessTestFixture
	{
		string MockApplicationName = "TestApplication";
		
		[Test]
		public void Test_LoadFromFile()
		{
			
			StrategyInfo strategy = CreateMockStrategy();
			
			SaveMockStrategy(strategy);
			
			StrategyLoader loader = new StrategyLoader();
			loader.FileNamer.StrategiesInfoDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			string fullPath = loader.FileNamer.CreateInfoFilePath(strategy);
			
			StrategyInfo foundStrategy = loader.LoadFromFile(fullPath);
			
			Assert.IsNotNull(foundStrategy, "The strategy wasn't saved or couldn't be loaded.");
		}
		
		[Test]
		public void Test_LoadFromDirectory()
		{
			
			StrategyInfo strategy = CreateMockStrategy();
			StrategyInfo strategy2 = CreateMockStrategy();
			strategy2.TypeName = "TestArticle";
			
			SaveMockStrategy(strategy);
			SaveMockStrategy(strategy2);
			
			StrategyLoader loader = new StrategyLoader();
			loader.FileNamer.StrategiesInfoDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			string fullPath = loader.FileNamer.CreateInfoFilePath(strategy);
			string fullPath2 = loader.FileNamer.CreateInfoFilePath(strategy2);
			
			StrategyInfo[] foundStrategies = loader.LoadFromDirectory();
			
			Assert.AreEqual(2, foundStrategies.Length, "Invalid number of strategies found.");
			
			// Switch the order around if necessary
			if (foundStrategies[0].TypeName == strategy2.TypeName)
			{
				foundStrategies = new StrategyInfo[]{
					foundStrategies[1],
					foundStrategies[0]
				};
			}
			
			Assert.AreEqual(strategy.TypeName, foundStrategies[0].TypeName);
			Assert.AreEqual(strategy2.TypeName, foundStrategies[1].TypeName);
		}
		
		public void SaveMockStrategy(StrategyInfo strategy)
		{
			StrategyFileNamer namer = new StrategyFileNamer();
			namer.StrategiesInfoDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			string fullPath = namer.CreateInfoFilePath(strategy);
			
			if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
				Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
						
			using (StreamWriter writer = File.CreateText(fullPath))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(StrategyInfo));
				serializer.Serialize(writer, strategy);
				writer.Close();
			}
			
		}
		
		public StrategyInfo CreateMockStrategy()
		{
			IStrategy mockStrategy = new MockRetrieveTestUserStrategy();
			
			return StrategyInfo.ExtractInfo(mockStrategy.GetType())[0];
		}
		
		public string GetMockStrategiesDirectoryPath(string applicationName)
		{
			return TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Strategies";
		}
	}
}
