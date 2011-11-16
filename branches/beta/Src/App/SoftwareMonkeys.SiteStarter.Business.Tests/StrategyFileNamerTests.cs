using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business.Tests.Security;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyFileNamerTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_CreateFileName()
		{
			IStrategy strategy = new MockRetrieveTestUserStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
						
			StrategyFileNamer namer = new StrategyFileNamer();
			
			string name = namer.CreateInfoFileName(info);
			
			string expected = "Retrieve_TestUser.strategy";
			
			Assert.AreEqual(expected, name);
		}
		
		[Test]
		public void Test_CreateFileName_AuthoriseReferenceStrategy()
		{
			IStrategy strategy = new AuthoriseReferenceMockPublicEntityStrategy();
			
			StrategyInfo info = StrategyInfo.ExtractInfo(strategy.GetType())[0];
						
			StrategyFileNamer namer = new StrategyFileNamer();
			
			string name = namer.CreateInfoFileName(info);
			
			string expected = info.Key + ".ar.strategy";
			
			Assert.AreEqual(expected, name);
	}
}
}
