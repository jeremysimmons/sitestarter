using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Configuration.Tests;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.IO;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// The base of all entity test fixtures.
	/// </summary>
	public class BaseEntityTestFixture : BaseConfigurationTestFixture
	{
		[SetUp]
		public override void Start()
		{
			base.Start();
			
			InitializeMockEntities();
		}
		
		[TearDown]
		public override void End()
		{
			DisposeMockEntities();
			
			base.End();
		}
		
		
		protected virtual void InitializeMockEntities()
		{
			string testsAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Tests").Location;
			
			string[] assemblyPaths = new String[] {testsAssemblyPath};
			
			EntityInitializer initializer = new EntityInitializer();
			
			// Set the specific assemblies used during testing as it can't do it automatically in the mock environment
			initializer.Scanner.AssemblyPaths = assemblyPaths;
			
			initializer.Initialize(true);
		}
		
		protected void DisposeMockEntities()
		{
			
		}
	}
}
