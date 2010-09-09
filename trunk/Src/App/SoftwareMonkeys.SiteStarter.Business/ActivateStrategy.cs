using System;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to activate entities by retrieving the corresponding references.
	/// </summary>
	[Strategy("Activate", "IEntity")]
	public class ActivateStrategy : IActivateStrategy
	{
		public ActivateStrategy()
		{
		}
		
		/// <summary>
		/// Activates the provided entity by retrieving the corresponding references.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		public void Activate(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			DataAccess.Data.Activator.Activate(entity);
		}
		
		/// <summary>
		/// Activates the provided entity by retrieving the corresponding references.
		/// </summary>
		/// <param name="entity">The entity to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		public void Activate(SoftwareMonkeys.SiteStarter.Entities.IEntity entity, string propertyName)
		{
			DataAccess.Data.Activator.Activate(entity, propertyName);
		}
		
		/// <summary>
		/// Activates the provided entities by retrieving the corresponding references.
		/// </summary>
		/// <param name="entities">The entities to activate.</param>
		public void Activate(SoftwareMonkeys.SiteStarter.Entities.IEntity[] entities)
		{
			DataAccess.Data.Activator.Activate(entities);
		}
	}
}
