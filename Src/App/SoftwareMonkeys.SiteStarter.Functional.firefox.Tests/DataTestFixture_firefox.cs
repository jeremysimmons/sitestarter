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
	public class DataTestFixture_firefox : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_Data()
		{
			selenium.SetTimeout("1000000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/Data.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("User-UserRole"), "Text 'User-UserRole' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("User"), "Text 'User' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("UserRole"), "Text 'UserRole' not found when it should be.");
			selenium.Click("link=User");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("SoftwareMonkeys.SiteStarter.Entities.User"), "Text 'SoftwareMonkeys.SiteStarter.Entities.User' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("default@softwaremonkeys.net"), "Text 'default@softwaremonkeys.net' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Username: admin"), "Text 'Username: admin' not found when it should be.");
			selenium.Click("link=Sign Out");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/Data.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");

		}
	}
}