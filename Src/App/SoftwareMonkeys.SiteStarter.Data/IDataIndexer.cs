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
		T[] GetEntities<T>(string propertyName, object value)
			where T : IEntity;
		
		IEntity[] GetEntities(Type type, Guid[] ids);

		T[] GetEntities<T>(Guid[] ids)
			where T : IEntity;

		T[] GetEntities<T>()
			where T : IEntity;

        IEntity[] GetEntities();

		IEntity[] GetEntities(Type type);

		IEntity[] GetEntities(Type type, IDictionary<string, object> parameters);

		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		T[] GetEntities<T>(IDictionary<string, object> parameters)
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

		T[] GetEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
			where T : IEntity;
		
		IEntity[] GetEntities(Type type, string propertyName, object propertyValue);
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		IEntity[] GetEntities(IDataFilter filter);
		
		
		
	}
}
