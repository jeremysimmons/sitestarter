using System;
using SoftwareMonkeys.SiteStarter.Tests;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// 
	/// </summary>
	public class MockProjectionScanner : ProjectionScanner
	{
		public MockProjectionScanner(BaseTestFixture fixture) : base(null)
		{
			ControlLoader = new MockControlLoader(fixture);
		}
	}
}
