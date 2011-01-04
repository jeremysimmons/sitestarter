using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Defines the simple interface adopted by most entities.
	/// </summary>
	public interface ISimple : IEntity
	{
		string Title {get;set;}
		string Description {get;set;}
		
	}
}
