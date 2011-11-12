using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Serializable]
	public abstract class BaseValidatePropertyAttribute : Attribute, IValidatePropertyAttribute
	{
		private string validatorName = String.Empty;
		public string ValidatorName
		{
			get { return validatorName; }
			set { validatorName = value; }
		}
		
		public BaseValidatePropertyAttribute()
		{
		}
	}
}
