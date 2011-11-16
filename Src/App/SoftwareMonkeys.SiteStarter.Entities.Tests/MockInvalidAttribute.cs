using System;

namespace SoftwareMonkeys.SiteStarter.Entities.Tests
{
	/// <summary>
	/// Treats the assigned property as invalid, to test the validation system.
	/// </summary>
	public class MockInvalidAttribute : BaseValidatePropertyAttribute
	{
		public MockInvalidAttribute()
		{
			ValidatorName = "MockInvalid";
		}
	}
}
