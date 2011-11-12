using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all validation strategies.
	/// </summary>
	public interface IValidateStrategy : IStrategy
	{
		/// <summary>
		/// Validates the provided entity and prepares a list of failures (if applicable).
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool Validate(IEntity entity);
		
		/// <summary>
		/// Holds a list of property names and validation attributes that have failed validation.
		/// </summary>
		Dictionary<string, IValidatePropertyAttribute> Failures { get; }
		
		/// <summary>
		/// Adds the provided property name and validation attribute type to the list of validation failures.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="attribute"></param>
		void AddFailure(string propertyName, IValidatePropertyAttribute attribute);
		
		/// <summary>
		/// Retrieves all potential validation failures for the provided entity. This allows potential failures to be checked against validation messages
		/// before validation occurs to ensure no messages are missing.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		Dictionary<string, IValidatePropertyAttribute> GetPotentialFailures(IEntity entity);
	}
}
