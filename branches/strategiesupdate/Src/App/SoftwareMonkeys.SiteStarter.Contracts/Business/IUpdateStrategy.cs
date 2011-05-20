using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all update strategies.
	/// </summary>
	public interface IUpdateStrategy : IStrategy
	{
		/// <summary>
		/// Gets the validator used to validate the entity before saving.
		/// </summary>
		IValidateStrategy Validator {get;}
		
		/// <summary>
		/// Updates the provided entity.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <returns>A value indicating whether the entity was valid and was therefore updated.</returns>
		bool Update(IEntity entity);
	}
}
