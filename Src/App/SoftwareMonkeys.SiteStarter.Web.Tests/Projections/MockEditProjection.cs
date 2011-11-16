using System;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Web.Projections;

namespace SoftwareMonkeys.SiteStarter.Web.Tests.Projections
{
	/// <summary>
	/// 
	/// </summary>
	public class MockEditProjection : BaseProjection
	{
		public MockEditProjection(Type entityType)
		{
			Command = new EditCommandInfo(entityType.Name);
		}
	}
}
