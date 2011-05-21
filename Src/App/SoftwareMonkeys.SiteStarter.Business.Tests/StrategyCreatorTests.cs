using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyCreatorTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_CreateStrategy()
		{
			IStrategy strategy = new RetrieveStrategy();
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
			
			IStrategy created = info.New();
			
			Assert.IsNotNull(created);
			
			Assert.AreEqual(strategy.GetType().ToString(), created.GetType().ToString());
			
			
		}
	}
}
