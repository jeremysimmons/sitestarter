﻿using System;
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

			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving entities by type and filter.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Filter type: " + filter.GetType().ToString());
				
				foreach (Type type in filter.Types)
				{
					
					AppLogger.Debug("Includes type: " + type.ToString());
					
					entities.AddRange(((Db4oDataStore)GetDataStore(type)).ObjectContainer.Query<IEntity>(delegate(IEntity entity)
					                                                                                     {
					                                                                                     	return filter.IsMatch(entity);
					                                                                                     	
					                                                                                     }));
				}

				foreach (IEntity entity in entities)
				{
					using (LogGroup logGroup2 = AppLogger.StartGroup("Entity found.", NLog.LogLevel.Debug))
					{
						AppLogger.Debug("Entity ID: " + entity.ID);
						AppLogger.Debug("Entity .ToString(): " + entity.ToString());
					}
				}

				if (entities.Count == 0)
					AppLogger.Debug("No entities retrieved.");
			}

			return Release((IEntity[])entities.ToArray());
		}
		
		
		/// <summary>
		/// Retrieves all the entities matching the filter group.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override IEntity[] GetEntities(FilterGroup group)
		{
			List<IEntity> entities = new List<IEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving entities by type and filter.", NLog.LogLevel.Debug))
			{

				if (group != null && group.Filters != null && group.Filters.Length > 0)
				{

					AppLogger.Debug("Group operator: " + group.Operator.ToString());

					List<Type> allTypes = new List<Type>();

					foreach (IDataFilter filter in group.Filters)
					{
						if (filter.Types != null)
							allTypes.AddRange(filter.Types);
					}

					// Loop through the types and load them
					foreach (Type type in allTypes)
					{

						AppLogger.Debug("Includes type: " + type.ToString());

						Db4oDataStore store = (Db4oDataStore)GetDataStore(type);


						entities.AddRange(store.ObjectContainer.Query<IEntity>(delegate(IEntity entity)
						                                                       {
						                                                       	return group.IsMatch(entity);

						                                                       }));
					}

					foreach (IEntity entity in entities)
					{
						using (LogGroup logGroup2 = AppLogger.StartGroup("Entity found.", NLog.LogLevel.Debug))
						{
							//IEntity entity = (IEntity)os.Next();
							AppLogger.Debug("Entity ID: " + entity.ID);
							AppLogger.Debug("Entity .ToString(): " + entity.ToString());
						}
					}

					if (entities.Count == 0)
						AppLogger.Debug("No entities retrieved.");
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
			ArrayList list = new ArrayList();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving entities of the specified type.", NLog.LogLevel.Debug))
			{
				IObjectContainer objectContainer = ((Db4oDataStore)GetDataStore(type)).ObjectContainer;
				
				// If the object container is closed then skip the query
				if (!objectContainer.Ext().IsClosed())
				{
					IQuery query = objectContainer.Query();
					query.Constrain(type);

					IObjectSet os = query.Execute();
					while (os.HasNext())
					{
						list.Add(os.Next());
					}
					
					AppLogger.Debug("Total: " + list.Count.ToString());
				}
			}

			return Release((IEntity[])list.ToArray(type));
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
		/// Retrieves the entities from the data store.
		/// </summary>
		/// <returns>The entities from the data store.</returns>
		public override IEntity[] GetEntities()
		{
			IQuery query = ((Db4oDataStore)GetDataStore(typeof(IEntity))).ObjectContainer.Query();
			query.Constrain(typeof(IEntity));

			List<IEntity> list = new List<IEntity>();

			IObjectSet os = query.Execute();
			while (os.HasNext())
			{
				list.Add((IEntity)os.Next());
			}

			return Release((IEntity[])list.ToArray());
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referenceID">The ID of the referenced entity.</param>
		/// <param name="location"></param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetPageOfEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID, PagingLocation location, string sortExpression)
		{
			
			Collection<T> page = new Collection<T>();
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			//{
			//AppLogger.Debug("Property name: " + propertyName);
			//AppLogger.Debug("Referenced entity ID: " + referencedEntityID);
			
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
			
			string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(typeof(T), propertyName);
			
			// Load the references all in one go, to avoid individual loads
			EntityReferenceCollection references = Provider.Referencer.GetReferences(referencedEntityType, referencedEntityID, mirrorPropertyName, typeof(T), false);
			
			List<Guid> entityIDList = new List<Guid>();
			
			foreach (EntityReference reference in references)
			{
				if (reference.Entity1ID == referencedEntityID)
					entityIDList.Add(reference.Entity2ID);
				else
					entityIDList.Add(reference.Entity1ID);
			}
			
			Guid[] entityIDs = entityIDList.ToArray();
			
			page.AddRange(container.Query<T>(
				delegate(T e)
				{
					bool matches = true;
					bool isInPage = false;
					
					//using (LogGroup logGroup2 = AppLogger.StartGroup("Querying entity.", NLog.LogLevel.Debug))
					//{

					//AppLogger.Debug("Checking type " + e.GetType().ToString());
					//AppLogger.Debug("Entity ID: " + e.ID);
					
					matches = Array.IndexOf(entityIDs, e.ID) > -1;
					
					isInPage = location.IsInPage(i);
					
					if (matches)
						i++;
					//AppLogger.Debug("Matches: " + matches);
					//}
					
					
					return matches && isInPage;
				},
				new DynamicComparer<T>(
					type,
					sortExpression)));
			
			location.AbsoluteTotal = i;
			

			// TODO: Check if needed
			// Code below is start of a performance improvement that may not be possible to get working
			/*
string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(typeof(T), propertyName);
				
				EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(typeof(T),
				                                                                                referencedEntityID,
				                                                                                mirrorPropertyName,
				                                                                                referencedEntityType,
				                                                                                false);
				
				foreach (EntityReference reference in references)
				{
					if (reference.Includes(referencedEntityID, mirrorPropertyName))
					{
						page.Add(Provider.Reader.GetEntityWithReference<T>(mirrorPropertyName, referenceEntityType, referencedEntityID));
					}
				}*/
			
			/*if (entities != null)
			{
				AppLogger.Debug("entities != null");
				
				location.AbsoluteTotal = i;
			}
			else
			{
				AppLogger.Debug("entities == null");
				
				location.AbsoluteTotal = 0;
			}*/
			
			//AppLogger.Debug("Absolute total objects: " + location.AbsoluteTotal);
			//}

			return Release((T[])page.ToArray());
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the data store.
		/// </summary>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referencedEntityID">The ID of the referenced entity to match.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetEntitiesWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			List<T> entities = null;
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Querying the data store based on the provided parameters.", NLog.LogLevel.Debug))
			//{
			//	AppLogger.Debug("Property name: " + propertyName);
			//	AppLogger.Debug("Referenced entity ID: " + referencedEntityID);
			
			//Type referencedEntityType = EntitiesUtilities.GetReferenceType(typeof(T), propertyName);
			
			
			//	if (referencedEntityType != null)
			//		AppLogger.Debug("Referenced entity type: " + referencedEntityType.ToString());
			//	else
			//		AppLogger.Debug("Referenced entity type: [null]");
			
			Type type = typeof(T);
			
			Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
			
			IObjectContainer container = store.ObjectContainer;
			
			entities = new List<T>(
				container
				.Query<T>(
					delegate(T e)
					{
						bool matches = true;
						
						//using (LogGroup logGroup2 = AppLogger.StartGroup("Querying entity to find one matching the specified reference.", NLog.LogLevel.Debug))
						//{

						//	AppLogger.Debug("Checking type " + e.GetType().ToString());
						//	AppLogger.Debug("Entity ID: " + e.ID);
						
						matches = Provider.Referencer.MatchReference(e.GetType(), e.ID, propertyName, referencedEntityType, referencedEntityID);
						
						//	AppLogger.Debug("Matches: " + matches);
						//}
						return matches;
					}));


			
			//if (entities != null)
			//{
			//AppLogger.Debug("entities != null");
			//}
			//else
			//{
			//AppLogger.Debug("entities == null");
			//}
			
			//AppLogger.Debug("Total objects: " + entities.Count);
			//}

			return Release(entities.ToArray());
		}
		
		/// <summary>
		/// Retrieves the entities of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entities of the specified type found in the data store.</returns>
		public override IEntity[] GetEntities(Type type, string propertyName, object propertyValue)
		{
			return (IEntity[])Reflector.InvokeGenericMethod(this, // Source object
			                                                "GetEntities", // Method name
			                                                new Type[] {type}, // Generic types
			                                                new object[] {propertyName, propertyValue}); // Method arguments);
			
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
			return Release((IEntity[])page.ToArray(type));
		}
		
		/// <summary>
		/// Retrieves the specified page of objects from the provided IObjectSet.
		/// </summary>
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetEntities<T>(FilterGroup filterGroup, string sortExpression)
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
		public override IEntity[] GetEntities(Type type, FilterGroup filterGroup, string sortExpression)
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
		/// <param name="type">The type of entities to retrieve.</param>
		/// <param name="filterGroup">The group to filter the query by.</param>
		/// <param name="location">The paging location to filter the query by.</param>
		/// <param name="sortExpression">The sort expression to apply before retrieving the page.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T[] GetPageOfEntities<T>(FilterGroup filterGroup, PagingLocation location, string sortExpression)
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
		public override IEntity[] GetPageOfEntities(Type type, FilterGroup filterGroup, PagingLocation location, string sortExpression)
		{
			Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
			
			Collection<IEntity> list = new Collection<IEntity>();
			
			int i = 0;
			
			if (store.DoesExist)
			{
				list.AddRange(
					store.ObjectContainer.Query<IEntity>(
						delegate(IEntity entity)
						{
							bool doesMatch = filterGroup.IsMatch(entity);
							
							bool isInPage = location.IsInPage(i);
							
							if (doesMatch)
								i++;
							
							return doesMatch && isInPage;
						},
						new DynamicComparer<IEntity>(
							type,
							sortExpression)
					)
				);
			}
			
			location.AbsoluteTotal = i;
			
			return Release((IEntity[])list.ToArray(type));
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
			
			using (LogGroup logGroup = AppLogger.StartGroup("Querying the data store based on the provided type and parameters.", NLog.LogLevel.Debug))
			{
				
				if (parameters == null)
					throw new ArgumentNullException("parameters");

				Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
				
				if (store.DoesExist)
				{
					
					entities = new List<IEntity>(store.ObjectContainer.Query<IEntity>(delegate(IEntity e)
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
				
				AppLogger.Debug("Total: " + entities.Count.ToString());

			}
			return Release((IEntity[])entities.ToArray());
		}

		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override T[] GetEntities<T>(Dictionary<string, object> parameters)
		{
			Collection<T> list = new Collection<T>();
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving entities of the specified type matching the provided parameters.", NLog.LogLevel.Debug))
			{
				foreach (IEntity entity in GetEntities(typeof(T), parameters))
				{
					if (entity is T && !list.Contains((T)entity))
						list.Add((T)entity);
					else
						throw new InvalidOperationException("Invalid type. Expected '" + typeof(T).ToString() + "' but was '" + entity.ToString() + "'.");
				}
				
				AppLogger.Debug("Total: " + list.Count);
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
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving a page of entities.", NLog.LogLevel.Debug))
			{
				if (location == null)
					throw new ArgumentNullException("location");
				
				// Output the parameters to the log
				AppLogger.Debug("Type: " + type.ToString());
				AppLogger.Debug("Page index: " + location.PageIndex);
				AppLogger.Debug("Page size: " + location.PageSize);
				AppLogger.Debug("Sort expression: " + sortExpression);
				
				// Get the corresponding data store
				Db4oDataStore store = (Db4oDataStore)GetDataStore(type);
				
				// Create the query object
				IQuery query = store.ObjectContainer.Query();
				
				// Constrain the query to the specified type
				query.Constrain(type);

				// Apply the sorting to the query
				ApplySorting(query, type, sortExpression);

				// Execute the query and get the object set
				IObjectSet os = query.Execute();
				
				int i = 0;
				
				// Loop through each index in the object set
				for (i = 0; i < os.Count; i++)
				{
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
				
				AppLogger.Debug("Absolute count: " + location.AbsoluteTotal.ToString());
				
				AppLogger.Debug("Entities count (single page): " + entities.Count.ToString());
			}
			
			// Return the entities
			return Release(entities.ToArray());
		}
		
		/// <summary>
		/// Retrieves all the entities of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override T[] GetEntities<T>(string propertyName, object propertyValue)
		{
			Collection<T> results = new Collection<T>();
			
			Type type = typeof(T);
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the entities of the specified type with a property matching the provided name and value.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type: " + type.ToString());
				AppLogger.Debug("Property name: " + propertyName);
				AppLogger.Debug("Property value: " + (propertyValue == null ? "[null]" : propertyValue.ToString()));
				
				if (type.Name == "EntityIDReference"
				    || type.Name == "EntityReference")
					throw new ArgumentException("The provided type cannot be EntityReference or EntityIDReference.");
				
				Db4oDataStore store = ((Db4oDataStore)GetDataStore(type));
				
				if (store != null)
				{
					if (store.ObjectContainer != null)
					{
						IQuery query = store.ObjectContainer.Query();
						query.Constrain(typeof(T));
						query.Descend(EntitiesUtilities.GetFieldName(typeof(T),propertyName)).Constrain(propertyValue);
						
						IObjectSet os = query.Execute();
						
						while (os.HasNext())
						{
							results.Add((T)os.Next());
						}
					}
				}
				
				AppLogger.Debug("Entities #: " + results.Count.ToString());
				
				// TODO: See if performance can be improved by switching to SODA using the code below.
				// Won't work because it can't pick up UniqueKeys, as they don't have a private field corresponding with them
				/*
				string fieldName = EntitiesUtilities.GetFieldName(type, propertyName);
				AppLogger.Debug("Field name: " + fieldName);
				
				IQuery query = ((Db4oDataStore)GetDataStore(type)).ObjectContainer.Query();
				query.Constrain(type).And(
					query.Descend(fieldName).Constrain(propertyValue).Equal());
				
				IObjectSet os = query.Execute();
				
				while (os.HasNext())
				{
					object obj = os.Next();
					if (type.IsAssignableFrom(obj.GetType()))
					{
						//object v = EntitiesUtilities.GetPropertyValue((IEntity)obj, propertyName);
						
						//if (propertyValue == v || (propertyValue != null && propertyValue.Equals(v)))
						//{
						AppLogger.Debug("Adding entity of type: " + obj.GetType().ToString() + " and with ID " + ((IEntity)obj).ID.ToString());
						results.Add((IEntity)obj);
						//}
						//else
						//	AppLogger.Error("Entity loaded when it doesn't match. Expected '" + propertyValue.ToString() + "' but was '" + v.ToString() + ".");
					}
					else
						throw new InvalidOperationException("Invalid type found. Expected '" + type.ToString() + "' but was '" + obj.GetType().ToString() + "'.");
				}
				 */
				AppLogger.Debug("Results: " + results.Count.ToString());
			}
			return Release<T>((T[])results.ToArray());
		}
		
		
		
		/// <summary>
		/// Applies the specified sort expression to the provided query.
		/// </summary>
		/// <param name="query">The query to apply the sort expression to.</param>
		/// <param name="sortExpression">The sort expression to apply to the query.</param>
		public void ApplySorting(IQuery query, Type type, string sortExpression)
		{
			if (query == null)
				throw new ArgumentNullException("query");
			
			// If there's no sort expression then skip it
			if (sortExpression != null && sortExpression != String.Empty)
			{
				// If the sort expression contains the word "Descending" then handle it
				if (sortExpression.IndexOf("Descending") > -1)
				{
					// Extract the name of the property
					string propertyName = sortExpression.Replace("Descending", String.Empty);
					
					// Get the name of the field from the property name
					string fieldName = EntitiesUtilities.GetFieldName(type, propertyName);
					
					// Apply the sorting to the field
					query.Descend(fieldName).OrderDescending();
				}
				
				// If the sort expression contains the word "Ascending" then handle it
				else if (sortExpression.IndexOf("Ascending") > -1)
				{
					// Extract the name of the property
					string propertyName = sortExpression.Replace("Ascending", String.Empty);
					
					// Get the name of the field from the property name
					string fieldName = EntitiesUtilities.GetFieldName(type, propertyName);
					
					// Apply the sorting to the field
					query.Descend(fieldName).OrderAscending();
				}
				else
				{
					// The sort expression is invalid
					throw new ArgumentException("The provided sort expression is invalid: " + sortExpression, "sortExpression");
				}
			}
		}

	}
}
