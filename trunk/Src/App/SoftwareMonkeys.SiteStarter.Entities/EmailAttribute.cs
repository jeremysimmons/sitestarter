using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Indicates that a property must have a unique value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	[Serializable]
	public class EmailAttribute : RegExAttribute
	{
		public EmailAttribute()
		{
			ValidatorName = "Unique";
			
			Expression = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
			+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
			+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
			+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
			
			// Regular expression is courtesy of:
			// http://www.codeproject.com/KB/recipes/EmailRegexValidator.aspx
		}
	}
}
