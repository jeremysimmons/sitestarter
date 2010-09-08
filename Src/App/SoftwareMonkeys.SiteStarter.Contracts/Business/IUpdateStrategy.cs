using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all update strategies.
	/// </summary>
	public interface IUpdateStrategy : IStrategy
	{
		/// <summary>
		/// Updates the provided entity.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		void Update(IEntity entity);
	}
}
