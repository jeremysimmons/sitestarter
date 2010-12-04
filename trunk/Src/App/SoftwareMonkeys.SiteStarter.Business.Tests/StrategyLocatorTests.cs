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
			Type type = typeof(BaseUniqueEntity);
			string action = "Save";
			
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			
			strategies.Add(new SaveStrategy());
			strategies.Add(new UniqueSaveStrategy());
			
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
			
			strategies.Add(new SaveStrategy());
			strategies.Add(new UniqueSaveStrategy());
			
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
			
			strategies.Add(new IndexStrategy());
			strategies.Add(new MockIndexWidgetStrategy());
			
			StrategyLocator locator = new StrategyLocator(strategies);
			
			StrategyInfo info = locator.Locate(action, type);
			
			Assert.IsNotNull(info, "No strategy info found.");
			
			Type mockStrategyType = new MockIndexWidgetStrategy().GetType();
			
			Assert.AreEqual(mockStrategyType.FullName + ", " + mockStrategyType.Assembly.FullName, info.StrategyType, "Wrong strategy type selected.");
		}
	}
}
