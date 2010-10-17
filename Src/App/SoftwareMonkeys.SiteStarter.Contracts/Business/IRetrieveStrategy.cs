using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// Defines the basic interface of all retriever strategy components, which are used to execute many variations of business logic, while dealing with other components.
	/// </summary>
	public interface IRetrieveStrategy : IStrategy
	{
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided unique key.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to retrieve.</param>
		/// <returns>The entity with the provided unique key.</returns>
		T Retrieve<T>(string uniqueKey)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided ID.
		/// </summary>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>The entity with the provided ID.</returns>
		T Retrieve<T>(Guid entityID)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="propertyName">The name of the property to match to the provided value.</param>
		/// <param name="value">The name of the value to match to the specified property.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		T Retrieve<T>(string propertyName, object value)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided unique key.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="uniqueKey">The unique key of the entity to retrieve.</param>
		/// <returns>The entity with the provided unique key.</returns>
		IEntity Retrieve(Type type, string uniqueKey);
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided ID.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>The entity with the provided ID.</returns>
		IEntity Retrieve(Type type, Guid entityID);
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="propertyName">The name of the property to match to the provided value.</param>
		/// <param name="value">The name of the value to match to the specified property.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		IEntity Retrieve(Type type, string propertyName, object value);
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="parameters">The parameters to use as filters when retrieving the entities. Corresponds with properties and their values.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		IEntity Retrieve(Type type, Dictionary<string, object> parameters);
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="parameters">The parameters to use as filters when retrieving the entities. Corresponds with properties and their values.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		T Retrieve<T>(Dictionary<string, object> parameters)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="type">The type of entity containing the references.</param>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		IEntity RetrieveWithReference(Type type, string propertyName, string referencedEntityType, Guid referencedEntityID);
		
		/// <summary>
		/// Retrieves the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		T RetrieveWithReference<T>(string propertyName, string referencedEntityType, Guid referencedEntityID)
			where T : IEntity;
		
	}
}
