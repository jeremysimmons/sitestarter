using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsReactionsAttribute : Attribute
	{
		private bool areTestReactions = false;
		public bool AreTestReactions
		{
			get { return areTestReactions; }
			set { areTestReactions = value; }
		}
		
		public AssemblyContainsReactionsAttribute()
		{
		}
		
		public AssemblyContainsReactionsAttribute(bool areTestReactions)
		{
			AreTestReactions = areTestReactions;
		}
	}
}
