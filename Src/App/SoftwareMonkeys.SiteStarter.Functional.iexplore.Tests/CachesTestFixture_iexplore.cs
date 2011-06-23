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
	public class CachesTestFixture_iexplore : SoftwareMonkeys.SiteStarter.Functional.Tests.BaseFunctionalTestFixture
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
		public void Test_Caches()
		{
			selenium.SetTimeout("1000000");
			selenium.Open("Admin/tests/testreset.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/QuickSetup.aspx");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/Cache.aspx");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Click("EntitiesCacheLink");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("BaseEntity"), "Text 'BaseEntity' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("User"), "Text 'User' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("SoftwareMonkeys.SiteStarter.Entities.dll"), "Text 'SoftwareMonkeys.SiteStarter.Entities.dll' not found when it should be.");
			selenium.Click("CacheIndexLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Strategies");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("ActivateStrategy"), "Text 'ActivateStrategy' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("AuthenticateStrategy"), "Text 'AuthenticateStrategy' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("SoftwareMonkeys.SiteStarter.Business.dll"), "Text 'SoftwareMonkeys.SiteStarter.Business.dll' not found when it should be.");
			selenium.Click("CacheIndexLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Reactions");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Click("CacheIndexLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Parts");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			selenium.Click("CacheIndexLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Controllers");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("CreateController"), "Text 'CreateController' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("CreateUserController"), "Text 'CreateUserController' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("SoftwareMonkeys.SiteStarter.Web.Controllers"), "Text 'SoftwareMonkeys.SiteStarter.Web.Controllers' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("SoftwareMonkeys.SiteStarter.Web.dll"), "Text 'SoftwareMonkeys.SiteStarter.Web.dll' not found when it should be.");
			selenium.Click("CacheIndexLink");
			selenium.WaitForPageToLoad("30000");
			selenium.Click("link=Projections");
			selenium.WaitForPageToLoad("30000");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("User-Create-Edit.ascx"), "Text 'User-Create-Edit.ascx' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("User-Index.ascx"), "Text 'User-Index.ascx' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("Settings-Index.ascx"), "Text 'Settings-Index.ascx' not found when it should be.");
			Assert.IsTrue(selenium.IsTextPresent("/Projections/"), "Text '/Projections/' not found when it should be.");
			selenium.Click("link=Sign Out");
			selenium.WaitForPageToLoad("30000");
			selenium.Open("Admin/Cache.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");
			selenium.Open("Admin/Entities.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");
			selenium.Open("Admin/Strategies.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");
			selenium.Open("Admin/Reactions.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");
			selenium.Open("Admin/Parts.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");
			selenium.Open("Admin/Controllers.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");
			selenium.Open("Admin/Projections.aspx");
			Assert.IsFalse(selenium.IsTextPresent("Exception"), "Text 'Exception' found when it shouldn't be.");
			Assert.IsTrue(selenium.IsTextPresent("Sign In Details"), "Text 'Sign In Details' not found when it should be.");

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