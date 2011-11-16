using System;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsElementsAttribute : Attribute
	{
		private bool areTestElements = false;
		public bool AreTestElements
		{
			get { return areTestElements; }
			set { areTestElements = value; }
		}
		
		public AssemblyContainsElementsAttribute()
		{
		}
		
		public AssemblyContainsElementsAttribute(bool areTestControls)
		{
			AreTestElements = areTestControls;
		}
	}
}
