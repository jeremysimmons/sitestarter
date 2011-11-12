using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyLocatorTests : BaseBusinessTestFixture
	{
		[Test]
		/// <summary>
		/// Tests the LocateFromInterfaces method where the matching interface is implemented by the provided type.
		/// </summary>
		public void Test_LocateFromInterfaces_Immediate()
		{
			Type type = typeof(IEntity);
			string action = "Save";
			
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			
			strategies.Add(typeof(SaveStrategy));
			strategies.Add(typeof(UpdateStrategy));
			
			StrategyLocator locator = new StrategyLocator(strategies);
			
			StrategyInfo info = locator.LocateFromInterfaces(action,type);
			
			Assert.IsNotNull(info, "No strategy info found.");
		}
		
		[Test]
		/// <summary>
		/// Tests the LocateFromInterfaces method where the matching interface is implemented by a base type.
		/// </summary>
		public void Test_LocateFromInterfaces_Heirarchy()
		{
			Type type = typeof(TestArticle);
			string action = "Save";
			
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			
			strategies.Add(typeof(SaveStrategy));
			strategies.Add(typeof(UpdateStrategy));
			
			StrategyLocator locator = new StrategyLocator(strategies);
			
			StrategyInfo info = locator.LocateFromInterfaces(action, type);
			
			Assert.IsNotNull(info, "No strategy info found.");
		}
		
		[Test]
		public void Test_Locate_CustomOverride()
		{			
			string type = "Widget";
			string action = "Index";
			
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			
			strategies.Add(typeof(IndexStrategy));
			strategies.Add(typeof(MockIndexWidgetStrategy));
			
			StrategyLocator locator = new StrategyLocator(strategies);
			
			StrategyInfo info = locator.Locate(action, type);
			
			Assert.IsNotNull(info, "No strategy info found.");
			
			Type mockStrategyType = new MockIndexWidgetStrategy().GetType();
			
			string expected = mockStrategyType.FullName + ", " + mockStrategyType.Assembly.GetName().Name;
			
			Assert.AreEqual(expected, info.StrategyType, "Wrong strategy type selected.");
		}
		
		[Test]
		public void Test_Locate_InterfaceOverride()
		{			
			string type = "MockInterfaceEntity";
			string action = "Index";
			
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			
			strategies.Add(typeof(IndexStrategy));
			strategies.Add(typeof(MockIndexWidgetStrategy));
			strategies.Add(typeof(MockIndexInterfaceStrategy));
			
			StrategyLocator locator = new StrategyLocator(strategies);
			
			StrategyInfo info = locator.Locate(action, type);
			
			Assert.IsNotNull(info, "No strategy info found.");
			
			Type mockStrategyType = new MockIndexInterfaceStrategy().GetType();
			
			string expected = mockStrategyType.FullName + ", " + mockStrategyType.Assembly.GetName().Name;
			
			Assert.AreEqual(expected, info.StrategyType, "Wrong strategy type selected.");
		}
		
		[Test]
		public void Test_Locate_MatchInheritedInterface()
		{			
			string type = "MockSubInterfaceEntity";
			string action = "Index";
			
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			
			strategies.Add(typeof(IndexStrategy));
			strategies.Add(typeof(MockIndexWidgetStrategy));
			strategies.Add(typeof(MockIndexInterfaceStrategy));
			
			StrategyLocator locator = new StrategyLocator(strategies);
			
			StrategyInfo info = locator.Locate(action, type);
			
			Assert.IsNotNull(info, "No strategy info found.");
			
			Type mockStrategyType = new MockIndexInterfaceStrategy().GetType();
			
			string expected = mockStrategyType.FullName + ", " + mockStrategyType.Assembly.GetName().Name;
			
			Assert.AreEqual(expected, info.StrategyType, "Wrong strategy type selected.");
		}
		
		[Test]
		public void Test_Locate()
		{			
			StrategyLocator locator = new StrategyLocator(StrategyState.Strategies);
			
			StrategyInfo info = locator.Locate("Delete", "TestArticle");
			
			string expectedType = typeof(DeleteStrategy).FullName + ", " + typeof(DeleteStrategy).Assembly.GetName().Name;
			
			Assert.AreEqual(expectedType, info.StrategyType, "Wrong strategy located.");
		}
		
		[Test]
		public void Test_Locate_Base_Delete()
		{			
			StrategyLocator locator = new StrategyLocator(StrategyState.Strategies);
			
			StrategyInfo info = locator.Locate("Delete", "IEntity");
			
			string expectedType = typeof(DeleteStrategy).FullName + ", " + typeof(DeleteStrategy).Assembly.GetName().Name;
			
			Assert.AreEqual(expectedType, info.StrategyType, "Wrong strategy located.");
		}
		
		
		[Test]
		public void Test_Locate_Base_Save()
		{			
			StrategyLocator locator = new StrategyLocator(StrategyState.Strategies);
			
			StrategyInfo info = locator.Locate("Save", "IEntity");
			
			string expectedType = typeof(SaveStrategy).FullName + ", " + typeof(SaveStrategy).Assembly.GetName().Name;
			
			Assert.AreEqual(expectedType, info.StrategyType, "Wrong strategy located.");
		}
	}
}
