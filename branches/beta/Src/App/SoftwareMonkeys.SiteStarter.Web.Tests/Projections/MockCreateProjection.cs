using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// 
	/// </summary>
	public class MockCreateProjection : BaseCreateEditProjection
	{
		public MockCreateProjection(Type entityType)
		{
			Command = new CreateComandInfo(entityType.Name);
		}
	}
}
