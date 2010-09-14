using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to save entities.
	/// </summary>
	[Strategy("Save", "IEntity")]
	public class SaveStrategy : BaseStrategy, ISaveStrategy
	{
		private IValidateStrategy validator;
		/// <summary>
		/// Gets/sets the strategy used to ensure entities are valid.
		/// </summary>
		public IValidateStrategy Validator
		{
			get {
				if (validator == null)
					validator = StrategyState.Strategies.Creator.NewValidator(TypeName);
				return validator; }
			set { validator = value; }
		}
		
		public SaveStrategy()
		{
		}
		
		/// <summary>
		/// Saves the provided strategy.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A value indicating whether the entity was valid and was therefore saved.</returns>
		public bool Save(IEntity entity)
		{
			bool saved = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Saving the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				AppLogger.Debug("Entity type: " + entity.GetType().FullName);
				
				if (Validate(entity))
				{
					AppLogger.Debug("Is valid.");
					
					DataAccess.Data.Saver.Save(entity);
					saved = true;
				}
				else
				{
					AppLogger.Debug("Is not valid.");
					
					saved = false;
				}
				
				AppLogger.Debug("Saved: " + saved.ToString());
			}
			return saved;
		}
		
		/// <summary>
		/// Validates the provided entity against any business rules. Should be overridden by derived strategies.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual bool Validate(IEntity entity)
		{
			bool valid = false;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Validating the provided entity.", NLog.LogLevel.Debug))
			{
				if (Validator == null)
					throw new InvalidOperationException("The validation strategy can't be found.");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				AppLogger.Debug("Entity type: " + entity.GetType().FullName);
				
				valid = Validator.Validate(entity);
				
				AppLogger.Debug("Valid: " + valid.ToString());
			}
			return valid;
		}
	}
}
