using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business;
using SoftwareMonkeys.SiteStarter.Business.Security;
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
		/// Saves the provided entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A value indicating whether the entity was valid and was therefore saved.</returns>
		public virtual bool Save(IEntity entity)
		{
			bool saved = false;
			using (LogGroup logGroup = LogGroup.Start("Saving the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Entity type: " + entity.GetType().FullName);
				
				if (RequireAuthorisation)
					AuthoriseSaveStrategy.New(entity.ShortTypeName).EnsureAuthorised(entity);
				
				if (Validate(entity))
				{
					LogWriter.Debug("Is valid.");
					
					DataAccess.Data.Saver.Save(entity);
					saved = true;
				}
				else
				{
					LogWriter.Debug("Is not valid.");
					
					saved = false;
				}
				
				LogWriter.Debug("Saved: " + saved.ToString());
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
			
			using (LogGroup logGroup = LogGroup.Start("Validating the provided entity.", NLog.LogLevel.Debug))
			{
				if (Validator == null)
					throw new InvalidOperationException("The validation strategy can't be found.");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Entity type: " + entity.GetType().FullName);
				
				valid = Validator.Validate(entity);
				
				LogWriter.Debug("Valid: " + valid.ToString());
			}
			return valid;
		}
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for saving the specified type.
		/// </summary>
		static public ISaveStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.NewSaver(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for saving the provided type.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A save strategy for the provided type.</returns>
		static public ISaveStrategy New(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			return StrategyState.Strategies.Creator.NewSaver(entity.ShortTypeName);
		}
		
		/// <summary>
		/// Creates a new strategy for saving the provided type.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="requireAuthorisation"></param>
		/// <returns>A save strategy for the provided type.</returns>
		static public ISaveStrategy New(IEntity entity, bool requireAuthorisation)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			ISaveStrategy strategy = StrategyState.Strategies.Creator.NewSaver(entity.ShortTypeName);
			strategy.RequireAuthorisation = requireAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for saving the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public ISaveStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.NewSaver(typeName);
		}
		
		/// <summary>
		/// Creates a new strategy for saving the specified type.
		/// </summary>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public ISaveStrategy New<T>(bool requiresAuthorisation)
		{
			ISaveStrategy strategy = StrategyState.Strategies.Creator.NewSaver(typeof(T).Name);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for saving the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public ISaveStrategy New(string typeName, bool requiresAuthorisation)
		{
			ISaveStrategy strategy = StrategyState.Strategies.Creator.NewSaver(typeName);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		#endregion
	}
}
