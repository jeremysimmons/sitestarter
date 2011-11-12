using System;
using System.Text.RegularExpressions;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to validate properties using a regular expression.
	/// </summary>
	[ValidateStrategy("RegEx", "IEntity")]
	public class ValidateRegExStrategy : BaseValidatePropertyStrategy
	{
		public ValidateRegExStrategy()
		{
		}
		
		public override bool IsValid(SoftwareMonkeys.SiteStarter.Entities.IEntity entity, System.Reflection.PropertyInfo property, SoftwareMonkeys.SiteStarter.Entities.IValidatePropertyAttribute attribute)
		{
			if (property.PropertyType != typeof(String))
				throw new InvalidOperationException("Cannot validate email address because the property type is '" + property.PropertyType.Name + "' when it needs to be string.");
			
			string value = GetStringValue(entity, property);
			
			RegExAttribute regExAttribute = (RegExAttribute)attribute;
			
			return Regex.IsMatch(value, regExAttribute.Expression);
		}
	}
}
