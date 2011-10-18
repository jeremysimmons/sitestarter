using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface for data indexer adapters.
	/// </summary>
	public interface IDataCounter : IDataAdapter
	{
		int CountEntities<T>()
			where T : IEntity;
				
		int CountEntities<T>(string propertyName, object value)
			where T : IEntity;
		
		int CountEntities(Type type, Guid[] ids);

		int CountEntities<T>(Guid[] ids)
			where T : IEntity;


        int CountEntities();

		int CountEntities(Type type);

		int CountEntities(Type type, Dictionary<string, object> parameters);

		/// <summary>
		/// Counts all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		int CountEntities<T>(Dictionary<string, object> parameters)
			where T : IEntity;
		
		/// <summary>
		/// Counts all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The total number of entities found.</returns>
		int CountEntities(IDataFilterGroup group);
		
		int CountEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
			where T : IEntity;
		
		/// <summary>
		/// Counts all the entities of the specified type with a reference to any of the provided entities.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the reference.</param>
		/// <param name="referencedEntities">An array of entities to check the reference to.</param>
		/// <returns>The total number of entities found.</returns>
		int CountEntitiesWithReference<T>(string propertyName, IEntity[] referencedEntities)
			where T : IEntity;
		
		int CountEntities(Type type, string propertyName, object propertyValue);
		
		int CountEntities(Type type, IDataFilterGroup filterGroup);
		
		int CountEntities<T>(IDataFilterGroup filterGroup)
			where T : IEntity;
		
		/// <summary>
		/// Counts all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The total number of entities found.</returns>
		int CountEntities(IDataFilter filter);
	}
}
