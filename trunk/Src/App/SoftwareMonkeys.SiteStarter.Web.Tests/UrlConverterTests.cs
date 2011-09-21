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
	public class UrlConverterTests : BaseWebTestFixture
	{
		public string ApplicationPath
		{
			get { return TestUtilities.GetTestingPath(this); }
		}

        [Test]
        public void Test_ToRelative()
        {
            string url = "http://www.Test.com/Folder1/folder2/file.html";

            string applicationPath = "http://www.test.com/folder1/";
            
            string expected = "/folder2/file.html";

            UrlConverter converter = new UrlConverter(applicationPath);
			converter.Protocol = "http";
			converter.Host = "localhost";
            
            string result = converter.ToRelative(url);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
        
        
        [Test]
        public void Test_ToAbsolute()
        {
            string url = "/Folder1/folder2/file.html";//HttpContext.Current.Request.Url.ToString();

            string expected = "http://localhost/Folder1/folder2/file.html";
            
			//string protocol = "http";
			
			//string host = "localhost";
            
			UrlConverter converter = new UrlConverter(ApplicationPath);		
			converter.Protocol = "http";
			converter.Host = "localhost";
            
			string result = converter.ToAbsolute(url);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
        
        [Test]
        public void Test_ConvertRelativeUrlToAbsoluteUrl_WithoutPreceedingSlash()
        {
            string url = "Folder1/folder2/file.html";//HttpContext.Current.Request.Url.ToString();

            string expected = "http://localhost/Folder1/folder2/file.html";
            
            UrlConverter converter = new UrlConverter("/");
			converter.Protocol = "http";
			converter.Host = "localhost";
			
			string result = converter.ToAbsolute(url);

            Assert.AreEqual(expected, result, "Doesn't match.");
        }
	}
}
