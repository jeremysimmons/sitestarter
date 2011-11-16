using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Thrown when a validation error message can't be found.
	/// </summary>
	public class ValidationMessageNotFoundException : Exception
	{
		public ValidationMessageNotFoundException(IEntity entity, string propertyName, string validatorName) : base("Can't find error message for failure of '" + validatorName + "' validator on '" + propertyName + "' property of '" + entity.ShortTypeName + "' entity type." + Environment.NewLine + "Add it via ValidationFacade.AddError(\"" + propertyName + "\", \"" + validatorName + "\", \"[errorLanguageKey]\") function.")
		{
		}
	}
}
