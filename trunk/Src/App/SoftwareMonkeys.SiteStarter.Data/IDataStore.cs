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
        BaseEntity[] GetAllEntities();
        BaseEntity[] GetEntities(Type entityType);
	// TODO: Remove if not necessary
        //BaseEntity[] GetEntities(BaseFilter filter);
        //BaseEntity[] GetEntities(FilterGroup group);
        BaseEntity[] GetEntities(Guid[] entityIDs);
        BaseEntity GetEntity(Type entityType, string propertyName, object propertyValue);
        BaseEntity GetEntity(Type entityType, IDictionary<string, object> parameters);
        BaseEntity[] GetEntities(Type entityType, IDictionary<string, object> parameters);
        BaseEntity[] GetEntities(Type entityType, string propertyName, object propertyValue);
        BaseEntity[] GetPage(Type type, int pageIndex, int pageSize, string sortExpression, out int totalObjects);
        BaseEntity[] GetPage(Type type, string fieldName, object fieldValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects);
        BaseEntity[] PreSave(BaseEntity entity);
        void Save(BaseEntity entity);
        BaseEntity[] PreUpdate(BaseEntity entity);
        void Update(BaseEntity entity);
        BaseEntity[] PreDelete(BaseEntity entity);
        void Delete(BaseEntity entity);
        bool IsStored(BaseEntity entity);
        void ApplySorting(string sortExpression);
        BaseEntity[] GetEntitiesContainingReverseReferences(BaseEntity entity, PropertyInfo property);

        #endregion
    }
}
