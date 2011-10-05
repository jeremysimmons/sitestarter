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

namespace SoftwareMonkeys.SiteStarter.Functional.iexplore.Tests
{
	[TestFixture]
	public class UserSettingsTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
	{
		private ISelenium selenium;
		private StringBuilder verificationErrors;
	
		[SetUp]
		public void Initialize()
		{
			RemoteWebDriver driver = new OpenQA.Selenium.IE.InternetExplorerDriver();
			
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
		public void Test_UserSettings()
		{
			selenium.SetTimeout("1000000");
			selenium.Open("Admin/tests/testreset.aspx?Config=true");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Settings");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=User Settings");
			selenium.WaitForPageToLoad("30000");
			if (selenium.IsChecked("ctl00_Body_ctl00_EnableUserRegistration"))
			selenium.Click("ctl00_Body_ctl00_EnableUserRegistration");
			selenium.Click("//input[@value='Update']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("updated successfully"), "Text 'updated successfully' not found when it should be.");
			selenium.Click("link=Sign Out");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Register"), "Text 'Register' found when it shouldn't be.");
			selenium.Open("Edit-UserSettings.aspx");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");

		}
	}
}