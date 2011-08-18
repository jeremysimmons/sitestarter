using System;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using NUnit.Framework;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Web.Parts;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	public class BaseWebTestFixture : BaseBusinessTestFixture
	{
		[SetUp]
		public override void Start()
		{
			base.Start();
			
			InitializeMockWeb();
		}
		
		[TearDown]
		public override void End()
		{
			DisposeMockWeb();
			
			base.End();
		}
		
		protected void InitializeMockWeb()
		{
			string webAssemblyPath = Assembly.Load("SoftwareMonkeys.SiteStarter.Web").Location;
			
			string[] assemblyPaths = new String[] {webAssemblyPath};
			
			ControllersInitializer initializer = new ControllersInitializer();
			
			ControllerScanner scanner = new ControllerScanner();
			
			// Set the specific assemblies used during testing as it can't do it automatically in the mock environment
			scanner.AssemblyPaths = assemblyPaths;
			
			initializer.Scanners = new ControllerScanner[] {scanner};
			
			initializer.Initialize(true);
		}
		
		protected void DisposeMockWeb()
		{
			ControllerState.Controllers = null;
			ProjectionState.Projections = null;
			PartState.Parts = null;
		}
	}
}
