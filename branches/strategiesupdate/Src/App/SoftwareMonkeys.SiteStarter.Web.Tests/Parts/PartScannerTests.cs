using System;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Web.Parts;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using NUnit.Framework;
using System.IO;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Parts
{
	[TestFixture]
	public class PartScannerTests : BaseWebTestFixture
	{
		[Test]
		public void Test_FindParts()
		{
			string appName = "MockApplication";
			
			PartScanner scanner = new PartScanner();
			scanner.ControlLoader = new MockPartLoader(this, String.Empty);
			scanner.FileNamer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			scanner.FileNamer.PartsDirectoryPath = TestUtilities.GetTestApplicationPath(this, appName) + Path.DirectorySeparatorChar + "Parts";
			
			CreateMockParts(scanner.FileNamer);
			
			PartInfo[] infos = scanner.FindParts();
			
			Assert.AreEqual(1, infos.Length, "Invalid number of parts found.");
			
			Assert.AreEqual("Create", infos[0].Action, "The action doesn't match.");
			Assert.AreEqual("User", infos[0].TypeName, "The type name doesn't match.");
		}
		
		
		[Test]
		public void ExtractPartInfo_OneAction()
		{
			string mockFileName = "User-Create.aspx";
			
			PartScanner scanner = new PartScanner();
			scanner.ControlLoader = new MockPartLoader(this, mockFileName);
			
			PartInfo[] infos = scanner.ExtractPartInfo(mockFileName);
			
			Assert.AreEqual(1, infos.Length, "Invalid number of parts found.");
		}
		
		[Test]
		public void ExtractPartInfo_TwoActions()
		{
			string mockFileName = "User-Create-Edit.aspx";
			
			PartScanner scanner = new PartScanner();
			scanner.ControlLoader = new MockPartLoader(this, mockFileName);
			
			PartInfo[] infos = scanner.ExtractPartInfo(mockFileName);
			
			Assert.AreEqual(2, infos.Length, "Invalid number of parts found.");
		}
		
		/// <summary>
		/// Creates mock parts that can be used during testing.
		/// </summary>
		public void CreateMockParts(PartFileNamer namer)
		{			
			
			PartInfo info1 = new PartInfo();
			info1.Action = "Create";
			info1.TypeName = "User";
			info1.PartFilePath = "Parts/User-Create.ascx";
			
			string part1Path = namer.CreatePartFilePath(info1);
			
			if (!Directory.Exists(Path.GetDirectoryName(part1Path)))
				Directory.CreateDirectory(Path.GetDirectoryName(part1Path));
			
			using (StreamWriter writer = File.CreateText(part1Path))
			{
				writer.Write("[mock content]");
				writer.Close();
			}
		}
	}
}
