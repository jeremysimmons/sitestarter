using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;
using System.IO;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	[TestFixture]
	public class UrlRewriterTests : BaseWebTestFixture
	{
		[Test]
		public void Test_GetAction_First()
		{
			string action = "TestAction";
			string typeName = "TestArticle";
			
			string friendlyUrl = "http://localhost/test/" + action + "-" + typeName + ".aspx";
			
			string applicationPath = "/test";
			IFileMapper fileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string foundAction = rewriter.GetAction(friendlyUrl);
			
			Assert.AreEqual(action, foundAction, "Didn't detect the action.");
		}
		
		[Test]
		public void Test_GetAction_Second()
		{
			string action = "TestAction";
			string typeName = "TestArticle";
			
			string friendlyUrl = "http://localhost/test/" + typeName + "-" + action + ".aspx";
			
			string applicationPath = "/test";
			IFileMapper fileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string foundAction = rewriter.GetAction(friendlyUrl);
			
			Assert.AreEqual(action, foundAction, "Didn't detect the action.");
		}
		
		
		[Test]
		public void Test_GetTypeName_First()
		{
			
			string action = "TestAction";
			string typeName = "TestArticle";
			
			string friendlyUrl = "http://localhost/test/" + typeName + "-" + action + ".aspx";
			
			string applicationPath = "/test";
			IFileMapper fileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string foundTypeName = rewriter.GetTypeName(friendlyUrl);
			
			Assert.AreEqual(typeName, foundTypeName, "Didn't detect the type name.");
		}
		
		[Test]
		public void Test_GetTypeName_Second()
		{
			string action = "TestAction";
			string typeName = "TestArticle";
			
			string friendlyUrl = "http://localhost/test/" + action + "-" + typeName + ".aspx";
			
			string applicationPath = "/test";
			IFileMapper fileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string foundTypeName = rewriter.GetTypeName(friendlyUrl);
			
			Assert.AreEqual(typeName, foundTypeName, "Didn't detect the type name.");
		}
		
		[Test]
		public void Test_GetCommand()
		{
			string action = "TestAction";
			string typeName = "TestArticle";
			
			string command = action + "-" + typeName;
			
			string friendlyUrl = "http://localhost/test/" + command + ".aspx";
			
			string applicationPath = "/test";
			IFileMapper fileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string foundCommand = rewriter.GetCommand(friendlyUrl);
			
			Assert.AreEqual(command, foundCommand, "Didn't detect the type name.");
		}
		
		
		[Test]
		public void Test_GetShortUrl()
		{
			string original = "http://localhost/test/testpage.aspx?a=test&t=testtype";
			
			string applicationPath = "/test";
			IFileMapper fileMapper = new MockFileMapper(this,TestUtilities.GetTestingPath(this));
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string result = rewriter.GetShortUrl(original);
			
			string expected = "/testpage.aspx";
			
			Assert.AreEqual(expected, result, "The resulting URL doesn't match what's expected.");
		}
		
		[Test]
		public void Test_IsRealFile_True()
		{
			string original = "/testfile.txt";
			
			string applicationPath = "/test";
			string testingPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "test";
			
			IFileMapper fileMapper = new MockFileMapper(this,testingPath);
			fileMapper.ApplicationPath = applicationPath;
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string filePath = fileMapper.MapApplicationRelativePath(original);
			
			if (!Directory.Exists(Path.GetDirectoryName(filePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			
			using (StreamWriter writer = File.CreateText(filePath))
			{
				writer.Write("mock content");
				writer.Close();
			}
			
			string expected = String.Empty;
			
			bool isRealFile = rewriter.IsRealFile(original);
			
			Assert.IsTrue(isRealFile, "Returned false when it should have returned true.");
			
		}
		
		[Test]
		public void Test_IsRealFile_False()
		{
			string original = "/testfile.txt";
			
			string applicationPath = "/test";
			
			
			string testingPath = TestUtilities.GetTestingPath(this) + Path.DirectorySeparatorChar + "test";
			
			IFileMapper fileMapper = new MockFileMapper(this,testingPath);
			fileMapper.ApplicationPath = applicationPath;
			
			UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
			
			string filePath = fileMapper.MapApplicationRelativePath(original);
			
			if (!Directory.Exists(Path.GetDirectoryName(filePath)))
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			
			// File creation skipped because this test should return false
			
			string expected = String.Empty;
			
			bool isRealFile = rewriter.IsRealFile(original);
			
			Assert.IsFalse(isRealFile, "Returned true when it should have returned false.");
			
		}
		
		
        [Test]
        public void Test_RewriteUrl_InvalidCommand()
        {
            InitializeMockProjections();
            
            string fullApplicationUrl = "http://localhost/SiteStarter";
            string applicationPath = "/SiteStarter";

            string action = "Action";
            string typeName = "Type";
            
            string original = fullApplicationUrl + "/" + action + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&f=Html";

            IFileMapper fileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this));
            
            UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
            
            string generated = rewriter.RewriteUrl(original, applicationPath, false);

            Assert.AreEqual("", generated.ToLower(), "Result doesn't match expected.");
        }
		
        [Test]
        public void Test_RewriteUrl_Action_Type()
        {
            InitializeMockProjections();
            
            string fullApplicationUrl = "http://localhost/SiteStarter";
            string applicationPath = "/SiteStarter";

            string action = "Action";
            string typeName = "Type";
            
            string original = fullApplicationUrl + "/" + action + "-" + typeName + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&f=Html";

            IFileMapper fileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this));
            
            UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
            
            string generated = rewriter.RewriteUrl(original, applicationPath, false);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }

        [Test]
        public void Test_RewriteUrl_Action_Type_ID()
        {
            InitializeMockProjections();

            string entityID = "A1FC7BA3-3832-467f-8989-058BF420D1D9";

            string fullApplicationUrl = "http://localhost/SiteStarter";
            string applicationPath = "/SiteStarter";
            
            string action = "Action";
            string typeName = "Type";

            string original = fullApplicationUrl + "/" + action + "-" + typeName + "/" + entityID + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&" + typeName + "-ID=" + entityID + "&f=Html";

            IFileMapper fileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this));
            
            UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
            
            string generated = rewriter.RewriteUrl(original, applicationPath, false);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }

        [Test]
        public void Test_RewriteUrl_Action_Type_ID_IgnoreText()
        {
        	
            InitializeMockProjections();

            string entityName = "TestProject";
            string entityID = "A1FC7BA3-3832-467f-8989-058BF420D1D9";

            string fullApplicationUrl = "http://localhost/SiteStarter";
            string applicationPath = "/SiteStarter";
            
            string action = "Action";
            string typeName = "TypeName";

            string original = fullApplicationUrl + "/" + action + "-" + typeName + "/" + entityID + "/I--" + entityName + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&" + typeName + "-ID=" + entityID + "&f=Html";

            IFileMapper fileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this));
            
            UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
            
            string generated = rewriter.RewriteUrl(original, applicationPath, false);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_RewriteUrl_Action_Type_ID_Key()
        {
        	
            InitializeMockProjections();

            string entityName = "TestProject";
            string entityID = "A1FC7BA3-3832-467f-8989-058BF420D1D9";

            string fullApplicationUrl = "http://localhost/SiteStarter";
            string applicationPath = "/SiteStarter";
            
            string action = "Action";
            string typeName = "TypeName";

            string original = fullApplicationUrl + "/" + action + "-" + typeName + "/" + entityID + "/K--" + entityName + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&" + typeName + "-ID=" + entityID + "&" + typeName + "-UniqueKey=" + entityName + "&f=Html";

            IFileMapper fileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this));
            
            UrlRewriter rewriter = new UrlRewriter(applicationPath, fileMapper);
            
            string generated = rewriter.RewriteUrl(original, applicationPath, false);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_MapPath()
        {
        	string applicationName = "TestApplication";
        	
        	string physicalApplicationPath = TestUtilities.GetTestApplicationPath(this, applicationName);
        	
        	IFileMapper fileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this));
        	fileMapper.ApplicationPath = applicationName;
        	
        	UrlRewriter rewriter = new UrlRewriter(physicalApplicationPath, fileMapper);
        	
        	string relativePath = "TestDir";
        	
        	string fullPath = rewriter.MapPath(relativePath);
        	
        	string expected = physicalApplicationPath + Path.DirectorySeparatorChar + relativePath;
        	
        	Assert.AreEqual(expected, fullPath, "The mapped path does match expected.");
        }
        
        public void InitializeMockProjections()
        {
        	string appName = "MockApplication";
        	
        	ProjectionFileNamer namer = new ProjectionFileNamer();
        	namer.FileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this), appName);
			namer.ProjectionsDirectoryPath = TestUtilities.GetTestApplicationPath(this, appName) + Path.DirectorySeparatorChar + "Projections";
			
        	
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
