using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// Description of MockProjection.
	/// </summary>
	public class MockProjection : BaseProjection
	{
		public MockProjection()
		{
			Command = new CommandInfo("MockAction", "MockEntity");
		}
	}
}
