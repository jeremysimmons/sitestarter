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
	public class EnforceUniqueUsernameTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_EnforceUniqueUsername()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Settings"), "Text 'Settings' not found when it should be.");
			selenium.Click("link=Settings");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=User Settings");
			selenium.WaitForPageToLoad("30000");
			if (!selenium.IsChecked("ctl00_Body_ctl00_AutoApproveNewUsers"))
			selenium.Click("ctl00_Body_ctl00_AutoApproveNewUsers");
			selenium.Click("ctl00_Body_ctl00_UpdateButton");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			selenium.Click("SignOutLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("RegisterLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "first");
			selenium.Type("ctl00_Body_ctl00_LastName", "last");
			selenium.Type("ctl00_Body_ctl00_Email", "duplicateuser@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "admin");
			selenium.Type("ctl00_Body_ctl00_Password", "pass");
			selenium.Type("ctl00_Body_ctl00_PasswordConfirm", "pass");
			selenium.Click("ctl00_Body_ctl00_EnableNotifications");
			selenium.Click("ctl00_Body_ctl00_RegisterButton");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("username is already in use"), "Text 'username is already in use' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("New User Details"), "Text 'New User Details' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Register"), "Text 'Register' not found when it should be.");

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