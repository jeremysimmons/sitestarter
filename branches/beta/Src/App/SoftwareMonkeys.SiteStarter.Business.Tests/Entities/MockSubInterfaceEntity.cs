using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Entities
{
	/// <summary>
	/// A mock entity used for testing strategy matching. Implements IMockSubInterface. 
	/// </summary>
	[Entity("MockSubInterfaceEntity")]
	public class MockSubInterfaceEntity : BaseEntity, IMockSubInterface
	{
		public MockSubInterfaceEntity()
		{
		}
	}
}
