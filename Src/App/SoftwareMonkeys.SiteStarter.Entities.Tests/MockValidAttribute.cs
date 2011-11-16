using System;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class MockValidAttribute : BaseValidatePropertyAttribute
	{
		public MockValidAttribute()
		{
			ValidatorName = "MockValid";
		}
	}
}
