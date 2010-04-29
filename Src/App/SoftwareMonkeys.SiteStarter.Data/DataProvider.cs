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
        
        public abstract bool IsStored(IEntity entity);
        
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
        
          
        public abstract IEntity GetEntity(IDataFilter filter);

        public abstract IEntity GetEntity(FilterGroup group);
        
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


        public abstract T GetEntityMatchReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
            where T : IEntity;
        public abstract T[] GetEntitiesMatchReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
        	where T : IEntity;

        public abstract T[] GetEntitiesPage<T>(int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        	where T : IEntity;
        public abstract T[] GetEntitiesPage<T>(string fieldName, object fieldValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        	where T : IEntity;
        
        public abstract IEntity[] GetEntitiesPage(Type type, int pageIndex, int pageSize, string sortExpression, out int totalObjects);
        public abstract IEntity[] GetEntitiesPage(Type type, string fieldName, object fieldValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects);

        public abstract T[] GetEntitiesPageMatchReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        	where T : IEntity;
        
        /// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public abstract IEntity[] GetEntities(Type type, IDictionary<string, object> parameters);
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public abstract IEntity GetEntity(Type type, IDictionary<string, object> parameters);
			
        /// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public abstract T[] GetEntities<T>(IDictionary<string, object> parameters)
			where T : IEntity;
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public abstract T GetEntity<T>(IDictionary<string, object> parameters)
			where T : IEntity;

        /// <summary>
        /// Retrieves all the references to the entities provided.
        /// </summary>
        /// <returns>The references to the entities provided.</returns>
        public abstract EntityReferenceCollection GetReferences(Type entityType, Guid entityID, string propertyName, Type referenceType, bool fullActivation);
        
        /// <summary>
        /// Retrieves the reference from the specified entity to the entity matching the specified type and the specified ID.
        /// </summary>
        /// <returns>The reference matching the parameters.</returns>
        public abstract EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll);
        
        public abstract EntityReferenceCollection GetObsoleteReferences(IEntity entity, string propertyName, Type referenceType, Guid[] idsOfEntitiesToKeep);
        	
        public abstract EntityReferenceCollection GetObsoleteReferences(IEntity entity, Guid[] idsOfEntitiesToKeep);
        	

	public abstract void Save(IEntity entity);
	public abstract void Update(IEntity entity);
	public abstract void Delete(IEntity entity);
	
	
	public abstract void Activate(IEntity[] entity);
	public abstract void Activate(IEntity[] entity, int depth);
	public abstract void Activate(IEntity entity);
	public abstract void Activate(IEntity entity, int depth);
	public abstract void Activate(IEntity entity, string propertyName);
	public abstract void Activate(IEntity entity, string propertyName, int depth);
	public abstract void Activate(IEntity entity, string propertyName, Type propertyType);
	public abstract void Activate(IEntity entity, string propertyName, Type propertyType, int depth);
	public abstract void ActivateReference(EntityReference reference);
	
	public abstract bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID, string mirrorPropertyName);
	public abstract bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID);
	#endregion
    }
}
