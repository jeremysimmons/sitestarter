using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;

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
		/// <param name="type">The type of entity being retrieved.</param>
		/// <param name="uniqueKey">The unique key of the entity to retrieve.</param>
		/// <returns>The entity with the provided unique key.</returns>
		public IEntity Retrieve(Type type, string uniqueKey)
		{
			return Retrieve(type, "UniqueKey", uniqueKey);
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided unique key.
		/// </summary>
		/// <param name="uniqueKey">The unique key of the entity to retrieve.</param>
		/// <returns>The entity with the provided unique key.</returns>
		public T Retrieve<T>(string uniqueKey)
			where T : IEntity
		{
			return (T)Retrieve(typeof(T), "UniqueKey", uniqueKey);
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided ID.
		/// </summary>
		/// <param name="type">The type of entity being retrieved.</param>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>The entity with the provided ID.</returns>
		public IEntity Retrieve(Type type, Guid entityID)
		{
			return Retrieve(type, "ID", entityID);
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the provided ID.
		/// </summary>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>The entity with the provided ID.</returns>
		public T Retrieve<T>(Guid entityID)
			where T : IEntity
		{
			return (T)Retrieve(typeof(T), "ID", entityID);
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="type">The type of entity being retrieved.</param>
		/// <param name="propertyName">The name of the property to match to the provided value.</param>
		/// <param name="value">The name of the value to match to the specified property.</param>
		/// <returns>The entity with the specified property matching the provided value.</returns>
		public IEntity Retrieve(Type type, string propertyName, object value)
		{
			return DataAccess.Data.Reader.GetEntity(type, propertyName, value);
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
			return (T)Retrieve(typeof(T), propertyName, value);
		}
	}
}
