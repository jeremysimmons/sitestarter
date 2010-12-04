using System;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using NUnit.Framework;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	[TestFixture]
	public class ProjectionScannerTests : BaseWebTestFixture
	{
		[Test]
		public void Test_GetFormatFromFileName_Html()
		{
			string appName = "MockApplication";
			
			ProjectionScanner scanner = new ProjectionScanner();
			scanner.FileNamer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			scanner.FileNamer.ProjectionsDirectoryPath = TestUtilities.GetTestApplicationPath(this, appName) + Path.DirectorySeparatorChar + "Projections";
			
			string fileName = "Test.ascx";
			
			ProjectionFormat format = scanner.GetFormatFromFileName(fileName);
			
			Assert.AreEqual(ProjectionFormat.Html, format, "The format doesn't match what's expected.");
		}
		
		[Test]
		public void Test_GetFormatFromFileName_Xml()
		{
			string appName = "MockApplication";
			
			ProjectionScanner scanner = new ProjectionScanner();
			scanner.FileNamer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			scanner.FileNamer.ProjectionsDirectoryPath = TestUtilities.GetTestApplicationPath(this, appName) + Path.DirectorySeparatorChar + "Projections";
			
			string fileName = "Test.xml.ascx";
			
			ProjectionFormat format = scanner.GetFormatFromFileName(fileName);
			
			Assert.AreEqual(ProjectionFormat.Xml, format, "The format doesn't match what's expected.");
		}
		
		[Test]
		public void Test_GetFormatFromFileName_Xslt()
		{
			string appName = "MockApplication";
			
			ProjectionScanner scanner = new ProjectionScanner();
			scanner.ControlLoader = new MockControlLoader(this, String.Empty);
			scanner.FileNamer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			scanner.FileNamer.ProjectionsDirectoryPath = TestUtilities.GetTestApplicationPath(this, appName) + Path.DirectorySeparatorChar + "Projections";
			
			string fileName = "Test.xslt.ascx";
			
			ProjectionFormat format = scanner.GetFormatFromFileName(fileName);
			
			Assert.AreEqual(ProjectionFormat.Xslt, format, "The format doesn't match what's expected.");
		}
		
		[Test]
		public void Test_FindProjections()
		{
			string appName = "MockApplication";
			
			ProjectionScanner scanner = new ProjectionScanner();
			scanner.ControlLoader = new MockControlLoader(this, String.Empty);
			scanner.FileNamer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			scanner.FileNamer.ProjectionsDirectoryPath = TestUtilities.GetTestApplicationPath(this, appName) + Path.DirectorySeparatorChar + "Projections";
			
			CreateMockProjections(scanner.FileNamer);
			
			ProjectionInfo[] infos = scanner.FindProjections();
			
			Assert.AreEqual(1, infos.Length, "Invalid number of projections found.");
			
			Assert.AreEqual("Create", infos[0].Action, "The action doesn't match.");
			Assert.AreEqual("User", infos[0].TypeName, "The type name doesn't match.");
		}
		
		
		[Test]
		public void ExtractProjectionInfo_OneAction()
		{
			string mockFileName = "User-Create.aspx";
			
			ProjectionScanner scanner = new ProjectionScanner();
			scanner.ControlLoader = new MockControlLoader(this, mockFileName);
			
			ProjectionInfo[] infos = scanner.ExtractProjectionInfo(mockFileName);
			
			Assert.AreEqual(1, infos.Length, "Invalid number of projections found.");
		}
		
		[Test]
		public void ExtractProjectionInfo_TwoActions()
		{
			string mockFileName = "User-Create-Edit.aspx";
			
			ProjectionScanner scanner = new ProjectionScanner();
			scanner.ControlLoader = new MockControlLoader(this, mockFileName);
			
			ProjectionInfo[] infos = scanner.ExtractProjectionInfo(mockFileName);
			
			Assert.AreEqual(2, infos.Length, "Invalid number of projections found.");
		}
		
		/// <summary>
		/// Creates mock projections that can be used during testing.
		/// </summary>
		public void CreateMockProjections(ProjectionFileNamer namer)
		{			
			
			ProjectionInfo info1 = new ProjectionInfo();
			info1.Action = "Create";
			info1.TypeName = "User";
			info1.ProjectionFilePath = "Projections/User-Create.ascx";
			
			string projection1Path = namer.CreateProjectionFilePath(info1);
			
			if (!Directory.Exists(Path.GetDirectoryName(projection1Path)))
				Directory.CreateDirectory(Path.GetDirectoryName(projection1Path));
			
			using (StreamWriter writer = File.CreateText(projection1Path))
			{
				writer.Write("[mock content]");
				writer.Close();
			}
		}
	}
}
