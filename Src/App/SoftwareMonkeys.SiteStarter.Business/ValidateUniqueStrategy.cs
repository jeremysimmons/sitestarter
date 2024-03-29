﻿using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to validate entity properties to ensure they're unique.
	/// </summary>
	[ValidateStrategy("Unique", "IEntity")]
	public class ValidateUniqueStrategy : BaseValidatePropertyStrategy, IValidateUniqueStrategy
	{		
		public ValidateUniqueStrategy()
		{
		}
		
		/// <summary>
		/// Checks whether the specified property of the provided entity is unique.
		/// </summary>
		/// <param name="entity">The entity to validate by checking whether the provided property value is unique.</param>
		/// <param name="property">The property that is to remain unique.</param>
		/// <param name="attribute">The validate property attribute that caused the validation.</param>
		/// <returns>A value to indicate whether the provided value is unique to the provided property.</returns>
		public override bool IsValid(IEntity entity, PropertyInfo property, IValidatePropertyAttribute attribute)
		{
			bool isTaken = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Validating the provided entity by ensuring the specified property value is unique."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				LogWriter.Debug("Property name: " + property.Name);
				
				LogWriter.Debug("Entity: " + entity.GetType().FullName);
				
				IRetrieveStrategy strategy = RetrieveStrategy.New(entity.ShortTypeName, false);
				
				object propertyValue = property.GetValue(entity, null);
				
				LogWriter.Debug("Property value: " + (propertyValue != null ? propertyValue.ToString() : String.Empty));
				
				IEntity existingEntity = (IEntity)strategy.Retrieve(property.Name, propertyValue);
				
				LogWriter.Debug("Existing entity found: " + (existingEntity != null).ToString());
				
				LogWriter.Debug("Provided entity ID: " + entity.ID.ToString());
				LogWriter.Debug("Existing entity ID: " + (existingEntity == null ? "[null]" : existingEntity.ID.ToString()));
				
				isTaken = (existingEntity != null && !existingEntity.ID.Equals(entity.ID));
				
				LogWriter.Debug("Is taken: " + isTaken);
			}
			
			return !isTaken;
		}
		
	}
}
