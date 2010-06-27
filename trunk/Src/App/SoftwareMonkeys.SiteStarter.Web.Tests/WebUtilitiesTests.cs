using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Web.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;
using System.Web;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	[TestFixture]
	public class WebUtilitiesTests
	{
		public string ApplicationPath
		{
			// TODO: Path MUST NOT be hard coded
			//   get { return @"f:\SoftwareMonkeys\WorkHub\Application 2\Web\"; }
			//     get { return System.Configuration.ConfigurationSettings.AppSettings["ApplicationPath"]; }
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalPath; }
		}
		
		public WebUtilitiesTests()
		{
			//Config.Initialize(ApplicationPath, "");
		}

        [Test]
        public void Test_ConvertAbsoluteUrlToRelativeUrl()
        {
            string url = "http://www.Test.com/Folder1/folder2/file.html";//HttpContext.Current.Request.Url.ToString();

            string applicationPath = "http://www.test.com/folder1/";
            
            string expected = "/folder2/file.html";

            string result = WebUtilities.ConvertAbsoluteUrlToRelativeUrl(url, applicationPath);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
        
        
        [Test]
        public void Test_ConvertRelativeUrlToAbsoluteUrl()
        {
            string url = "/Folder1/folder2/file.html";//HttpContext.Current.Request.Url.ToString();

            string expected = "http://localhost/Folder1/folder2/file.html";
            

            string result = WebUtilities.ConvertRelativeUrlToAbsoluteUrl(url);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
        
        [Test]
        public void Test_ConvertRelativeUrlToAbsoluteUrl_WithoutPreceedingSlash()
        {
            string url = "Folder1/folder2/file.html";//HttpContext.Current.Request.Url.ToString();

            string expected = "http://localhost/Folder1/folder2/file.html";
            

            string result = WebUtilities.ConvertRelativeUrlToAbsoluteUrl(url);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
	}
}
