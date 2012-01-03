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
	/// Description of Db4oDataIndexer.
	/// </summary>
	public class Db4oDataIndexer : DataIndexer, IDataIndexer
	{
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataIndexer(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		/// <summary>
		/// Retrieves all the entities matching the provided filter from the data store.
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The entities matching the provided filter.</returns>
		public override IEntity[] GetEntities(IDataFilter filter)
		{
			List<IEntity> entities = new List<IEntity>();

			using (LogGroup logGroup = LogGroup.Start("Retrieving entities by type and filter.", NLog.LogLevel.Debug))
			{
				LogWriter.Debug("Filter type: " + filter.GetType().ToString());
				
				foreach (Type type in filter.Types)
				{
					
					LogWriter.Debug("Includes type: " + type.ToString());
					
					entities.AddRange(((Db4oDataStore)GetDataStore(type)).ObjectContainer.Query<IEntity>(delegate(IEntity entity)
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

			return Release((IEntity[])entities.ToArray());
		}
		
		
		/// <summary>
		/// Retrieves all the entities matching the filter group.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override IEntity[] GetEntities(IDataFilterGroup group)
		{
			Collection<IEntity> entities = new Collection<IEntity>();

			using (LogGroup logGroup = LogGroup.Start("Retrieving entities by type and filter.", NLog.LogLevel.Debug))
			{

				if (group != null && group.Filters != null && group.Filters.Length > 0)
				{

					LogWriter.Debug("Group operator: " + group.Operator.ToString());

					List<Type> allTypes = new List<Type>();

					foreach (IDataFilter filter in group.Filters)
					{
						if (filter.Types != null)
						{
							foreach (Type type in filter.Types)
								if (!allTypes.Contains(type))
									allTypes.Add(type);
						}
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

			return Release((IEntity[])entities.ToArray());
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type from the data store.
		/// </summary>
		/// <param name="entityIDs">The IDs of the entities to retrieve.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override T[] GetEntities<T>(Guid[] entityIDs)
		{
			if (entityIDs == null || entityIDs.Length == 0)
				return new T[]{};
			
			List<T> list = new List<T>(
				((Db4oDataStore)GetDataStore(typeof(T))).ObjectContainer.Query<T>(
					delegate(T e)
					{
						return Array.IndexOf(entityIDs, e.ID) > -1;
					}));
			
			return Release((T[])list.ToArray());
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type with the specified IDs from the data store.
		/// </summary>
		/// <param name="ids">The IDs of the entities to retrieve.</param>
		/// <returns>The entity of the specified type with the specified IDs.</returns>
		public override IEntity[] GetEntities(Type type, Guid[] ids)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(this, // Source object
			                                                "GetEntities", // Method name
			                                                new Type[] {type}, // Generic types
			                                                new object[] {ids}); // Method arguments);
		}
		
		
		/// <summary>
		/// Retrieves all entities of the specified type from the corresponding data store.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <returns>An array of the specified type of entities.</returns>
		public override IEntity[] GetEntities(Type type)
		{
			List<IEntity> list = new List<IEntity>();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving entities of the specified type."))
			{
				IObjectContainer objectContainer = ((Db4oDataStore)GetDataStore(type)).ObjectContainer;
				
				// If the object container is closed then skip the query
				if (objectContainer != null && !objectContainer.Ext().IsClosed())
				{
					IQuery query = objectContainer.Query();
					query.Constrain(type);

					IObjectSet os = query.Execute();
					
					for (int i = 0; i < os.Count; i++)
						list.Add((IEntity)os[i]);
					
					LogWriter.Debug("Total: " + list.Count.ToString());
				}
			}

			return Release((IEntity[])list.ToArray());
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type from the data store.
		/// </summary>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override T[] GetEntities<T>()
		{
			return Collection<T>.ConvertAll(GetEntities(typeof(T)));
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type from the data store.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override T[] GetEntities<T>(string sortExpression)
		{
			Collection<T> collection = new Collection<T>(Collection<T>.ConvertAll(GetEntities<T>()));
			
			collection.Sort(sortExpression);
			
			return collection.ToArray();
		}

		/// <summary>
		/// Retrieves all entities in the attached data store, or if it's null then all entities from all all stores.
		/// </summary>
		/// <returns>All entities retrieved.</returns>
		public override IEntity[] GetEntities()
		{
			IEntity[] entities = new IEntity[]{};
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving all entities."))
			{
				List<IEntity> list = new List<IEntity>();
				
				List<IDataStore> stores = new List<IDataStore>();
				
				// Get the list of stores
				// If the DataStore property is null then get all stores.
				if (DataStore == null)
				{
					LogWriter.Debug("DataStore property is null. Retrieving from all.");
					
					foreach (string dataStoreName in DataAccess.Data.GetDataStoreNames())
					{
						stores.Add(Provider.Stores[dataStoreName]);
					}
				}
				// Otherwise use the single store attached to this adapter
				else
				{
					LogWriter.Debug("DataStore property is set. Using.");
					
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
							while (os.HasNext())
							{
								list.Add((IEntity)os.Next());
							}
						}
					}
				}
				
				LogWriter.Debug("Total: " + list.Count.ToString());
				
				entities = Release((IEntity[])list.ToArray());
			}
			return entities;
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referenceID">The ID of the referenced entity.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetPageOfEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID, PagingLocation location, string sortExpression)
		{
			
			Collection<T> page = new Collection<T>();
			
			//using (LogGroup logGroup = LogGroup.Start("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			//{
			//LogWriter.Debug("Property name: " + propertyName);
			//LogWriter.Debug("Referenced entity ID: " + referencedEntityID);
			
			if (location == null)
				throw new ArgumentNullException("location");
			
			Type type = typeof(T);
			
			
			string sortPropertyName = sortExpression.Replace("Ascending", "").Replace("Descending", "");
			
			Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
			
			if (store == null)
				throw new ArgumentException("Can't find data store for type '" + type.Name + "'.");
			
			IObjectContainer container = store.ObjectContainer;
			
			if (container == null)
				throw new Exception("No object container for store '" + store.Name + "'.");
			
			Predicate matches = new MatchReferencePredicate(Provider, typeof(T), propertyName, referencedEntityType, referencedEntityID);
			
			IObjectSet os = store.ObjectContainer.Query(matches,
			                                            new DynamicComparer(
			                                            	type,
			                                            	sortExpression));
			
			page.AddRange(GetPage(os, location));

			return Release((T[])page.ToArray());
		}
		
		/// <summary>
		/// Retrieves the specified page of entities from the data store.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the reference.</param>
		/// <param name="referencedEntities">An array of entities to check the reference to.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetPageOfEntitiesWithReference<T>(string propertyName, IEntity[] referencedEntities, PagingLocation location, string sortExpression)
		{
			if (referencedEntities.Length == 0)
				return new T[]{};
			
			Collection<T> page = new Collection<T>();
			
			//using (LogGroup logGroup = LogGroup.Start("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			//{
			//LogWriter.Debug("Property name: " + propertyName);
			//LogWriter.Debug("Referenced entity ID: " + referencedEntityID);
			
			if (location == null)
				throw new ArgumentNullException("location");
			
			Type type = typeof(T);
			
			
			string sortPropertyName = sortExpression.Replace("Ascending", "").Replace("Descending", "");
			
			Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
			
			if (store == null)
				throw new ArgumentException("Can't find data store for type '" + type.Name + "'.");
			
			IObjectContainer container = store.ObjectContainer;
			
			if (container == null)
				throw new Exception("No object container for store '" + store.Name + "'.");
			
			int i = 0;
			
			PropertyInfo property = typeof(T).GetType().GetProperty(propertyName);
			
			EntityReferenceCollection references = new EntityReferenceCollection();
			
			// Load the references of all the referenced entities
			foreach (IEntity referencedEntity in referencedEntities)
			{
				references.AddRange(Provider.Referencer.GetReferences(referencedEntity.GetType(), referencedEntity.ID, typeof(T), false));
				
			}
			
			Type referencedEntityType = referencedEntities[0].GetType();
						
			Predicate matches = new MatchReferencesPredicate(Provider, typeof(T), propertyName, referencedEntityType, references);
			
			IObjectSet os = store.ObjectContainer.Query(matches,
			                                            new DynamicComparer(
			                                            	type,
			                                            	sortExpression));
			
			page.AddRange(GetPage(os, location));
			

			//LogWriter.Debug("Absolute total objects: " + location.AbsoluteTotal);
			//}

			return Release((T[])page.ToArray());
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type with the specified reference.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the reference.</param>
		/// <param name="referencedEntityType">The type of the referenced entity to match.</param>
		/// <param name="referencedEntityID">The ID of the referenced entity to match.</param>
		/// <returns>An array of the references retrieved.</returns>
		public override T[] GetEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
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
				
				// Load the references all in one go, to avoid individual loads
				EntityReferenceCollection references = Provider.Referencer.GetReferences(referencedEntityType, referencedEntityID, typeof(T), false);

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
								
								string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(e, propertyName);
								
								bool foundReference = references.Includes(e.ID, propertyName, referencedEntityID, mirrorPropertyName);
								
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

			return Release(entities.ToArray());
		}
		
		
		/// <summary>
		/// Retrieves all the entities of the specified type with a reference to any of the provided entities.
		/// </summary>
		/// <param name="propertyName">The name of the property containing the reference.</param>
		/// <param name="referencedEntities">An array of entities to check the reference to.</param>
		/// <returns>An array of the references retrieved.</returns>
		public override T[] GetEntitiesWithReference<T>(string propertyName, IEntity[] referencedEntities)
		{
			List<T> entities = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Querying the data store based on the provided parameters."))
			{
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Referenced entities #: " + referencedEntities.Length);
				
				if (referencedEntities.Length > 0)
				{
					Type referencedEntityType = referencedEntities[0].GetType();
					
					if (referencedEntityType != null)
						LogWriter.Debug("Referenced entity type: " + referencedEntityType.ToString());
					else
						LogWriter.Debug("Referenced entity type: [null]");
					
					Type type = typeof(T);
					
					EntityReferenceCollection references = new EntityReferenceCollection();
					
					foreach (IEntity referencedEntity in referencedEntities)
					{
						// Load the references all in one go, to avoid individual loads
						references.AddRange(Provider.Referencer.GetReferences(referencedEntity.GetType(), referencedEntity.ID, typeof(T), false));
						
					}
					
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
									
									bool foundReference = references.Includes(e.ID, propertyName);
									
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
			}
			return Release(entities.ToArray());
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="propertyName">The name of the property to check the value of.</param>
		/// <param name="propertyValue">The value to check for on the specified property.</param>
		/// <returns>The entities that match the provided parameters.</returns>
		public override IEntity[] GetEntities(Type type, string propertyName, object propertyValue)
		{
			Collection<IEntity> results = new Collection<IEntity>();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the entities of the specified type with a property matching the provided name and value."))
			{
				if (type == null)
					throw new ArgumentNullException("type");
				
				if (propertyName == null || propertyName == String.Empty)
					throw new ArgumentException("A property name must be provided.", "propertyName");
				
				if (!EntityState.IsType(type))
					throw new ArgumentException("The provided '" + type.Name + "' type is not registered as a valid entity type.");
				
				LogWriter.Debug("Type: " + type.ToString());
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Property value: " + (propertyValue == null ? "[null]" : propertyValue.ToString()));
				
				Db4oDataStore store = ((Db4oDataStore)GetDataStore(type));
				
				if (store != null)
				{
					if (store.ObjectContainer != null)
					{
						IQuery query = store.ObjectContainer.Query();
						query.Constrain(type);
						query.Descend(EntitiesUtilities.GetFieldName(type,propertyName))
							.Constrain(propertyValue);
						
						IObjectSet os = query.Execute();
						
						while (os.HasNext())
						{
							results.Add((IEntity)os.Next());
						}
					}
				}
				
				LogWriter.Debug("Entities #: " + results.Count.ToString());
				
			}
			return Release(results.ToArray());
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="propertyValue">The value of the property to query for.</param>
		/// <param name="location">The paging location to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetPageOfEntities<T>(string propertyName, object propertyValue, PagingLocation location, string sortExpression)
		{
			return Collection<T>.ConvertAll(GetPageOfEntities(typeof(T), propertyName, propertyValue, location, sortExpression));
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="propertyValue">The value of the property to query for.</param>
		/// <param name="location">The paging location to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override IEntity[] GetPageOfEntities(Type type, string propertyName, object propertyValue, PagingLocation location, string sortExpression)
		{
			IQuery query = ((Db4oDataStore)GetDataStore(type)).ObjectContainer.Query();
			query.Constrain(type);
			query.Descend(propertyName).Constrain(propertyValue);

			ApplySorting(query, type, sortExpression);

			IObjectSet os = query.Execute();

			ArrayList page = new ArrayList();
			
			page.Add(GetPage(os, location));
			
			return Release((IEntity[])page.ToArray(type));
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetEntities<T>(IDataFilterGroup filterGroup, string sortExpression)
		{
			return Collection<T>.ConvertAll(GetEntities(typeof(T), filterGroup, sortExpression));
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override IEntity[] GetEntities(Type type, IDataFilterGroup filterGroup, string sortExpression)
		{
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
			return Release((IEntity[])list.ToArray(type));
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <param name="location">The paging location to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetPageOfEntities<T>(IDataFilterGroup filterGroup, PagingLocation location, string sortExpression)
		{
			return Collection<T>.ConvertAll(GetPageOfEntities(typeof(T), filterGroup, location, sortExpression));
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <param name="location">The paging location to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override IEntity[] GetPageOfEntities(Type type, IDataFilterGroup filterGroup, PagingLocation location, string sortExpression)
		{
			Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
			
			Collection<IEntity> list = new Collection<IEntity>();
			
			IObjectSet os = null;
			
			if (store.DoesExist)
			{
				Predicate matches = new MatchFilterGroupPredicate(filterGroup);
				
				os = store.ObjectContainer.Query(matches,
				                                 new DynamicComparer(
				                                 	type,
				                                 	sortExpression));
				
				list.AddRange(GetPage(os, location));
			}
			
			return Release((IEntity[])list.ToArray(type));
		}
		
		protected virtual IEntity[] GetPage(IObjectSet objectSet, PagingLocation location)
		{
			int i = 0;
			
			List<IEntity> list = new List<IEntity>();
			
			// Loop through each index in the object set
			for (i = 0; i < objectSet.Count; i++)
			{
				// If it's not in the current page then skip it
				if (location.IsInPage(i))
				{
					// Add the entity to the collection
					list.Add((IEntity)objectSet[i]);
				}
			}
			
			location.AbsoluteTotal = i;
			
			return list.ToArray();
		}
		
		/// <summary>
		/// Retrieves a single page (at the specified location) of the specified type of entities from the corresponding data store.
		/// </summary>
		/// <param name="location">The coordinates of the page being retrieved.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the entities, including only the current page.</returns>
		public override T[] GetPageOfEntities<T>(PagingLocation location, string sortExpression)
		{
			// Pass this call through to another overload
			// Convert/cast the entities on the way
			return Collection<T>.ConvertAll(GetPageOfEntities(typeof(T), location, sortExpression));
		}
		
		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override IEntity[] GetEntities(Type type, Dictionary<string, object> parameters)
		{
			
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
					
					
					entities = new List<IEntity>(
						store.ObjectContainer.Query<IEntity>(
							delegate(IEntity e)
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
			return Release((IEntity[])(entities != null ? entities.ToArray() : new IEntity[]{}));
		}

		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override T[] GetEntities<T>(Dictionary<string, object> parameters)
		{
			Collection<T> list = new Collection<T>();
			using (LogGroup logGroup = LogGroup.Start("Retrieving entities of the specified type matching the provided parameters.", NLog.LogLevel.Debug))
			{
				foreach (IEntity entity in GetEntities(typeof(T), parameters))
				{
					if (entity is T && !list.Contains((T)entity))
						list.Add((T)entity);
					else
						throw new InvalidOperationException("Invalid type. Expected '" + typeof(T).ToString() + "' but was '" + entity.ToString() + "'.");
				}
				
				LogWriter.Debug("Total: " + list.Count);
			}
			return Release(list.ToArray());
		}
		
		/// <summary>
		/// Retrieves a single page (at the specified location) of the specified type of entities from the corresponding data store.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="location">The coordinates of the page being retrieved.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the entities, including only the current page.</returns>
		public override IEntity[] GetPageOfEntities(Type type, PagingLocation location, string sortExpression)
		{
			// Create the collection
			List<IEntity> entities = new List<IEntity>();
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving a page of entities.", NLog.LogLevel.Debug))
			{
				if (type == null)
					throw new ArgumentNullException("type");
				
				if (location == null)
					throw new ArgumentNullException("location");
				
				// Output the parameters to the log
				LogWriter.Debug("Type: " + type.ToString());
				LogWriter.Debug("Page index: " + location.PageIndex);
				LogWriter.Debug("Page size: " + location.PageSize);
				LogWriter.Debug("Sort expression: " + sortExpression);
				
				// Get the corresponding data store
				Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
				
				if (store == null)
					throw new Exception("Can't find data store.");
				
				if (store.ObjectContainer == null)
					throw new Exception("store.ObjectContainer is null");
				
				// Create the query object
				IQuery query = store.ObjectContainer.Query();
				
				// Constrain the query to the specified type
				query.Constrain(type);

				// Apply the sorting to the query
				ApplySorting(query, type, sortExpression);

				// Execute the query and get the object set
				IObjectSet os = query.Execute();

				if (os == null)
					throw new Exception("IObjectSet is null");
				
				int i = 0;
				
				// Loop through each index in the object set
				for (i = 0; i < os.Count; i++)
				{
					// ONLY execute during debug because it slows the indexing down
					if (new ModeDetector().IsDebug)
					{
						LogWriter.Debug("At absolute position " + i + ": " + ((IEntity)os[i]).ToString());
					}
					
					// If it's not in the current page then skip it
					if (location.IsInPage(i))
					{
						// Add the entity to the collection
						entities.Add((IEntity)os[i]);
					}
				}
				
				// Set the absolute total to the paging location
				// This is the total count including items on ALL pages, not just the current one
				location.AbsoluteTotal = i;
				
				LogWriter.Debug("Absolute count: " + location.AbsoluteTotal.ToString());
				
				LogWriter.Debug("Entities count (single page): " + entities.Count.ToString());
			}
			
			// Return the entities
			return Release(entities.ToArray());
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type with the specified property matching the provided value.
		/// </summary>
		/// <param name="propertyName">The name of the property to check the value of.</param>
		/// <param name="propertyValue">The value to check for on the specified property.</param>
		/// <returns></returns>
		public override T[] GetEntities<T>(string propertyName, object propertyValue)
		{
			return Collection<T>.ConvertAll(GetEntities(typeof(T), propertyName, propertyValue));
		}
		
		
		
		/// <summary>
		/// Applies the specified sort expression to the provided query.
		/// </summary>
		/// <param name="query">The query to apply the sort expression to.</param>
		/// <param name="type">The type involved in the query.</param>
		/// <param name="sortExpression">The sort expression to apply to the query.</param>
		public void ApplySorting(IQuery query, Type type, string sortExpression)
		{
			// TODO: Clean up obsolete code
			
			using (LogGroup logGroup = LogGroup.Start("Applying sorting to query.", NLog.LogLevel.Debug))
			{
				if (query == null)
					throw new ArgumentNullException("query");
				
				if (type == null)
					throw new ArgumentNullException("type");
				
				SortDirection direction = SortDirection.Ascending;
				string propertyName;
				
				LogWriter.Debug("Sort expression: " + sortExpression);
				LogWriter.Debug("Type: " + type.ToString());
				
				// If there's no sort expression then skip it
				if (sortExpression != null && sortExpression != String.Empty)
				{
					// If the sort expression contains the word "Descending" then handle it
					if (sortExpression.IndexOf("Descending") > -1)
					{
						direction = SortDirection.Descending;
						
						// Extract the name of the property
						propertyName = sortExpression.Replace("Descending", String.Empty);
						
						LogWriter.Debug("Property name: " + propertyName);
						
						// Get the name of the field from the property name
						/*string fieldName = EntitiesUtilities.GetFieldName(type, propertyName);
						
						LogWriter.Debug("Field name: " + fieldName);
						
						// Apply the sorting to the field
						query.Descend(fieldName).OrderDescending();
						
						LogWriter.Debug("Ordered descending");*/
					}
					
					// If the sort expression contains the word "Ascending" then handle it
					else if (sortExpression.IndexOf("Ascending") > -1)
					{
						direction = SortDirection.Ascending;
						
						// Extract the name of the property
						propertyName = sortExpression.Replace("Ascending", String.Empty);
						
						/*LogWriter.Debug("Property name: " + propertyName);
						
						// Get the name of the field from the property name
						string fieldName = EntitiesUtilities.GetFieldName(type, propertyName);
						
						LogWriter.Debug("Field name: " + fieldName);
						
						// Apply the sorting to the field
						query.Descend(fieldName).OrderAscending();
						
						LogWriter.Debug("Ordered ascending");*/
					}
					else
					{
						// The sort expression is invalid
						throw new ArgumentException("The provided sort expression is invalid: " + sortExpression, "sortExpression");
					}
					
					query.SortBy(new DynamicQueryComparator<IEntity>(type, propertyName, direction));
				}
			}
		}

	}
}
