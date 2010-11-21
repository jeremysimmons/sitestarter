using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Defines the interface of all entities that are required to be unique according to a specific property such as UniqueKey.
	/// </summary>
	public interface IUniqueEntity : IEntity
	{
		string UniqueKey {get;}
	}
}
