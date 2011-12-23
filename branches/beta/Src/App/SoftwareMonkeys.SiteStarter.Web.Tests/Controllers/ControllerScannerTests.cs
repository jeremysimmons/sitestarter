using System;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Web.Controllers;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using NUnit.Framework;
using System.IO;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Controllers
{
	[TestFixture]
	public class ControllerScannerTests : BaseWebTestFixture
	{
		[Test]
		public void Test_FindControllers()
		{
			string appName = "MockApplication";
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Web").Location
			};
			
			ControllerScanner scanner = new ControllerScanner();
			scanner.FileNamer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			scanner.AssemblyPaths = assemblyPaths;
			
			//CreateMockControllers(scanner.FileNamer);
			
			ControllerInfo[] infos = scanner.FindControllers(true);
			
			Assert.Greater(infos.Length, 1, "Invalid number of controllers found.");
			
			string expectedLongType = typeof(CreateController).FullName + ", " + typeof(CreateController).Assembly.GetName().Name;
			
			Assert.AreEqual("Create", infos[0].Action, "The action doesn't match.");
			Assert.AreEqual("IEntity", infos[0].TypeName, "The type name doesn't match.");
			Assert.AreEqual(expectedLongType, infos[0].ControllerType, "The full controller component type name doesn't match.");
		}
		
		[Test]
		public void Test_IsController()
		{
			
			ControllerScanner scanner = new ControllerScanner();
			
			CreateController controller = new CreateController();
			
			bool isController = scanner.IsController(controller.GetType());
			
			Assert.IsTrue(isController, "CreateController class is not recognised as a controller as it should be.");
		}
		
		// TODO: Remove if not needed
		/*[Test]
		public void ExtractControllerInfo_OneAction()
		{
			string mockFileName = "User-Create.aspx";
			
			ControllerScanner scanner = new ControllerScanner();
			
			ControllerInfo[] infos = scanner.ExtractControllerInfo(mockFileName);
			
			Assert.AreEqual(1, infos.Length, "Invalid number of controllers found.");
		}
		
		[Test]
		public void ExtractControllerInfo_TwoActions()
		{
			string mockFileName = "User-Create-Edit.aspx";
			
			ControllerScanner scanner = new ControllerScanner();
			
			ControllerInfo[] infos = scanner.ExtractControllerInfo(mockFileName);
			
			Assert.AreEqual(2, infos.Length, "Invalid number of controllers found.");
		}*/
		
		/// <summary>
		/// Creates mock controllers that can be used during testing.
		/// </summary>
		public void CreateMockControllers(ControllerFileNamer namer)
		{			
			throw new NotImplementedException();
			
			ControllerInfo info1 = new ControllerInfo();
			info1.Action = "Create";
			info1.TypeName = "User";
			info1.ControllerType = "Test";
			
			/*string controller1Path = namer.CreateInfoFilePath(info1);
			
			if (!Directory.Exists(Path.GetDirectoryName(controller1Path)))
				Directory.CreateDirectory(Path.GetDirectoryName(controller1Path));
			
			using (StreamWriter writer = File.CreateText(controller1Path))
			{
				writer.Write("[mock content]");
				writer.Close();
			}*/
		}
	}
}
