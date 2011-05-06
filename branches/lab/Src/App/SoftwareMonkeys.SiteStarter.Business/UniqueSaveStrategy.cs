﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to save entities while enforcing a strict unique field/property rule.
	/// </summary>
	[Strategy("Save", "IUniqueEntity")]
	public class UniqueSaveStrategy : SaveStrategy, IUniqueSaveStrategy
	{
		private string uniquePropertyName;
		/// <summary>
		/// Gets/sets the name of the unique property on the entity being saved.
		/// </summary>
		public string UniquePropertyName
		{
			get {
				if (uniquePropertyName == null || uniquePropertyName == String.Empty)
				{
					uniquePropertyName = UniqueValidator.UniquePropertyName;
					
					if (uniquePropertyName == null || uniquePropertyName == String.Empty)
						uniquePropertyName = "UniqueKey";
				}
				return uniquePropertyName; }
			set { uniquePropertyName = value; }
		}
		
		private UniqueValidateStrategy uniqueValidator;
		/// <summary>
		/// Gets/sets the validation strategy used to ensure entities are unique.
		/// </summary>
		public UniqueValidateStrategy UniqueValidator
		{
			get {
				if (uniqueValidator == null)
					uniqueValidator = UniqueValidateStrategy.New(TypeName);
				return uniqueValidator; }
			set { uniqueValidator = value; }
		}
		
		/// <summary>
		/// Validates the provided entity by checking to see if the specified property of the provided entity is unique.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public override bool Validate(SoftwareMonkeys.SiteStarter.Entities.IEntity entity)
		{
			bool valid = false;
			
			using (LogGroup logGroup = LogGroup.Start("Validating the provided entity.", NLog.LogLevel.Debug))
			{				
				if (UniqueValidator == null)
					throw new InvalidOperationException("The validation strategy can't be found. Check the Strategy attribute on the validation strategy class.");
				
				if (UniquePropertyName == null || UniquePropertyName == String.Empty)
					throw new InvalidOperationException("The UniquePropertyName property hasn't been set.");
				
				LogWriter.Debug("Entity type: " + entity.GetType().FullName);
				
				LogWriter.Debug("Unique property name: " + UniquePropertyName);
				
				valid = UniqueValidator.Validate(entity, UniquePropertyName);
				
			}
			return valid;
		}
	}
}