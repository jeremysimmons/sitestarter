using System;
using NUnit.Framework;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web;
using System.IO;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Web.Tests
{
	[TestFixture]
	public class SiteMapTests : BaseWebTestFixture
	{
		
		[Test]
		public void Test_Add()
		{
			
			SiteMapNode homeNode = new SiteMapNode();
			homeNode.Action = "Home";
			homeNode.TypeName = "Home";
			
			SiteMapNode item = new SiteMapNode();
			item.Action = "TestAction";
			item.TypeName = "TestType";
			
			SiteMap map = new SiteMap();
			map.UrlCreator = new MockUrlCreator(this);
			map.Add(new SiteMapNode("Home", "Default.aspx"));
			
			map.Add(item);
			
			Assert.AreEqual(1, map.ChildNodes.Count, "Invalid number of child nodes.");
			Assert.AreEqual(1, map.ChildNodes[0].ChildNodes.Count, "Invalid number of sub child nodes.");
		}
		
		[Test]
		public void Test_Remove()
		{
			SiteMapNode homeNode = new SiteMapNode();
			homeNode.Action = "Home";
			homeNode.TypeName = "Home";
			
			SiteMapNode item = new SiteMapNode();
			item.Action = "TestAction";
			item.TypeName = "TestType";
			
			SiteMap map = new SiteMap();
			map.UrlCreator = new MockUrlCreator(this);
			map.Add(new SiteMapNode("Home", "Default.aspx"));
			
			map.Add(item);
			
			
			Assert.AreEqual(1, map.ChildNodes.Count, "Invalid number of child nodes.");
			Assert.AreEqual(1, map.ChildNodes[0].ChildNodes.Count, "Invalid number of sub child nodes.");
			
			
			map.Remove(item);
			
			Assert.AreEqual(1, map.ChildNodes.Count, "Invalid number of child nodes after removal.");
			Assert.AreEqual(0, map.ChildNodes[0].ChildNodes.Count, "Invalid number of sub child nodes after removal.");
		}
	}
}
