using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataIndexer.
	/// </summary>
	public abstract class DataIndexer : DataAdapter, IDataIndexer
	{		
		public abstract IEntity[] GetEntities(Type type, Guid[] ids);

		public abstract T[] GetEntities<T>(Guid[] ids)
			where T : IEntity;

        public abstract IEntity[] GetEntities();

		public abstract T[] GetEntities<T>()
			where T : IEntity;
		
		public abstract IEntity[] GetEntities(Type type);

		public abstract IEntity[] GetEntities(Type type, IDictionary<string, object> parameters);

		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public abstract T[] GetEntities<T>(IDictionary<string, object> parameters)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public abstract IEntity[] GetEntities(FilterGroup group);
		
		public abstract T[] GetPageOfEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID, PagingLocation location, string sortExpression)
			where T : IEntity;
		
		public abstract T[] GetPageOfEntities<T>(PagingLocation location, string sortExpression)
			where T : IEntity;
		
		public abstract T[] GetPageOfEntities<T>(string fieldName, object fieldValue, PagingLocation location, string sortExpression)
			where T : IEntity;
		
		public abstract IEntity[] GetPageOfEntities(Type type, PagingLocation location, string sortExpression);
		
		public abstract IEntity[] GetPageOfEntities(Type type, string fieldName, object fieldValue, PagingLocation location, string sortExpression);

		public abstract T[] GetEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
			where T : IEntity;
		
		public abstract IEntity[] GetEntities(Type type, string propertyName, object propertyValue);
		
		public abstract T[] GetEntities<T>(string propertyName, object propertyValue)
			where T : IEntity;
		
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public abstract IEntity[] GetEntities(IDataFilter filter);


	}
}
