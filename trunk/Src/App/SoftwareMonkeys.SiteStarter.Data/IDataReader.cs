using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of IDataReader.
	/// </summary>
	public interface IDataReader : IDataAdapter
	{
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		IEntity GetEntity(Type type, Dictionary<string, object> parameters);
			
		
        IEntity GetEntity(IDataFilter filter);

        IEntity GetEntity(FilterGroup group);
        
        
        T GetEntity<T>(IDataFilter filter)
        	where T : IEntity;

        T GetEntity<T>(FilterGroup group)
        	where T : IEntity;
        
        T GetEntity<T>(string propertyName, object propertyValue)
        	where T : IEntity;
        
        
		T GetEntity<T>(Dictionary<string, object> parameters)
			where T : IEntity;
		
        IEntity GetEntity(Type type, string propertyName, object propertyValue);
		
		T GetEntityWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
            where T : IEntity;
		
		IEntity GetEntity(IEntity entity);
	}
}
