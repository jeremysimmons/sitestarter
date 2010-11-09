using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to validate entities and ensure they're unique.
	/// </summary>
	[Strategy("Validate", "IUniqueEntity")]
	public class UniqueValidateStrategy : ValidateStrategy, IUniqueValidateStrategy
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
			bool isTaken = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Validating the provided entity by ensuring the specified property value is unique.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				AppLogger.Debug("Property name: " + propertyName);
				
				AppLogger.Debug("Entity: " + entity.GetType().FullName);
				
				IRetrieveStrategy strategy = RetrieveStrategy.New(entity.ShortTypeName, false);
				
				object propertyValue = EntitiesUtilities.GetPropertyValue(entity, propertyName);
				
				AppLogger.Debug("Property value: " + propertyValue.ToString());
				
				IEntity existingEntity = (IEntity)strategy.Retrieve(entity.GetType(), propertyName, propertyValue);
				
				AppLogger.Debug("Existing entity found: " + (existingEntity != null).ToString());
				
				AppLogger.Debug("Provided entity ID: " + entity.ID.ToString());
				AppLogger.Debug("Existing entity ID: " + (existingEntity == null ? "[null]" : existingEntity.ID.ToString()));
				
				isTaken = (existingEntity != null && !existingEntity.ID.Equals(entity.ID));
				
				AppLogger.Debug("Is taken: " + isTaken);
			}
			
			return !isTaken;
		}
	}
}
