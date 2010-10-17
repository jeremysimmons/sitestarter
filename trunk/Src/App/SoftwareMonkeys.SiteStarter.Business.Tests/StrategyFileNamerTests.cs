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
						
			StrategyFileNamer namer = new StrategyFileNamer();
			
			string name = namer.CreateFileName(strategy);
			
			string expected = "TestUser-Retrieve.strategy";
			
			Assert.AreEqual(expected, name);
		}
	}
}
