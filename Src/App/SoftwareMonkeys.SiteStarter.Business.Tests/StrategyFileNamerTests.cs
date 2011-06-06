using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyFileNamerTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_CreateFileName()
		{
			IStrategy strategy = new MockRetrieveStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
						
			StrategyFileNamer namer = new StrategyFileNamer();
			
			string name = namer.CreateInfoFileName(info);
			
			string expected = "TestUser-Retrieve.strategy";
			
			Assert.AreEqual(expected, name);
		}
	}
}
