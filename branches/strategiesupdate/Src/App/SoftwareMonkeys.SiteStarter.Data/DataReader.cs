using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataReader.
	/// </summary>
	public abstract class DataReader : DataAdapter, IDataReader
	{
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public abstract IEntity GetEntity(Type type, Dictionary<string, object> parameters);
			
		
        public abstract IEntity GetEntity(IDataFilter filter);

        public abstract IEntity GetEntity(FilterGroup group);
        
        
        public abstract T GetEntity<T>(IDataFilter filter)
        	where T : IEntity;

        public abstract T GetEntity<T>(FilterGroup group)
        	where T : IEntity;
        
        public abstract T GetEntity<T>(string propertyName, object propertyValue)
        	where T : IEntity;
		
        public abstract IEntity GetEntity(Type type, string propertyName, object propertyValue);
		
		public abstract T GetEntityWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
            where T : IEntity;
		
		
		public abstract T GetEntity<T>(Dictionary<string, object> parameters)
			where T : IEntity;
		
		
		public abstract IEntity GetEntity(IEntity entity);
		
	}
}
