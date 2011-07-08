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
	public class BackupTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_Backup()
		{
			selenium.SetTimeout("1000000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("You are signed in as"), "Text 'You are signed in as' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Backup"), "Text 'Backup' not found when it should be.");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Click("link=Backup");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsFalse(selenium.IsTextPresent("aren't authorised"), "Text 'aren't authorised' found when it shouldn't be.");
			selenium.Click("//input[@value='Start']");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("successfully"), "Text 'successfully' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Total files backed up: 5"), "Text 'Total files backed up: 5' not found when it should be.");
			selenium.Click("link=Sign Out");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/Backup.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");

		}
	}
}