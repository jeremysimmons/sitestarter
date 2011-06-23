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
	public class CreateEditDeleteRoleTestFixture_firefox : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_CreateEditDeleteRole()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Roles");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("//input[@value='Create Role']");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_Name", "Test Role #1");
			selenium.Click("ctl00_Body_ctl00_SaveButton");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Test Role #1"), "Text 'Test Role #1' not found when it should be.");
			selenium.Click("ctl00_Body_ctl00_IndexGrid_ctl04_EditButton");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_Name", "Test Role #1");
			selenium.Click("//input[@value='Update']");
			selenium.WaitForPageToLoad("30000");
			selenium.ChooseOkOnNextConfirmation();
			selenium.Click("ctl00_Body_ctl00_IndexGrid_ctl04_DeleteButton");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.GetConfirmation() != null && selenium.GetConfirmation().IndexOf("Are you sure you want to delete this role?") > -1, "Confirm box didn't appear when expected.");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("deleted successfully"), "Text 'deleted successfully' not found when it should be.");
			Assert.IsFalse(selenium.IsTextPresent("Test Role"), "Text 'Test Role' found when it shouldn't be.");

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