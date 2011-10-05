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
	public class EditOwnUsernameTestFixture_firefox : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_EditOwnUsername()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Settings"), "Text 'Settings' not found when it should be.");
			selenium.Click("link=Settings");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsFalse(selenium.IsTextPresent("aren't authorised"), "Text 'aren't authorised' found when it shouldn't be.");
			selenium.Click("link=User Settings");
			selenium.WaitForPageToLoad("30000");
			if (!selenium.IsChecked("ctl00_Body_ctl00_EnableUserRegistration"))
			selenium.Click("ctl00_Body_ctl00_EnableUserRegistration");
			if (!selenium.IsChecked("ctl00_Body_ctl00_AutoApproveNewUsers"))
			selenium.Click("ctl00_Body_ctl00_AutoApproveNewUsers");
			selenium.Click("ctl00_Body_ctl00_UpdateButton");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			selenium.Click("SignOutLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("RegisterLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "Approved");
			selenium.Type("ctl00_Body_ctl00_LastName", "User");
			selenium.Type("ctl00_Body_ctl00_Email", "approveduser@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "approveduser");
			selenium.Type("ctl00_Body_ctl00_Password", "pass");
			selenium.Type("ctl00_Body_ctl00_PasswordConfirm", "pass");
			selenium.Click("ctl00_Body_ctl00_EnableNotifications");
			selenium.Click("ctl00_Body_ctl00_RegisterButton");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("My Details"), "Text 'My Details' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("approveduser@softwaremonkeys.net"), "Text 'approveduser@softwaremonkeys.net' not found when it should be.");
			selenium.Click("//input[@id='ctl00_Body_ctl00_ViewEditButton']");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("First Name:"), "Text 'First Name:' not found when it should be.");
			selenium.Type("ctl00_Body_ctl00_Username", "newusername");
			selenium.Click("ctl00_Body_ctl00_UpdateButton");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("My Details"), "Text 'My Details' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("updated successfully"), "Text 'updated successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("newusername"), "Text 'newusername' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("approveduser@softwaremonkeys.net"), "Text 'approveduser@softwaremonkeys.net' not found when it should be.");
			selenium.Click("SignOutLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_Login_UserName", "newusername");
			selenium.Type("ctl00_Body_ctl00_Login_Password", "pass");
			selenium.Click("ctl00_Body_ctl00_Login_LoginButton");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Sign Out"), "Text 'Sign Out' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("My Details"), "Text 'My Details' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("You are signed in as:"), "Text 'You are signed in as:' not found when it should be.");

		}
	}
}