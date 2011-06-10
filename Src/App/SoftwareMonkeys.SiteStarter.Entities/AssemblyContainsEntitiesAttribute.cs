using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of AssemblyContainsEntitiesAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsEntitiesAttribute : Attribute
	{
		public AssemblyContainsEntitiesAttribute()
		{
		}
	}
}
