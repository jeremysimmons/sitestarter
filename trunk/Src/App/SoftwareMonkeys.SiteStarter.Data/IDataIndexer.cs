using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of IDataIndexer.
	/// </summary>
	public interface IDataIndexer : IDataAdapter
	{
		T[] GetEntities<T>()
			where T : IEntity;
		
		T[] GetEntities<T>(string sortExpression)
			where T : IEntity;
		
		T[] GetEntities<T>(string propertyName, object value)
			where T : IEntity;
		
		IEntity[] GetEntities(Type type, Guid[] ids);

		T[] GetEntities<T>(Guid[] ids)
			where T : IEntity;


        IEntity[] GetEntities();

		IEntity[] GetEntities(Type type);

		IEntity[] GetEntities(Type type, Dictionary<string, object> parameters);

		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		T[] GetEntities<T>(Dictionary<string, object> parameters)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		IEntity[] GetEntities(FilterGroup group);
		
		T[] GetPageOfEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID, PagingLocation location, string sortExpression)
			where T : IEntity;
		
		T[] GetPageOfEntities<T>(PagingLocation location, string sortExpression)
			where T : IEntity;
		
		T[] GetPageOfEntities<T>(string fieldName, object fieldValue, PagingLocation location, string sortExpression)
			where T : IEntity;
		
		IEntity[] GetPageOfEntities(Type type, PagingLocation location, string sortExpression);
		
		IEntity[] GetPageOfEntities(Type type, string propertyName, object fieldValue, PagingLocation location, string sortExpression);
		
		IEntity[] GetPageOfEntities(Type type, FilterGroup filterGroup, PagingLocation location, string sortExpression);
		
		T[] GetPageOfEntities<T>(FilterGroup filterGroup, PagingLocation location, string sortExpression)
			where T : IEntity;

		T[] GetEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves all the entities of the specified type with a reference to any of the provided entities.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the reference.</param>
		/// <param name="referencedEntities">An array of entities to check the reference to.</param>
		/// <returns>An array of the references retrieved.</returns>
		T[] GetEntitiesWithReference<T>(string propertyName, IEntity[] referencedEntities)
			where T : IEntity;
		
		IEntity[] GetEntities(Type type, string propertyName, object propertyValue);
		
		IEntity[] GetEntities(Type type, FilterGroup filterGroup, string sortExpression);
		
		T[] GetEntities<T>(FilterGroup filterGroup, string sortExpression)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		IEntity[] GetEntities(IDataFilter filter);
		
		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referenceID">The ID of the referenced entity.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		T[] GetPageOfEntitiesWithReference<T>(string propertyName, IEntity[] referencedEntities, PagingLocation location, string sortExpression)
			where T : IEntity;
	}
}
