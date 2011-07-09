using System;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsControllersAttribute : Attribute
	{
		private bool areTestControllers = false;
		public bool AreTestControllers
		{
			get { return areTestControllers; }
			set { areTestControllers = value; }
		}
		
		public AssemblyContainsControllersAttribute()
		{
		}
		
		public AssemblyContainsControllersAttribute(bool areTestControllers)
		{
			AreTestControllers = areTestControllers;
		}
	}
}
