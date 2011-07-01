using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using Selenium;
using System.Net;
using System.Net.Sockets;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace SoftwareMonkeys.SiteStarter.Functional.firefox.Tests
{
	[TestFixture]
	public class TestBreakableLoggingTestFixture_firefox : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
	{
		private ISelenium selenium;
		private StringBuilder verificationErrors;
	
		[SetUp]
		public void Initialize()
		{
			RemoteWebDriver driver = new OpenQA.Selenium.Firefox.FirefoxDriver();
			
			selenium = new WebDriverBackedSelenium(driver, "http://localhost/SiteStarter");
			
			selenium.Start();
			verificationErrors = new StringBuilder();
		}
		
		[TearDown]
		public void Dispose()
		{
			try
			{
				selenium.Stop();
			}
			catch (Exception)
			{
				// Ignore errors if unable to close the browser
			}
			Assert.AreEqual("", verificationErrors.ToString());
		}
		
		[Test]
		public void Test_TestBreakableLogging()
		{
			selenium.SetTimeout("1000000");
			selenium.Open("Admin/Tests/TestReset.aspx?Log=true");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/Tests/TestBreakableLogging.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Open("");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Open("Admin/Tests/LogContains.aspx?Query=Test group %231");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");
			selenium.Open("Admin/Tests/LogContains.aspx?Query=Test group %232");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");
			selenium.Open("Admin/Tests/LogContains.aspx?Query=Test group %233");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");
			selenium.Open("Admin/Tests/LogContains.aspx?Query=Test entry %231");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");
			selenium.Open("Admin/Tests/LogContains.aspx?Query=Test entry %232");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");

		}

		private int GeneratePortNumber()
		{
			TcpListener listener = new TcpListener(IPAddress.Any, 0);
			listener.Start();
			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			listener.Stop();
			return port;
		}
	}
}