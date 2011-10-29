using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Entities.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Security
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class AuthoriseReferenceStrategyLocatorTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Locate_Basic_MockEntity_MockRestrictedEntity()
		{
			//StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			//strategies.Clear();
			//strategies.Add(typeof(AuthoriseReferenceMockPublicEntityStrategy));
			
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator(StrategyState.Strategies);
			
			StrategyInfo strategy = locator.Locate("MockEntity", "RestrictedEntities", "MockRestrictedEntity", String.Empty);
			
			Assert.IsNotNull(strategy, "No strategy found.");
			
			string expectedType = typeof(AuthoriseReferenceMockRestrictedEntityStrategy).FullName + ", " + typeof(AuthoriseReferenceMockRestrictedEntityStrategy).Assembly.GetName().Name;
			
			Assert.AreEqual(expectedType, strategy.StrategyType, "Wrong strategy info located.");
		}
		
		[Test]
		public void Test_Locate_Basic_MockEntity_PublicEntities_MockPublicEntity()
		{
			StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			strategies.Clear();
			strategies.Add(typeof(AuthoriseReferenceMockPublicEntityStrategy));
			
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator(strategies);
			
			StrategyInfo strategy = locator.Locate("MockEntity", "PublicEntities", "MockPublicEntity", String.Empty);
			
			Assert.IsNotNull(strategy, "No strategy found.");
			
			string expectedType = typeof(AuthoriseReferenceMockPublicEntityStrategy).FullName + ", " + typeof(AuthoriseReferenceMockPublicEntityStrategy).Assembly.GetName().Name;
			
			Assert.AreEqual(expectedType, strategy.StrategyType, "Wrong strategy info located.");
		}
		
		/// <summary>
		/// Ensures that the locator will match properties to wild cards in the strategy attribute.
		/// </summary>
		[Test]
		public void Test_Locate_Wildcards()
		{
			//StrategyStateNameValueCollection strategies = new StrategyStateNameValueCollection();
			//strategies.Clear();
			//strategies.Add(typeof(AuthoriseReferenceMockPublicEntityStrategy));
			
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator(StrategyState.Strategies);
			
			StrategyInfo strategy = locator.Locate("TestArticle", "PublicEntities", "TestCategory", String.Empty);
			
			Assert.IsNotNull(strategy, "No strategy found.");
			
			string expectedType = typeof(AuthoriseReferenceStrategy).FullName + ", " + typeof(AuthoriseReferenceStrategy).Assembly.GetName().Name;
			
			Assert.AreEqual(expectedType, strategy.StrategyType, "Wrong strategy info located.");
		}
		
		[Test]
		public void Test_TypesMatch_UserVsIEntity()
		{
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator(StrategyState.Strategies);
			
			bool matches = locator.TypesMatch("User", "IEntity");
			
			Assert.IsTrue(matches, "Types don't match when they should.");
		}
		
		[Test]
		public void Test_IsMoreSpecific_EntityVsInterface()
		{
			Type specificType = typeof(MockEntity);
			Type generalType = typeof(IEntity);
			
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator(StrategyState.Strategies);
			bool isMoreSpecific = locator.IsMoreSpecific(specificType, generalType);
			
			Assert.IsTrue(isMoreSpecific, "Returned false when it should be true.");
		}
		
		[Test]
		public void Test_IsMoreSpecific_EntityVsBaseClass()
		{
			Type specificType = typeof(MockEntity);
			Type generalType = typeof(BaseEntity);
			
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator(StrategyState.Strategies);
			bool isMoreSpecific = locator.IsMoreSpecific(specificType, generalType);
			
			Assert.IsTrue(isMoreSpecific, "Returned false when it should be true.");
		}
		
		[Test]
		public void Test_PropertiesMatch_EmptyVsWildCard()
		{
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator();
			bool propertiesMatch = locator.PropertiesMatch("", "*");
			
			Assert.IsTrue(propertiesMatch, "Didn't match when they should have.");
		}
		
		[Test]
		public void Test_PropertiesMatch_MocPropertyVsWildCard()
		{
			AuthoriseReferenceStrategyLocator locator = new AuthoriseReferenceStrategyLocator();
			bool propertiesMatch = locator.PropertiesMatch("MockProperty", "*");
			
			Assert.IsTrue(propertiesMatch, "Didn't match when they should have.");
		}
	}
}
