using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyContainsEntitiesAttribute : Attribute
	{
		private bool areTestEntities = false;
		/// <summary>
		/// Gets/sets a flag indicating whether the entities in this assembly are test/mock entities.
		/// </summary>
		public bool AreTestEntities
		{
			get  { return areTestEntities; }
			set  { areTestEntities = value; }
		}
		
		public AssemblyContainsEntitiesAttribute()
		{
		}
		
		public AssemblyContainsEntitiesAttribute(bool areTestEntities)
		{
			AreTestEntities = areTestEntities;
		}
	}
}
