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
	public class UsersTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_Users()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Users");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Manage Users"), "Text 'Manage Users' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Edit"), "Text 'Edit' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Delete"), "Text 'Delete' not found when it should be.");
			selenium.Click("//input[@value='Create User']");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "FirstName");
			selenium.Type("ctl00_Body_ctl00_LastName", "LastName");
			selenium.Type("ctl00_Body_ctl00_Email", "testuser@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "TestUser");
			selenium.Type("ctl00_Body_ctl00_Password", "pass");
			selenium.Type("ctl00_Body_ctl00_PasswordConfirm", "pass");
			selenium.Click("ctl00_Body_ctl00_IsApproved");
			selenium.AddSelection("ctl00_Body_ctl00_UserRoles", "label=Administrator");
			selenium.Click("//input[@value='Save']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("saved successfully"), "Text 'saved successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Manage Users"), "Text 'Manage Users' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("TestUser"), "Text 'TestUser' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("FirstName"), "Text 'FirstName' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("LastName"), "Text 'LastName' not found when it should be.");
			selenium.Click("link=Edit");
			selenium.WaitForPageToLoad("30000");
			Assert.AreEqual("Administrator", selenium.GetSelectedLabel("ctl00_Body_ctl00_UserRoles"));
			selenium.Type("ctl00_Body_ctl00_FirstName", "System2");
			selenium.Type("ctl00_Body_ctl00_LastName", "Administrator2");
			selenium.Type("ctl00_Body_ctl00_Email", "default@softwaremonkeys.net2");
			selenium.Click("//input[@value='Update']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("updated successfully"), "Text 'updated successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("System2"), "Text 'System2' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Administrator2"), "Text 'Administrator2' not found when it should be.");

		}
	}
}