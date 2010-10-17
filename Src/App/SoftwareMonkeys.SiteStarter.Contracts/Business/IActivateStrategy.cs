using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface for all activate strategies.
	/// </summary>
	public interface IActivateStrategy : IStrategy
	{
		/// <summary>
		/// Activates the provided entity by retrieving the corresponding references.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		void Activate(IEntity entity);
		
		/// <summary>
		/// Activates the specified property of the provided entity by retrieving the corresponding references.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		void Activate(IEntity entity, string propertyName);
		
		/// <summary>
		/// Activates the provided entities by retrieving the corresponding references.
		/// </summary>
		/// <param name="entities">The entities to activate.</param>
		void Activate(IEntity[] entities);
		
		/// <summary>
		/// Activates the specified property of the provided entities by retrieving the corresponding references.
		/// </summary>
		/// <param name="entities">The entities to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		void Activate(IEntity[] entities, string propertyName);
		
	}
}
