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
	public class EditOwnAccountTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_EditOwnAccount()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Settings"), "Text 'Settings' not found when it should be.");
			selenium.Click("link=Settings");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("User Settings"), "Text 'User Settings' not found when it should be.");
			selenium.Click("link=User Settings");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			if (!selenium.IsChecked("ctl00_Body_ctl00_AutoApproveNewUsers"))
			selenium.Click("ctl00_Body_ctl00_AutoApproveNewUsers");
			selenium.Click("ctl00_Body_ctl00_UpdateButton");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			selenium.Click("SignOutLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("RegisterLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "Approved");
			selenium.Type("ctl00_Body_ctl00_LastName", "User");
			selenium.Type("ctl00_Body_ctl00_Email", "approveduser@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "testuser");
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
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("First Name:"), "Text 'First Name:' not found when it should be.");
			selenium.Type("ctl00_Body_ctl00_FirstName", "NewFirst");
			selenium.Type("ctl00_Body_ctl00_LastName", "NewLast");
			selenium.Type("ctl00_Body_ctl00_Email", "new@softwaremonkeys.net");
			selenium.Click("ctl00_Body_ctl00_UpdateButton");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("My Details"), "Text 'My Details' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("updated successfully"), "Text 'updated successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("NewFirst"), "Text 'NewFirst' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("NewLast"), "Text 'NewLast' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("new@softwaremonkeys.net"), "Text 'new@softwaremonkeys.net' not found when it should be.");
			selenium.Click("SignOutLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_Login_UserName", "testuser");
			selenium.Type("ctl00_Body_ctl00_Login_Password", "pass");
			selenium.Click("ctl00_Body_ctl00_Login_LoginButton");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Sign Out"), "Text 'Sign Out' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("My Details"), "Text 'My Details' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("You are signed in as:"), "Text 'You are signed in as:' not found when it should be.");

		}
	}
}