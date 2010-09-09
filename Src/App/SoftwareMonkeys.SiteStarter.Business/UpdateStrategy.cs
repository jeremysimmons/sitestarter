using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

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
					validator = StrategyState.Strategies.Creator.NewValidator(typeof(IEntity).Name);
				return validator; }
			set { validator = value; }
		}
		
		public UpdateStrategy()
		{
		}
		
		/// <summary>
		/// Updates the provided strategy.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <returns>A value indicating whether the entity was valid and was therefore saved.</returns>
		public bool Update(IEntity entity)
		{
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
	}
}
