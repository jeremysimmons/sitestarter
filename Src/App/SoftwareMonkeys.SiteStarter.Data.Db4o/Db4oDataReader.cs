﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Used to retrieve entity data from the data stores.
	/// </summary>
	public class Db4oDataReader : DataReader
	{
		
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataReader(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		/// <summary>
		/// Retrieves the entity matching the filter .
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The matching entity.</returns>
		public override T GetEntity<T>(IDataFilter filter)
		{
			return (T)GetEntity(filter);
		}
		
		/// <summary>
		/// Retrieves the entity matching the filter .
		/// </summary>
		/// <param name="filter">The filter to apply to the query.</param>
		/// <returns>The matching entity.</returns>
		public override IEntity GetEntity(IDataFilter filter)
		{
			IDataIndexer indexer = Provider.InitializeDataIndexer();
			indexer.AutoRelease = AutoRelease;
			indexer.DataStore = DataStore;
			
			IEntity[] entities = indexer.GetEntities(filter);
			
			if (entities != null && entities.Length > 0)
				return entities[0];
			else
				return null;
		}
		
		/// <summary>
		/// Retrieves the entity matching the filter group.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The matching entity.</returns>
		public override T GetEntity<T>(FilterGroup group)
		{
			return (T)GetEntity(group);
		}
		
		/// <summary>
		/// Retrieves the entity matching the filter group.
		/// </summary>
		/// <param name="group">The group of filters to apply to the query.</param>
		/// <returns>The matching entity.</returns>
		public override IEntity GetEntity(FilterGroup group)
		{
			IDataIndexer indexer = Provider.InitializeDataIndexer();
			indexer.AutoRelease = AutoRelease;
			indexer.DataStore = DataStore;
			
			IEntity[] entities = indexer.GetEntities(group);
			
			if (entities != null && entities.Length > 0)
				return entities[0];
			else
				return null;
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="type">The type of entity to retrieve.</param>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override IEntity GetEntity(Type type, Dictionary<string, object> parameters)
		{
			IEntity entity = null;
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the entity of the specified type matching the provided parameters."))
			{
				foreach (string key in parameters.Keys)
				{
					LogWriter.Debug("Parameter: " + key + " = " + parameters[key].ToString());
				}
				
				IDataIndexer indexer = Provider.InitializeDataIndexer();
				indexer.AutoRelease = AutoRelease;
				indexer.DataStore = DataStore;
				
				IEntity[] entities = indexer.GetEntities(type, parameters);
				
				// TODO: Check if this should be ignored.
				if (entities.Length > 1)
					throw new Exception("More than one match found when there should only be one.");
				
				if (entities == null || entities.Length == 0)
					entity = null;
				else
					entity = entities[0];
			}
			return entity;
		}
		
		
		/// <summary>
		/// Retrieves the entity of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entity of the specified type found in the data store.</returns>
		public override IEntity GetEntity(Type type, string propertyName, object propertyValue)
		{
			IEntity entity = null;
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the entity of the specified type with a property matching the provided property name and value."))
			{
				IDataIndexer indexer = Provider.InitializeDataIndexer();
				indexer.AutoRelease = AutoRelease;
				indexer.DataStore = DataStore;
				
				IEntity[] entities = indexer.GetEntities(type, propertyName, propertyValue);

				if (entities != null && entities.Length > 0)
					entity = entities[0];
				
				if (entity == null)
					LogWriter.Debug("Entity: [null]");
				else
					LogWriter.Debug("Entity ID: " + entity.ID.ToString());
			}
			
			return entity;
		}
		
		/// <summary>
		/// Retrieves the entity of the specified type from the data store.
		/// </summary>
		/// <param name="propertyName">Name of the property to match.</param>
		/// <param name="propertyValue">The value of the property to match.</param>
		/// <returns>The entity of the specified type found in the data store.</returns>
		public override T GetEntity<T>(string propertyName, object propertyValue)
		{
			T entity = default(T);
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the entity of the specified type matching the provided property value."))
			{
				
				IDataIndexer indexer = Provider.InitializeDataIndexer();
				indexer.AutoRelease = AutoRelease;
				indexer.DataStore = DataStore;
				
				T[] entities = indexer.GetEntities<T>(propertyName, propertyValue);

				if (entities == null || entities.Length == 0)
					entity = default(T);
				else
					entity = entities[0];
			}
			return entity;
		}
		
		/// <summary>
		/// Retrieves the first/only entity that has a reference matching the provided parameters.
		/// </summary>
		/// <param name="propertyName">The name of the property to query for.</param>
		/// <param name="referencedEntityType">The type of entity being referenced.</param>
		/// <param name="referencedEntityID">The ID of the referenced entity to match.</param>
		/// <returns>An array of the objects retrieved.</returns>
		public override T GetEntityWithReference<T>(string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			IDataIndexer indexer = Provider.InitializeDataIndexer();
			indexer.AutoRelease = AutoRelease;
			indexer.DataStore = DataStore;
			
			T[] entities = indexer.GetEntitiesWithReference<T>(propertyName, referencedEntityType, referencedEntityID);

			if (entities.Length == 0)
				return default(T);
			else
				return entities[0];
		}
		
		
		/// <summary>
		/// Retrieves the entity of the specified type matching the specified values.
		/// </summary>
		/// <param name="parameters">The parameters to query with.</param>
		/// <returns></returns>
		public override T GetEntity<T>(Dictionary<string, object> parameters)
		{
			T entity = default(T);
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the entity of the specified type matching the provided entities."))
			{
				
				IDataIndexer indexer = Provider.InitializeDataIndexer();
				indexer.AutoRelease = AutoRelease;
				indexer.DataStore = DataStore;
				
				T[] entities = indexer.GetEntities<T>(parameters);
				if (entities == null || entities.Length == 0)
					entity = default(T);
				else
					entity = entities[0];
			}
			return entity;
		}
		
		/// <summary>
		/// Retrieves a bound version of the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>A bound version of the provided entity.</returns>
		public override IEntity GetEntity(IEntity entity)
		{
			IEntity found = null;
			
			// If it's a reference entity then use the referencer to retrieve a bound version
			if (entity is EntityReference)
			{
				IDataReferencer referencer = Provider.InitializeDataReferencer();
				referencer.AutoRelease = AutoRelease;
				referencer.DataStore = DataStore;
				
				EntityReference reference = (EntityReference)entity;
				
				found = referencer.GetReference(EntityState.GetType(reference.Type1Name),
				                               reference.Entity1ID,
				                               reference.Property1Name,
				                               EntityState.GetType(reference.Type2Name),
				                               reference.Entity2ID,
				                               reference.Property2Name,
				                               false);
			}
			else
				found = GetEntity(entity.GetType(), "ID", entity.ID);
			
			if (found == null)
				throw new Exception("No entity found.");
			
			return found;
		}
	}
}
