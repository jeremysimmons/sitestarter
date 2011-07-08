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
	public class LoggingTestFixture_firefox : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_Logging()
		{
			selenium.SetTimeout("1000000");
			selenium.Open("Admin/tests/testreset.aspx?Log=true&amp;Config=true");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Open("Admin/tests/TestLogging.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Open("Admin/tests/LogContains.aspx?Query=Test group %231");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");
			selenium.Open("Admin/tests/LogContains.aspx?Query=Test entry %231");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");
			selenium.Open("Admin/tests/LogContains.aspx?Query=Test group %232");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");
			selenium.Open("Admin/tests/LogContains.aspx?Query=Test entry %232");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("LogContains=True"), "Text 'LogContains=True' not found when it should be.");

		}
	}
}