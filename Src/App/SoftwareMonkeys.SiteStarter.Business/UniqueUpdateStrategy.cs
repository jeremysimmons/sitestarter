﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to update entities while enforcing a strict unique field/property rule.
	/// </summary>
	[Strategy("Update", "IUniqueEntity")]
	public class UniqueUpdateStrategy : UpdateStrategy, IUniqueUpdateStrategy
	{
		private string uniquePropertyName;
		/// <summary>
		/// Gets/sets the name of the unique property on the entity being updated.
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
			
			if (UniqueValidator == null)
				throw new InvalidOperationException("The validation strategy can't be found. Check the Strategy attribute on the validation strategy class.");
			
			if (UniquePropertyName == null || UniquePropertyName == String.Empty)
				throw new InvalidOperationException("The UniquePropertyName property hasn't been set.");
			
			valid = UniqueValidator.Validate(entity, UniquePropertyName);
			
			return valid;
		}
	}
}