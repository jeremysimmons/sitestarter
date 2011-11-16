using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class ValidateStrategyAttribute : StrategyAttribute
	{
		public ValidateStrategyAttribute(string validatorName, string typeName) : base("Validate" + validatorName, typeName)
		{}
	}
}
