using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsStrategiesAttribute : Attribute
	{
		private bool areTestStrategies = false;
		/// <summary>
		/// Gets/sets a flag indicating whether the strategies in this assembly are test/mock strategies.
		/// </summary>
		public bool AreTestStrategies
		{
			get  { return areTestStrategies; }
			set  { areTestStrategies = value; }
		}
		
		public AssemblyContainsStrategiesAttribute()
		{
		}
		
		public AssemblyContainsStrategiesAttribute(bool areTestStrategies)
		{
			AreTestStrategies = areTestStrategies;
		}
	}
}
