using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[ValidateStrategy("Required", "IEntity")]
	public class ValidateRequiredStrategy : BaseValidatePropertyStrategy
	{
		public ValidateRequiredStrategy()
		{
		}
		
		public override bool IsValid(IEntity entity, System.Reflection.PropertyInfo property, IValidatePropertyAttribute attribute)
		{
			object value = property.GetValue(entity, null);
			
			if (property.PropertyType == typeof(string))
				return value != String.Empty;
			else
				return value != null;

			// TODO: Add support for more types such as
			// int, DateTime, Guid, etc.
			
		}
	}
}
