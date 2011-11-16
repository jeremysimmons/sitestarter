using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Indicates that the corresponding property is to be validated using the provided regular expression.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class RegExAttribute : BaseValidatePropertyAttribute
	{
		public string Expression = String.Empty;
		
		public RegExAttribute(string expression)
		{
			Expression = expression;
		}
		
		public RegExAttribute()
		{}
	}
}
