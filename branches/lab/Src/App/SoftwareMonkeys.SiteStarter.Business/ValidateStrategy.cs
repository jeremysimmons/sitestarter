using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to validate entities before storing or updating them.
	/// </summary>
	[Strategy("Validate", "IEntity")]
	public class ValidateStrategy : BaseStrategy, IValidateStrategy
	{
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
			bool isValid = true;
			
			using (LogGroup logGroup = LogGroup.Start("Validating the provided entity.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("No validation rules applied.");
			}
			
			return isValid;
		}
		
		static public IValidateStrategy New(string typeName)
		{
			IValidateStrategy strategy = StrategyState.Strategies.Creator.NewValidator(typeName);
			return strategy;
		}
		
		static public IValidateStrategy New(string typeName, bool requireAuthorisation)
		{
			IValidateStrategy strategy = StrategyState.Strategies.Creator.NewValidator(typeName);
			strategy.RequireAuthorisation = requireAuthorisation;
			return strategy;
		}
	}
}
