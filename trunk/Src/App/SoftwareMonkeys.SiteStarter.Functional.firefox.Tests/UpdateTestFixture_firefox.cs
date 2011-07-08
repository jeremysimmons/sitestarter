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
	public class UpdateTestFixture_firefox : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_Update()
		{
			selenium.SetTimeout("1000000");
			selenium.Open("Admin/Tests/TestReset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Users");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsFalse(selenium.IsTextPresent("aren't authorised"), "Text 'aren't authorised' found when it shouldn't be.");
			selenium.Click("//input[@value='Create User']");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "another");
			selenium.Type("ctl00_Body_ctl00_LastName", "user");
			selenium.Type("ctl00_Body_ctl00_Email", "anotheruser@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "anotheruser");
			selenium.Type("ctl00_Body_ctl00_Password", "pass");
			selenium.Type("ctl00_Body_ctl00_PasswordConfirm", "pass");
			selenium.AddSelection("ctl00_Body_ctl00_UserRoles", "label=Administrator");
			selenium.Click("//input[@value='Save']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			selenium.Click("link=Roles");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("ctl00_Body_ctl00_CreateButton");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_Name", "another role");
			selenium.AddSelection("ctl00_Body_ctl00_Users", "label=another user");
			selenium.Click("//input[@value='Save']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			selenium.Click("UpdateApplicationLink");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Application Update"), "Text 'Application Update' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("current application version"), "Text 'current application version' not found when it should be.");
			selenium.Click("//input[@value='Begin »']");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Ready for Upload"), "Text 'Ready for Upload' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Your data has been backed up"), "Text 'Your data has been backed up' not found when it should be.");
			selenium.Click("//input[@value='Continue »']");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Update Complete"), "Text 'Update Complete' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			selenium.Click("link=Users");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("anotheruser"), "Text 'anotheruser' not found when it should be.");
			selenium.Click("link=Roles");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("another role"), "Text 'another role' not found when it should be.");
			selenium.Click("link=Sign Out");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_Login_UserName", "admin");
			selenium.Type("ctl00_Body_ctl00_Login_Password", "pass");
			selenium.Click("ctl00_Body_ctl00_Login_LoginButton");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/Data.aspx");
			selenium.Click("link=User-UserRole");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("System Administrator"), "Text 'System Administrator' not found when it should be.");
			selenium.Click("UpdateApplicationLink");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Application Update"), "Text 'Application Update' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("current application version"), "Text 'current application version' not found when it should be.");
			selenium.Click("//input[@value='Begin »']");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Ready for Upload"), "Text 'Ready for Upload' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Your data has been backed up"), "Text 'Your data has been backed up' not found when it should be.");
			selenium.Click("//input[@value='Continue »']");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Update Complete"), "Text 'Update Complete' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");

		}
	}
}