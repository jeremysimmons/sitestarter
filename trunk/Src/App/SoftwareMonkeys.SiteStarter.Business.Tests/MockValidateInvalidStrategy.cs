using System;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Used to test a validation failure.
	/// </summary>
	[ValidateStrategy("MockInvalid", "IEntity")]
	public class MockValidateInvalidStrategy : BaseValidatePropertyStrategy
	{
		public MockValidateInvalidStrategy()
		{
		}
		
		public override bool IsValid(SoftwareMonkeys.SiteStarter.Entities.IEntity entity, System.Reflection.PropertyInfo property, SoftwareMonkeys.SiteStarter.Entities.IValidatePropertyAttribute attribute)
		{
			return false;
		}
	}
}
