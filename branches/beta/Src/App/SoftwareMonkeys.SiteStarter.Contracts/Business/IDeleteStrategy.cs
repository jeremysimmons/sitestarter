using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all delete strategies.
	/// </summary>
	public interface IDeleteStrategy : IStrategy
	{
		/// <summary>
		/// Deletes the provided entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		void Delete(IEntity entity);
	}
}
