using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// 
	/// </summary>
	public interface IValidatePropertyAttribute
	{
		/// <summary>
		/// Gets/sets the name of the validator used to validate the corresponding property.
		/// </summary>
		string ValidatorName { get;set; }
	}
}
