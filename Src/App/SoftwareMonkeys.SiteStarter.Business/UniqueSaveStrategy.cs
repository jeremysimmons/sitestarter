using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Business;

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
		/// Gets/sets the validation strategy uniqued to ensure entities are unique.
		/// </summary>
		public UniqueValidateStrategy UniqueValidator
		{
			get {
				if (uniqueValidator == null)
					uniqueValidator = StrategyState.Strategies["ValidateUnique", "IEntity"].New<UniqueValidateStrategy>();
				return uniqueValidator; }
			set { uniqueValidator = value; }
		}
		
		/// <summary>
		/// Saves the provided entity if it's unique.
		/// </summary>
		/// <param name="entity">The entity to save if it's unique.</param>
		/// <returns>A value indicating whether the provided entity was save, and therefore must have been unique.</returns>
		public bool Save(IEntity entity)
		{
			if (Validate(entity))
			{
				base.Save(entity);
				return true;
			}
			else
				return false;
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
