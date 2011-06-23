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
	public class RegisterPendingUserTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_RegisterPendingUser()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("You are signed in as"), "Text 'You are signed in as' not found when it should be.");
			selenium.Click("link=Settings");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsFalse(selenium.IsTextPresent("aren't authorised"), "Text 'aren't authorised' found when it shouldn't be.");
			selenium.Click("link=User Settings");
			selenium.WaitForPageToLoad("30000");
			if (selenium.IsChecked("ctl00_Body_ctl00_AutoApproveNewUsers"))\n\t\t\tselenium.Click("ctl00_Body_ctl00_AutoApproveNewUsers");
			selenium.Click("ctl00_Body_ctl00_UpdateButton");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("SignOutLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("RegisterLink");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_FirstName", "Pending");
			selenium.Type("ctl00_Body_ctl00_LastName", "User");
			selenium.Type("ctl00_Body_ctl00_Email", "pendinguser@softwaremonkeys.net");
			selenium.Type("ctl00_Body_ctl00_Username", "pendinguser");
			selenium.Type("ctl00_Body_ctl00_Password", "pass");
			selenium.Type("ctl00_Body_ctl00_PasswordConfirm", "pass");
			selenium.Click("ctl00_Body_ctl00_EnableNotifications");
			selenium.Click("ctl00_Body_ctl00_RegisterButton");
			selenium.WaitForPageToLoad("30000");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Pending Approval"), "Text 'Pending Approval' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In"), "Text 'Sign In' not found when it should be.");

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