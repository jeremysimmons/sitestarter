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
			Assembly assembly = Assembly.Load("SoftwareMonkeys.SiteStarter.Business");
			
			EntityScanner scanner = new EntityScanner();
			
			bool doesMatch = scanner.ContainsEntities(assembly);
			
			Assert.IsFalse(doesMatch, "Matched when it shouldn't have.");
		}
	}
}
