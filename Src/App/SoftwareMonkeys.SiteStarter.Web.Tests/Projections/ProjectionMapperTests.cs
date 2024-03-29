﻿using System;
using System.IO;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using SoftwareMonkeys.SiteStarter.IO;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Web.Projections;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class ProjectionMapperTests : BaseWebTestFixture
	{
		[Test]
		public void Test_GetDynamicID()
		{
			Guid expectedID = new Guid("2a7ecbc7-b22c-4bca-908f-955f7a59ef51");
			
			string originalUrl = "http://localhost/MockApplication/Edit-UserRole/" + expectedID.ToString() + "/Administrator.aspx";
			
			ProjectionMapper mapper = new ProjectionMapper();
			mapper.Converter = new MockUrlConverter();
			
			string shortPath = mapper.GetShortPath(originalUrl);
			
			Guid foundID = mapper.GetDynamicID(originalUrl);
			
			Assert.AreEqual(expectedID.ToString(), foundID.ToString(), "ID doesn't match expected.");
		}
		
        [Test]
        public void Test_GetInternalPath_Skip()
        {
        	string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";

            string original = fullApplicationUrl + "/Admin/Setup.aspx";

            string expected = "";

            ProjectionMapper mapper = new ProjectionMapper();
            mapper.ApplicationPath = applicationPath;
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, true);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_GetInternalPath_ProjectionName()
        {
            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";

            string original = fullApplicationUrl + "/Settings.aspx";

            string expected = applicationPath + "/Projector.aspx?n=Settings&f=Html";
            
            // Create the mock settings projection
            ProjectionInfo info = new ProjectionInfo();
            info.Name = "Settings";
            ProjectionState.Projections.Add(info);

            ProjectionMapper mapper = new ProjectionMapper();
            mapper.ApplicationPath = applicationPath;
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }
		
        [Test]
        public void Test_GetInternalPath_Action_Type()
        {
            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";

            string action = "Edit";
            string typeName = "TestUser";
            
            string original = fullApplicationUrl + "/" + action + "-" + typeName + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&f=Html";

            ProjectionMapper mapper = new ProjectionMapper();
            mapper.ApplicationPath = applicationPath;
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }

        [Test]
        public void Test_GetInternalPath_Action_Type_ID()
        {
            string entityID = "A1FC7BA3-3832-467f-8989-058BF420D1D9";

            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";
            
            string action = "Edit";
            string typeName = "TestUser";

            string original = fullApplicationUrl + "/" + action + "-" + typeName + "/" + entityID + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&f=Html&" + typeName + "-ID=" + entityID;

            ProjectionMapper mapper = new ProjectionMapper();
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }

        [Test]
        public void Test_GetInternalPath_Action_Type_ID_IgnoreText()
        {
        	string entityName = "TestUsername";
            string entityID = "A1FC7BA3-3832-467f-8989-058BF420D1D9";

            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";
            
            string action = "Edit";
            string typeName = "TestUser";

            string original = fullApplicationUrl + "/" + action + "-" + typeName + "/" + entityID + "/I--" + entityName + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&f=Html&" + typeName + "-ID=" + entityID;

            IFileMapper fileMapper = new MockFileMapper(this, TestUtilities.GetTestingPath(this));
            
            ProjectionMapper mapper = new ProjectionMapper();
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_GetInternalPath_Action_Type_ID_Key()
        {
        	string entityName = "TestUsername";
            string entityID = "A1FC7BA3-3832-467f-8989-058BF420D1D9";

            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";
            
            string action = "Edit";
            string typeName = "TestUser";

            string original = fullApplicationUrl + "/" + action + "-" + typeName + "/" + entityID + "/K--" + entityName + ".aspx";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&f=Html&" + typeName + "-ID=" + entityID + "&" + typeName + "-UniqueKey=" + entityName;

            ProjectionMapper mapper = new ProjectionMapper();
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }        
        
        [Test]
        public void Test_GetInternalPath_ProjectionName_KeepExistingQueryStrings()
        {
            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";

            string original = fullApplicationUrl + "/Settings.aspx?TestKey=TestValue";

            string expected = applicationPath + "/Projector.aspx?n=Settings&f=Html&TestKey=TestValue";
            
            // Create the mock settings projection
            ProjectionInfo info = new ProjectionInfo();
            info.Name = "Settings";
            ProjectionState.Projections.Add(info);

            ProjectionMapper mapper = new ProjectionMapper();
            mapper.ApplicationPath = applicationPath;
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_ExtractOriginalQueryStrings()
        {
        	Dictionary<string, string> parameters = new Dictionary<string, string>();
        	
        	// Add a parameter to ensure it isn't added twice
        	parameters.Add("a", "TestValueA");
        	
        	string originalPath = "http://localhost/MockApplication/TestPage.aspx?a=TestValueA&b=TestValueB&c=TestValueC";
        	
        	new ProjectionMapper().ExtractOriginalQueryStrings(originalPath, parameters);
        	
        	Assert.AreEqual(3, parameters.Count, "Invalid number of parameters extracted.");
        }
        
        
        [Test]
        public void Test_GetInternalPath_Action_Type_KeepOriginalQueryStrings()
        {
            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";

            string action = "Edit";
            string typeName = "TestUser";
            
            string original = fullApplicationUrl + "/" + action + "-" + typeName + ".aspx?TestKey=TestValue";

            string expected = applicationPath + "/Projector.aspx?a=" + action + "&t=" + typeName + "&f=Html&TestKey=TestValue";

            ProjectionMapper mapper = new ProjectionMapper();
            mapper.ApplicationPath = applicationPath;
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string generated = mapper.GetInternalPath(original);

            Assert.AreEqual(expected.ToLower(), generated.ToLower(), "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_GetCommandName()
        {
        
            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";

            string action = "Edit";
            string typeName = "TestUser";
            
            string original = fullApplicationUrl + "/" + action + "-" + typeName + ".aspx?TestKey=TestValue&TestKey2=TestValue2";

            string expected = action + "-" + typeName;

            ProjectionMapper mapper = new ProjectionMapper();
            mapper.ApplicationPath = applicationPath;
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string cmd = mapper.GetCommandName(original);

            Assert.AreEqual(expected, cmd, "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_GetCommandName_XmlProjection()
        {
        
            string fullApplicationUrl = "http://localhost/MockApplication";
            string applicationPath = "/MockApplication";

            string action = "Index";
            string typeName = "TestUser";
            
            string original = fullApplicationUrl + "/" + action + "-" + typeName + ".xml.aspx?TestKey=TestValue&TestKey2=TestValue2";

            string expected = action + "-" + typeName;

            ProjectionMapper mapper = new ProjectionMapper();
            mapper.ApplicationPath = applicationPath;
			mapper.Converter = new MockUrlConverter();
			mapper.FileMapper = new MockFileMapper(this);
			mapper.FileExistenceChecker = new MockFileExistenceChecker(this, false);
            
            string cmd = mapper.GetCommandName(original);

            Assert.AreEqual(expected, cmd, "Result doesn't match expected.");
        }
        
        [Test]
        public void Test_GetFormat_Xml()
        {
        	ProjectionMapper mapper = new ProjectionMapper();
        	
        	string fileName = "TestPage.xml.aspx";
        	
        	ProjectionFormat format = mapper.GetFormat(fileName);
        	
        	Assert.AreEqual(ProjectionFormat.Xml, format, "Doesn't match what is expected.");
        }
        
        
        [Test]
        public void Test_GetFormat_Html()
        {
        	ProjectionMapper mapper = new ProjectionMapper();
        	
        	string fileName = "TestPage.aspx";
        	
        	ProjectionFormat format = mapper.GetFormat(fileName);
        	
        	Assert.AreEqual(ProjectionFormat.Html, format, "Doesn't match what is expected.");
        }
        
        [Test]
        public void Test_GetFormat_Xslt()
        {
        	ProjectionMapper mapper = new ProjectionMapper();
        	
        	string fileName = "TestPage.xslt.aspx";
        	
        	ProjectionFormat format = mapper.GetFormat(fileName);
        	
        	Assert.AreEqual(ProjectionFormat.Xslt, format, "Doesn't match what is expected.");
        }
	}
}
