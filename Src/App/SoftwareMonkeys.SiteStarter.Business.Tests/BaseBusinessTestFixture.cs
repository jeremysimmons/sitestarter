using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using NUnit.Framework;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	public class BaseBusinessTestFixture : BaseDataTestFixture
	{
		[SetUp]
		public void Initialize()
		{
			InitializeBusiness();
		}
		
		public BaseBusinessTestFixture()
		{
			
		}
		
		public void InitializeBusiness()
		{
			
			string businessAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Business").Location;
			
			string[] assemblyPaths = new String[] {businessAssemblyPath};
			
			StrategyInitializer initializer = new StrategyInitializer();
			
			// Set the specific assemblies used during testing as it can't do it automatically in the mock environment
			initializer.Scanner.AssemblyPaths = assemblyPaths;
			
			initializer.Initialize();
			
		}
		
		
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
	}
}
