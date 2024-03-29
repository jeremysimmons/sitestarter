﻿using System;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to validate entities before storing or updating them.
	/// </summary>
	[Strategy("Validate", "IEntity")]
	[Serializable]
	public class ValidateStrategy : BaseStrategy, IValidateStrategy
	{
		private Dictionary<string, IValidatePropertyAttribute> failures;
		/// <summary>
		/// Gets/sets the properties and validation attributes that have failed validation.
		/// </summary>
		public Dictionary<string, IValidatePropertyAttribute> Failures
		{
			get {
				if (failures == null)
					failures = new Dictionary<string, IValidatePropertyAttribute>();
				return failures; }
			set { failures = value; }
		}
		
		/// <summary>
		/// Adds the provided property name and attribute type to the list of validation failures.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="attribute"></param>
		public void AddFailure(string propertyName, IValidatePropertyAttribute attribute)
		{
			AddFailure(propertyName, attribute, Failures);
		}
		
		
		/// <summary>
		/// Adds the provided property name and attribute type to the list of validation failures.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="attribute"></param>
		public void AddFailure(string propertyName, IValidatePropertyAttribute attribute, Dictionary<string, IValidatePropertyAttribute> failures)
		{
			failures[propertyName + "_" + attribute.ValidatorName] = attribute;
		}
		
		public ValidateStrategy()
		{
		}
		
		/// <summary>
		/// Validates the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual bool Validate(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Validating the provided entity."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				// Clear old failures to start fresh
				Failures.Clear();
				
				foreach (PropertyInfo property in entity.GetType().GetProperties())
				{
					ValidateProperty(entity, property);
				}
			}
			
			return Failures.Count == 0;
		}
		
		
		public virtual void ValidateProperty(IEntity entity, PropertyInfo property)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Validating the provided '" + property.Name + "' property on the provided '" + entity.ShortTypeName + "' entity."))
			{
				foreach (IValidatePropertyAttribute attribute in GetValidationAttributes(entity, property))
				{
					using (LogGroup logGroup2 = LogGroup.StartDebug("Applying validation according to '" + attribute.ValidatorName + "' validation attribute."))
					{
						string name = attribute.ValidatorName;
						
						if (name == null || name == String.Empty)
							// TODO: Create and use a specific exception
							throw new Exception("No validor name has been set on the validation attribute of the '" + property.Name + "' property on the '" + entity.ShortTypeName + "' type.");
						
						string action = name;
						// If the action doesn't include the "Validate" prefix then add it
						if (action.IndexOf("Validate") != 0)
							action = "Validate" + action;
						
						LogWriter.Debug("Validation strategy action: " + action);
						
						IValidatePropertyStrategy strategy = StrategyState.Strategies.Creator.New<IValidatePropertyStrategy>(action, entity.GetType());
						
						bool isValid = strategy.IsValid(entity, property, attribute);
						
						LogWriter.Debug("Is valid: " + isValid);
						
						if (!isValid)
							AddFailure(property.Name, attribute);
					}
				}
			}
		}
		
		public virtual IValidatePropertyAttribute[] GetValidationAttributes(IEntity entity, PropertyInfo property)
		{
			List<IValidatePropertyAttribute> list = new List<IValidatePropertyAttribute>();
			
			foreach (Attribute attribute in property.GetCustomAttributes(typeof(IValidatePropertyAttribute), true))
			{
				if (attribute is IValidatePropertyAttribute)
					list.Add((IValidatePropertyAttribute)attribute);
			}
			
			return list.ToArray();
		}
		
		/// <summary>
		/// Retrieves all potential validation failures for the provided entity. This allows potential failures to be checked against validation messages
		/// before validation occurs to ensure no messages are missing.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public Dictionary<string, IValidatePropertyAttribute> GetPotentialFailures(IEntity entity)
		{
			Dictionary<string, IValidatePropertyAttribute> dict = new Dictionary<string, IValidatePropertyAttribute>();
			
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				IValidatePropertyAttribute[] attributes = GetValidationAttributes(entity, property);
				foreach (IValidatePropertyAttribute attribute in attributes)
					AddFailure(property.Name, attribute, dict);
			}
			
			return dict;
		}
		
		static public IValidateStrategy New(IEntity entity)
		{
			IValidateStrategy strategy = StrategyState.Strategies.Creator.NewValidator(entity.GetType());
			return strategy;
		}
		
	}
}
