using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business.Security;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to update entities.
	/// </summary>
	[Strategy("Update", "IEntity")]
	public class UpdateStrategy : BaseStrategy, IUpdateStrategy
	{
		private IValidateStrategy validator;
		/// <summary>
		/// Gets/sets the strategy used to ensure entities are valid.
		/// </summary>
		public IValidateStrategy Validator
		{
			get {
				if (validator == null)
				{
					validator = StrategyState.Strategies.Creator.NewValidator(TypeName);
					validator.RequireAuthorisation = RequireAuthorisation;
				}
				return validator; }
			set { validator = value; }
		}
		
		public UpdateStrategy()
		{
		}
		
		public UpdateStrategy(string typeName) : base(typeName)
		{}
		
		/// <summary>
		/// Updates the provided entity.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <returns>A value indicating whether the entity was valid and was therefore saved.</returns>
		public virtual bool Update(IEntity entity)
		{
			bool didSucceed = false;
			
			using (LogGroup logGrop = LogGroup.Start("Updating the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Type: " + entity.ShortTypeName);
				
				if (RequireAuthorisation)
					AuthoriseUpdateStrategy.New(entity.ShortTypeName).EnsureAuthorised(entity);
				
				if (Validate(entity))
				{
					DataAccess.Data.Updater.Update(entity);
					didSucceed = true;
				}
				else
					didSucceed = false;
				
				LogWriter.Debug("Did succeed: " + didSucceed);
			}
			return didSucceed;
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
				
				valid = Validator.Validate(entity);
				
				LogWriter.Debug("Is valid: " + valid.ToString());
			}
			return valid;
		}
		
		
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for updating the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the strategy.</param>
		static public IUpdateStrategy New(IEntity entity)
		{
			return New(entity.ShortTypeName);
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		static public IUpdateStrategy New<T>()
		{
			return New(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IUpdateStrategy New(string typeName)
		{
			IUpdateStrategy strategy = null;
			using (LogGroup logGroup = LogGroup.Start("Creating new update strategy.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type name: " + typeName);
				strategy = StrategyState.Strategies.Creator.NewUpdater(typeName);
			}
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IUpdateStrategy New<T>(bool requiresAuthorisation)
		{
			IUpdateStrategy strategy = null;
			using (LogGroup logGroup = LogGroup.Start("Creating new update strategy.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type name: " + typeof(T).Name);
				strategy = StrategyState.Strategies.Creator.NewUpdater(typeof(T).Name);
				strategy.RequireAuthorisation = requiresAuthorisation;
			}
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IUpdateStrategy New(string typeName, bool requiresAuthorisation)
		{
			IUpdateStrategy strategy = null;
			using (LogGroup logGroup = LogGroup.Start("Creating new update strategy.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type name: " + typeName);
				strategy = StrategyState.Strategies.Creator.NewUpdater(typeName);
				strategy.RequireAuthorisation = requiresAuthorisation;
			}
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IUpdateStrategy New(IEntity entity, bool requiresAuthorisation)
		{
			IUpdateStrategy strategy = null;
			using (LogGroup logGroup = LogGroup.Start("Creating new update strategy.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Type name: " + entity.ShortTypeName);
				strategy = StrategyState.Strategies.Creator.NewUpdater(entity.ShortTypeName);
				strategy.RequireAuthorisation = requiresAuthorisation;
			}
			return strategy;
		}
		#endregion
	}
}
