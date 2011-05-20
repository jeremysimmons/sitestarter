using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class StrategyInfoTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Constructor_StrategyParameter()
		{
			MockIndexWidgetStrategy strategy = new MockIndexWidgetStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			Assert.AreEqual("Widget", info.TypeName, "Type name doesn't match what's expected.");
			Assert.AreEqual("Index", info.Action, "Action doesn't match what's expected.");
			//Assert.AreEqual("Index_Widget", info.Key, "Key doesn't match what's expected.");
			Assert.AreEqual(strategy.GetType().FullName + ", " + strategy.GetType().Assembly.GetName().Name, info.StrategyType, "Strategy type doesn't match what's expected.");
		}
	}
}
