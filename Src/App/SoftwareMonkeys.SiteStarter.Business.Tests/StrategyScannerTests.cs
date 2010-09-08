using NUnit.Framework;
using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Tests.Entities;


namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyScannerTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_FindStrategies()
		{
			StrategyScanner scanner = new StrategyScanner();
			
			StrategyInfo[] strategies = scanner.FindStrategies();
			
			Assert.Greater(strategies.Length, 0);
		}
		
		[Test]
		public void Test_IsStrategy()
		{
			
			StrategyScanner scanner = new StrategyScanner();
			
			bool isStrategy = scanner.IsStrategy(typeof(RetrieveStrategy));
			
			bool notStrategy = scanner.IsStrategy(typeof(TestUser));
			
			Assert.IsTrue(isStrategy);
			Assert.IsFalse(notStrategy);
		}
	}
}
