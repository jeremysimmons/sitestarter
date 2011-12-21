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
		
		[SetUp]
		public override void Start()
		{
			// Disable business state to avoid it already being initialized,
			// as this test fixture is intended to test the initialization process
			// and it can't effectively if already initialized
			EnableBusinessState = false;
			
			base.Start();
		}
		
		[Test]
		public void Test_Initialize_StrategiesProvided()
		{
			IStrategy strategy = new MockRetrieveTestUserStrategy();
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			StrategyInfo[] strategies = new StrategyInfo[]{info};
			
			StrategyInitializer initializer = new StrategyInitializer();
			
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
			
			IStrategy strategy = new MockActionStrategy();
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			StrategyInfo[] strategies = new StrategyInfo[]{info};
			
			StrategySaver saver = new StrategySaver();
			saver.FileNamer = initializer.FileNamer;
			
			saver.SaveToFile(new StrategyInfo[]{info});
			
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
