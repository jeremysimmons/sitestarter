using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Business.Security;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// A business strategy used for retrieveing a single entity.
	/// </summary>
	[Strategy("Retrieve", typeof(IEntity))]
	public class RetrieveStrategy : BaseStrategy, IRetrieveStrategy, IStrategy
	{
		public RetrieveStrategy()
		{
		}
		
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided unique key.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to retrieve.</param>
		/// <returns>The entity with the provided unique key.</returns>
		public IEntity Retrieve(string uniqueKey)
		{
			IEntity entity = Retrieve("UniqueKey", uniqueKey);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New(TypeName).EnsureAuthorised(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided unique key.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to retrieve.</param>
		/// <returns>The entity with the provided unique key.</returns>
		public T Retrieve<T>(string uniqueKey)
			where T : IEntity
		{
			T entity = (T)Retrieve("UniqueKey", uniqueKey);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New<T>().EnsureAuthorised(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided ID.
		/// </summary>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>The entity with the provided ID.</returns>
		public IEntity Retrieve(Guid entityID)
		{
			IEntity entity = Retrieve("ID", entityID);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New(TypeName).EnsureAuthorised(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided ID.
		/// </summary>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>The entity with the provided ID.</returns>
		public T Retrieve<T>(Guid entityID)
			where T : IEntity
		{
			T entity = (T)Retrieve("ID", entityID);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New<T>().EnsureAuthorised(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="propertyName">The name of the property to match to the provided value.</param>
		/// <param name="value">The name of the value to match to the specified property.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		public IEntity Retrieve(string propertyName, object value)
		{
			IEntity entity = DataAccess.Data.Reader.GetEntity(EntitiesUtilities.GetType(TypeName),
			                                                  propertyName,
			                                                  value);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New(TypeName).EnsureAuthorised(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="propertyName">The name of the property to match to the provided value.</param>
		/// <param name="value">The name of the value to match to the specified property.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		public T Retrieve<T>(string propertyName, object value)
			where T : IEntity
		{
			T entity = DataAccess.Data.Reader.GetEntity<T>(propertyName, value);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New<T>().EnsureAuthorised(entity);
			
			return entity;
		}
		
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="parameters">The parameters to use as filters when retrieving the entities. Corresponds with properties and their values.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		public T Retrieve<T>(Dictionary<string, object> parameters)
			where T : IEntity
		{
			T entity = DataAccess.Data.Reader.GetEntity<T>(parameters);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New<T>().EnsureAuthorised(entity);
			
			return entity;
		}
		
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="parameters">The parameters to use as filters when retrieving the entities. Corresponds with properties and their values.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		public IEntity Retrieve(Dictionary<string, object> parameters)
		{
			IEntity entity = DataAccess.Data.Reader.GetEntity(EntityState.Entities[TypeName].GetEntityType(),
			                                                  parameters);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New(TypeName).EnsureAuthorised(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="type">The type of entity containing the references.</param>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		public IEntity RetrieveWithReference(Type type, string propertyName, string referencedEntityType, Guid referencedEntityID)
		{
			IEntity entity = (IEntity)Reflector.InvokeGenericMethod(this,
			                                                        "RetrieveWithReference",
			                                                        new Type[] {EntitiesUtilities.GetType(referencedEntityType)},
			                                                        new object[] {propertyName, referencedEntityType, referencedEntityID});
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New(type.Name).EnsureAuthorised(entity);
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity with a references that matches the provided parameters.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the references.</param>
		/// <param name="referencedEntityType">The type of the entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the entity being referenced.</param>
		/// <returns>The entity matching the provided parameters.</returns>
		public T RetrieveWithReference<T>(string propertyName, string referencedEntityType, Guid referencedEntityID)
			where T : IEntity
		{
			T entity = (T)DataAccess.Data.Reader.GetEntityWithReference<T>(propertyName, EntitiesUtilities.GetType(referencedEntityType), referencedEntityID);
			
			if (RequireAuthorisation && entity != null)
				AuthoriseRetrieveStrategy.New<T>().EnsureAuthorised(entity);
			
			return entity;
		}
		
		
		#region New functions
		/// <summary>
		/// Creates a new strategy for retrieving the specified type.
		/// </summary>
		static public IRetrieveStrategy New<T>()
		{
			return StrategyState.Strategies.Creator.NewRetriever(typeof(T).Name);
		}
		
		/// <summary>
		/// Creates a new strategy for retrieving the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		static public IRetrieveStrategy New(string typeName)
		{
			return StrategyState.Strategies.Creator.NewRetriever(typeName);
		}
		
		/// <summary>
		/// Creates a new strategy for retrieving the specified type.
		/// </summary>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IRetrieveStrategy New<T>(bool requiresAuthorisation)
		{
			IRetrieveStrategy strategy = StrategyState.Strategies.Creator.NewRetriever(typeof(T).Name);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		
		/// <summary>
		/// Creates a new strategy for retrieving the specified type.
		/// </summary>
		/// <param name="typeName">The short name of the type involved in the strategy.</param>
		/// <param name="requiresAuthorisation">A value indicating whether the strategy requires authorisation.</param>
		static public IRetrieveStrategy New(string typeName, bool requiresAuthorisation)
		{
			IRetrieveStrategy strategy = StrategyState.Strategies.Creator.NewRetriever(typeName);
			strategy.RequireAuthorisation = requiresAuthorisation;
			return strategy;
		}
		#endregion
	}
}
