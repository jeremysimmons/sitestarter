using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Business.Security;

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
		/// Updates the provided strategy.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <returns>A value indicating whether the entity was valid and was therefore saved.</returns>
		public bool Update(IEntity entity)
		{
			if (RequireAuthorisation)
				AuthoriseUpdateStrategy.New(entity.ShortTypeName).EnsureAuthorised(entity);
			
			if (Validate(entity))
			{
				DataAccess.Data.Updater.Update(entity);
				return true;
			}
			else
				return false;
		}
		
		/// <summary>
		/// Validates the provided entity against any business rules. Should be overridden by derived strategies.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public virtual bool Validate(IEntity entity)
		{
			bool valid = false;
			
			if (Validator == null)
				throw new InvalidOperationException("The validation strategy can't be found.");
			
			valid = Validator.Validate(entity);
			
			return valid;
		}
		
		
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		static public IUpdateStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.NewUpdater(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IUpdateStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.NewUpdater(typeName);
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IUpdateStrategy New<T>(bool requiresAuthorisation)
		{
			IUpdateStrategy strategy = StrategyState.Strategies.Creator.NewUpdater(typeof(T).Name);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for updating the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IUpdateStrategy New(string typeName, bool requiresAuthorisation)
		{
			IUpdateStrategy strategy = StrategyState.Strategies.Creator.NewUpdater(typeName);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		#endregion
	}
}
