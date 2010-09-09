using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to validate entities and ensure they're unique.
	/// </summary>
	[Strategy("Validate", "IUniqueEntity")]
	public class UniqueValidateStrategy : IUniqueValidateStrategy
	{
		private string uniquePropertyName;
		/// <summary>
		/// Gets/sets the name of the property on the target entity that is unique.
		/// </summary>
		public string UniquePropertyName
		{
			get { return uniquePropertyName; }
			set { uniquePropertyName = value; }
		}
				
		public UniqueValidateStrategy()
		{
		}
		
		/// <summary>
		/// Validates the provided entity by checking whether the provided value of the specified unique property is unique to all entities currently in the system.
		/// </summary>
		/// <param name="entity">The entity to validate by checking whether the provided property value is unique.</param>
		/// <param name="propertyName">The name of the property that is to remain unique.</param> 
		/// <returns>A value to indicate whether the provided value is unique to the provided property.</returns>
		public bool Validate(IEntity entity, string propertyName)
		{
			IRetrieveStrategy strategy = StrategyState.Strategies.Creator.NewRetriever(entity.ShortTypeName);
			
			object propertyValue = EntitiesUtilities.GetPropertyValue(entity, propertyName);
			
			IEntity existingEntity = (IEntity)strategy.Retrieve(entity.GetType(), propertyName, propertyValue);
			
			bool isTaken = (existingEntity != null && !existingEntity.ID.Equals(entity.ID));
			
			return !isTaken;
		}
	}
}
