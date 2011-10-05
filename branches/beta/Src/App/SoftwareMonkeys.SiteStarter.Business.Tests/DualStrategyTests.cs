using System;
using NUnit.Framework;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class DualStrategyTests : BaseBusinessTestFixture
	{
		public DualStrategyTests()
		{
		}
		
		[Test]
		public void Test_Found()
		{
			Assert.IsNotNull(StrategyState.Strategies["MockAction1", "MockNonEntity1"], "Didn't detect first strategy attribute.");
			
			Assert.IsNotNull(StrategyState.Strategies["MockAction2", "MockNonEntity2"], "Didn't detect second strategy attribute.");
		}
	}
}
