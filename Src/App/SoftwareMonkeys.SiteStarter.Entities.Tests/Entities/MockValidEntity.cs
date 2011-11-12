using System;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[Entity]
	public class MockValidEntity : BaseTestEntity
	{
		private string testProperty;
		[MockValid]
		public string TestProperty
		{
			get { return testProperty; }
			set { testProperty = value; }
		}
		
		public MockValidEntity()
		{
		}
	}
}
