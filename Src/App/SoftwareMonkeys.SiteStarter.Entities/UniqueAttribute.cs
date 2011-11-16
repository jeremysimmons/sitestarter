using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Indicates that a property must have a unique value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Serializable]
	public class UniqueAttribute : BaseValidatePropertyAttribute
	{
		public UniqueAttribute()
		{
			ValidatorName = "Unique";
		}
	}
}
