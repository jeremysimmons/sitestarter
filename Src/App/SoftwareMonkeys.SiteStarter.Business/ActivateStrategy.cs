using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to activate entities by retrieving the corresponding references.
	/// </summary>
	[Strategy("Activate", "IEntity")]
	public class ActivateStrategy : BaseStrategy, IActivateStrategy
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
		/// Activates the specified property of the provided entity by retrieving the corresponding references.
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
		
		/// <summary>
		/// Activates the specified property of the provided entities by retrieving the corresponding references.
		/// </summary>
		/// <param name="entities">The entities to activate.</param>
		/// <param name="propertyName">The name of the property to activate.</param>
		public void Activate(SoftwareMonkeys.SiteStarter.Entities.IEntity[] entities, string propertyName)
		{
			foreach (IEntity entity in entities)
			{
				DataAccess.Data.Activator.Activate(entity, propertyName);
			}
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for activating the specified type.
		/// </summary>
		static public IActivateStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.NewActivator(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for activating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IActivateStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.NewActivator(typeName);
		}
		
		/// <summary>
		/// Creates a new strategy for activating the specified type.
		/// </summary>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IActivateStrategy New<T>(bool requiresAuthorisation)
		{
			IActivateStrategy strategy = StrategyState.Strategies.Creator.NewActivator(typeof(T).Name);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for activating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IActivateStrategy New(string typeName, bool requiresAuthorisation)
		{
			IActivateStrategy strategy = StrategyState.Strategies.Creator.NewActivator(typeName);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		#endregion
	}
}
