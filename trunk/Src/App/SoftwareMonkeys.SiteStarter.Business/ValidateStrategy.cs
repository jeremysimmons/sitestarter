using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used to validate entities before storing or updating them.
	/// </summary>
	[Strategy("Validate", "IEntity")]
	public class ValidateStrategy : IValidateStrategy
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
			return true;
		}
	}
}
