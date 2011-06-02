using System;

namespace SoftwareMonkeys.SiteStarter.Business.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[Strategy("InvalidValidate", "IEntity")]
	public class MockInvalidValidateEntityStrategy : ValidateStrategy
	{
		public MockInvalidValidateEntityStrategy()
		{
		}
		
		public override bool Validate(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			return false;
		}
	}
}
