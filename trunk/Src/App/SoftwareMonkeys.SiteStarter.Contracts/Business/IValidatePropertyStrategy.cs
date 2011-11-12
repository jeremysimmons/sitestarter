using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all property validation strategies.
	/// </summary>
	public interface IValidatePropertyStrategy : IStrategy
	{
		/*/// <summary>
		/// Validates the provided entity by ensuring that the value of the specified property is valid.
		/// </summary>
		/// <param name="entity">The entity to validate.</param>
		/// <param name="property">The property to validate.</param>
		/// <param name="attribute"></param>
		bool Validate(IEntity entity, PropertyInfo property, IValidatePropertyAttribute attribute);*/
		
		/// <summary>
		/// Checks whether the specified property is valid.
		/// </summary>
		/// <param name="entity">The entity to validate.</param>
		/// <param name="property">The property to validate.</param>
		/// <param name="property">The property to validate.</param>
		/// <returns>A value indicating whether the property is valid.</returns>
		bool IsValid(IEntity entity, PropertyInfo property, IValidatePropertyAttribute attribute);
	}
}
