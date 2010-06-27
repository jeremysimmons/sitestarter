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
using Db4objects.Db4o.Config;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	public class Db4oDataStore : IDataStore
	{
		private IConfiguration db4oConfiguration;
		
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
		
		public Db4oDataStore()
		{
			
		}
		
		public Db4oDataStore(IConfiguration db4oConfiguration)
		{
			this.db4oConfiguration = db4oConfiguration;
		}

		public void OpenServer()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Opening data server.", NLog.LogLevel.Info))
			{
				Db4oFactory.Configure().ActivationDepth(2);
				
				Db4oFactory.Configure().ObjectClass(typeof(IEntity)).ObjectField("id").Indexed(true);
				
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("property1Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("type1Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("entity1ID").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("property2Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("type2Name").Indexed(true);
				Db4oFactory.Configure().ObjectClass(typeof(EntityIDReference)).ObjectField("entity2ID").Indexed(true);
				
				string fileName = Name;
				
				AppLogger.Debug("Store name: " + Name);

				string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
				if (prefix != null && prefix != String.Empty && prefix != Guid.Empty.ToString())
				{
					fileName = @"VS\" + prefix + @"\" + fileName;
				}

				string fullName = Config.Application.PhysicalPath + @"\App_Data\" + fileName + ".yap";
				
				
				AppLogger.Debug("Full file name: " + fullName);
				
				if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					Directory.CreateDirectory(Path.GetDirectoryName(fullName));
				
				ObjectServer = Db4oFactory.OpenServer(db4oConfiguration,
				                                      fullName, 0);
				
				AppLogger.Debug("Server opened");
				//objectContainer = ObjectServer.OpenClient();
			}
		}
		
		public void Open()
		{
		}
		
		public void OpenContainer()
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Opening data container.", NLog.LogLevel.Info))
			{
				string fileName = Name;
				
				AppLogger.Debug("Name: " + Name);

				string prefix = (string)StateAccess.State.GetSession("VirtualServerID");
				if (prefix != null && prefix != String.Empty && prefix != Guid.Empty.ToString())
				{
					fileName = @"VS\" + prefix + @"\" + fileName;
				}

				string fullName = Config.Application.PhysicalPath + @"\App_Data\" + fileName + ".yap";
				
				AppLogger.Debug("Full file name: " + fullName);
				
				if (!Directory.Exists(Path.GetDirectoryName(fullName)))
					Directory.CreateDirectory(Path.GetDirectoryName(fullName));
				//ObjectServer = Db4oFactory.OpenServer(fullName, 0);
				ObjectContainer = ObjectServer.OpenClient();//Db4oFactory.OpenFile(fullName);
				
				AppLogger.Debug("Container opened");
			}
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
				ObjectContainer = null;
				ObjectServer.Close();
				ObjectServer.Dispose();
				ObjectServer = null;
			}
			//ObjectServer.Close();
		}

		#endregion

		public void PreSave(IEntity entity, out IEntity[] entitiesToUpdate, out IEntity[] entitiesToDelete)
		{
			List<IEntity> toUpdate = new List<IEntity>();
			List<IEntity> toDelete = new List<IEntity>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing entity for saving: " + entity.GetType().ToString(), NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				Type entityType = entity.GetType();
				
				AppLogger.Debug("Entity type: " + entityType.ToString());
				
				foreach (EntityIDReference reference in EntitiesUtilities.GetRemovedReferenceIDs(entity))
				{
					toDelete.Add(reference);
				}
				
				foreach (EntityIDReference reference in EntitiesUtilities.GetReferences(entity))
				{
					toUpdate.Add(reference.ToData());
				}
				
				//  Clear all the references from the entity once they're ready to be saved separately
				foreach (PropertyInfo property in entityType.GetProperties())
				{
					if (EntitiesUtilities.IsReference(entityType, property.Name, property.PropertyType))
					{
						Reflector.SetPropertyValue(entity, property.Name, null);
					}
				}
				
				DataUtilities.StripReferences(entity);
			}
			
			entitiesToUpdate = toUpdate.ToArray();
			entitiesToDelete = toDelete.ToArray();
		}
		
		public void Save(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving entity.", NLog.LogLevel.Debug))
			{
				using (Batch batch = Batch.StartBatch())
				{
					if (EntitiesUtilities.IsReference(entity.GetType()) && DataAccess.Data.IsStored(entity))
					{
						AppLogger.Debug("Existing reference found. Skipping save.");
						// Just skip the saving altogether, if the reference already exists
					}
					else
					{
						if (entity == null)
							throw new ArgumentNullException("entity");
						
						AppLogger.Debug("Entity type: " + entity.GetType().ToString());
						AppLogger.Debug("Entity ID: " + entity.ID.ToString());
						
						IEntity[] toUpdate = new IEntity[]{};
						IEntity[] toDelete = new IEntity[]{};
						
						// Clone the entity, but do it in reverse so the data store is dealing with the bound instance.
						// The entity needs to be cloned so that the one currently in memory doesn't reflect the preparations applied before saving/updating.
						IEntity clonedEntity = entity;
						entity = clonedEntity.Clone();
						
						PreSave(clonedEntity, out toUpdate, out toDelete);

						// Update any entities that were modified (eg. references)
						foreach (IEntity entityToUpdate in toUpdate)
						{
							DataAccess.Data.Stores[entityToUpdate].Update((IEntity)entityToUpdate);
						}
						
						// Not necessary for saving
						// Delete any entities that are obsolete (eg. references)
						//foreach (IEntity entityToDelete in toDelete)
						//{
						//	DataAccess.Data.Stores[entityToDelete].Delete((IEntity)entityToDelete);
						//}


						if (clonedEntity != null)
						{
							// Save the entity
							ObjectContainer.Store(clonedEntity);
							Commit();
							
							AppLogger.Debug("Entity stored in '" + Name + "' store.");
						}
						else
						{
							AppLogger.Debug("Cloned entity == null. Not stored.");
							
						}
					}
				}
			}
		}

		public void PreUpdate(IEntity entity, out IEntity[] entitiesToUpdate, out IEntity[] entitiesToDelete)
		{
			List<IEntity> toUpdate = new List<IEntity>();
			List<IEntity> toDelete = new List<IEntity>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Preparting entity to be updated.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Entity ID : " + entity.ID);
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				EntityReferenceCollection latestReferences = EntitiesUtilities.GetReferences(entity);
				
				using (LogGroup logGroup2 = AppLogger.StartGroup("Creating list of deletable obsolete references.", NLog.LogLevel.Debug))
				{
					// Delete the old references
					//foreach (EntityIDReference reference in DataAccess.Data.GetObsoleteReferences(entity, new Collection<IEntity>(DataUtilities.GetReferencedEntities(latestReferences, entity)).IDs))
					foreach (EntityIDReference reference in DataAccess.Data.GetObsoleteReferences(entity, new Guid[]{}))
					{
						DataAccess.Data.ActivateReference((EntityReference)reference);
						
						AppLogger.Debug("Reference type #1: " + reference.Type1Name);
						AppLogger.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
						AppLogger.Debug("Reference type #2: " + reference.Type2Name);
						AppLogger.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
						
						toDelete.Add(reference);
					}
				}
				
				AppLogger.Debug("# to delete: " + toDelete.Count);
				
				using (LogGroup logGroup2 = AppLogger.StartGroup("Creating list of references to be updated.", NLog.LogLevel.Debug))
				{
					foreach (EntityIDReference reference in latestReferences)
					{
						AppLogger.Debug("Reference type #1: " + reference.Type1Name);
						AppLogger.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
						AppLogger.Debug("Reference type #2: " + reference.Type2Name);
						AppLogger.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
						
						/*EntityReference existingReference = DataAccess.Data.GetReference(
							EntitiesUtilites.GetType(reference.Type1Name),
							reference.Entity1ID,
							reference.Property1Name,
							EntitiesUtilities.GetType(reference.Type2Name),
							reference.Entity2ID,
							reference.Property2Name,
							false);
						
						if (existingReference != null)
							reference = existingReference;*/
						
						//if (!DataAccess.Data.IsStored(reference))
						//{
						
						Data.DataAccess.Data.ActivateReference((EntityReference)reference);
						toUpdate.Add(reference.ToData());
						//}
					}
				}
				
				AppLogger.Debug("# to update: " + toUpdate.Count);
				
				
				DataUtilities.StripReferences(entity);
				
				
				entitiesToUpdate = toUpdate.ToArray();
				entitiesToDelete = toDelete.ToArray();
			}
		}

		public void Update(IEntity entity)
		{
			IEntity[] toUpdate = new IEntity[]{};
			IEntity[] toDelete = new IEntity[]{};
			
			using (LogGroup logGroup = AppLogger.StartGroup("Updating the provided entity.", NLog.LogLevel.Debug))
			{
				using (Batch batch = Batch.StartBatch())
				{
					if (entity == null)
						throw new ArgumentNullException("entity");
					
					//DataAccess.Data.Activate(entity);
					
					AppLogger.Debug("Entity type: " + entity.GetType().ToString());
					AppLogger.Debug("Entity ID: " + entity.ID);
					
					// Clone the entity, but do it in reverse so the data store is dealing with the bound instance/
					// The entity needs to be cloned so that the one currently in memory doesn't reflect the preparations applied before saving/updating.
					IEntity clonedEntity = entity;
					entity = clonedEntity.Clone();
					
					AppLogger.Debug("Entity cloned");
					
					if (clonedEntity == entity)
						AppLogger.Debug("Cloned entity == original entity.");
					else
						AppLogger.Debug("Cloned entity == separate instance.");
					
					// Preupdate must be called to ensure all references are correctly stored
					PreUpdate(clonedEntity, out toUpdate, out toDelete);
					
					using (LogGroup logGroup2 = AppLogger.StartGroup("Deleting all entities that need to be deleted.", NLog.LogLevel.Debug))
					{
						// Delete any entities that are obsolete (eg. references)
						foreach (IEntity entityToDelete in toDelete)
						{
							DataAccess.Data.Stores[entityToDelete].Delete((IEntity)entityToDelete);
						}
					}
					
					using (LogGroup logGroup2 = AppLogger.StartGroup("Updating all other entities that need updating.", NLog.LogLevel.Debug))
					{
						// Update any entities that were modified (eg. references)
						foreach (IEntity entityToUpdate in toUpdate)
						{
							DataAccess.Data.Stores[entityToUpdate].Save((IEntity)entityToUpdate);
						}
					}
					
					if (clonedEntity != null)
					{
						ObjectContainer.Store(clonedEntity);
						AppLogger.Debug("Entity updated.");
						
						Commit();
						AppLogger.Debug("ObjectContainer committed.");
					}
				}
				
				
			}
		}

		public void PreDelete(IEntity entity, out IEntity[] entitiesToUpdate, out IEntity[] entitiesToDelete)
		{

			List<IEntity> toUpdate = new List<IEntity>();
			List<IEntity> toDelete = new List<IEntity>();

			if (entity != null)
			{
				EntityReferenceCollection latestReferences = EntitiesUtilities.GetReferences(entity);
				
				// Delete all references
				foreach (EntityIDReference reference in DataAccess.Data.GetObsoleteReferences(entity, new Guid[]  {}))
				{
					toDelete.Add(reference);
				}
				
			}
			
			entitiesToUpdate = toUpdate.ToArray();
			entitiesToDelete = toDelete.ToArray();
		}

		/// <summary>
		/// Deletes the provided entity and all referenced entities (where marked for cascading deletion).
		/// </summary>
		/// <param name="entity"></param>
		public void Delete(IEntity entity)
		{
			using (Batch batch = Batch.StartBatch())
			{
				IEntity[] toUpdate = new IEntity[]{};
				IEntity[] toDelete = new IEntity[]{};
				
				// The entity doesn't need to be cloned in this one
				
				// Preupdate must be called to ensure all references are correctly managed
				PreDelete(entity, out toUpdate, out toDelete);

				// Update any entities that were modified (eg. references)
				foreach (IEntity entityToUpdate in toUpdate)
				{
					DataAccess.Data.Stores[entityToUpdate].Update((IEntity)entityToUpdate);
				}
				
				// Delete any entities that are obsolete (eg. references)
				foreach (IEntity entityToDelete in toDelete)
				{
					DataAccess.Data.Stores[entityToDelete].Delete((IEntity)entityToDelete);
				}

				// Delete the entity
				if (entity != null)
				{
					ObjectContainer.Delete(entity);
					
					Commit();
				}
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
		public IEntity GetEntityByTypeAndProperties(Type type, IDictionary<string, object> parameters)
		{
			//return (IEntity)Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
			//                                              "GetEntity", // Method name
			//                                              new Type[] {type}, // Generic types
			//                                              new object[] {parameters}); // Method arguments);
			
			IEntity[] entities = GetEntitiesByTypeAndProperties(type, parameters);
			if (entities == null || entities.Length == 0)
			{
				return null;
			}
			else
			{
				return entities[0];
			}
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
		public IEntity GetEntityByTypeAndProperty(Type type, string propertyName, object propertyValue)
		{
			IEntity entity = null;
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the entity matching the specified type and property value.", NLog.LogLevel.Debug))
			//{
			/*entity = (IEntity)Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
				                                                "GetEntity", // Method name
				                                                new Type[] {type}, // Generic types
				                                                new object[] {propertyName, propertyValue}); // Method arguments);*/
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add(propertyName, propertyValue);
			
			entity = GetEntityByTypeAndProperties(type, parameters);
			
			//if (entity == null)
			//AppLogger.Debug("Retrieved entity: [null]");
			//else
			//{
			//AppLogger.Debug("Retrieved entity ID: " + entity.ID);
			//AppLogger.Debug("Retrieved entity type: " + entity.GetType().ToString());
			//AppLogger.Debug("Retrieved entity: " + entity.ToString());
			//}
			
			//}
			
			return entity;
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
		IEntity[] IDataStore.GetEntitiesByTypeAndProperty(Type type, string propertyName, object propertyValue)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			if (propertyName == null || propertyName == String.Empty)
				throw new ArgumentNullException("propertyName");
			
			List<IEntity> entities = new List<IEntity>();
			
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			
			parameters.Add(propertyName, propertyValue);
			
			entities.AddRange(GetEntitiesByTypeAndProperties(type, parameters));
			
			/*IList<IEntity> results = ObjectContainer.Query<IEntity>(delegate(IEntity e)
			                                            {
			                                            	if (e.GetType() == typeof(IEntity))
			                                            	{
			                                            		PropertyInfo property = e.GetType().GetProperty(propertyName);
			                                            		object value = property.GetValue(e, null);
			                                            		
			                                            		if (value == propertyValue || value.Equals(propertyValue))
			                                            			return true;
			                                            	}
			                                            	
			                                            	return false;
			                                            });
			
			entities.AddRange(results);*/
			
			return (IEntity[])entities.ToArray();
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
		public IEntity[] GetEntitiesByTypeAndProperties(Type type, IDictionary<string, object> parameters)
		{
			/*return (IEntity[])Reflector.InvokeGenericMethod(DataAccess.Data, // Source object
			                                                "GetEntities", // Method name
			                                                new Type[] {type}, // Generic types
			                                                new object[] {parameters}); // Method arguments);*/
			
			List<IEntity> entities = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Querying the data store based on the provided type and parameters.", NLog.LogLevel.Debug))
			{
				
				if (parameters == null)
					throw new ArgumentNullException("parameters");

				entities = new List<IEntity>(ObjectContainer.Query<IEntity>(delegate(IEntity e)
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

			return (IEntity[])entities.ToArray();
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
			
			return GetEntities<T>(parameters);
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
				
				if (parameters == null)
					throw new ArgumentNullException("parameters");

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
			T entity = default(T);
			
			using (LogGroup logGroup= AppLogger.StartGroup("Retrieving the entity matching the provided parameters.", NLog.LogLevel.Debug))
			{
				T[] entities = GetEntities<T>(parameters);
				if (entities == null || entities.Length == 0)
					entity = default(T);
				else
					entity = entities[0];
				
				if (entity == null)
					AppLogger.Debug("Retrieved entity: [null]");
				else
				{
					AppLogger.Debug("Retrieved entity ID: " + entity.ID);
					AppLogger.Debug("Retrieved entity type: " + entity.GetType().ToString());
					AppLogger.Debug("Retrieved entity: " + entity.ToString());
				}
			}
			
			return (T)entity;
		}
		
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <returns></returns>
		public T GetEntity<T>(string propertyName, object propertyValue)
			where T : IEntity
		{
			T entity = default(T);
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the entity matching the specified property value.", NLog.LogLevel.Debug))
			{
				
				IDictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add(propertyName, propertyValue);
				
				entity = GetEntity<T>(parameters);
				
				if (entity == null)
					AppLogger.Debug("Retrieved entity: [null]");
				else
				{
					AppLogger.Debug("Retrieved entity ID: " + entity.ID);
					AppLogger.Debug("Retrieved entity type: " + entity.GetType().ToString());
					AppLogger.Debug("Retrieved entity: " + entity.ToString());
				}
			}
			
			return entity;
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
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public IEntity[] GetEntitiesPage(Type type, PagingLocation location, string sortExpression)
		{
			List<IEntity> entities = new List<IEntity>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving a page of entities.", NLog.LogLevel.Debug))
			{
				if (location == null)
					throw new ArgumentNullException("location");
				
				AppLogger.Debug("Type: " + type.ToString());
				AppLogger.Debug("Page index: " + location.PageIndex);
				AppLogger.Debug("Page size: " + location.PageSize);
				AppLogger.Debug("Sort expression: " + sortExpression);
				
				IQuery query = ObjectContainer.Query();
				query.Constrain(type);

				ApplySorting(query, type, sortExpression);

				IObjectSet os = query.Execute();
				
				int i = 0;
				
				for (i = 0; i < os.Count; i++)
				{
					if (EntitiesUtilities.IsInPage(i, location.PageIndex, location.PageSize))
					{
						entities.Add((IEntity)os[i]);
					}
				}
				
				//while (os.HasNext())
				//{
				//	if (DataUtilities.IsInPage(i, location.PageIndex, location.PageSize))//(i >= pageIndex * pageSize) && (i < (pageIndex + 1) * pageSize))
				//	{
				//		entities.Add((IEntity)os.Next());
				//	}
				//	else
				//		os.Next();
				//	i++;
				//}
				location.AbsoluteTotal = i;
				
				AppLogger.Debug("Absolute count: " + location.AbsoluteTotal.ToString());
				
				AppLogger.Debug("Entities count (single page): " + entities.Count.ToString());
			}
			return entities.ToArray();
		}

		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public T[] GetEntitiesPage<T>(PagingLocation location, string sortExpression)
			where T : IEntity
		{
			return Collection<T>.ConvertAll(GetEntitiesPage(typeof(T), location, sortExpression));
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="propertyValue">The value of the property to query for.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public IEntity[] GetEntitiesPage(Type type, string propertyName, object propertyValue, PagingLocation location, string sortExpression)
		{
			IQuery query = ObjectContainer.Query();
			query.Constrain(type);
			query.Descend(propertyName).Constrain(propertyValue);

			ApplySorting(query, type, sortExpression);

			IObjectSet os = query.Execute();

			int i = 0;
			//        os.Reset();

			ArrayList page = new ArrayList();
			while (os.HasNext())
			{
				if ((i >= location.PageIndex * location.PageSize) && (i < (location.PageIndex + 1) * location.PageSize))
				{
					page.Add(os.Next());
				}
				else
					os.Next();
				i++;
			}
			location.AbsoluteTotal = i;
			return (IEntity[])page.ToArray(type);
		}

		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="propertyValue">The value of the property to query for.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public T[] GetEntitiesPage<T>(string propertyName, object propertyValue, PagingLocation location, string sortExpression)
			where T : IEntity
		{
			return Collection<T>.ConvertAll(GetEntitiesPage(typeof(T), propertyName, propertyValue, location, sortExpression));
		}

		/// <summary>
		/// Applies the specified sort expression to the provided query.
		/// </summary>
		/// <param name="query">The query to apply the sort expression to.</param>
		/// <param name="sortExpression">The sort expression to apply to the query.</param>
		public void ApplySorting(IQuery query, Type type, string sortExpression)
		{
			if (sortExpression != null && sortExpression != String.Empty)
			{
				if (query != null && sortExpression != null)
				{
					if (sortExpression.IndexOf("Descending") > -1)
					{
						string propertyName = sortExpression.Replace("Descending", String.Empty);
						string fieldName = EntitiesUtilities.GetFieldName(type, propertyName);
						query.Descend(fieldName).OrderDescending();
					}
					else if (sortExpression.IndexOf("Ascending") > -1)
					{
						string propertyName = sortExpression.Replace("Ascending", String.Empty);
						string fieldName = EntitiesUtilities.GetFieldName(type, propertyName);
						query.Descend(fieldName).OrderAscending();
					}
					else
						throw new ArgumentException("The provided sort expression is invalid: " + sortExpression, "sortExpression");
				}
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

		/*static public string ToCamelCase(string text)
		{
			// TODO: Check if this is done properly
			if (text == string.Empty)
				return String.Empty;

			string firstChar = text.Substring(0, 1);

			text = text.Substring(1, text.Length - 1);

			text = firstChar.ToLower() + text;

			return text;
		}*/

		
		/*public T[] GetEntitiesPage<T>(PagingLocation location, string sortExpression)
			where T : IEntity
		{
			int total = 0;
			
			T[] output = GetEntitiesPage<T>(location.PageIndex, location.PageSize, sortExpression, out total);
			
			location.AbsoluteTotal = total;
			
			return output;
		}*/
		/*
		public T[] GetEntitiesPage<T>(string fieldName, object fieldValue, PagingLocation location, string sortExpression)
			where T : IEntity
		{
			
			int total = 0;
			
			
			T[] output = GetEntitiesPage<T>(fieldName, fieldValue, location.PageIndex, location.PageSize, sortExpression, out total);
			
			location.AbsoluteTotal = total;
			
			return output;
		}*/
		
		
		public void Commit()
		{
			Commit(false);
		}
		
		
		public void Commit(bool forceCommit)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Committing the data store (or adding to batch for later).", NLog.LogLevel.Debug))
			{
				// Only commit if there's no batch running
				if (forceCommit || !Batch.IsRunning)
				{
					AppLogger.Debug("No batch running. Committing immediately.");
					
					if (ObjectContainer != null)
						ObjectContainer.Commit();
					else
						throw new InvalidOperationException("ObjectContainer == null");
				}
				// If a batch is running then the commit should be skipped. It'll be commit once the batch is complete.
				else
				{
					AppLogger.Debug("Batch running. Adding data source to batch. It will be committed when the batch is over.");
					
					Batch.Handle(this);
				}
			}
		}
	}
}
