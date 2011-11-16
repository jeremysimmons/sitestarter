using NUnit.Framework;
using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Business.Tests.Security;
using SoftwareMonkeys.SiteStarter.Tests.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class StrategyScannerTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_FindStrategies()
		{
			StrategyScanner scanner = new StrategyScanner();
			
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business").Location
			};
			
			scanner.AssemblyPaths = assemblyPaths;
			
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
		
		[Test]
		public void Test_ContainsEntities_MatchingAssembly()
		{
			Assembly matchingAssembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Business");
			
			StrategyScanner scanner = new StrategyScanner();
			
			bool doesMatch = scanner.ContainsStrategies(matchingAssembly);
			
			Assert.IsTrue(doesMatch, "Failed to match when it should.");
		}
		
		[Test]
		public void Test_ContainsEntities_NonMatchingAssembly()
		{
			Assembly assembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Entities");
			
			StrategyScanner scanner = new StrategyScanner();
			
			bool doesMatch = scanner.ContainsStrategies(assembly);
			
			Assert.IsFalse(doesMatch, "Matched when it shouldn't have.");
		}
		
		
		[Test]
		public void Test_ContainsEntities_MatchingAssembly_IncludeTestStrategies()
		{
			Assembly matchingAssembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests");
			
			StrategyScanner scanner = new StrategyScanner();
			
			bool doesMatch = scanner.ContainsStrategies(matchingAssembly, true);
			
			Assert.IsTrue(doesMatch, "Failed to match when it should.");
		}
		
		[Test]
		public void Test_ContainsEntities_NonMatchingAssembly_ExcludeTestStrategies()
		{
			Assembly assembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests");
			
			StrategyScanner scanner = new StrategyScanner();
			
			bool doesMatch = scanner.ContainsStrategies(assembly, false);
			
			Assert.IsFalse(doesMatch, "Matched when it shouldn't have.");
		}
		
		[Test]
		public void Test_FindStrategies_FindsAuthoriseReferenceStrategy()
		{
			StrategyScanner scanner = new StrategyScanner();
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests").Location
			};
			
			scanner.AssemblyPaths = assemblyPaths;
			
			StrategyInfo[] strategies = scanner.FindStrategies(true);
			
			Assert.Greater(strategies.Length, 0);
			
			bool authoriseReferenceStrategyFound = false;
			
			foreach (StrategyInfo strategy in strategies)
			{
				if (strategy is AuthoriseReferenceStrategyInfo)
					authoriseReferenceStrategyFound = true;
	}
			
			Assert.IsTrue(authoriseReferenceStrategyFound, "No authorise reference strategies found.");
}
		
		
		[Test]
		public void Test_FindStrategies_FindsAuthoriseReferenceMockRestrictedEntityStrategy()
		{
			StrategyScanner scanner = new StrategyScanner();
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests").Location
			};
			
			scanner.AssemblyPaths = assemblyPaths;
			
			StrategyInfo[] strategies = scanner.FindStrategies(true);
			
			Assert.Greater(strategies.Length, 0);
			
			bool authoriseReferenceStrategyFound = false;
			
			string expectedTypeString = typeof(AuthoriseReferenceMockRestrictedEntityStrategy).FullName + ", " + typeof(AuthoriseReferenceMockRestrictedEntityStrategy).Assembly.GetName().Name;
			
			foreach (StrategyInfo strategy in strategies)
			{
				if (strategy is AuthoriseReferenceStrategyInfo)
				//if (typeof(AuthoriseReferenceStrategyInfo).IsAssignableFrom(strategy.GetType()))
				{
					if (strategy.StrategyType == expectedTypeString)
						authoriseReferenceStrategyFound = true;
				}
			}
			
			Assert.IsTrue(authoriseReferenceStrategyFound, "AuthoriseReferenceMockRestrictedEntityStrategy info not found.");
		}
		
		[Test]
		public void Test_FindStrategies_FindsAuthoriseReferenceMockPublicEntityStrategy()
		{
			StrategyScanner scanner = new StrategyScanner();
			
			string[] assemblyPaths = new string[] {
				Assembly.Load("SoftwareMonkeys.SiteStarter.Business.Tests").Location
			};
			
			scanner.AssemblyPaths = assemblyPaths;
			
			StrategyInfo[] strategies = scanner.FindStrategies(true);
			
			Assert.Greater(strategies.Length, 0);
			
			bool authoriseReferenceStrategyFound = false;
			
			string expectedTypeString = typeof(AuthoriseReferenceMockPublicEntityStrategy).FullName + ", " + typeof(AuthoriseReferenceMockPublicEntityStrategy).Assembly.GetName().Name;
			
			foreach (StrategyInfo strategy in strategies)
			{
				if (strategy is AuthoriseReferenceStrategyInfo)
				{
					if (strategy.StrategyType == expectedTypeString)
					{
						authoriseReferenceStrategyFound = true;
					}
				}
			}
			
			Assert.IsTrue(authoriseReferenceStrategyFound, "AuthoriseReferenceMockPublicEntityStrategy info not found.");
		}
	}
}
