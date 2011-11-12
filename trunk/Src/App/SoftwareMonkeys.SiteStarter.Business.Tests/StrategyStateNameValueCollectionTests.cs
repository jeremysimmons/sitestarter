using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyStateNameValueCollectionTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_this()
		{
			StrategyInfo testStrategy = StrategyInfo.ExtractInfo(typeof(MockRetrieveTestUserStrategy))[0];
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string type = "TestUser";
			string action = "Retrieve";
			
			collection.Add(testStrategy);
			
			StrategyInfo foundStrategy = collection[action, type];
			
			Assert.IsNotNull(foundStrategy, "Strategy not found.");
			
		}
		
		[Test]
		[ExpectedException("SoftwareMonkeys.SiteStarter.Business.StrategyNotFoundException")]
		public void Test_this_StrategyNotFound()
		{
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string action = "MockAction";
			string type = "MockType";
			
			StrategyInfo notFoundStrategy = collection[action + "Mismatch", type];
		}
		
		[Test]
		public void Test_this_Interface()
		{
			StrategyInfo testStrategy = StrategyInfo.ExtractInfo(typeof(RetrieveStrategy))[0];
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string type = "TestArticle";
			string action = "Retrieve";
			
			collection.Add(testStrategy);
			
			StrategyInfo foundStrategy = collection[action, type];
			
			Assert.IsNotNull(foundStrategy);
		}
		
		[Test]
		[ExpectedException("SoftwareMonkeys.SiteStarter.Business.StrategyNotFoundException")]
		public void Test_this_Interface_StrategyNotFound()
		{
			StrategyInfo testStrategy = StrategyInfo.ExtractInfo(typeof(SaveStrategy))[0];
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string type = "TestUser";
			string action = "TestAction";
			
			collection[action, type] = testStrategy;
			
			StrategyInfo notFoundStrategy = collection[action + "Mismatch", type];
			
			Assert.IsNull(notFoundStrategy);
		
		}
		
	}
}
