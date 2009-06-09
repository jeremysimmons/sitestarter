using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Defines the interface required for all data providers.
    /// </summary>
    public abstract class DataProvider : ProviderBase
    {
        public abstract DataStoreCollection Stores
        { get; }

       // public abstract void Initialize(string name, NameValueCollection settings);

       // void Dispose();

        public abstract IDataStore InitializeDataStore(string dataStoreName);
        //public abstract string GetDataStoreName(Type objectType);
        public abstract string[] GetDataStoreNames();

        public abstract IDataFilter CreateFilter(Type baseType);

	#region Data access functions
        /// <summary>
        /// Retrieves all the entities of the specified type from the data store.
        /// </summary>
	/// <param name="filter">The filter to apply to the query.</param>
        /// <returns>The entities of the specified type found in the data store.</returns>
        public abstract IEntity[] GetEntities(IDataFilter filter);

        /// <summary>
        /// Retrieves all the entities of the specified type from the data store.
        /// </summary>
	/// <param name="group">The group of filters to apply to the query.</param>
        /// <returns>The entities of the specified type found in the data store.</returns>
        public abstract IEntity[] GetEntities(FilterGroup group);
        
        public abstract IEntity[] GetEntities(Type type);
        
        public abstract IEntity[] GetEntities(Type type, Guid[] ids);
        public abstract T[] GetEntities<T>(Guid[] ids)
        	where T : IEntity;
        
        public abstract T[] GetEntities<T>()
        	where T : IEntity;

        public abstract T[] GetEntities<T>(string propertyName, object propertyValue)
        	where T : IEntity;
        public abstract T GetEntity<T>(string propertyName, object propertyValue)
        	where T : IEntity;
        
        public abstract IEntity[] GetEntities(Type type, string propertyName, object propertyValue);
        public abstract IEntity GetEntity(Type type, string propertyName, object propertyValue);


        /// <summary>
        /// Retrieves all the entities with references to the one provided.
        /// </summary>
	/// <param name="entity">The entity to retrieve the reverse referenced entities for.</param>
	/// <param name="property">The property which mirrors the reverse references being identified.</param>
        /// <returns>The entities containing references to the one provided.</returns>
        public abstract IEntity[] GetEntitiesContainingReverseReferences(IEntity entity, PropertyInfo property);

	public abstract void Save(IEntity entity);
	public abstract void Update(IEntity entity);
	public abstract void Delete(IEntity entity);
	#endregion
    }
}
