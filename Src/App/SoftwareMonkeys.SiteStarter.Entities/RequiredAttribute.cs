using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Indicates that the corresponding property is required to have a value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Serializable]
	public class RequiredAttribute : BaseValidatePropertyAttribute
	{
		public RequiredAttribute()
		{
			ValidatorName = "Required";
		}
	}
}
