using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	[Strategy("MockAction", "TestUser")]
	public class MockActionStrategy : BaseStrategy
	{
		public MockActionStrategy()
		{
		}
		
		public void Execute(IEntity entity)
		{
			// This function isn't needed. It's just an example
			throw new NotSupportedException();
		}
		
		static public MockActionStrategy New()
		{
			return New(true);
		}
		
		static public MockActionStrategy New(bool requiresAuthorisation)
		{
			MockActionStrategy strategy = new MockActionStrategy();

			strategy.RequireAuthorisation = requiresAuthorisation;
			
			StrategyState
				.Strategies
				.Creator
				.AttachReactions("MockAction", strategy);
			
			return strategy;
		}
	}
}
