using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[Entity("MockInterfaceEntity")]
	public class MockInterfaceEntity : BaseEntity, IMockInterface
	{
		public MockInterfaceEntity()
		{
		}
	}
}
