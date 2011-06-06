using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyInitializerTests : BaseBusinessTestFixture
	{
		string MockApplicationName = "TestApplication";
		
		[Test]
		public void Test_Initialize_StrategiesProvided()
		{
			IStrategy strategy = new MockRetrieveStrategy();
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			StrategyInfo[] strategies = new StrategyInfo[]{info};
			
			StrategyInitializer initializer = new StrategyInitializer();
			initializer.FileNamer.StrategiesInfoDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			initializer.Initialize(strategies);
			
			Assert.IsTrue(StrategyState.IsInitialized, "Strategies weren't initialized or initialization wasn't detected.");
			Assert.Greater(StrategyState.Strategies.Count, 0, "Invalid number of strategies initialized.");
			
			StrategyInfo foundInfo = StrategyState.Strategies[info.Action, info.TypeName];
			
			Assert.IsNotNull(foundInfo, "The module info wasn't found.");
		}
		
		[Test]
		public void Test_Initialize_StrategiesScanned()
		{
			StrategyInitializer initializer = new StrategyInitializer();
			initializer.FileNamer.StrategiesInfoDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business").Location
			};
			
			initializer.Scanner.AssemblyPaths = assemblyPaths;
			
			initializer.Initialize();
			
			Assert.IsTrue(StrategyState.IsInitialized, "Strategies weren't initialized or initialization wasn't detected.");
			Assert.Greater(StrategyState.Strategies.Count, 0, "Invalid number of strategies initialized.");
		
		}
		
		[Test]
		public void Test_Initialize_StrategiesLoaded()
		{
			StrategyInitializer initializer = new StrategyInitializer();
			initializer.FileNamer.StrategiesInfoDirectoryPath = GetMockStrategiesDirectoryPath(MockApplicationName);
			
			IStrategy strategy = new MockRetrieveStrategy();
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			info.Action = "MockAction";
			info.TypeName = "TestUser";
			info.StrategyType = strategy.GetType().FullName;
			
			StrategyInfo[] strategies = new StrategyInfo[]{info};
			
			StrategySaver saver = new StrategySaver();
			saver.FileNamer = initializer.FileNamer;
			
			saver.SaveToFile(info);
			
			initializer.Initialize();
			
			Assert.IsTrue(StrategyState.IsInitialized, "Strategies weren't initialized or initialization wasn't detected.");
			Assert.Greater(StrategyState.Strategies.Count, 0, "Invalid number of strategies initialized.");
			
			
			StrategyInfo foundInfo = StrategyState.Strategies[info.Action, info.TypeName];
			
			Assert.IsNotNull(foundInfo, "The module info wasn't found.");
		
		}
		
		public string GetMockStrategiesDirectoryPath(string applicationName)
		{
			
			return TestUtilities.GetTestDataPath(this, applicationName) + Path.DirectorySeparatorChar + "Strategies";
		}
	}
}
