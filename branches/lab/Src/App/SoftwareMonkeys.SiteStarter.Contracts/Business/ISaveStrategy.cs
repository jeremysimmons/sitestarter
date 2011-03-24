using System;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all save strategies.
	/// </summary>
	public interface ISaveStrategy : IStrategy
	{
		/// <summary>
		/// Gets the validator used to validate the entity before saving.
		/// </summary>
		IValidateStrategy Validator {get;set;}
		
		/// <summary>
		/// Saves the provided entity.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A value indicating whether the entity was valid and was therefore saved.</returns>
		bool Save(IEntity entity);
	}
}
