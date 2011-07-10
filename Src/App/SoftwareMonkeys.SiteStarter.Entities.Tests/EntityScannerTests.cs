using System;
using System.Reflection;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Tests;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class EntityScannerTests : BaseTestFixture
	{
		public EntityScannerTests()
		{
		}
		
		[Test]
		public void Test_ContainsEntities_MatchingAssembly()
		{
			Assembly matchingAssembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Entities");
			
			EntityScanner scanner = new EntityScanner();
			
			bool doesMatch = scanner.ContainsEntities(matchingAssembly);
			
			Assert.IsTrue(doesMatch, "Failed to match when it should.");
		}
		
		[Test]
		public void Test_ContainsEntities_NonMatchingAssembly()
		{
			Assembly assembly = Assembly.Load("SoftwareMonkeys.SiteStarter.State");
			
			EntityScanner scanner = new EntityScanner();
			
			bool doesMatch = scanner.ContainsEntities(assembly);
			
			Assert.IsFalse(doesMatch, "Matched when it shouldn't have.");
		}
		
		[Test]
		public void Test_ContainsEntities_MatchingAssembly_IncludeTestEntities()
		{
			Assembly matchingAssembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Entities.Tests");
			
			EntityScanner scanner = new EntityScanner();
			
			bool includeTestEntities = true;
			
			bool doesMatch = scanner.ContainsEntities(matchingAssembly, includeTestEntities);
			
			Assert.IsTrue(doesMatch, "Failed to match when it should.");
		}
		
		[Test]
		public void Test_ContainsEntities_NonMatchingAssembly_ExcludeTestEntities()
		{
			Assembly assembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Entities.Tests");
			
			EntityScanner scanner = new EntityScanner();
			
			bool includeTestEntities = false;
			
			bool doesMatch = scanner.ContainsEntities(assembly, includeTestEntities);
			
			Assert.IsFalse(doesMatch, "Matched when it shouldn't have.");
		}
	}
}
