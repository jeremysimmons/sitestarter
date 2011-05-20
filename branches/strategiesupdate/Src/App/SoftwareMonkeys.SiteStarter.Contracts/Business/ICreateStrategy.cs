using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all create entity strategies.
	/// </summary>
	public interface ICreateStrategy : IStrategy
	{
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		/// <returns>The newly instantiated entity.</returns>
		T Create<T>()
			where T : IEntity;
		
		/// <summary>
		/// Creates a new instance of the specified type of entity.
		/// </summary>
		/// <returns>The newly instantiated entity.</returns>
		IEntity Create();
	}
}
