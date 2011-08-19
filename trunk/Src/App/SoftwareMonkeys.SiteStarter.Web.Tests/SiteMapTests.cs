using System;
using NUnit.Framework;
using System.Web;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Web;
using System.IO;
using SoftwareMonkeys.SiteStarter.Business.Tests;
using SoftwareMonkeys.SiteStarter.Tests;
using System.Collections.Generic;

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
			homeNode.Title = "Home";
			
			SiteMapNode item = new SiteMapNode();
			item.Action = "TestAction";
			item.TypeName = "TestType";
			item.Title = "Test Title";
			
			SiteMap map = new SiteMap();
			map.UrlCreator = new MockUrlCreator(this);
			map.Add(new SiteMapNode("Home", "Default.aspx"));
			
			map.Add(item);
			
			Assert.AreEqual(2, map.ChildNodes.Count, "Invalid number of child nodes.");
		}
		
		[Test]
		public void Test_Remove()
		{
			SiteMapNode homeNode = new SiteMapNode();
			homeNode.Action = "Home";
			homeNode.TypeName = "Home";
			homeNode.Title = "Home";
			
			SiteMapNode item = new SiteMapNode();
			item.Action = "TestAction";
			item.TypeName = "TestType";
			item.Title = "Test Title";
			
			SiteMap map = new SiteMap();
			map.UrlCreator = new MockUrlCreator(this);
			map.Add(homeNode);
			map.Add(item);
			
			Assert.AreEqual(2, map.ChildNodes.Count, "Invalid number of nodes before removal.");
			
			map.Remove(item);
			
			Assert.AreEqual(1, map.ChildNodes.Count, "Invalid number of nodes after removal.");
			Assert.AreEqual(0, map.ChildNodes[0].ChildNodes.Count, "Invalid number of child nodes after removal.");
		}
		
		[Test]
		public void Test_GetNodeByTitle_TwoLevelTitle()
		{
			SiteMap siteMap = new SiteMap();
			
			// Create the base node and add it
			SiteMapNode baseNode = new SiteMapNode("Category", "TestUrl");
			
			List<SiteMapNode> baseNodes = new List<SiteMapNode>();
			baseNodes.Add(baseNode);
			
			siteMap.ChildNodes = baseNodes;
			
			// Create the sub node and add it
			SiteMapNode subNode = new SiteMapNode("Category", "SubNode", "TestAction", "TestType");
			
			List<SiteMapNode> subNodes = new List<SiteMapNode>();
			subNodes.Add(subNode);
			
			baseNode.ChildNodes = subNodes;
			
			// Try to get the node by providing the 2 level title
			SiteMapNode node = siteMap.GetNodeByTitle(baseNodes, "Category/SubNode");
			
			Assert.IsNotNull(node, "No node found.");
			
			
		}
		
		[Test]
		public void Test_Add_SubCategory()
		{
			SiteMap siteMap = new SiteMap();
			siteMap.UrlCreator = new MockUrlCreator(this);
			
			// Create the base node and add it
			SiteMapNode baseNode = new SiteMapNode("Category", "TestUrl");
			
			List<SiteMapNode> baseNodes = new List<SiteMapNode>();
			baseNodes.Add(baseNode);
			
			siteMap.ChildNodes = baseNodes;
			
			
			// Create the sub category node and add it
			SiteMapNode subCategoryNode = new SiteMapNode("Category", "SubCategory", "TestAction", "TestType");
			
			List<SiteMapNode> subCategoryNodes = new List<SiteMapNode>();
			subCategoryNodes.Add(subCategoryNode);
			
			baseNode.ChildNodes = subCategoryNodes;
			
			// Create the sub node and add it
			SiteMapNode subNode = new SiteMapNode("Category/SubCategory", "SubNode", "TestAction", "TestType");
			
			// Add the sub node - it should be placed within the sub category
			siteMap.Add(subNode);
			
			Assert.IsNotNull(subCategoryNode.ChildNodes, "Child nodes collection is null");
			
			Assert.AreEqual(1, subCategoryNode.ChildNodes.Count, "Wrong number of sub nodes found.");
		}
		
		[Test]
		public void Test_Add_SubCategory_AutoCreateParent()
		{
			SiteMap siteMap = new SiteMap();
			siteMap.UrlCreator = new MockUrlCreator(this);
			
			// Create the sub node and add it
			SiteMapNode subNode = new SiteMapNode("Category/SubCategory", "SubNode", "TestAction", "TestType");
			
			// Add the sub node - it should be placed within the sub category
			siteMap.Add(subNode);
			
			Assert.IsNotNull(siteMap.ChildNodes, "Child nodes collection is null");
			
			Assert.AreEqual(1, siteMap.ChildNodes.Count, "Wrong number of nodes found.");
			
			Assert.AreEqual("Category", siteMap.ChildNodes[0].Title, "First level category title is incorrect.");
			
			Assert.IsNotNull(siteMap.ChildNodes[0].ChildNodes, "Second level child nodes collection is null");
			
			Assert.AreEqual(1, siteMap.ChildNodes[0].ChildNodes.Count, "Wrong number of second level nodes found.");
			
			Assert.AreEqual("SubCategory", siteMap.ChildNodes[0].ChildNodes[0].Title, "Second level category title is incorrect.");
			
			Assert.IsNotNull(siteMap.ChildNodes[0].ChildNodes[0].ChildNodes, "Third level child nodes collection is null");
			
			Assert.AreEqual(1, siteMap.ChildNodes[0].ChildNodes[0].ChildNodes.Count, "Wrong number of third level nodes found.");
		}
	}
}
