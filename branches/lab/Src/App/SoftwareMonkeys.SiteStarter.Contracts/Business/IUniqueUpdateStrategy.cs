using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all update strategies that enforce a unique property rule.
	/// </summary>
	public interface IUniqueUpdateStrategy : IUpdateStrategy
	{
		/// <summary>
		/// Updates the provided entity if it's unique.
		/// </summary>
		/// <param name="entity">The entity to update if it's unique.</param>
		/// <returns>A value indicating whether or not the entity was updated. If false then the entity must not have been unique.</returns>
		bool Update(IEntity entity);
	}
}
