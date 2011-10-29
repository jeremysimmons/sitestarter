using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// A mock retrieve strategy.
	/// </summary>
	[Strategy("Retrieve", "TestUser")]
	public class MockRetrieveTestUserStrategy : RetrieveStrategy
	{
		public MockRetrieveTestUserStrategy() : base()
		{
		}
	}
}
