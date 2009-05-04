using System;
using System.Collections.Generic;
using System.Text;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;
using Db4objects.Db4o.Query;
using Db4objects.Db4o;
using System.Collections;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
    public class Db4oDataStore : IDataStore
    {
        private IQuery activeQuery;
        /// <summary>
        /// Gets/sets the active db4o query.
        /// </summary>
        public IQuery ActiveQuery
        {
            get
            {
                if (activeQuery == null)
                    activeQuery = ObjectContainer.Query();
                return activeQuery;
            }
            set { activeQuery = value; }
        }

        private IObjectContainer objectContainer;
        /// <summary>
        /// Gets/sets the corresponding db4o object container.
        /// </summary>
        public IObjectContainer ObjectContainer
        {
            get
            {
                if (objectContainer == null)
                    Open();
                return objectContainer;
            }
            //set { objectContainer = value; }
        }

        #region IDataStore Members

        private string name;
        /// <summary>
        /// Gets/sets the name of the data store.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public void Open()
        {
            objectContainer = Db4oFactory.OpenFile(Config.Application.PhysicalPath + @"\App_Data\" + Name + ".yap");
        }

        public void Dispose()
        {
		Close();
            objectContainer = null;
        }

        public void Close()
        {
            ObjectContainer.Commit();
            ObjectContainer.Close();
        }

        #endregion

        public BaseEntity[] PreSave(BaseEntity entity)
        {
            List<BaseEntity> toSave = new List<BaseEntity>();

            if (entity != null)
            {
		DataUtilities.TransferReferenceIDs(entity);

                // Loop through all the properties on the entity class
                foreach (PropertyInfo property in entity.GetType().GetProperties())
                {
                    // Get the custom attributes
                    object[] attributes = (object[])property.GetCustomAttributes(true);

                    // Loop through all the attributes
                    foreach (object attribute in attributes)
                    {
                        if (attribute is BaseEntityReferenceAttribute)
                        {
                            BaseEntityReferenceAttribute reference = (BaseEntityReferenceAttribute)attribute;

                            // TODO: IDs should be saved even when ExcludeFromDataStore=true
                            // if (!reference.ExcludeFromDataStore)
                            // {
                            // Get the referenced entities from the property
                            object referenceValue = property.GetValue(entity, null);

                            if (referenceValue != null)
                            {
                                // If the property type is an entity array
                                if (referenceValue is BaseEntity[])
                                {
                                    toSave.AddRange(PreSaveEntitiesReference(entity, property, reference));
                                }
                                // Otherwise the type is a Guid array
                                else if (referenceValue is Guid[])
                                {
                                    toSave.AddRange(PreSaveIDsReference(entity, property, reference));
                                }
                                // If the property type is an entity
                                else if (referenceValue is BaseEntity)
                                {
                                    toSave.AddRange(PreSaveEntityReference(entity, property, reference));
                                }
                                else if (referenceValue is Guid)
                                {
                                    toSave.AddRange(PreSaveIDReference(entity, property, reference));
                                }
				else
					throw new ArgumentException("entity", "The object types are invalid.");
                            }
                        }
                    }
                }

		DataUtilities.ApplyExclusions(entity);
            }

            return (BaseEntity[])toSave.ToArray();
        }

        public void Save(BaseEntity entity)
        {
            BaseEntity[] toUpdate = PreSave(entity);

            foreach (object entityToUpdate in toUpdate)
            {
                DataAccess.Data.Stores[entityToUpdate.GetType()].Update((BaseEntity)entityToUpdate);
            }

            if (entity != null)
            {
                ObjectContainer.Store(entity);
                ObjectContainer.Commit();
            }
        }

        public BaseEntity[] PreUpdate(BaseEntity entity)
        {
            System.Diagnostics.Trace.WriteLine(entity.GetType() + ": " + entity.ToString(), "Db4oHelper.Update");

            List<BaseEntity> toUpdate = new List<BaseEntity>();

		if (entity != null)
		{

		DataUtilities.TransferReferenceIDs(entity);

	            // Loop through all the properties on the entity class
	            foreach (PropertyInfo property in entity.GetType().GetProperties())
	            {
	                // Get the custom attributes
	                object[] attributes = (object[])property.GetCustomAttributes(true);
	
	                // Loop through all the attributes
	                foreach (object attribute in attributes)
	                {
	                    if (attribute is BaseEntityReferenceAttribute)
	                    {
	                        BaseEntityReferenceAttribute reference = (BaseEntityReferenceAttribute)attribute;
	
	                        // TODO: Check if needed
	                       // if (!reference.ExcludeFromDataStore)
	                      //  {
	                            // Get the referenced entities from the property
	                            object referenceValue = property.GetValue(entity, null);
	
	                            if (referenceValue != null)
	                            {
	                                // If the property type is an entity array
	                                if (referenceValue is BaseEntity[])
	                                {
	                                    toUpdate.AddRange(PreUpdateEntitiesReference(entity, property, reference));
	                                }
	                                // Otherwise the type is a Guid array
	                                else if (referenceValue is Guid[])
	                                {
	                                    toUpdate.AddRange(PreUpdateIDsReference(entity, property, reference));
	                                }
	                                // If the property type is an entity
	                                else if (referenceValue is BaseEntity)
	                                {
	                                    toUpdate.AddRange(PreUpdateEntityReference(entity, property, reference));
	                                }
	                                // If the property type is an entity
	                                else if (referenceValue is Guid)
	                                {
	                                    toUpdate.AddRange(PreUpdateIDReference(entity, property, reference));
	                                }
	                            }
	                       // }
	                    }
	                }
	            }


			DataUtilities.ApplyExclusions(entity);
		}

            return (BaseEntity[])toUpdate.ToArray();
        }

        public void Update(BaseEntity entity)
        {
            // Preupdate must be called to ensure all references are correctly stored
            BaseEntity[] toUpdate = PreUpdate(entity);

            // TODO: Update entities
            foreach (BaseEntity entityToUpdate in toUpdate)
	{
		//ApplyExclusions(entityToUpdate);

                DataAccess.Data.Stores[entityToUpdate.GetType()].Update(entityToUpdate);
	}

            if (entity != null)
            {
                ObjectContainer.Store(entity);
                
                ObjectContainer.Commit();
        	}
        }

        public BaseEntity[] PreDelete(BaseEntity entity)
        {

                List<BaseEntity> toUpdate = new List<BaseEntity>();

            if (entity != null)
            {

		DataUtilities.TransferReferenceIDs(entity);

                System.Diagnostics.Trace.WriteLine(entity.GetType() + ": " + entity.ToString(), "Db4oHelper.Delete");


                // Loop through all the properties on the entity class
                foreach (PropertyInfo property in entity.GetType().GetProperties())
                {
                    // Get the custom attributes
                    object[] attributes = (object[])property.GetCustomAttributes(true);

                    // Loop through all the attributes
                    foreach (object attribute in attributes)
                    {
                        if (attribute is BaseEntityReferenceAttribute)
                        {
                            BaseEntityReferenceAttribute reference = (BaseEntityReferenceAttribute)attribute;

                            // TODO: IDs should be saved even when ExcludeFromDataStore=true
                            // if (!reference.ExcludeFromDataStore)
                            // {
                            // Get the referenced entities from the property
                            object referenceValue = property.GetValue(entity, null);

                            if (referenceValue != null)
                            {
                                // If the property type is an entity array
                                if (referenceValue is BaseEntity[])
                                {
                                    toUpdate.AddRange(PreDeleteEntitiesReference(entity, property, reference));
                                }
                                // Otherwise the type is a Guid array
                                else if (referenceValue is Guid[])
                                {
                                    toUpdate.AddRange(PreDeleteIDsReference(entity, property, reference));
                                }
                                // If the property type is an entity
                                else if (referenceValue is BaseEntity)
                                {
                                    toUpdate.AddRange(PreDeleteEntityReference(entity, property, reference));
                                }
                                // If the property type is an entity
                                else if (referenceValue is Guid)
                                {
                                    toUpdate.AddRange(PreDeleteIDReference(entity, property, reference));
                                }
                            }
                        }
                    }
                }

		DataUtilities.ApplyExclusions(entity);
            }

                return (BaseEntity[])toUpdate.ToArray();
        }

        /// <summary>
        /// Deletes the provided entity and all referenced entities (where marked for cascading deletion).
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(BaseEntity entity)
        {
            // Preupdate must be called to ensure all references are correctly stored
            BaseEntity[] toUpdate = PreDelete(entity);

            // Update all modified entities
            foreach (BaseEntity entityToUpdate in toUpdate)
			{
			//ApplyExclusions(entityToUpdate);
	                DataAccess.Data.Stores[entityToUpdate.GetType()].Update(entityToUpdate);
			}

            // Delete the entity
            if (entity != null)
            {
                ObjectContainer.Delete(entity);
                
                ObjectContainer.Commit();
            }
        }

        public bool IsStored(BaseEntity entity)
        {
            return ObjectContainer.Ext().IsStored(entity);
        }

        /// <summary>
        /// Retrieves all the entities of the specified type from the data store.
        /// </summary>
        /// <param name="type">The type of entity to retrieve.</param>
        /// <returns>The entities of the specified type found in the data store.</returns>
        public BaseEntity[] GetEntities(Type type)
        {
            IQuery query = ObjectContainer.Query();
            query.Constrain(type);

            List<BaseEntity> list = new List<BaseEntity>();

            IObjectSet os = query.Execute();
            while (os.HasNext())
            {
                list.Add((BaseEntity)os.Next());
            }

            return (BaseEntity[])list.ToArray();
        }
        
        /// <summary>
        /// Retrieves all the entities of the specified type from the object set.
        /// </summary>
        /// <param name="type">The type of entity to retrieve.</param>
        /// <param name="os">The object set to retrieve the entities from.</param>
        /// <returns>The entities of the specified type found in the data store.</returns>
        public BaseEntity[] GetEntities(Type type, IObjectSet os)
        {
            ArrayList list = new ArrayList();

            while (os.HasNext())
            {
                list.Add(os.Next());
            }

            return (BaseEntity[])list.ToArray(type);
        }

        /// <summary>
        /// Retrieves all the entities of the specified type from the data store.
        /// </summary>
        /// <param name="type">The type of entity to retrieve.</param>
        /// <returns>The entities of the specified type found in the data store.</returns>
        public BaseEntity[] GetEntities(Guid[] entityIDs)
        {
             List<BaseEntity> list = new List<BaseEntity>(ObjectContainer.Query<BaseEntity>(delegate(BaseEntity e)
	                {
	                    return Array.IndexOf(entityIDs, e.ID) > -1;
	                }));
            return (BaseEntity[])list.ToArray();
        }

        /// <summary>
        /// Retrieves a single entity of the specified type with data matching the provided parameter.
        /// </summary>
        /// <param name="type">The type of object to retrieve.</param>
        /// <param name="fieldName">The name of the field to match.</param>
        /// <param name="fieldValue">The value of the field to match.</param>
        /// <returns>The object of the specified type with data matching the parameter.</returns>
        public BaseEntity GetEntity(Type type, string fieldName, object fieldValue)
        {
            IQuery query = ObjectContainer.Query();
            query.Constrain(type);
            query.Descend(fieldName).Constrain(fieldValue);

            IObjectSet os = query.Execute();
            if (os.HasNext())
                return (BaseEntity)os.Next();
            else
                return null;
        }

        /// <summary>
        /// Retrieves all the entities of the specified type matching the specified value.
        /// </summary>
        /// <param name="type">The type of entity to retrieve.</param>
        /// <param name="fieldName">The name of the property</param>
        /// <param name="fieldValue">The value of the field to query for.</param>
        /// <returns></returns>
        public BaseEntity[] GetEntities(Type type, string fieldName, object fieldValue)
        {
            IQuery query = ObjectContainer.Query();
            query.Constrain(type);
            query.Descend(fieldName).Constrain(fieldValue);

            ArrayList list = new ArrayList();

            IObjectSet os = query.Execute();
            while (os.HasNext())
            {
                list.Add(os.Next());
            }

            return (BaseEntity[])list.ToArray(type);
        }

        /// <summary>
        /// Retrieves all the entities of the specified type matching the specified values.
        /// </summary>
        /// <param name="type">The type of entity to retrieve.</param>
        /// <param name="parameters">The parameters to query with.</param>
        /// <returns></returns>
        public BaseEntity[] GetEntities(Type type, IDictionary<string, object> parameters)
        {
            IQuery query = ObjectContainer.Query();
            query.Constrain(type);
            foreach (string key in parameters.Keys)
            {
                query.Descend(key).Constrain(parameters[key]);
            }

            ArrayList list = new ArrayList();

            IObjectSet os = query.Execute();
            while (os.HasNext())
            {
                list.Add(os.Next());
            }

            return (BaseEntity[])list.ToArray(type);
        }

        /// <summary>
        /// Retrieves the entity of the specified type matching the specified values.
        /// </summary>
        /// <param name="type">The type of entity to retrieve.</param>
        /// <param name="parameters">The parameters to query with.</param>
        /// <returns></returns>
        public BaseEntity GetEntity(Type type, IDictionary<string, object> parameters)
        {
            IQuery query = ObjectContainer.Query();
            query.Constrain(type);
            foreach (string key in parameters.Keys)
            {
                query.Descend(key).Constrain(parameters[key]);
            }
            IObjectSet os = query.Execute();
            if (os.HasNext())
            {
                return (BaseEntity)os.Next();
            }

            return null;
        }

        /// <summary>
        /// Retrieves all of the entities from the data store.
        /// </summary>
        /// <returns>An array of all the entities in the data store.</returns>
        public BaseEntity[] GetAllEntities()
        {
            ActiveQuery = ObjectContainer.Query();

            ArrayList list = new ArrayList();

            IObjectSet os = ActiveQuery.Execute();
            while (os.HasNext())
            {
                object obj = os.Next();
                if (obj is BaseEntity)
                {
                    list.Add((BaseEntity)obj);
                }
            }

            return (BaseEntity[])list.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Retrieves the specified page of objects from the data store.
        /// </summary>
        /// <param name="type">The type of objects being retrieved.</param>
        /// <param name="pageIndex">The index of the page to retrieve.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
        /// <param name="totalObjects">The total number of objects found.</param>
        /// <returns>An array of the objects retrieved.</returns>
        public BaseEntity[] GetPage(Type type, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        {
            ActiveQuery = ObjectContainer.Query();
            ActiveQuery.Constrain(type);

            ApplySorting(sortExpression);

            IObjectSet os = ActiveQuery.Execute();

            int i = 0;
            //        os.Reset();

            ArrayList page = new ArrayList();
            while (os.HasNext())
            {
                if ((i >= pageIndex * pageSize) && (i < (pageIndex + 1) * pageSize))
                {
                    page.Add(os.Next());
                }
                else
                    os.Next();
                i++;
            }
            totalObjects = i;
            return (BaseEntity[])page.ToArray(type);
        }

        /// <summary>
        /// Retrieves the specified page of objects from the provided IObjectSet.
        /// </summary>
        /// <param name="type">The type of objects being retrieved.</param>
        /// <param name="fieldName">The name of the field to query for.</param>
        /// <param name="propertyValue">The value of the field to query for.</param>
        /// <param name="pageIndex">The index of the page to retrieve.</param>
        /// <param name="pageSize">The size of each page.</param>
        /// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
        /// <param name="totalObjects">The total number of objects found.</param>
        /// <returns>An array of the objects retrieved.</returns>
        public BaseEntity[] GetPage(Type type, string fieldName, object propertyValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
        {
            ActiveQuery = ObjectContainer.Query();
            ActiveQuery.Constrain(type);
            ActiveQuery.Descend(fieldName).Constrain(propertyValue);

            ApplySorting(sortExpression);

            IObjectSet os = ActiveQuery.Execute();

            int i = 0;
            //        os.Reset();

            ArrayList page = new ArrayList();
            while (os.HasNext())
            {
                if ((i >= pageIndex * pageSize) && (i < (pageIndex + 1) * pageSize))
                {
                    page.Add(os.Next());
                }
                else
                    os.Next();
                i++;
            }
            totalObjects = i;
            return (BaseEntity[])page.ToArray(type);
        }

        /// <summary>
        /// Applies the specified sort expression to the provided query.
        /// </summary>
        /// <param name="query">The query to apply the sort expression to.</param>
        /// <param name="sortExpression">The sort expression to apply to the query.</param>
        public void ApplySorting(string sortExpression)
        {
            if (ActiveQuery != null && sortExpression != null)
            {
                if (sortExpression.IndexOf("Descending") > -1)
                {
                    string propertyName = sortExpression.Replace("Descending", String.Empty);
                    propertyName = ToCamelCase(propertyName);
                    ActiveQuery.Descend(propertyName).OrderDescending();
                }
                else if (sortExpression.IndexOf("Ascending") > -1)
                {
                    string propertyName = sortExpression.Replace("Ascending", String.Empty);
                    propertyName = ToCamelCase(propertyName);
                    ActiveQuery.Descend(propertyName).OrderAscending();
                }
                else
                    throw new ArgumentException("The provided sort expression is invalid: " + sortExpression);
            }
        }

        #region Db4o specific functions

        /// <summary>
        /// Creates a new query in the data store that's constrained to the specified type.
        /// </summary>
        /// <param name="type">The type to constrain the query to.</param>
        /// <returns>The newly created query.</returns>
        public IQuery Query(Type type)
        {
            IQuery query = ObjectContainer.Query();
            query.Constrain(type);
            return query;
        }

        public BaseEntity GetEntity(IObjectSet os)
        {
            throw new NotImplementedException();
        }
        #endregion

        static public string ToCamelCase(string text)
        {
            // TODO: Check if this is done properly
            if (text == string.Empty)
                return String.Empty;

            string firstChar = text.Substring(0, 1);

            text = text.Substring(1, text.Length - 1);

            text = firstChar.ToLower() + text;

            return text;
        }



        /// <summary>
        /// Prepares the provided reference for update. Does NOT synchronise mirrors because that's done by the IDs references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreSaveEntitiesReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Preparing entities reference to be saved.", NLog.LogLevel.Debug))
            {
		        AppLogger.Debug("Entity type: " + entity.GetType().ToString());
		        AppLogger.Debug("Property name: " + property.Name);
		        AppLogger.Debug("Property type: " + property.PropertyType.ToString());

                Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

                object referenceValue = property.GetValue(entity, null);

                // Check if the save is to cascade
                if (attribute.CascadeSave)
                {
                    AppLogger.Debug("attribute.CascadeSave == true");
                    // Save the reference entities
                    foreach (BaseEntity referencedEntity in (BaseEntity[])referenceValue)
                    {
                        // Delete the original referenced entity
                        Delete(GetEntity(referencedEntity.GetType(), "id", referencedEntity.ID));

                        toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));

                        // Save the new referenced entity
                        Save((BaseEntity)referencedEntity);
                    }
                }
                else
                {
                    AppLogger.Debug("attribute.CascadeSave == false");


			        toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property.Name)));
                    /*BaseEntity[] newReferences = (BaseEntity[])property.GetValue(entity, null);

                    // Entity[] references = (Entity[])property.GetValue(originalEntity, null);

                    if (newReferences != null && newReferences.Length > 0)
                    {
                        AppLogger.Debug("References found");

                        for (int i = 0; i < newReferences.Length; i++)
                        {
                            using (LogGroup logGroup2 = AppLogger.StartGroup("Checking reference.", NLog.LogLevel.Debug))
                            {

                                AppLogger.Debug("Referenced entity ID: " + newReferences[i].ID);

                                AppLogger.Debug("Referenced type: " + newReferences[i].GetType().ToString());

                                AppLogger.Debug("attribute.IDsPropertyName: " + attribute.IDsPropertyName);

                                // If the references are being stored on this property then update mirrors.
                                // If the references are being stored on an IDs property then this should be skipped
                                if (attribute.IDsPropertyName == String.Empty)
                                {
                                    AppLogger.Debug("attribute.IDsPropertyName == String.Empty");
                                    toUpdate.Add(DataUtilities.AddReferences(newReferences[i], entity, DataUtilities.GetMirrorProperty(property)));
                                }
                                else
                                {
                                    AppLogger.Debug("attribute.IDsPropertyName != String.Empty");

                                    PropertyInfo idsProperty = entity.GetType().GetProperty(attribute.IDsPropertyName);

                                    if (idsProperty == null)
                                    {
                                        AppLogger.Debug("Invalid IDsPropertyName [idsProperty == null]");
                                        throw new Exception("Invalid IDsPropertyName of '" + attribute.IDsPropertyName + "' on the attribute for the '" + property.Name + "' property of the '" + entity.GetType().ToString() + "' class.");
                                    }
                                    else
                                    {
                                        AppLogger.Debug("[idsProperty != null]");


                                        AppLogger.Debug("Setting the IDs property '" + attribute.IDsPropertyName + "' with an array of '" + newReferences.Length + "' entities.");
                                        idsProperty.SetValue(entity, Collection<BaseEntity>.GetIDs(newReferences), null);


                                        // Loop through all the attributes
                                        foreach (object idsPropertyAttribute in idsProperty.GetCustomAttributes(true))
                                        {
                                            if (idsPropertyAttribute is BaseEntityReferenceAttribute)
                                            {
                                                // Todo: remove
						//toUpdate.Add(DataUtilities.AddReferences(entity, newReferences[i], property, attribute));
                                                toUpdate.Add(DataUtilities.AddReferences(newReferences[i], entity, DataUtilities.GetMirrorProperty(idsProperty)));
                                            }
                                        }
                                    }
                                }

                                // TODO: Check if needed
                                //        if (Array.IndexOf(Collection<Entity>.GetIDs(newReferences), originalReferences[i].ID) > -1)
                                //        {
                                //            references.Add(newReferences[i]);
                                //        }
                            }
                        }
                    }
                    else
                        AppLogger.Debug("No references found");*/

                    // Set a bound copy of the referenced object to the property to ensure it won't get duplicated
                    // if (references != null && references.Count > 0)
                    //     property.SetValue(entity, references.ToArray(originalReferences[0].GetType()), null);
                    // else
                    //     property.SetValue(entity, null, null);
                }

                return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
            }
        }

        /// <summary>
        /// Prepares the provided reference for update. Does NOT synchronise mirrors because that's done by the IDs references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreSaveEntityReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute reference)
        {
            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();
            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (reference.CascadeUpdate)
            {
                BaseEntity referencedEntity = (BaseEntity)referenceValue;

                // Delete the original referenced entity
                Delete(GetEntity(referencedEntity.GetType(), "id", referencedEntity.ID));

                toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));

                // Save the new referenced entity
                Save((BaseEntity)referenceValue);
            }
            else
            {
                // If the reference is not being stored by an IDs property, a mirror is specified, and the property is not to be excluded
                if (reference.IDsPropertyName == String.Empty && reference.MirrorName != String.Empty)
                {
			toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property.Name)));

                    //toUpdate.Add(DataUtilities.AddReferences((BaseEntity)referenceValue, entity, DataUtilities.GetMirrorProperty(property)));

                    // Set a bound copy of the referenced object to the property to ensure it won't get duplicated
                    //property.SetValue(entity, GetEntity(property.PropertyType, "id", ((BaseEntity)referenceValue).ID), null);
                }

                if (reference.ExcludeFromDataStore)
                    property.SetValue(entity, null, null);
            }

            return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for update. Synchronises mirror references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreSaveIDsReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
		if (entity == null)
	 		throw new ArgumentNullException("entity");

		if (property == null)
			throw new ArgumentNullException("property");

		if (attribute == null)
			throw new ArgumentNullException("attribute");

            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

		using (LogGroup logGroup = AppLogger.StartGroup("Preparing IDs reference for saving.", NLog.LogLevel.Debug))
		{
	
	            object referenceValue = property.GetValue(entity, null);

			AppLogger.Debug("Reference value: " + referenceValue.ToString());
			AppLogger.Debug("Entity type: " + entity.GetType());
			AppLogger.Debug("Property name: " + property.Name);
			AppLogger.Debug("Property type: " + property.PropertyType);
	
	            // Check if the save is to cascade
	            if (attribute.CascadeSave)
	            {
			AppLogger.Debug("attribute.CascadeSave == true");

	                // Save the reference entities
	                foreach (Guid referencedEntityID in (Guid[])referenceValue)
	                {
	                    BaseEntity referencedEntity = GetEntity(typeof(BaseEntity), "id", referencedEntityID);
	
	                    // Delete the original referenced entity
	                    Delete(referencedEntity);

                        toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));
	
	                    // Save the new referenced entity
	                    Save((BaseEntity)referencedEntity);
	                }
	            }
	            else
	            {
			AppLogger.Debug("attribute.CascadeSave == false");

                    Type type = entity.GetType();

            PropertyInfo entitiesProperty = type.GetProperty(attribute.EntitiesPropertyName);

            if (entitiesProperty == null)
            {
                throw new Exception("The entities property '" + attribute.EntitiesPropertyName + "' could not be found on the type '" + type.ToString() + "'.");
            }

	                Type referenceEntityType = entitiesProperty.PropertyType.GetElementType();
	
			if (attribute.MirrorName != String.Empty)
			{

		                PropertyInfo mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);
	
				if (mirrorProperty == null)
					throw new Exception("Mirror property '" + attribute.MirrorName + "' not found on the type '" + referenceEntityType.ToString());

				AppLogger.Debug("Mirror property: " + mirrorProperty.Name);
				AppLogger.Debug("Mirror property type: " + mirrorProperty.PropertyType);
				AppLogger.Debug("Reference entity type: " + referenceEntityType.ToString());

				toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[referenceEntityType].GetEntities(DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property.Name)));
		
		                /*Collection<BaseEntity> references = new Collection<BaseEntity>();
		
		                references.Add(GetEntities(DataUtilities.GetReferenceIDs(entity, property)));
		
		                // If references were specified
		                if (references.Count > 0)
		                {
					using (LogGroup logGroup2 = AppLogger.StartGroup("Looping through each of the references.", NLog.LogLevel.Debug));
					{
			                    // Loop through all the referenced entities
			                    foreach (BaseEntity r in references)
			                    {
						AppLogger.Debug("Reference ID: " + r.ID.ToString());
	
			                        if (mirrorProperty != null)
			                        {
							if (mirrorProperty.PropertyType.Equals(typeof(Guid[])))
							{
				                            object mirrorValue = (Guid[])mirrorProperty.GetValue(r, null);
			
								AppLogger.Debug("Mirror value: " + mirrorValue);
				
				                            // If the reference is already there then don't bother creating it
				                            if (mirrorValue == null || Array.IndexOf((Guid[])mirrorValue, entity.ID) == -1)
				                            {
								AppLogger.Debug("Adding reference.");
			
				                                // Update the mirror references
		                        			toUpdate.Add(DataUtilities.AddReferences(r, entity, DataUtilities.GetMirrorProperty(property)));
				                            }
							}
							else
							{
								Guid mirrorValue = (Guid)mirrorProperty.GetValue(r, null);
			
								AppLogger.Debug("Mirror value: " + mirrorValue);
				
				                            // If the reference is already there then don't bother creating it
				                            if (mirrorValue != entity.ID)
				                            {
								AppLogger.Debug("Adding reference.");
			
				                                // Update the mirror references
		                        			toUpdate.Add(DataUtilities.AddReferences(r, entity, DataUtilities.GetMirrorProperty(property)));
				                            }
							}
			                        }
			                    }
					}
		                }
				else
					AppLogger.Debug("No references found.");*/
		
		                // TODO: Remove if not needed
		                // Set a bound copy of the referenced object to the property to ensure it won't get duplicated
		                /*if (references != null && references.Count > 0)
		                    property.SetValue(entity, references.GetIDs(), null);
		                else
		                    property.SetValue(entity, null, null);*/
			}
			else
				AppLogger.Debug("No mirror property name specified.");
	            }

		}

            return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for update. Synchronises mirror references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreSaveIDReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            System.Diagnostics.Trace.WriteLine("Db4oDataStore.PreSaveIDReference");
            System.Diagnostics.Trace.Indent();

            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (attribute.CascadeSave)
            {
                BaseEntity referencedEntity = null;
                if (referenceValue is Guid)
                {
                    referencedEntity = GetEntity(typeof(BaseEntity), "id", (Guid)referenceValue);
                }
                else if (referenceValue is BaseEntity)
                {
                    referencedEntity = GetEntity(typeof(BaseEntity), "id", ((BaseEntity)referenceValue).ID);
                }

                toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));

                // Save the new referenced entity
                Save(referencedEntity);
            }
            else
            {
                BaseEntity reference = GetEntity(typeof(BaseEntity), "id", (Guid)property.GetValue(entity, null));

                Type referenceEntityType = reference != null ? reference.GetType() : null;
                //entity.GetType().GetProperty(attribute.EntitiesPropertyName).PropertyType.GetElementType();

                if (attribute.MirrorName != String.Empty)
                {
			toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[referenceEntityType].GetEntities(DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property.Name)));
                    /*PropertyInfo mirrorProperty = null;
                    if (referenceEntityType != null && attribute != null && attribute.MirrorName != null)
                    {
                        mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);

                        if (mirrorProperty == null)
                            System.Diagnostics.Trace.WriteLine("Mirror property was specified but not found: '" + attribute.MirrorName + "' on " + referenceEntityType.ToString());

                        if (reference != null && mirrorProperty != null)
                        {
                            object mirrorValue = mirrorProperty.GetValue(reference, null);

                            // If the reference is already there then don't bother creating it
                            if (mirrorProperty.PropertyType.Equals(typeof(Guid)))
                            {
                                if (mirrorValue == null || (Guid)mirrorValue != entity.ID)
                                {
                                    // Update the mirror references
                                    toUpdate.Add(DataUtilities.AddReferences(reference, entity, DataUtilities.GetMirrorProperty(property)));

                                    // Update the referenced entity
                                    // TODO: Shouldn't be needed
                                    //toUpdate.Add(r);
                                }
                            }
                            else if (mirrorProperty.PropertyType.Equals(typeof(Guid[])))
                            {
                                if (mirrorValue == null || Array.IndexOf((Guid[])mirrorValue, entity.ID) == -1)
                                {
                                    // Update the mirror references
                                    toUpdate.Add(DataUtilities.AddReferences(reference, entity, DataUtilities.GetMirrorProperty(property)));

                                    // Update the referenced entity
                                    // TODO: Shouldn't be needed
                                    //toUpdate.Add(r);
                                }
                            }
                        }
                    }*/
                }
            }

            System.Diagnostics.Trace.Unindent();

            return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for update. Does NOT synchronise mirrors because that's done by the IDs references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreUpdateEntitiesReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (attribute.CascadeUpdate)
            {
                // Save the reference entities
                foreach (BaseEntity referencedEntity in (BaseEntity[])referenceValue)
                {
                    // Delete the original referenced entity
                    Delete(GetEntity(referencedEntity.GetType(), "id", referencedEntity.ID));

                    toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));

                    // Save the new referenced entity
                    Save((BaseEntity)referencedEntity);
                }
            }
            else
            {
                Type referenceEntityType = entity.GetType().GetProperty(property.Name).PropertyType.GetElementType();

                BaseEntity[] newReferences = (BaseEntity[])property.GetValue(entity, null);

		if (attribute.MirrorName != String.Empty)
		{
			toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[referenceEntityType].GetEntities(DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property.Name)));

	                

		}
		else
			AppLogger.Debug("Mirror name is blank. Skipping.");

            }

            return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for update. Does NOT synchronise mirrors because that's done by the IDs references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreUpdateEntityReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute reference)
        {
            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (reference.CascadeUpdate)
            {
                BaseEntity referencedEntity = (BaseEntity)referenceValue;

                // Delete the original referenced entity
                Delete(GetEntity(referencedEntity.GetType(), "id", referencedEntity.ID));

                toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));

                // Save the new referenced entity
                Save((BaseEntity)referenceValue);
            }
            else
            {
                toUpdate.Add(DataUtilities.AddReferences((BaseEntity)referenceValue, entity, DataUtilities.GetMirrorProperty(property)));

                // Set a bound copy of the referenced object to the property to ensure it won't get duplicated
                property.SetValue(entity, GetEntity(property.PropertyType, "id", ((BaseEntity)referenceValue).ID), null);
            }

            return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for update. Synchronises mirror references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreUpdateIDsReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

		using (LogGroup logGroup = AppLogger.StartGroup("Preparing IDs reference for update.", NLog.LogLevel.Debug))
		{

	            object referenceValue = property.GetValue(entity, null);
	
	            // Check if the save is to cascade
	            if (attribute.CascadeUpdate)
	            {
			        AppLogger.Debug("Cascade update: false");

	                // Save the reference entities
	                foreach (Guid referencedEntityID in (Guid[])referenceValue)
	                {
	                    BaseEntity referencedEntity = GetEntity(typeof(BaseEntity), "id", referencedEntityID);
	
	                    // Delete the original referenced entity
	                    Delete(referencedEntity);
	
	                    toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));
	
	                    // Save the new referenced entity
	                    Save((BaseEntity)referencedEntity);
	                }
	            }
	            else
	            {
			AppLogger.Debug("Cascade update: false");

	                Type referenceEntityType = DataUtilities.GetReferenceType(entity, property);
	
	                BaseEntity[] oldReferences = (BaseEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property.Name);
	                //Entity[] originalReferences = GetEntities(referenceEntityType, query.Execute());

			AppLogger.Debug("# of old references: " + oldReferences.Length.ToString());
				
			        if (attribute.MirrorName != String.Empty)
			        {
                        		toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(DataUtilities.GetReferenceIDs(entity, property)), oldReferences));
			        }
	
	
	                // Set a bound copy of the referenced object to the property to ensure it won't get duplicated
	                /*if (references != null && references.Count > 0)
	                    property.SetValue(entity, references.GetIDs(), null);
	                else
	                    property.SetValue(entity, null, null);*/
	            }

		}

            return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for update. Synchronises mirror references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreUpdateIDReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Preparing ID reference for update.", NLog.LogLevel.Debug))
            {
                object referenceValue = property.GetValue(entity, null);

                // Check if the save is to cascade
                if (attribute.CascadeUpdate)
                {
                    AppLogger.Debug("attribute.CascadeUpdate == true");

                    BaseEntity referencedEntity = GetEntity(typeof(BaseEntity), "id", (Guid)referenceValue);

                    // Delete the original referenced entity
                    Delete(referencedEntity);

                    toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(property)));

                    // Save the new referenced entity
                    Save((BaseEntity)referencedEntity);
                }
                else
                {
                    AppLogger.Debug("attribute.CascadeUpdate == false");

                    PropertyInfo entitiesProperty = entity.GetType().GetProperty(attribute.EntitiesPropertyName);

                    AppLogger.Debug("Entities property name: " + entitiesProperty.Name);

                    Type referenceEntityType = DataUtilities.GetReferenceType(entity, property);

                    AppLogger.Debug("Reference entity type: " + referenceEntityType.ToString());

                    if (attribute.MirrorName != String.Empty)
                    {
                        PropertyInfo mirrorProperty = referenceEntityType.GetProperty(DataUtilities.GetMirrorProperty(property));

                        AppLogger.Debug("Mirror property name: " + attribute.MirrorName);


                        toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property.Name)));
                    }
                    else
                    {
                        AppLogger.Debug("Mirror property name: String.Empty");
                    }

                    // TODO: Check if needed
                    // Set a bound copy of the referenced object to the property to ensure it won't get duplicated
                    /*if (references != null && references.Count > 0)
                        property.SetValue(entity, references.GetIDs(), null);
                    else
                        property.SetValue(entity, null, null);*/
                }
            }

            return (BaseEntity[])toUpdate.ToArray(typeof(BaseEntity));
        }

        protected BaseEntity[] PreDeleteEntitiesReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            Collection<BaseEntity> toDelete = new Collection<BaseEntity>();

            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (attribute.CascadeDelete)
            {
                // Save the reference entities
                foreach (BaseEntity referencedEntity in (BaseEntity[])referenceValue)
                {
                    // Delete the original referenced entity
                    Delete(GetEntity(referencedEntity.GetType(), "id", referencedEntity.ID));
                }
            }
            else
            {
                Type referenceEntityType = entity.GetType().GetProperty(property.Name).PropertyType.GetElementType();

                BaseEntity[] newReferences = (BaseEntity[])property.GetValue(entity, null);

                PropertyInfo mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);

                IList<BaseEntity> originalEntities = ObjectContainer.Query<BaseEntity>(delegate(BaseEntity e)
                {
                    if (mirrorProperty != null && e.GetType() == referenceEntityType)
                    {
                        object mirrorValue = mirrorProperty.GetValue(e, null);
                        if (mirrorValue != null && mirrorValue is BaseEntity[])
                            return Array.IndexOf(Collection<BaseEntity>.GetIDs((BaseEntity[])mirrorValue), entity.ID) > -1;
                        else
                            return false;
                    }
                    else
                        return false;
                });

                BaseEntity[] originalReferences = (BaseEntity[])new List<BaseEntity>(originalEntities).ToArray();

                ArrayList references = new ArrayList();

                // If a reference exists in both old and new copy then keep it in the new
                if (originalReferences != null)
                {
                    for (int i = 0; i < originalReferences.Length; i++)
                    {
                        // TODO: See if this check is necessary
                        // If the references are being stored on this property then delete mirrors.
                        // If the references are being stored on an IDs property then this should be skipped
                        if (attribute.IDsPropertyName == String.Empty && !attribute.ExcludeFromDataStore)
                        {
                            object mirrorValue = null;
                            if (mirrorProperty != null)
                                mirrorValue = mirrorProperty.GetValue(originalReferences[i], null);

                            // If the mirror contains a reference but the deleted entity then remove the old reference
                            if ((mirrorProperty == null || Array.IndexOf(Collection<BaseEntity>.GetIDs((BaseEntity[])mirrorValue), entity.ID) > -1)
                                && Array.IndexOf(Collection<BaseEntity>.GetIDs(newReferences), originalReferences[i].ID) == -1)
                                toDelete.Add(DataUtilities.RemoveReferences(originalReferences[i], entity, DataUtilities.GetMirrorProperty(property)));
                        }
                    }
                }
            }

            return (BaseEntity[])toDelete.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for delete. Does NOT synchronise mirrors because that's done by the IDs references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreDeleteEntityReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute reference)
        {
            Collection<BaseEntity> toDelete = new Collection<BaseEntity>();

            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (reference.CascadeDelete)
            {
                BaseEntity referencedEntity = (BaseEntity)referenceValue;

                // Delete the original referenced entity
                Delete(GetEntity(referencedEntity.GetType(), "id", referencedEntity.ID));
            }
            else
            {
                // Don't do anything
            }

            return (BaseEntity[])toDelete.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for delete. Synchronises mirror references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreDeleteIDsReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            Collection<BaseEntity> toDelete = new Collection<BaseEntity>();

            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (attribute.CascadeDelete)
            {
                // Save the reference entities
                foreach (Guid referencedEntityID in (Guid[])referenceValue)
                {
                    BaseEntity referencedEntity = GetEntity(typeof(BaseEntity), "id", referencedEntityID);

                    // Delete the original referenced entity
                    Delete(referencedEntity);
                }
            }
            else
            {
                Type referenceEntityType = entity.GetType().GetProperty(attribute.EntitiesPropertyName).PropertyType.GetElementType();

                PropertyInfo mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);

                IList<BaseEntity> entities = ObjectContainer.Query<BaseEntity>(delegate(BaseEntity e)
                {
                    if (e.GetType() == referenceEntityType)
                    {
                        object mirrorValue = mirrorProperty.GetValue(e, null);
                        if (mirrorValue is Guid[])
                        {
                            if (mirrorValue != null)
                                return Array.IndexOf((Guid[])mirrorValue, entity.ID) > -1;
                            else
                                return false;
                        }
                        else
                        {
                            if (mirrorValue != null)
                                return (Guid)mirrorValue == entity.ID;
                            else
                                return false;
                        }
                    }
                    else
                        return false;
                });

                BaseEntity[] originalReferences = (BaseEntity[])new List<BaseEntity>(entities).ToArray();
                //Entity[] originalReferences = GetEntities(referenceEntityType, query.Execute());

                Collection<BaseEntity> references = new Collection<BaseEntity>();

                references.Add(GetEntities((Guid[])property.GetValue(entity, null)));

                // If a reference exists in both old and new copy then keep it in the new
                if (originalReferences != null)
                {
                    for (int i = 0; i < originalReferences.Length; i++)
                    {
                        // If the reference is not still being kept then allow it to be removed
                        if (!references.Contains(originalReferences[i]))
                        {
                            // Delete the mirror references
                            toDelete.Add(DataUtilities.RemoveReferences(originalReferences[i], entity, DataUtilities.GetMirrorProperty(property)));
                        }
                    }
                }
            }

            return (BaseEntity[])toDelete.ToArray(typeof(BaseEntity));
        }

        /// <summary>
        /// Prepares the provided reference for delete. Synchronises mirror references.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="property"></param>
        protected BaseEntity[] PreDeleteIDReference(BaseEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            Collection<BaseEntity> toDelete = new Collection<BaseEntity>();

            object referenceValue = property.GetValue(entity, null);

            // Check if the save is to cascade
            if (attribute.CascadeDelete)
            {
                BaseEntity referencedEntity = GetEntity(typeof(BaseEntity), "id", (Guid)referenceValue);

                // Delete the original referenced entity
                Delete(referencedEntity);
            }
            else
            {
                PropertyInfo entitiesProperty = entity.GetType().GetProperty(attribute.EntitiesPropertyName);
                Type referenceEntityType = null;
                if (entitiesProperty.PropertyType.GetElementType() != null)
                    referenceEntityType = entitiesProperty.PropertyType.GetElementType();
                else
                    referenceEntityType = entitiesProperty.PropertyType;

                Collection<BaseEntity> references = new Collection<BaseEntity>();

                if (attribute.MirrorName != String.Empty)
                {
                    PropertyInfo mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);

                    IList<BaseEntity> entities = ObjectContainer.Query<BaseEntity>(delegate(BaseEntity e)
                    {
                        if (e.GetType() == referenceEntityType)
                        {
                            object mirrorValue = (Guid[])mirrorProperty.GetValue(e, null);
                            if (mirrorValue != null)
                                return Array.IndexOf((Guid[])mirrorValue, entity.ID) > -1;
                            else
                                return false;
                        }
                        else
                            return false;
                    });

                    BaseEntity[] originalReferences = (BaseEntity[])new List<BaseEntity>(entities).ToArray();
                    //Entity[] originalReferences = GetEntities(referenceEntityType, query.Execute());

                    object propertyValue = property.GetValue(entity, null);

                    if (propertyValue is Guid[])
                        references.Add(GetEntities((Guid[])propertyValue));
                    else
                        references.Add(GetEntity(referenceEntityType, "id", (Guid)propertyValue));

                    // If a reference exists in both old and new copy then keep it in the new
                    if (originalReferences != null)
                    {
                        for (int i = 0; i < originalReferences.Length; i++)
                        {
                            // If the reference is not still being kept then allow it to be removed
                            if (!references.Contains(originalReferences[i]))
                            {
                                // Delete the mirror references
                                toDelete.Add(DataUtilities.RemoveReferences(originalReferences[i], entity, DataUtilities.GetMirrorProperty(property)));
                            }
                        }
                    }
                }
            }

            return (BaseEntity[])toDelete.ToArray(typeof(BaseEntity));
        }

    }
}
