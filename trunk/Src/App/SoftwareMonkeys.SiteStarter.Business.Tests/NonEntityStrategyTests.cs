using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class NonEntityStrategyTests : BaseBusinessTestFixture
	{
		public NonEntityStrategyTests()
		{
		}
		
		[Test]
		public void Test_Found()
		{
			Assert.IsNotNull(StrategyState.Strategies["MockAction", "NonEntity"], "Didn't find the strategy.");
		}
	}
}
