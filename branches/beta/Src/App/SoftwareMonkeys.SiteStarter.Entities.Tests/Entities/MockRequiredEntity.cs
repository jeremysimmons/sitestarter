using System;
using SoftwareMonkeys.SiteStarter.Tests.Entities;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[Entity]
	public class MockRequiredEntity : BaseTestEntity
	{
		private string testProperty = String.Empty;
		[Required]
		public string TestProperty
		{
			get { return testProperty; }
			set { testProperty = value; }
		}
		
		public MockRequiredEntity()
		{
		}
	}
}
