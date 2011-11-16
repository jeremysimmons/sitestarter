using System;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[Entity]
	public class MockInvalidEntity : BaseTestEntity
	{
		private string testProperty;
		[MockInvalid]
		public string TestProperty
		{
			get { return testProperty; }
			set { testProperty = value; }
		}
		
		public MockInvalidEntity()
		{
		}
	}
}
