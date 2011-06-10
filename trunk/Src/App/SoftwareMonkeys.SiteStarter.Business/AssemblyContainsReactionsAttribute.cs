using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsReactionsAttribute : Attribute
	{
		public AssemblyContainsReactionsAttribute()
		{
		}
	}
}
