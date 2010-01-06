using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides an interface for interacting with entities.
	/// </summary>
    [DataObject(true)]
	public class EntityFactory
    {
        // TODO: Remove if not needed
        /*/// <summary>
        /// Gets the data store containing the objects that this factory interact with.
        /// </summary>
        static public IObjectContainer DataStore
        {
            get { return DataAccess.Data.Stores.GetDataStore(typeof(Entities.BaseEntity), true); }
        }*/

		#region Retrieve functions
	    /*/// <summary>
		/// Retrieves all the entities from the DB.
		/// </summary>
		/// <returns>An array of the retrieved entities.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
		static public IEntity[] GetEntities()
		{
            return (BaseEntity[])(DataAccess.Data.Stores[typeof(BaseEntity)].GetEntities(typeof(Entities.BaseEntity)));
		}*/

		/// <summary>
		/// Retrieves all the specified entities from the DB.
		/// </summary>
		/// <param name="entityIDs">An array of IDs of entities to retrieve.</param>
		/// <returns>A BaseEntitySet containing the retrieved entities.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public T[] GetEntities<T>(Guid[] entityIDs)
        	where T : IEntity
		{
			// Create a new entity collection
            Collection<T> entities = new Collection<T>();

			// Loop through the IDs and add each entity to the collection
			foreach (Guid entityID in entityIDs)
			{
				if (entityID != Guid.Empty)
					entities.Add(GetEntity<T>(entityID));
			}

			// Return the collection
			return (T[])entities.ToArray(typeof(T));
		}

		/// <summary>
		/// Retrieves the specified entity from the DB.
		/// </summary>
		/// <param name="entityID">The ID of the entity to retrieve.</param>
		/// <returns>A BaseEntity object containing the requested info.</returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        static public T GetEntity<T>(Guid entityID)
        	where T : IEntity
		{
            // If the ID is empty return null
            if (entityID == Guid.Empty)
            	return default(T);

            return (T)DataAccess.Data.GetEntity<T>("ID", entityID);
		}
		#endregion

		#region Save functions
		/// <summary>
		/// Saves the provided entity to the DB.
		/// </summary>
		/// <param name="entity">The entity to save.</param>
		/// <returns>A boolean value indicating whether the entityname is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
        static public void SaveEntity(Entities.BaseEntity entity)
        {
            // Save the object.
            DataAccess.Data.Stores[entity.GetType()].Save(entity);
        }
		#endregion

		#region Update functions
		/// <summary>
		/// Updates the provided entity to the DB.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		/// <returns>A boolean value indicating whether the entityname is taken.</returns>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
        static public void UpdateEntity(Entities.BaseEntity entity)
        {
            // Update the object.
            DataAccess.Data.Stores[entity.GetType()].Update(entity);
        }
		#endregion

		#region Delete functions
		/// <summary>
		/// Deletes the provided entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        static public void DeleteEntity(Entities.BaseEntity entity)
		{
            if (entity != null)
            {
                DataAccess.Data.Stores[entity.GetType()].Delete(entity);
            }
		}
		#endregion

        static public Type GetType(string type)
        {
            return Type.GetType(type);
        }

		static public void Compress(BaseEntity entity)
		{
			// TODO: Add compression functionality. Remove all referenced objects and leave only their IDs.
		}

        /// <summary>
        /// Retrieves the value of the specified property on the provided entity.
        /// </summary>
        /// <param name="entity">The entity to retrieve the property value from.</param>
        /// <param name="propertyName">The name of the property to retrieve the value of.</param>
        /// <returns>The value of the specified property on the provided entity.</returns>
        static public object GetPropertyValue(IEntity entity, string propertyName)
        {
            // todo: add unit testing
            PropertyInfo property = entity.GetType().GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("The provided property '" + propertyName + "' was not found on the entity type '" + entity.GetType().ToString() + "'.");
            return property.GetValue(entity, null);
        }
	}
}
