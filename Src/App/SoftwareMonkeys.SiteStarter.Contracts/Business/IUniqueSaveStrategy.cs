using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all save strategies that enforce a unique property rule.
	/// </summary>
	public interface IUniqueSaveStrategy : ISaveStrategy
	{
		/// <summary>
		/// Saves the provided entity if it's unique.
		/// </summary>
		/// <param name="entity">The entity to save if it's unique.</param>
		/// <returns>A value indicating whether or not the entity was saved. If false then the entity must not have been unique.</returns>
		bool Save(IEntity entity);
	}
}
