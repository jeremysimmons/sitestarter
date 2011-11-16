using System;
using SoftwareMonkeys.SiteStarter.Data.Tests;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities.Tests;
using SoftwareMonkeys.SiteStarter.Entities;
using NUnit.Framework;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	public class BaseBusinessTestFixture : BaseDataTestFixture
	{
		public bool EnableBusinessState = true;
		
		[SetUp]
		public override void Start()
		{
			base.Start();
			
			InitializeMockBusiness();
		}
		
		[TearDown]
		public override void End()
		{
			DisposeMockBusiness();
			
			base.End();
		}
		
		public BaseBusinessTestFixture()
		{
			
		}
		
		protected override void InitializeMockEntities()
		{
			string testsAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Tests").Location;
			string entitiesAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Entities").Location;
			string entitiesTestsAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Entities.Tests").Location;
			string businessTestsAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests").Location;
			
			string[] assemblyPaths = new String[] {testsAssemblyPath, entitiesAssemblyPath, entitiesTestsAssemblyPath, businessTestsAssemblyPath};
			
			EntityInitializer initializer = new EntityInitializer();
			
			// Set the specific assemblies used during testing as it can't do it automatically in the mock environment
			initializer.Scanner.AssemblyPaths = assemblyPaths;
			
			initializer.Initialize(true);
		}
		
		public virtual void InitializeMockBusiness()
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Initializing mock business tier."))
			{
			if (EnableBusinessState)
			{
				string businessAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Business").Location;
				string businessTestsAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests").Location;
				
				string[] assemblyPaths = new String[]
				{
					businessAssemblyPath,
					businessTestsAssemblyPath
				};
				
				// Strategies
				
				StrategyInitializer initializer = new StrategyInitializer();
				
				// Set the specific assemblies used during testing as it can't do it automatically in the mock environment
				initializer.Scanner.AssemblyPaths = assemblyPaths;
				
				initializer.Initialize(true);
				
				
				// Reactions
				
				ReactionInitializer reactionsInitializer = new ReactionInitializer();
				
				// Set the specific assemblies used during testing as it can't do it automatically in the mock environment
				reactionsInitializer.Scanner.AssemblyPaths = assemblyPaths;
				
				reactionsInitializer.Initialize(true);
			}
		}
		}
		
		public void DisposeMockBusiness()
		{
			StrategyState.Strategies = null;
			ReactionState.Reactions = null;
		}
		
		public override void InitializeMockData()
		{
			new MockDb4oDataProviderInitializer(this).Initialize();
		}
	}
}
