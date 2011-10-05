using System;
using System.IO;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using NUnit.Framework;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Web.Parts;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Web.Tests.Projections;

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
			InitializeMockControllers();
			InitializeMockProjections();
		}
		
		protected void InitializeMockControllers()
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
		
		protected void InitializeMockProjections()
		{
			CreateMockProjections();
			
			ProjectionsInitializer initializer = new ProjectionsInitializer(new BasePage());
		
			MockProjectionScanner scanner = new MockProjectionScanner(this);
			
			initializer.Scanners = new ProjectionScanner[] {scanner};
			
			initializer.Initialize();
		}
		
		protected void CreateMockProjections()
		{
			string appName = "MockApplication";
        	
        	ProjectionFileNamer namer = new ProjectionFileNamer();
        	namer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			namer.ProjectionsDirectoryPath = TestUtilities.GetTestApplicationPath(this, appName) + Path.DirectorySeparatorChar + "Projections";
			
        	
			ProjectionInfo info1 = new ProjectionInfo();
			info1.Action = "Edit";
			info1.TypeName = "TestUser";
			info1.ProjectionFilePath = "/Projections/TestUser-Edit.ascx";
			
			string projection1Path = namer.CreateProjectionFilePath(info1);
			
			if (!Directory.Exists(Path.GetDirectoryName(projection1Path)))
				Directory.CreateDirectory(Path.GetDirectoryName(projection1Path));
			
			string content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ProjectionInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <Name>User-Edit</Name>
  <Action>Edit</Action>
  <TypeName>User</TypeName>
  <ProjectionFilePath>/Projections/User-Create-Edit.ascx</ProjectionFilePath>
  <Format>Html</Format>
  <MenuTitle />
  <MenuCategory />
  <ShowOnMenu>false</ShowOnMenu>
  <Enabled>true</Enabled>
</ProjectionInfo>";
			
			using (StreamWriter writer = File.CreateText(projection1Path))
			{
				writer.Write(content);
				writer.Close();
			}
		}
		
		protected void DisposeMockWeb()
		{
			ControllerState.Controllers = null;
			ProjectionState.Projections = null;
			PartState.Parts = null;
		}
	}
}
