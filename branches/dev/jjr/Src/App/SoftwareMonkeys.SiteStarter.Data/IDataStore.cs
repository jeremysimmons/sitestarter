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
        IEntity GetEntity(Type type, IDictionary<string, object> parameters);
        T GetEntity<T>(IDictionary<string, object> parameters)
        	where T : IEntity;
        
        IEntity[] GetEntities(Type type, IDictionary<string, object> parameters);
        T[] GetEntities<T>(IDictionary<string, object> parameters)
        	where T : IEntity;
        
        
        IEntity[] GetEntities(Type type, string propertyName, object propertyValue);
        T[] GetEntities<T>(string propertyName, object propertyValue)
        	where T : IEntity;
        IEntity GetEntity(Type type, string propertyName, object propertyValue);
        T GetEntity<T>(string propertyName, object propertyValue)
        	where T : IEntity;
        
        //IEntity[] GetEntities(Type entityType, string propertyName, object propertyValue);
        T[] GetPage<T>(int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        	where T : IEntity;
        T[] GetPage<T>(string fieldName, object fieldValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        	where T : IEntity;
        IEntity[] PreSave(IEntity entity);
        void Save(IEntity entity);
        IEntity[] PreUpdate(IEntity entity);
        void Update(IEntity entity);
        IEntity[] PreDelete(IEntity entity);
        void Delete(IEntity entity);
        bool IsStored(IEntity entity);
        void ApplySorting(string sortExpression);

        #endregion
    }
}
