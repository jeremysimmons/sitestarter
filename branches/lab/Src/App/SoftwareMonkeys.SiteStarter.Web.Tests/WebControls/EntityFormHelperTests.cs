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
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Web.WebControls;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.WebControls
{
	[TestFixture]
	public class EntityFormHelperTests : BaseWebTestFixture
	{
		public string ApplicationPath
		{
			get { return SoftwareMonkeys.SiteStarter.Configuration.Config.Application.PhysicalApplicationPath; }
		}
		
		public EntityFormHelperTests()
		{
			//Config.Initialize(ApplicationPath, "");
		}
		
		[Test]
		public void Test_Convert_Enum()
		{
			TestEnum e = TestEnum.One;
			
			object value = EntityFormHelper.Convert(e, typeof(TestEnum));
			
			Assert.IsTrue(Enum.IsDefined(typeof(TestEnum), e), "The returned value isn't the correct type.");
			Assert.AreEqual(TestEnum.One, value, "The returned value didn't match.");
		}
		
		[Test]
		public void Test_Convert_Enum_FromInt()
		{			
			object value = EntityFormHelper.Convert(1, typeof(TestEnum));
			
			Assert.IsTrue(Enum.IsDefined(typeof(TestEnum), value), "The returned value isn't the correct type.");
			Assert.AreEqual(TestEnum.One, value, "The returned value didn't match.");
		}
		
		[Test]
		public void Test_Convert_Enum_FromString()
		{
			object value = EntityFormHelper.Convert("One", typeof(TestEnum));
			
			Assert.IsTrue(Enum.IsDefined(typeof(TestEnum), value), "The returned value isn't the correct type.");
			Assert.AreEqual(TestEnum.One, value, "The returned value didn't match.");
		}
		
		[Test]
		public void Test_Convert_Int()
		{
			object value = EntityFormHelper.Convert(10, typeof(Int32));
			
			Assert.IsTrue(value is Int32, "The returned value isn't the correct type.");
			Assert.AreEqual(10, value, "The returned value didn't match.");
		}
		
		[Test]
		public void Test_Convert_String()
		{
			object value = EntityFormHelper.Convert("Test", typeof(string));
			
			Assert.IsTrue(value is string, "The returned value isn't the correct type.");
			Assert.AreEqual("Test", value, "The returned value didn't match.");
		}
		
		[Test]
		public void Test_Convert_String_FromInt()
		{
			object value = EntityFormHelper.Convert(10, typeof(string));
			
			Assert.IsTrue(value is string, "The returned value isn't the correct type.");
			Assert.AreEqual("10", value, "The returned value didn't match.");
		}
	}
}
