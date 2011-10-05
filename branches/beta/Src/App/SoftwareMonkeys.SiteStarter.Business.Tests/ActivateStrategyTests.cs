using System;
using NUnit.Framework;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[TestFixture]
	public class ActivateStrategyTests : BaseBusinessTestFixture
	{
		[Test]
		public void Test_Found_ForIEntityInterface()
		{
			StrategyInfo strategy = StrategyState.Strategies["Activate", "IEntity"];
			
			Assert.IsNotNull(strategy);
		}
	}
}
