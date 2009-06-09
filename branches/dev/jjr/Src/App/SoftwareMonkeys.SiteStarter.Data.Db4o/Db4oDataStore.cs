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
using SoftwareMonkeys.SiteStarter.State;
using System.IO;

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
		
		/// <summary>
		/// Gets/sets the corresponding db4o object container.
		/// </summary>
		public IObjectServer ObjectServer
		{
			get
			{
				IObjectServer server = (IObjectServer)StateAccess.State.GetApplication(ObjectServerKey);
				if (server == null)
					OpenServer();
				return (IObjectServer)StateAccess.State.GetApplication(ObjectServerKey);
			}
			set {
				StateAccess.State.SetApplication(ObjectServerKey, value);}
		}
		
		public string ObjectServerKey
		{
			get
			{
				string key = "ObjectServer_" + this.Name;
				string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
				if (prefix != null && prefix != String.Empty)
				{
					key = prefix + "_" + key;
				}
				return key;
			}
		}

		public string ObjectContainerKey
		{
			get
			{
				string key = "ObjectContainer_" + this.Name;
				string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
				if (prefix != null && prefix != String.Empty)
				{
					key = prefix + "_" + key;
				}
				return key;
			}
		}

		/// <summary>
		/// Gets/sets the corresponding db4o object container.
		/// </summary>
		public IObjectContainer ObjectContainer
		{
			get
			{
				IObjectContainer objectContainer = (IObjectContainer)StateAccess.State.GetApplication(ObjectContainerKey);
				if (objectContainer == null)
					OpenContainer();
				
				return (IObjectContainer)StateAccess.State.GetApplication(ObjectContainerKey);
				//return (IObjectContainer)StateAccess.State.GetApplication(ObjectContainerKey);
				//return ObjectServer.OpenClient();
			}
			set { StateAccess.State.SetApplication(ObjectContainerKey, value); }
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

		public void OpenServer()
		{
			string fileName = Name;

			string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
			if (prefix != null && prefix != String.Empty && prefix != Guid.Empty.ToString())
			{
				fileName = @"VS\" + prefix + @"\" + fileName;
			}

			string fullName = Config.Application.PhysicalPath + @"\App_Data\" + fileName + ".yap";
			if (!Directory.Exists(Path.GetDirectoryName(fullName)))
				Directory.CreateDirectory(Path.GetDirectoryName(fullName));
			ObjectServer = Db4oFactory.OpenServer(fullName, 0);
			//objectContainer = ObjectServer.OpenClient();
		}
		
		public void Open()
		{
		}
		
		public void OpenContainer()
		{
			string fileName = Name;

			string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
			if (prefix != null && prefix != String.Empty && prefix != Guid.Empty.ToString())
			{
				fileName = @"VS\" + prefix + @"\" + fileName;
			}

			string fullName = Config.Application.PhysicalPath + @"\App_Data\" + fileName + ".yap";
			if (!Directory.Exists(Path.GetDirectoryName(fullName)))
				Directory.CreateDirectory(Path.GetDirectoryName(fullName));
			//ObjectServer = Db4oFactory.OpenServer(fullName, 0);
			ObjectContainer = ObjectServer.OpenClient();//Db4oFactory.OpenFile(fullName);
		}

		public void Dispose()
		{
			Close();
			StateAccess.State.SetApplication(ObjectContainerKey, null);
			//objectServer = null;
		}

		public void Close()
		{
			if (ObjectContainer != null && !ObjectContainer.Ext().IsClosed())
			{
				ObjectContainer.Commit();
				ObjectContainer.Close();
			}
			//ObjectServer.Close();
		}

		#endregion

		public IEntity[] PreSave(IEntity entity)
		{
			List<IEntity> toSave = new List<IEntity>();

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
								if (referenceValue is IEntity[])
								{
									toSave.AddRange(PreSaveEntitiesReference(entity, property, reference));
								}
								// Otherwise the type is a Guid array
								else if (referenceValue is Guid[])
								{
									toSave.AddRange(PreSaveIDsReference(entity, property, reference));
								}
								// If the property type is an entity
								else if (referenceValue is IEntity)
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

			return (IEntity[])toSave.ToArray();
		}

		public void Save(IEntity entity)
		{
			IEntity[] toUpdate = PreSave(entity);

			foreach (object entityToUpdate in toUpdate)
			{
				DataAccess.Data.Stores[entityToUpdate.GetType()].Update((IEntity)entityToUpdate);
			}

			if (entity != null)
			{
				ObjectContainer.Store(entity);
				ObjectContainer.Commit();
			}
		}

		public IEntity[] PreUpdate(IEntity entity)
		{
			System.Diagnostics.Trace.WriteLine(entity.GetType() + ": " + entity.ToString(), "Db4oHelper.Update");

			List<IEntity> toUpdate = new List<IEntity>();

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
								if (referenceValue is IEntity[])
								{
									toUpdate.AddRange(PreUpdateEntitiesReference(entity, property, reference));
								}
								// Otherwise the type is a Guid array
								else if (referenceValue is Guid[])
								{
									toUpdate.AddRange(PreUpdateIDsReference(entity, property, reference));
								}
								// If the property type is an entity
								else if (referenceValue is IEntity)
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

			return (IEntity[])toUpdate.ToArray();
		}

		public void Update(IEntity entity)
		{
			// Preupdate must be called to ensure all references are correctly stored
			IEntity[] toUpdate = PreUpdate(entity);

			// TODO: Update entities
			foreach (IEntity entityToUpdate in toUpdate)
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

		public IEntity[] PreDelete(IEntity entity)
		{

			List<IEntity> toUpdate = new List<IEntity>();

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
								if (referenceValue is IEntity[])
								{
									toUpdate.AddRange(PreDeleteEntitiesReference(entity, property, reference));
								}
								// Otherwise the type is a Guid array
								else if (referenceValue is Guid[])
								{
									toUpdate.AddRange(PreDeleteIDsReference(entity, property, reference));
								}
								// If the property type is an entity
								else if (referenceValue is IEntity)
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

			return (IEntity[])toUpdate.ToArray();
		}

		/// <summary>
		/// Deletes the provided entity and all referenced entities (where marked for cascading deletion).
		/// </summary>
		/// <param name="entity"></param>
		public void Delete(IEntity entity)
		{
			// Preupdate must be called to ensure all references are correctly stored
			IEntity[] toUpdate = PreDelete(entity);

			// Update all modified entities
			foreach (IEntity entityToUpdate in toUpdate)
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

		public bool IsStored(IEntity entity)
		{
			return ObjectContainer.Ext().IsStored(entity);
		}

		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public T[] GetEntities<T>()
			where T : IEntity
		{
			IQuery query = ObjectContainer.Query();
			query.Constrain(typeof(T));

			List<T> list = new List<T>();

			IObjectSet os = query.Execute();
			while (os.HasNext())
			{
				list.Add((T)os.Next());
			}

			return (T[])list.ToArray();
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the object set.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="os">The object set to retrieve the entities from.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public T[] GetEntities<T>(IObjectSet os)
		{
			ArrayList list = new ArrayList();

			while (os.HasNext())
			{
				list.Add(os.Next());
			}

			return (T[])list.ToArray(typeof(T));
		}
		
		/// <summary>
		/// Retrieves the entity with the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public IEntity GetEntity(Type type, IDictionary<string, object> parameters)
		{
			return (IEntity)Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
			                                              "GetEntity", // Method name
			                                              new Type[] {type}, // Generic types
			                                              new object[] {parameters}); // Method arguments);
		}
		
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="ids"></param>
		/// <returns></returns>
		public IEntity[] GetEntities(Type type, Guid[] ids)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
			                                              "GetEntities", // Method name
			                                              new Type[] {type}, // Generic types
			                                              new object[] {ids}); // Method arguments);
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="entityIDs">The IDs of the entities to retrieve.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public T[] GetEntities<T>(Guid[] entityIDs)
			where T : IEntity
		{
			List<T> list = new List<T>(ObjectContainer.Query<T>(delegate(T e)
			                                                    {
			                                                    	return Array.IndexOf(entityIDs, e.ID) > -1;
			                                                    }));
			return (T[])list.ToArray();
		}
		
		/// <summary>
		/// Retrieves the entity with the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns></returns>
		public IEntity GetEntity(Type type, string propertyName, object propertyValue)
		{
			return (IEntity)Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
			                                              "GetEntity", // Method name
			                                              new Type[] {type}, // Generic types
			                                              new object[] {propertyName, propertyValue}); // Method arguments);
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		T IDataStore.GetEntity<T>(string propertyName, object propertyValue)
		{
			T[] entities = GetEntities<T>(propertyName, propertyValue);

			if (entities == null || entities.Length == 0)
				return default(T);
			else
				return entities[0];
		}
		
		
		/// <summary>
		/// Retrieves the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The matching entities.</returns>
		IEntity[] IDataStore.GetEntities(Type type, string propertyName, object propertyValue)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
			                                              "GetEntities", // Method name
			                                              new Type[] {type}, // Generic types
			                                              new object[] {propertyName, propertyValue}); // Method arguments);
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		T[] IDataStore.GetEntities<T>(string propertyName, object propertyValue)
		{
			List<T> entities = new List<T>();
			
			IList<T> results = ObjectContainer.Query<T>(delegate(T e)
			                                            {
			                                            	if (e.GetType() == typeof(T))
			                                            	{
			                                            		PropertyInfo property = e.GetType().GetProperty(propertyName);
			                                            		object value = property.GetValue(e, null);
			                                            		
			                                            		if (value.Equals(propertyValue))
			                                            			return true;
			                                            	}
			                                            	
			                                            	return false;
			                                            });
			
			entities.AddRange(results);
			
			return (T[])entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public IEntity[] GetEntities(Type type, IDictionary<string, object> parameters)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
			                                                "GetEntities", // Method name
			                                                new Type[] {type}, // Generic types
			                                                new object[] {parameters}); // Method arguments);
		}

		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public T[] GetEntities<T>(string propertyName, object propertyValue)
			where T : IEntity
		{
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add(propertyName, propertyValue);
			
			return (T[])GetEntities<T>(parameters);
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public T[] GetEntities<T>(IDictionary<string, object> parameters)
			where T : IEntity
		{
			
			List<T> entities = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			{

				entities = new List<T>(ObjectContainer.Query<T>(delegate(T e)
				                                                {
				                                                	AppLogger.Debug("Checking type " + e.GetType().ToString());
				                                                	
				                                                	bool matches = true;
				                                                	foreach (string key in parameters.Keys)
				                                                	{
				                                                		AppLogger.Debug("Checking parameter '" + key + "' for value '" + parameters[key].ToString() + "'");
				                                                		
				                                                		PropertyInfo property = e.GetType().GetProperty(key, parameters[key].GetType());
				                                                		if (property == null)
				                                                			throw new InvalidOperationException("The property '" + key + "' was not found on the type " + e.GetType().ToString());
				                                                		else
				                                                		{
				                                                			object value = property.GetValue(e, null);
				                                                			
				                                                			AppLogger.Debug("Actual value is: " + (value == null ? "null" : value.ToString()));
				                                                			
				                                                			if (parameters[key] != value && parameters[key] != null && !parameters[key].Equals(value))
				                                                			{
				                                                				AppLogger.Debug("Parameter match failed for '" + key + "'.");
				                                                				matches = false;
				                                                			}
				                                                		}
				                                                	}
				                                                	AppLogger.Debug("Matches: " + matches.ToString());
				                                                	return matches;
				                                                }));


		
			}

			return (T[])entities.ToArray();
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public T GetEntity<T>(IDictionary<string, object> parameters)
			where T : IEntity
		{
			T[] entities = GetEntities<T>(parameters);
			if (entities == null || entities.Length == 0)
				return default(T);
			else
				return entities[0];
		}

		/* /// <summary>
        /// Retrieves the entity of the specified type matching the specified values.
        /// </summary>
        /// <param name="type">The type of entity to retrieve.</param>
        /// <param name="parameters">The parameters to query with.</param>
        /// <returns></returns>
        public IEntity GetEntity(Type type, IDictionary<string, object> parameters)
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
                return (IEntity)os.Next();
            }

            return null;
        }*/

		/// <summary>
		/// Retrieves all of the entities from the data store.
		/// </summary>
		/// <returns>An array of all the entities in the data store.</returns>
		public IEntity[] GetAllEntities()
		{
			ActiveQuery = ObjectContainer.Query();

			ArrayList list = new ArrayList();

			IObjectSet os = ActiveQuery.Execute();
			while (os.HasNext())
			{
				object obj = os.Next();
				if (obj is IEntity)
				{
					list.Add((IEntity)obj);
				}
			}

			return (IEntity[])list.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="pageIndex">The index of the page to retrieve.</param>
		/// <param name="pageSize">The size of each page.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <param name="totalObjects">The total number of objects found.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public T[] GetPage<T>(int pageIndex, int pageSize, string sortExpression, out int totalObjects)
			where T : IEntity
		{
			ActiveQuery = ObjectContainer.Query();
			ActiveQuery.Constrain(typeof(T));

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
			return (T[])page.ToArray(typeof(T));
		}

		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="fieldName">The name of the field to query for.</param>
		/// <param name="propertyValue">The value of the field to query for.</param>
		/// <param name="pageIndex">The index of the page to retrieve.</param>
		/// <param name="pageSize">The size of each page.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <param name="totalObjects">The total number of objects found.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public T[] GetPage<T>(string fieldName, object propertyValue, int pageIndex, int pageSize, string sortExpression, out int totalObjects)
			where T : IEntity
		{
			ActiveQuery = ObjectContainer.Query();
			ActiveQuery.Constrain(typeof(T));
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
			return (T[])page.ToArray(typeof(T));
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
		/// <returns>The newly created query.</returns>
		public IQuery Query<T>()
		{
			IQuery query = ObjectContainer.Query();
			query.Constrain(typeof(T));
			return query;
		}

		/*public IEntity GetEntity(IObjectSet os)
		{
			throw new NotImplementedException();
		}*/
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
		protected IEntity[] PreSaveEntitiesReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing entities reference to be saved.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Property name: " + property.Name);
				AppLogger.Debug("Property type: " + property.PropertyType.ToString());

				Collection<IEntity> toUpdate = new Collection<IEntity>();

				object referenceValue = property.GetValue(entity, null);

				// Check if the save is to cascade
				if (attribute.CascadeSave)
				{
					AppLogger.Debug("attribute.CascadeSave == true");
					// Save the reference entities
					foreach (IEntity referencedEntity in (IEntity[])referenceValue)
					{
						// Delete the original referenced entity
						Delete(DataAccess.Data.GetEntity(referencedEntity.GetType(), "ID", referencedEntity.ID));

						toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));

						// Save the new referenced entity
						DataAccess.Data.Save((IEntity)referencedEntity);
					}
				}
				else
				{
					AppLogger.Debug("attribute.CascadeSave == false");


					toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(DataUtilities.GetReferenceType(entity, property), DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property)));
				}

				return (IEntity[])toUpdate.ToArray(typeof(IEntity));
			}
		}

		/// <summary>
		/// Prepares the provided reference for update. Does NOT synchronise mirrors because that's done by the IDs references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreSaveEntityReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute reference)
		{
			Collection<IEntity> toUpdate = new Collection<IEntity>();
			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (reference.CascadeUpdate)
			{
				IEntity referencedEntity = (IEntity)referenceValue;

				// Delete the original referenced entity
				Delete(DataAccess.Data.GetEntity(referencedEntity.GetType(),"ID", referencedEntity.ID));

				toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));

				// Save the new referenced entity
				DataAccess.Data.Save((IEntity)referenceValue);
			}
			else
			{
				// If the reference is not being stored by an IDs property, a mirror is specified, and the property is not to be excluded
				if (reference.IDsPropertyName == String.Empty && reference.MirrorName != String.Empty)
				{
					toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity,
					                                                        property,
					                                                        DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(DataUtilities.GetReferenceType(entity, property),
					                                                                                                                                                         DataUtilities.GetReferenceIDs(entity, property)),					                                                        DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property)));

					//toUpdate.Add(DataUtilities.AddReferences((IEntity)referenceValue, entity, DataUtilities.GetMirrorPropertyName(property)));

					// Set a bound copy of the referenced object to the property to ensure it won't get duplicated
					//property.SetValue(entity, GetEntity(property.PropertyType, "id", ((IEntity)referenceValue).ID), null);
				}

				if (reference.ExcludeFromDataStore)
					property.SetValue(entity, null, null);
			}

			return (IEntity[])toUpdate.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for update. Synchronises mirror references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreSaveIDsReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (property == null)
				throw new ArgumentNullException("property");

			if (attribute == null)
				throw new ArgumentNullException("attribute");

			Collection<IEntity> toUpdate = new Collection<IEntity>();

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
						IEntity referencedEntity = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", referencedEntityID);
						
						// Delete the original referenced entity
						DataAccess.Data.Delete(referencedEntity);

						toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));
						
						// Save the new referenced entity
						DataAccess.Data.Save((IEntity)referencedEntity);
					}
				}
				else
				{
					AppLogger.Debug("attribute.CascadeSave == false");

					Type type = entity.GetType();

					PropertyInfo entitiesProperty = DataUtilities.GetEntitiesProperty(property);
					
					if (entitiesProperty == null)
					{
						throw new Exception("The entities property '" + attribute.EntitiesPropertyName + "' of type '" + attribute.EntitiesPropertyType.ToString() + "' could not be found on the type '" + type.ToString() + "'.");
					}

					Type referenceEntityType = entitiesProperty.PropertyType.GetElementType();
					
					if (attribute.MirrorName != String.Empty)
					{
						
						PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(entity, property);
						
						if (mirrorProperty == null)
							throw new Exception("Mirror property '" + attribute.MirrorName + "' not found on the type '" + referenceEntityType.ToString());
						
						AppLogger.Debug("Mirror property: " + mirrorProperty.Name);
						AppLogger.Debug("Mirror property type: " + mirrorProperty.PropertyType);
						AppLogger.Debug("Reference entity type: " + referenceEntityType.ToString());
						
						toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[referenceEntityType].GetEntities(referenceEntityType, DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property)));
						
						
					}
					else
						AppLogger.Debug("No mirror property name specified.");
				}
				
			}

			return (IEntity[])toUpdate.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for update. Synchronises mirror references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreSaveIDReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			System.Diagnostics.Trace.WriteLine("Db4oDataStore.PreSaveIDReference");
			System.Diagnostics.Trace.Indent();

			Collection<IEntity> toUpdate = new Collection<IEntity>();

			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (attribute.CascadeSave)
			{
				IEntity referencedEntity = null;
				if (referenceValue is Guid)
				{
					referencedEntity = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", (Guid)referenceValue);
				}
				else if (referenceValue is IEntity)
				{
					referencedEntity = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", ((IEntity)referenceValue).ID);
				}

				toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));

				// Save the new referenced entity
				Save(referencedEntity);
			}
			else
			{
				//IEntity reference = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", (Guid)property.GetValue(entity, null));

				Type referenceEntityType = DataUtilities.GetReferenceType(entity, property);
				//entity.GetType().GetProperty(attribute.EntitiesPropertyName).PropertyType.GetElementType();

				if (attribute.MirrorName != String.Empty)
				{
					toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[referenceEntityType].GetEntities(referenceEntityType, DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property)));
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
                                    toUpdate.Add(DataUtilities.AddReferences(reference, entity, DataUtilities.GetMirrorPropertyName(property)));

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
                                    toUpdate.Add(DataUtilities.AddReferences(reference, entity, DataUtilities.GetMirrorPropertyName(property)));

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

			return (IEntity[])toUpdate.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for update. Does NOT synchronise mirrors because that's done by the IDs references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreUpdateEntitiesReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			Collection<IEntity> toUpdate = new Collection<IEntity>();

			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (attribute.CascadeUpdate)
			{
				// Save the reference entities
				foreach (IEntity referencedEntity in (IEntity[])referenceValue)
				{
					// Delete the original referenced entity
					DataAccess.Data.Delete(DataAccess.Data.GetEntity(referencedEntity.GetType(), "ID", referencedEntity.ID));

					toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));

					// Save the new referenced entity
					DataAccess.Data.Save((IEntity)referencedEntity);
				}
			}
			else
			{
				Type referenceEntityType = entity.GetType().GetProperty(property.Name).PropertyType.GetElementType();

				IEntity[] newReferences = (IEntity[])property.GetValue(entity, null);

				if (attribute.MirrorName != String.Empty)
				{
					toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[referenceEntityType].GetEntities(referenceEntityType, DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property)));

					

				}
				else
					AppLogger.Debug("Mirror name is blank. Skipping.");

			}

			return (IEntity[])toUpdate.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for update. Does NOT synchronise mirrors because that's done by the IDs references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreUpdateEntityReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute reference)
		{
			Collection<IEntity> toUpdate = new Collection<IEntity>();

			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (reference.CascadeUpdate)
			{
				IEntity referencedEntity = (IEntity)referenceValue;

				// Delete the original referenced entity
				DataAccess.Data.Delete(DataAccess.Data.GetEntity(referencedEntity.GetType(), "ID", referencedEntity.ID));

				toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));

				// Save the new referenced entity
				DataAccess.Data.Save((IEntity)referenceValue);
			}
			else
			{
				toUpdate.Add(DataUtilities.AddReferences((IEntity)referenceValue, entity, DataUtilities.GetMirrorProperty(entity, property)));

				// Set a bound copy of the referenced object to the property to ensure it won't get duplicated
				property.SetValue(entity, DataAccess.Data.GetEntity(property.PropertyType, "ID", ((IEntity)referenceValue).ID), null);
			}

			return (IEntity[])toUpdate.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for update. Synchronises mirror references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreUpdateIDsReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			Collection<IEntity> toUpdate = new Collection<IEntity>();

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
						IEntity referencedEntity = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", referencedEntityID);
						
						// Delete the original referenced entity
						DataAccess.Data.Delete(referencedEntity);
						
						toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));
						
						// Save the new referenced entity
						DataAccess.Data.Save((IEntity)referencedEntity);
					}
				}
				else
				{
					AppLogger.Debug("Cascade update: false");

					Type referenceEntityType = DataUtilities.GetReferenceType(entity, property);
					
					IEntity[] oldReferences = (IEntity[])DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property);
					//Entity[] originalReferences = GetEntities(referenceEntityType, query.Execute());

					AppLogger.Debug("# of old references: " + oldReferences.Length.ToString());
					
					if (attribute.MirrorName != String.Empty)
					{
						toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(referenceEntityType, DataUtilities.GetReferenceIDs(entity, property)), oldReferences));
					}
					
					
					// Set a bound copy of the referenced object to the property to ensure it won't get duplicated
					/*if (references != null && references.Count > 0)
	                    property.SetValue(entity, references.GetIDs(), null);
	                else
	                    property.SetValue(entity, null, null);*/
				}

			}

			return (IEntity[])toUpdate.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for update. Synchronises mirror references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreUpdateIDReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			Collection<IEntity> toUpdate = new Collection<IEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Preparing ID reference for update.", NLog.LogLevel.Debug))
			{
				object referenceValue = property.GetValue(entity, null);

				// Check if the save is to cascade
				if (attribute.CascadeUpdate)
				{
					AppLogger.Debug("attribute.CascadeUpdate == true");

					IEntity referencedEntity = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", (Guid)referenceValue);

					// Delete the original referenced entity
					DataAccess.Data.Delete(referencedEntity);

					toUpdate.Add(DataUtilities.AddReferences(referencedEntity, entity, DataUtilities.GetMirrorProperty(entity, property)));

					// Save the new referenced entity
					DataAccess.Data.Save((IEntity)referencedEntity);
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
						PropertyInfo mirrorProperty = referenceEntityType.GetProperty(DataUtilities.GetMirrorPropertyName(property));

						AppLogger.Debug("Mirror property name: " + attribute.MirrorName);


						toUpdate.Add(DataUtilities.SynchroniseReverseReferences(entity, property, DataAccess.Data.Stores[DataUtilities.GetDataStoreNameForReference(entity, property)].GetEntities(referenceEntityType, DataUtilities.GetReferenceIDs(entity, property)), DataAccess.Data.GetEntitiesContainingReverseReferences(entity, property)));
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

			return (IEntity[])toUpdate.ToArray(typeof(IEntity));
		}

		protected IEntity[] PreDeleteEntitiesReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			Collection<IEntity> toDelete = new Collection<IEntity>();

			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (attribute.CascadeDelete)
			{
				// Save the reference entities
				foreach (IEntity referencedEntity in (IEntity[])referenceValue)
				{
					// Delete the original referenced entity
					DataAccess.Data.Delete(DataAccess.Data.GetEntity(referencedEntity.GetType(), "ID", referencedEntity.ID));
				}
			}
			else
			{
				Type referenceEntityType = entity.GetType().GetProperty(property.Name).PropertyType.GetElementType();

				IEntity[] newReferences = (IEntity[])property.GetValue(entity, null);

				PropertyInfo mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);

				IList<IEntity> originalEntities = ObjectContainer.Query<IEntity>(delegate(IEntity e)
				                                                                 {
				                                                                 	if (mirrorProperty != null && e.GetType() == referenceEntityType)
				                                                                 	{
				                                                                 		object mirrorValue = mirrorProperty.GetValue(e, null);
				                                                                 		if (mirrorValue != null && mirrorValue is IEntity[])
				                                                                 			return Array.IndexOf(Collection<IEntity>.GetIDs((IEntity[])mirrorValue), entity.ID) > -1;
				                                                                 		else
				                                                                 			return false;
				                                                                 	}
				                                                                 	else
				                                                                 		return false;
				                                                                 });

				IEntity[] originalReferences = (IEntity[])new List<IEntity>(originalEntities).ToArray();

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
							if ((mirrorProperty == null || Array.IndexOf(Collection<IEntity>.GetIDs((IEntity[])mirrorValue), entity.ID) > -1)
							    && Array.IndexOf(Collection<IEntity>.GetIDs(newReferences), originalReferences[i].ID) == -1)
								toDelete.Add(DataUtilities.RemoveReferences(originalReferences[i], entity, DataUtilities.GetMirrorProperty(entity, property)));
						}
					}
				}
			}

			return (IEntity[])toDelete.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for delete. Does NOT synchronise mirrors because that's done by the IDs references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreDeleteEntityReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute reference)
		{
			Collection<IEntity> toDelete = new Collection<IEntity>();

			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (reference.CascadeDelete)
			{
				IEntity referencedEntity = (IEntity)referenceValue;

				// Delete the original referenced entity
				DataAccess.Data.Delete(DataAccess.Data.GetEntity(referencedEntity.GetType(), "ID", referencedEntity.ID));
			}
			else
			{
				// Don't do anything
			}

			return (IEntity[])toDelete.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for delete. Synchronises mirror references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreDeleteIDsReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			Collection<IEntity> toDelete = new Collection<IEntity>();

			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (attribute.CascadeDelete)
			{
				// Save the reference entities
				foreach (Guid referencedEntityID in (Guid[])referenceValue)
				{
					IEntity referencedEntity = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", referencedEntityID);

					// Delete the original referenced entity
					DataAccess.Data.Delete(referencedEntity);
				}
			}
			else
			{
				Type referenceEntityType = entity.GetType().GetProperty(attribute.EntitiesPropertyName).PropertyType.GetElementType();

				PropertyInfo mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);

				IList<IEntity> entities = ObjectContainer.Query<IEntity>(delegate(IEntity e)
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

				IEntity[] originalReferences = (IEntity[])new List<IEntity>(entities).ToArray();
				//Entity[] originalReferences = GetEntities(referenceEntityType, query.Execute());

				Collection<IEntity> references = new Collection<IEntity>();

				references.Add(GetEntities(referenceEntityType, (Guid[])property.GetValue(entity, null)));

				// If a reference exists in both old and new copy then keep it in the new
				if (originalReferences != null)
				{
					for (int i = 0; i < originalReferences.Length; i++)
					{
						// If the reference is not still being kept then allow it to be removed
						if (!references.Contains(originalReferences[i]))
						{
							// Delete the mirror references
							toDelete.Add(DataUtilities.RemoveReferences(originalReferences[i], entity, DataUtilities.GetMirrorProperty(entity, property)));
						}
					}
				}
			}

			return (IEntity[])toDelete.ToArray(typeof(IEntity));
		}

		/// <summary>
		/// Prepares the provided reference for delete. Synchronises mirror references.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		protected IEntity[] PreDeleteIDReference(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			Collection<IEntity> toDelete = new Collection<IEntity>();

			object referenceValue = property.GetValue(entity, null);

			// Check if the save is to cascade
			if (attribute.CascadeDelete)
			{
				IEntity referencedEntity = DataAccess.Data.GetEntity(DataUtilities.GetReferenceType(entity, property), "ID", (Guid)referenceValue);

				// Delete the original referenced entity
				DataAccess.Data.Delete(referencedEntity);
			}
			else
			{
				PropertyInfo entitiesProperty = entity.GetType().GetProperty(attribute.EntitiesPropertyName);
				Type referenceEntityType = null;
				if (entitiesProperty.PropertyType.GetElementType() != null)
					referenceEntityType = entitiesProperty.PropertyType.GetElementType();
				else
					referenceEntityType = entitiesProperty.PropertyType;

				Collection<IEntity> references = new Collection<IEntity>();

				if (attribute.MirrorName != String.Empty)
				{
					PropertyInfo mirrorProperty = referenceEntityType.GetProperty(attribute.MirrorName);

					IList<IEntity> entities = ObjectContainer.Query<IEntity>(delegate(IEntity e)
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

					IEntity[] originalReferences = (IEntity[])new List<IEntity>(entities).ToArray();
					//Entity[] originalReferences = GetEntities(referenceEntityType, query.Execute());

					object propertyValue = property.GetValue(entity, null);

					if (propertyValue is Guid[])
						references.Add(GetEntities(referenceEntityType, (Guid[])propertyValue));
					else
					{
						references.Add(DataAccess.Data.GetEntity(referenceEntityType, "ID", (Guid)propertyValue));
					}

					// If a reference exists in both old and new copy then keep it in the new
					if (originalReferences != null)
					{
						for (int i = 0; i < originalReferences.Length; i++)
						{
							// If the reference is not still being kept then allow it to be removed
							if (!references.Contains(originalReferences[i]))
							{
								// Delete the mirror references
								toDelete.Add(DataUtilities.RemoveReferences(originalReferences[i], entity, DataUtilities.GetMirrorProperty(entity, property)));
							}
						}
					}
				}
			}

			return (IEntity[])toDelete.ToArray(typeof(IEntity));
		}

	}
}
