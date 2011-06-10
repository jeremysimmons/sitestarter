using System;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsControllersAttribute : Attribute
	{
		public AssemblyContainsControllersAttribute()
		{
		}
	}
}
