using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using SoftwareMonkeys.SiteStarter.Entities.Tests;
using SoftwareMonkeys.SiteStarter.Entities;
using NUnit.Framework;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	public class BaseBusinessTestFixture : BaseDataTestFixture
	{
		[SetUp]
		public new void Initialize()
		{
			InitializeMockEntities();
			InitializeMockBusiness();
		}
		
		public BaseBusinessTestFixture()
		{
			
		}
		
		protected override void InitializeMockEntities()
		{
			string testsAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Tests").Location;
			string entitiesAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Entities").Location;
			string businessTestsAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests").Location;
			
			string[] assemblyPaths = new String[] {testsAssemblyPath, entitiesAssemblyPath, businessTestsAssemblyPath};
			
			EntityInitializer initializer = new EntityInitializer();
			
			// Set the specific assemblies used during testing as it can't do it automatically in the mock environment
			initializer.Scanner.AssemblyPaths = assemblyPaths;
			
			initializer.Initialize();
		}
		
		public void InitializeMockBusiness()
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
