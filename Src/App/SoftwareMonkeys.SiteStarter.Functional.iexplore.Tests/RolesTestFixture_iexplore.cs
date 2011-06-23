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
	public class RolesTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_Roles()
		{
			selenium.SetTimeout("100000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("MockCreate-User.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Roles");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Manage Roles"), "Text 'Manage Roles' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Edit"), "Text 'Edit' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Delete"), "Text 'Delete' not found when it should be.");
			selenium.Click("//input[@value='Create Role']");
			selenium.WaitForPageToLoad("30000");
			selenium.Type("ctl00_Body_ctl00_Name", "A Test Role");
			selenium.AddSelection("ctl00_Body_ctl00_Users", "label=FirstName1 LastName1");
			selenium.Click("//input[@value='Save']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("saved successfully"), "Text 'saved successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Manage Roles"), "Text 'Manage Roles' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Test Role"), "Text 'Test Role' not found when it should be.");
			selenium.Click("link=Edit");
			selenium.WaitForPageToLoad("30000");
			Assert.AreEqual("System Administrator", selenium.GetSelectedLabel("ctl00_Body_ctl00_Users"));
			selenium.AddSelection("ctl00_Body_ctl00_Users", "FirstName1 LastName1");
			selenium.Click("//input[@value='Update']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("updated successfully"), "Text 'updated successfully' not found when it should be.");
			selenium.Click("link=Edit");
			selenium.WaitForPageToLoad("30000");
			Assert.AreEqual("FirstName1 LastName1,System Administrator", String.Join(",", selenium.GetSelectedLabels("ctl00_Body_ctl00_Users")));

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