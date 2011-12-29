using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Defines the interface for entities that have an author. The author generally has ownership and the rights associated with it.
	/// </summary>
	public interface IAuthored : IEntity
	{
		IUser Author { get;set; }
		bool IsPublic { get;set; }
	}
}
