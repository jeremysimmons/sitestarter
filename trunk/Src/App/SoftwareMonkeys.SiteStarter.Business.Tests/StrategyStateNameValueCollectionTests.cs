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
			StrategyInfo testStrategy = StrategyInfo.ExtractInfo(typeof(RetrieveStrategy))[0];
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string type = "TestUser";
			string action = "TestAction";
			
			collection[action, type] = testStrategy;
			
			StrategyInfo foundStrategy = collection[action, type];
			
			Assert.IsNotNull(foundStrategy, "Strategy not found.");
			
		}
		
		[Test]
		[ExpectedException("SoftwareMonkeys.SiteStarter.Business.StrategyNotFoundException")]
		public void Test_this_StrategyNotFound()
		{
			StrategyInfo testStrategy = StrategyInfo.ExtractInfo(typeof(RetrieveStrategy))[0];
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string type = "TestUser";
			string action = "TestAction";
			
			StrategyInfo notFoundStrategy = collection[action + "Mismatch", type];
		}
		
		[Test]
		public void Test_this_Interface()
		{
			StrategyInfo testStrategy = StrategyInfo.ExtractInfo(typeof(RetrieveStrategy))[0];
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string type = "TestArticle";
			string action = "Retrieve";
			
			string key = collection.GetStrategyKey(testStrategy.Action, testStrategy.TypeName);
			
			collection.Add(testStrategy);
			
			StrategyInfo foundStrategy = collection[action, type];
			
			Assert.IsNotNull(foundStrategy);
		}
		
		[Test]
		public void Test_this_IUniqueEntityInterface()
		{
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			string type = "TestUser";
			string action = "Validate";
			
			collection.Add(typeof(ValidateStrategy));
			collection.Add(typeof(UniqueValidateStrategy));
			
			StrategyInfo foundStrategy = collection[action, type];
			
			Assert.IsNotNull(foundStrategy);
			
			Assert.AreEqual("UniqueValidateStrategy", foundStrategy.New().GetType().Name, "Loaded the wrong type.");
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
		
		// TODO: Remove if not needed
		/*
		[Test]
		public void Test_GetStrategyFromInterfaces()
		{
			string interfaceType = "IEntity";
			string type = "TestArticle";
			string action = "TestAction";
			
			StrategyInfo testStrategy = new StrategyInfo(new RetrieveStrategy());
			testStrategy.TypeName = interfaceType;
			testStrategy.Action = action;
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			
			collection[action, interfaceType] = testStrategy;
			
			StrategyInfo foundStrategy = collection.GetStrategyFromInterfaces(typeof(TestArticle), action, type);
			StrategyInfo notFoundStrategy = collection.GetStrategyFromInterfaces(typeof(TestArticle), action + "Mismatch", type + "Mismatch");
			
			Assert.IsNotNull(foundStrategy);
			Assert.IsNull(notFoundStrategy);
		}
		
		[Test]
		public void Test_GetStrategyFromBaseTypes()
		{
			string baseType = "BaseEntity";
			string type = "TestArticle";
			string action = "TestAction";
			
			StrategyInfo testStrategy = new StrategyInfo(new RetrieveStrategy());
			testStrategy.TypeName = baseType;
			testStrategy.Action = action;
			
			StrategyStateNameValueCollection collection = new StrategyStateNameValueCollection();
			
			
			collection[action, baseType] = testStrategy;
			
			StrategyInfo foundStrategy = collection.GetStrategyFromBaseTypes(action, type);
			StrategyInfo notFoundStrategy = collection.GetStrategyFromBaseTypes(action + "Mismatch", type);
			
			Assert.IsNotNull(foundStrategy);
			Assert.IsNull(notFoundStrategy);
		}
		*/
		
	}
}
