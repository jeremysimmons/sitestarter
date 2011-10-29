using System;
using System.Collections.ObjectModel;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Business.Security
{
	/// <summary>
	/// Defines the interface of all authorise strategies.
	/// </summary>
	public interface IAuthoriseStrategy : IStrategy
	{
		/// <summary>
		/// Checks whether the current user is authorised to perform an operation involving an entity of the specified type.
		/// </summary>
		/// <param name="shortTypeName">The type of entity involved in the operation in which authorisation is required.</param>
		/// <returns>A value indicating whether the current user is authorised to perform an operation involving an entity of the specified type.</returns>
		bool IsAuthorised(string shortTypeName);
		
		/// <summary>
		/// Checks whether the current user is authorised to perform an operation involving the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the operation in which authorisation is required.</param>
		/// <returns>A value indicating whether the current user is authorised to perform an operation involving the provided entity.</returns>
		bool IsAuthorised(IEntity entity);
		
		/// <summary>
		/// Ensures that the current user is authorised to perform an operation involving an entity of the specified type and throws an exception if unauthorised.
		/// </summary>
		/// <param name="shortTypeName">The type of entity involved in the operation in which authorisation is required.</param>
		void EnsureAuthorised(string shortTypeName);
		
		/// <summary>
		/// Checks whether the current user is authorised to perform an operation involving the provided entity.
		/// </summary>
		/// <param name="entity">The entity involved in the operation in which authorisation is required.</param>
		/// <returns>The authorised entity. (Or null if not authorised)</returns>
		IEntity Authorise(IEntity entity);
		
		/// <summary>
		/// Ensures that the current user is authorised to perform an operation involving the provided entity and throws an exception if unauthorised.
		/// </summary>
		/// <param name="entity">The entity involved in the operation in which authorisation is being ensured.</param>
		void EnsureAuthorised(IEntity entity);
		
	}
}
