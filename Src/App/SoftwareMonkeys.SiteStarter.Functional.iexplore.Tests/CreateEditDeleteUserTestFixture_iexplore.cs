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
	public class CreateEditDeleteUserTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_CreateEditDeleteUser()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Users");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("ctl00_Body_ctl00_CreateButton");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "Test1");
			selenium.Type("ctl00_Body_ctl00_LastName", "Test1");
			selenium.Type("ctl00_Body_ctl00_Email", "test1@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "test1");
			selenium.Type("ctl00_Body_ctl00_Password", "pass");
			selenium.Type("ctl00_Body_ctl00_PasswordConfirm", "pass");
			selenium.AddSelection("ctl00_Body_ctl00_UserRoles", "label=Administrator");
			selenium.Click("ctl00_Body_ctl00_SaveButton");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Test1"), "Text 'Test1' not found when it should be.");
			selenium.Click("ctl00_Body_ctl00_IndexGrid_ctl04_EditButton");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "Test2");
			selenium.Type("ctl00_Body_ctl00_LastName", "Test2");
			selenium.Type("ctl00_Body_ctl00_Email", "test2@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "test2");
			selenium.RemoveSelection("ctl00_Body_ctl00_UserRoles", "label=Administrator");
			selenium.Click("ctl00_Body_ctl00_UpdateButton");
			selenium.WaitForPageToLoad("30000");
			selenium.ChooseOkOnNextConfirmation();
			selenium.Click("ctl00_Body_ctl00_IndexGrid_ctl04_DeleteButton");
			Assert.IsTrue(selenium.GetConfirmation() != null && selenium.GetConfirmation().IndexOf("Are you sure you want to delete this user?") > -1, "Confirm box didn't appear when expected.");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			Assert.IsFalse(selenium.IsTextPresent("Test1"), "Text 'Test1' found when it shouldn't be.");

		}
	}
}