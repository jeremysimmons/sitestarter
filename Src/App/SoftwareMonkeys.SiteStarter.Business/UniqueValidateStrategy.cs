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
		private string uniquePropertyName = "UniqueKey";
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
		
		public override bool Validate(IEntity entity)
		{
			return base.Validate(entity) // Run base validation
				&& Validate(entity, UniquePropertyName); // and unique validation
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
			
			using (LogGroup logGroup = LogGroup.Start("Validating the provided entity by ensuring the specified property value is unique.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (propertyName == null || propertyName == String.Empty)
					throw new ArgumentNullException("propertyName");
				
				LogWriter.Debug("Property name: " + propertyName);
				
				LogWriter.Debug("Entity: " + entity.GetType().FullName);
				
				IRetrieveStrategy strategy = RetrieveStrategy.New(entity.ShortTypeName, false);
				
				object propertyValue = EntitiesUtilities.GetPropertyValue(entity, propertyName);
				
				LogWriter.Debug("Property value: " + propertyValue.ToString());
				
				IEntity existingEntity = (IEntity)strategy.Retrieve(propertyName, propertyValue);
				
				LogWriter.Debug("Existing entity found: " + (existingEntity != null).ToString());
				
				LogWriter.Debug("Provided entity ID: " + entity.ID.ToString());
				LogWriter.Debug("Existing entity ID: " + (existingEntity == null ? "[null]" : existingEntity.ID.ToString()));
				
				isTaken = (existingEntity != null && !existingEntity.ID.Equals(entity.ID));
				
				LogWriter.Debug("Is taken: " + isTaken);
			}
			
			return !isTaken;
		}
		
		new static public UniqueValidateStrategy New(IEntity entity)
		{
			return New(entity.ShortTypeName);
		}
		
		new static public UniqueValidateStrategy New(string typeName)
		{
			UniqueValidateStrategy strategy = null;
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new validator strategy.", NLog.LogLevel.Debug))
			{
				strategy = (UniqueValidateStrategy)StrategyState.Strategies.Creator.NewValidator(typeName);
			}
			return strategy;
		}
		
		new static public UniqueValidateStrategy New(IEntity entity, bool requireAuthorisation)
		{
			return New(entity.ShortTypeName, requireAuthorisation);
		}
		
		new static public UniqueValidateStrategy New(string typeName, bool requireAuthorisation)
		{
			UniqueValidateStrategy strategy = null;
			
			using (LogGroup logGroup = LogGroup.Start("Creating a new validator strategy.", NLog.LogLevel.Debug))
			{
				strategy = (UniqueValidateStrategy)StrategyState.Strategies.Creator.NewValidator(typeName);
				
				strategy.RequireAuthorisation = requireAuthorisation;
			}
			return strategy;
		}
	}
}
