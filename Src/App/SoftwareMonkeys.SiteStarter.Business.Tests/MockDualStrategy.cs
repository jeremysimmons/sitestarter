using System;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("MockAction1", "MockNonEntity1")]
	[Strategy("MockAction2", "MockNonEntity2")]
	public class MockDualStrategy : BaseStrategy
	{
		public MockDualStrategy()
		{
		}
		
	}
}
