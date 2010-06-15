using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
    /// <summary>
    /// Defines the interface for all data stores.
    /// </summary>
    public interface IDataStore
    {
        #region Properties
        string Name { get; }
        #endregion

        #region Initialize/open/close/dispose functions
        void Open();
        void Dispose();
        void Close();
        #endregion

        #region C.R.U.D. and related functions
        IEntity[] GetAllEntities();
        T[] GetEntities<T>()
        	where T : IEntity;
	// TODO: Remove if not necessary
        //IEntity[] GetEntities(BaseFilter filter);
        //IEntity[] GetEntities(FilterGroup group);
        IEntity[] GetEntities(Type type, Guid[] entityIDs);
        T[] GetEntities<T>(Guid[] entityIDs)
        	where T : IEntity;
        //IEntity GetEntity(Type entityType, string propertyName, object propertyValue);
        IEntity GetEntityByTypeAndProperties(Type type, IDictionary<string, object> parameters);
        T GetEntity<T>(IDictionary<string, object> parameters)
        	where T : IEntity;
        
        IEntity[] GetEntitiesByTypeAndProperties(Type type, IDictionary<string, object> parameters);
        T[] GetEntities<T>(IDictionary<string, object> parameters)
        	where T : IEntity;
        
        
        IEntity[] GetEntitiesByTypeAndProperty(Type type, string propertyName, object propertyValue);
        T[] GetEntities<T>(string propertyName, object propertyValue)
        	where T : IEntity;
        IEntity GetEntityByTypeAndProperty(Type type, string propertyName, object propertyValue);
        T GetEntity<T>(string propertyName, object propertyValue)
        	where T : IEntity;
        
        T[] GetEntitiesPage<T>(PagingLocation location, string sortExpression)
        	where T : IEntity;
        
        T[] GetEntitiesPage<T>(string fieldName, object fieldValue, PagingLocation location, string sortExpression)
        	where T : IEntity;
        
        // TODO: Clean up
        //IEntity[] GetEntities(Type entityType, string propertyName, object propertyValue);
        /*T[] GetEntitiesPage<T>(int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        	where T : IEntity;*/
        /*T[] GetEntitiesPage<T>(string fieldName, object fieldValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        	where T : IEntity;*/
        
 		IEntity[] GetEntitiesPage(Type type, PagingLocation location, string sortExpression);
        IEntity[] GetEntitiesPage(Type type, string fieldName, object fieldValue, PagingLocation location, string sortExpression);
        
        void PreSave(IEntity entity, out IEntity[] entitiesToUpdate, out IEntity[] entitiesToDelete);
        void Save(IEntity entity);
        void PreUpdate(IEntity entity, out IEntity[] entitiesToUpdate, out IEntity[] entitiesToDelete);
        void Update(IEntity entity);
        void PreDelete(IEntity entity, out IEntity[] entitiesToUpdate, out IEntity[] entitiesToDelete);
        void Delete(IEntity entity);
        bool IsStored(IEntity entity);

        #endregion
        
        void Commit();
        void Commit(bool forceCommit);
    }
}
