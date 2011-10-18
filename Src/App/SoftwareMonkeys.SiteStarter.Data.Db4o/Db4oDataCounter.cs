using System;
using SoftwareMonkeys.SiteStarter.Data;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// 
	/// </summary>
	public class Db4oDataCounter : DataCounter, IDataCounter
	{
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataCounter(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		/// <summary>
		/// Counts all the entities matching the provided filter from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities(IDataFilter filter)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			List<IEntity> entities = new List<IEntity>();

			using (LogGroup logGroup = LogGroup.Start("Counting entities by type and filter.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Filter type: " + filter.GetType().ToString());
				
				foreach (Type type in filter.Types)
				{
					
					LogWriter.Debug("Includes type: " + type.ToString());
					
					entities.AddRange(((Db4oDataStore)GetDataStore(type)).
					                  ObjectContainer.Query<IEntity>(delegate(IEntity entity)
					                                                 {
					                                                 	return filter.IsMatch(entity);
					                                                 	
					                                                 }));
				}

				foreach (IEntity entity in entities)
				{
					using (LogGroup logGroup2 = LogGroup.Start("Entity found.", NLog.LogLevel.Debug))
					{
						LogWriter.Debug("Entity ID: " + entity.ID);
						LogWriter.Debug("Entity .ToString(): " + entity.ToString());
					}
				}

				if (entities.Count == 0)
					LogWriter.Debug("No entities retrieved.");
			}

			return entities.Count;
		}
		
		
		/// <summary>
		/// Counts all the entities matching the filter group.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities(IDataFilterGroup group)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			
			List<IEntity> entities = new List<IEntity>();

			using (LogGroup logGroup = LogGroup.Start("Counting entities by type and filter.", NLog.LogLevel.Debug))
			{

				if (group != null && group.Filters != null && group.Filters.Length > 0)
				{

					LogWriter.Debug("Group operator: " + group.Operator.ToString());

					List<Type> allTypes = new List<Type>();

					foreach (IDataFilter filter in group.Filters)
					{
						if (filter.Types != null)
							allTypes.AddRange(filter.Types);
					}

					// Loop through the types and load them
					foreach (Type type in allTypes)
					{

						LogWriter.Debug("Includes type: " + type.ToString());

						Db4oDataStore store = (Db4oDataStore)GetDataStore(type);


						entities.AddRange(store.ObjectContainer.Query<IEntity>(delegate(IEntity entity)
						                                                       {
						                                                       	return group.IsMatch(entity);

						                                                       }));
					}

					foreach (IEntity entity in entities)
					{
						using (LogGroup logGroup2 = LogGroup.Start("Entity found.", NLog.LogLevel.Debug))
						{
							//IEntity entity = (IEntity)os.Next();
							LogWriter.Debug("Entity ID: " + entity.ID);
							LogWriter.Debug("Entity .ToString(): " + entity.ToString());
						}
					}

					if (entities.Count == 0)
						LogWriter.Debug("No entities retrieved.");
				}
				else
					throw new ArgumentException("The provided filter group is empty.", "group");
			}

			return entities.Count;
		}
		
		/// <summary>
		/// Counts all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="entityIDs">The IDs of the entities to retrieve.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities<T>(Guid[] entityIDs)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			
			List<T> list = new List<T>(
				((Db4oDataStore)GetDataStore(typeof(T))).ObjectContainer.Query<T>(
					delegate(T e)
					{
						return Array.IndexOf(entityIDs, e.ID) > -1;
					}));
			
			return list.Count;
		}
		
		/// <summary>
		/// Counts the entity of the specified type with the specified IDs from the data store.
		/// </summary>
		/// <param name="type">The type of entities to count.</param>
		/// <param name="ids">The IDs of the entities to retrieve.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities(Type type, Guid[] ids)
		{
			return (int)Reflector.InvokeGenericMethod(this, // Source object
			                                                "CountEntities", // Method name
			                                                new Type[] {type}, // Generic types
			                                                new object[] {ids}); // Method arguments);
		}
		
		
		/// <summary>
		/// Counts all entities of the specified type from the corresponding data store.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities(Type type)
		{
			int total = 0;
			
			using (LogGroup logGroup = LogGroup.Start("Counting entities of the specified type.", NLog.LogLevel.Debug))
			{
				IObjectContainer objectContainer = ((Db4oDataStore)GetDataStore(type)).ObjectContainer;
				
				// If the object container is closed then skip the query
				if (objectContainer != null && !objectContainer.Ext().IsClosed())
				{
					IQuery query = objectContainer.Query();
					query.Constrain(type);

					IObjectSet os = query.Execute();
					
					total = os.Count;
					
					LogWriter.Debug("Total: " + total.ToString());
				}
			}

			return total;
		}
		
		/// <summary>
		/// Counts the entities of the specified type from the data store.
		/// </summary>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities<T>()
		{
			return CountEntities(typeof(T));
		}
		
		/// <summary>
		/// Counts all entities in the attached data store, or if it's null then all entities from all all stores.
		/// </summary>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities()
		{
			int total = 0;
			
			List<IDataStore> stores = new List<IDataStore>();
			
			// Count the list of stores
			// If the DataStore property is null then get all stores.
			if (DataStore == null)
			{
				foreach (string dataStoreName in DataAccess.Data.GetDataStoreNames())
				{
					stores.Add(Provider.Stores[dataStoreName]);
				}
			}
			// Otherwise use the single store attached to this adapter
			else
			{
				stores.Add(DataStore);
			}
			
			// Load data from the stores
			foreach (IDataStore store in stores)
			{
				if (!store.IsClosed)
				{
					IObjectContainer container = ((Db4oDataStore)store).ObjectContainer;
					
					if (!container.Ext().IsClosed())
					{
						IQuery query = container.Query();
						
						query.Constrain(typeof(IEntity));

						IObjectSet os = query.Execute();
						
						total = os.Count;
					}
				}
			}

			return total;
		}
		
		/// <summary>
		/// Counts all the entities of the specified type with the specified reference.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the reference.</param>
		/// <param name="referencedEntityType">The type of the referenced entity to match.</param>
		/// <param name="referencedEntityID">The ID of the referenced entity to match.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			
			List<T> entities = null;
			
			using (LogGroup logGroup = LogGroup.Start("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Referenced entity ID: " + referencedEntityID);
				
				if (referencedEntityType != null)
					LogWriter.Debug("Referenced entity type: " + referencedEntityType.ToString());
				else
					LogWriter.Debug("Referenced entity type: [null]");
				
				Type type = typeof(T);
				
				string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(typeof(T), propertyName);
				
				// Load the references all in one go, to avoid individual loads
				EntityReferenceCollection references = Provider.Referencer.GetReferences(referencedEntityType, referencedEntityID, mirrorPropertyName, typeof(T), false);
				
				Guid[] entityIDs = references.GetEntityIDs(referencedEntityID);
				
				Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
				
				IObjectContainer container = store.ObjectContainer;
				
				entities = new List<T>(
					container
					.Query<T>(
						delegate(T e)
						{
							bool matches = true;
							
							using (LogGroup logGroup2 = LogGroup.Start("Querying entity.", NLog.LogLevel.Debug))
							{

								LogWriter.Debug("Checking type " + e.GetType().ToString());
								LogWriter.Debug("Entity ID: " + e.ID);
								
								bool foundReference = Array.IndexOf(entityIDs, e.ID) > -1;
								
								// If a referenced entity ID is specified then entities match if a reference exists
								if (referencedEntityID != Guid.Empty)
									matches = foundReference;
								// Otherwise the calling code is trying to get entities where NO reference exists, therefore it matches when no reference is found
								else
									matches = !foundReference;
								
								LogWriter.Debug("Matches: " + matches);
							}
							return matches;
						}));


				
				if (entities != null)
				{
					LogWriter.Debug("entities != null");
				}
				else
				{
					LogWriter.Debug("entities == null");
				}
				
				LogWriter.Debug("Total objects: " + entities.Count);
			}

			return entities.Count;
		}
		
		
		/// <summary>
		/// Counts all the entities of the specified type with a reference to any of the provided entities.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the reference.</param>
		/// <param name="referencedEntities">An array of entities to check the reference to.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntitiesWithReference<T>(string propertyName, IEntity[] referencedEntities)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			
			List<T> entities = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Querying the data store based on the provided parameters."))
			{
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Referenced entities #: " + referencedEntities.Length);
				
				Type referencedEntityType = EntitiesUtilities.GetReferenceType(typeof(T), propertyName);
				
				if (referencedEntityType != null)
					LogWriter.Debug("Referenced entity type: " + referencedEntityType.ToString());
				else
					LogWriter.Debug("Referenced entity type: [null]");
				
				Type type = typeof(T);
				
				string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(typeof(T), propertyName);
				
				List<Guid> entityIDList = new List<Guid>();
				
				foreach (IEntity referencedEntity in referencedEntities)
				{
					// Load the references all in one go, to avoid individual loads
					EntityReferenceCollection references = Provider.Referencer.GetReferences(referencedEntity.GetType(), referencedEntity.ID, mirrorPropertyName, typeof(T), false);
					
					entityIDList.AddRange(references.GetEntityIDs(referencedEntity.ID));
				}
				
				Guid[] entityIDs = entityIDList.ToArray();
				
				Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
				
				IObjectContainer container = store.ObjectContainer;
				
				entities = new List<T>(
					container
					.Query<T>(
						delegate(T e)
						{
							bool matches = true;
							
							using (LogGroup logGroup2 = LogGroup.Start("Querying entity.", NLog.LogLevel.Debug))
							{

								LogWriter.Debug("Checking type " + e.GetType().ToString());
								LogWriter.Debug("Entity ID: " + e.ID);
								
								bool foundReference = Array.IndexOf(entityIDs, e.ID) > -1;
								
								// If referenced entities were provided then entities match if a reference exists
								if (referencedEntities != null && referencedEntities.Length > 0)
									matches = foundReference;
								// Otherwise the calling code is trying to get entities where NO reference exists, therefore it matches when no reference is found
								else
									matches = !foundReference;
								
								LogWriter.Debug("Matches: " + matches);
							}
							return matches;
						}));


				
				if (entities != null)
				{
					LogWriter.Debug("entities != null");
				}
				else
				{
					LogWriter.Debug("entities == null");
				}
				
				LogWriter.Debug("Total objects: " + entities.Count);
			}

			return entities.Count;
		}
		
		/// <summary>
		/// Counts the entities of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities(Type type, string propertyName, object propertyValue)
		{
			return (int)Reflector.InvokeGenericMethod(this, // Source object
			                                                "CountEntities", // Method name
			                                                new Type[] {type}, // Generic types
			                                                new object[] {propertyName, propertyValue}); // Method arguments);
		}
		
		/// <summary>
		/// Counts the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities<T>(IDataFilterGroup filterGroup)
		{
			return CountEntities(typeof(T), filterGroup);
		}
		
		/// <summary>
		/// Counts the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities(Type type, IDataFilterGroup filterGroup)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			
			Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
			
			Collection<IEntity> list = new Collection<IEntity>();
			
			int i = 0;
			
			if (store.DoesExist)
			{
				
				list = new Collection<IEntity>(store.ObjectContainer.Query<IEntity>(delegate(IEntity e)
				                                                                    {
				                                                                    	bool matches = filterGroup.IsMatch(e);
				                                                                    	i++;
				                                                                    	return matches;
				                                                                    }));
				

				
			}
			return list.Count;
		}
		
		/// <summary>
		/// Counts all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override int CountEntities(Type type, Dictionary<string, object> parameters)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			
			List<IEntity> entities = new List<IEntity>();
			
			using (LogGroup logGroup = LogGroup.Start("Querying the data store based on the provided type and parameters.", NLog.LogLevel.Debug))
			{
				if (type == null)
					throw new ArgumentNullException("type");
				
				if (parameters == null)
					throw new ArgumentNullException("parameters");

				Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
				
				if (store == null)
					throw new Exception("No data store found.");
				
				if (store.DoesExist)
				{
					
					entities = new List<IEntity>(store.ObjectContainer.Query<IEntity>(delegate(IEntity e)
					                                                                  {
					                                                                  	LogWriter.Debug("Checking type " + e.GetType().ToString());
					                                                                  	
					                                                                  	bool matches = true;
					                                                                  	foreach (string key in parameters.Keys)
					                                                                  	{
					                                                                  		LogWriter.Debug("Checking parameter '" + key + "' for value '" + parameters[key].ToString() + "'");
					                                                                  		
					                                                                  		Type parameterType = parameters[key] != null ? parameters[key].GetType() : null;
					                                                                  		
					                                                                  		PropertyInfo property = e.GetType().GetProperty(key, parameterType);
					                                                                  		if (property == null)
					                                                                  			throw new InvalidOperationException("The property '" + key + "' was not found on the type " + e.GetType().ToString());
					                                                                  		else
					                                                                  		{
					                                                                  			object value = property.GetValue(e, null);
					                                                                  			
					                                                                  			LogWriter.Debug("Actual value is: " + (value == null ? "null" : value.ToString()));
					                                                                  			
					                                                                  			if (parameters[key] != value && parameters[key] != null && !parameters[key].Equals(value))
					                                                                  			{
					                                                                  				LogWriter.Debug("Parameter match failed for '" + key + "'.");
					                                                                  				matches = false;
					                                                                  			}
					                                                                  		}
					                                                                  	}
					                                                                  	LogWriter.Debug("Matches: " + matches.ToString());
					                                                                  	return matches;
					                                                                  }));
					

					
				}
				
				LogWriter.Debug("Total: " + entities != null ? entities.Count.ToString() : 0.ToString());

			}
			return (entities != null ? entities.Count : 0);
		}

		
		/// <summary>
		/// Counts all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities<T>(Dictionary<string, object> parameters)
		{
			// TODO: Boost performance by looping through an object set without actually loading the entities
			
			int total = 0;
			
			Collection<T> list = new Collection<T>();
			using (LogGroup logGroup = LogGroup.Start("Counting entities of the specified type matching the provided parameters.", NLog.LogLevel.Debug))
			{
				total = CountEntities(typeof(T), parameters);
								
				LogWriter.Debug("Total: " + total);
			}
			return total;
		}
		
		/// <summary>
		/// Counts all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns>The total number of entities counted.</returns>
		public override int CountEntities<T>(string propertyName, object propertyValue)
		{
			int total = 0;
			
			Type type = typeof(T);
			
			//using (LogGroup logGroup = LogGroup.Start("Counting the entities of the specified type with a property matching the provided name and value.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Type: " + type.ToString());
			//	LogWriter.Debug("Property name: " + propertyName);
			//	LogWriter.Debug("Property value: " + (propertyValue == null ? "[null]" : propertyValue.ToString()));
			
			Db4oDataStore store = ((Db4oDataStore)GetDataStore(type));
			
			if (store != null)
			{
				if (store.ObjectContainer != null)
				{
					IQuery query = store.ObjectContainer.Query();
					query.Constrain(typeof(T));
					query.Descend(EntitiesUtilities.GetFieldName(typeof(T),propertyName)).Constrain(propertyValue);
					
					IObjectSet os = query.Execute();
					
					total = os.Count;
				}
			}
			
			//LogWriter.Debug("Results: " + results.Count.ToString());
			//}
			return total;
		}
	}
}
