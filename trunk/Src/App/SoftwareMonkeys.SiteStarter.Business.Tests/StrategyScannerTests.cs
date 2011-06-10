using NUnit.Framework;
using System;
using SoftwareMonkeys.SiteStarter.Business;
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
	}
}
