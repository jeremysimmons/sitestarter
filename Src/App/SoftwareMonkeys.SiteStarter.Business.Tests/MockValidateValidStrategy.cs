using System;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// Used to test a validation success.
	/// </summary>
	[ValidateStrategy("MockValid", "IEntity")]
	public class MockValidateValidStrategy : BaseValidatePropertyStrategy
	{
		public MockValidateValidStrategy()
		{
		}
		
		public override bool IsValid(SoftwareMonkeys.SiteStarter.Entities.IEntity entity, System.Reflection.PropertyInfo property, SoftwareMonkeys.SiteStarter.Entities.IValidatePropertyAttribute attribute)
		{
			return true;
		}
	}
}
