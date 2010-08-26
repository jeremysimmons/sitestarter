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
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;
using System.Web;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	[TestFixture]
	public class WebUtilitiesTests : BaseWebTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(this); }
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
            
			bool isSecure = false;
			
			string host = "localhost";
            
            string result = WebUtilities.ConvertRelativeUrlToAbsoluteUrl(url, host, isSecure);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
        
        [Test]
        public void Test_ConvertRelativeUrlToAbsoluteUrl_WithoutPreceedingSlash()
        {
            string url = "Folder1/folder2/file.html";//HttpContext.Current.Request.Url.ToString();

            string expected = "http://localhost/Folder1/folder2/file.html";
            
			bool isSecure = false;
			
			string host = "localhost";
            
            string result = WebUtilities.ConvertRelativeUrlToAbsoluteUrl(url, host, isSecure);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
	}
}
