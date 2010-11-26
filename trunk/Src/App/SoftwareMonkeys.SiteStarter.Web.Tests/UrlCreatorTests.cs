using System;
using NUnit.Framework;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using System.IO;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	[TestFixture]
	public class UrlCreatorTests : BaseWebTestFixture
	{
		[Test]
		public void Test_CreateFriendlyUrl()
		{

			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test";
			
			string action = "Create";
			string typeName = "User";
			
			
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			creator.EnableFriendlyUrls = true;
			
			string url = creator.CreateFriendlyUrl(action, typeName);
			
			string expectedUrl = "/Test/" + typeName + "/" + action + ".aspx";
			
			Assert.AreEqual(expectedUrl, url, "The URL doesn't match what's expected.");
		
		}
		
		
		[Test]
		public void Test_CreateFriendlyUrl_MatchProperty()
		{

			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test";
			
			string action = "Create";
			string typeName = "User";
						
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			
			string propertyName = "ID";
			string dataKey = Guid.NewGuid().ToString();
			
			string url = creator.CreateFriendlyUrl(action, typeName, propertyName, dataKey);
			
			string expectedUrl = "/Test/" + creator.PrepareForUrl(typeName) + "/" + creator.PrepareForUrl(action) + "/" + creator.PrepareForUrl(propertyName) + "/" + creator.PrepareForUrl(dataKey) + ".aspx";
			
			Assert.AreEqual(expectedUrl, url, "The URL doesn't match what's expected.");
		
		}
		
		[Test]
		public void Test_CreateXmlUrl()
		{
			
			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test";
			
			string action = "Create";
			string typeName = "User";
			
			
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			creator.EnableFriendlyUrls = true;
			
			string url = creator.CreateXmlUrl(action, typeName);
			
			string expectedUrl = "/Test/" + typeName + "/" + action + ".xml.aspx";
			
			Assert.AreEqual(expectedUrl, url, "The URL doesn't match what's expected.");
		}
		
		
		[Test]
		public void Test_CreateExternalXmlUrl()
		{
			
			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test";
			
			string action = "Create";
			string typeName = "User";
			
			
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			creator.EnableFriendlyUrls = true;
			
			string url = creator.CreateExternalXmlUrl(action, typeName);
			
			string expectedUrl = originalUrl + "/" + typeName + "/" + action + ".xml.aspx";
			
			Assert.AreEqual(expectedUrl, url, "The URL doesn't match what's expected.");
		
		}
		
		[Test]
		public void Test_CreateStandardUrl()
		{
			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test";
			
			string action = "MockAction";
			string typeName = "MockType";
			
			
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			
			string url = creator.CreateStandardUrl(action, typeName);

			string expectedUrl = "/Test/Projector.aspx?a=" + creator.PrepareForUrl(action) + "&t=" + creator.PrepareForUrl(typeName) + "&f=Html";
			
			Assert.AreEqual(expectedUrl, url, "The URL doesn't match what's expected.");
		}
		
		
		[Test]
		public void Test_CreateStandardUrl_MatchProperty()
		{
			InitializeMockProjections();
			
			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test";
			
			string action = "Create";
			string typeName = "User";
			
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			
			string propertyName = "ID";
			string dataKey = Guid.NewGuid().ToString();
			
			string url = creator.CreateStandardUrl(action, typeName, propertyName, dataKey);
			
			string expectedUrl = "/Test/Projector.aspx?a=" + creator.PrepareForUrl(action) + "&t=" + creator.PrepareForUrl(typeName) + "&f=Html&" + typeName + "-" + propertyName + "=" + creator.PrepareForUrl(dataKey);
			
			Assert.AreEqual(expectedUrl, url, "The URL doesn't match what's expected.");
		
		}
		
		[Test]
		public void Test_CreateXsltUrl()
		{	
			string applicationPath = "/Test";
			string currentUrl = "http://localhost/Test";
			
			UrlCreator creator = new UrlCreator(applicationPath, currentUrl);
			
			string action = "TestAction";
			string type = "TestType";
			
			string url = creator.CreateXsltUrl(action, type);
			
			string expected = "/Test/" + type + "/" + action + ".xslt.aspx";
			
			Assert.AreEqual(expected, url, "The returned URL doesn't match what's expected.");
		}
		
		[Test]
		public void Test_AddResult()
		{
			string applicationPath = "/Test";
			string currentUrl = "http://localhost/Test";
			
			UrlCreator creator = new UrlCreator(applicationPath, currentUrl);
			
			string original = "http://localhost/Test/Path/";
			
			string resultText = "Test message";
			bool resultIsError = false;
			
			string result = creator.AddResult(original, resultText, resultIsError);
			
			string expected = "http://localhost/Test/Path/?Result=" + creator.PrepareForUrl(resultText) + "&ResultIsError=" + resultIsError.ToString();
			
			Assert.AreEqual(expected, result, "The resulting URL doesn't match what's expected.");
		}
		
		[Test]
		public void Test_ResultAlreadyExists_False()
		{
			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test";
			
			string resultText = "Test message";
			
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			
			bool alreadyExists = creator.ResultAlreadyExists(resultText);
			
			Assert.IsFalse(alreadyExists, "Returned true when it should have returned false.");	
		}
		
		[Test]
		public void Test_ResultAlreadyExists_True()
		{
			string resultText = "Test message";
			
			string applicationPath = "/Test";
			string originalUrl = "http://localhost/Test/?Result=" + HttpUtility.UrlEncode(resultText);
			
			UrlCreator creator = new UrlCreator(applicationPath, originalUrl);
			
			bool alreadyExists = creator.ResultAlreadyExists(resultText);
			
			Assert.IsTrue(alreadyExists, "Returned false when it should have returned true.");			
		}
		
        [Test]
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
