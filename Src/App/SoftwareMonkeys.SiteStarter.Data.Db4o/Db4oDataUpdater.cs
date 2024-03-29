﻿using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Description of Db4oDataUpdater.
	/// </summary>
	public class Db4oDataUpdater : DataUpdater
	{
		
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataUpdater(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		/// <summary>
		/// Prepares the provided entity to be updated and handles references.
		/// </summary>
		/// <param name="entity">The entity to be prepared for update.</param>
		public override void PreUpdate(IEntity entity)
		{			
			using (LogGroup logGroup = LogGroup.Start("Preparing entity to be updated.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Entity type: " + entity.GetType().ToString());
				LogWriter.Debug("Entity ID : " + entity.ID);
				
				entity.PreStore();
				
				// If the entity is NOT a reference object (ie. it's a standard entity)
				// then prepare the update.
				// If it IS a reference object the preparation is not needed and should be skipped
				if (!(entity is EntityReference))
				{
					Provider.Referencer.MaintainReferences(entity);
				}
			}
		}
		
		public void PostUpdate(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Executing post-update."))
			{
				Provider.Referencer.SetMirrorCountProperties(entity);
			}
		}
		
		/// <summary>
		/// Updates the provided entity.
		/// </summary>
		/// <param name="entity">The entity to update.</param>
		public override void Update(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Updating the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				Db4oDataStore store = (Db4oDataStore)GetDataStore(entity);
				
				if (!store.IsStored(entity))
					throw new ArgumentException("entity", "The provided entity of type '" + entity.GetType() + "' is not found in the store with name '" + store.Name + "'.");
				
				if (entity.ID == Guid.Empty)
					throw new ArgumentException("entity.ID must be set.");
				
				using (Batch batch = BatchState.StartBatch())
				{
					LogWriter.Debug("Entity type: " + entity.GetType().ToString());
					LogWriter.Debug("Entity ID: " + entity.ID);
					
					// Preupdate must be called to ensure all references are correctly stored
					PreUpdate(entity);
					
					// Get a bound copy of the entity
					IDataReader reader = Provider.InitializeDataReader();
					reader.AutoRelease = false; // Tell the reader not to unbind the entity from the store because it's still within the data access system
					
					IEntity existingEntity = reader.GetEntity(entity);
					
					if (existingEntity != null)
					{
						// Activate the found entity
						Provider.Activator.Activate(existingEntity);
						
						// Copy the provided data to the bound entity
						entity.CopyTo(existingEntity);
						
						// Remove all the referenced entities
						existingEntity.Deactivate();
						
						store.ObjectContainer.Store(existingEntity);
						
						LogWriter.Debug("Entity updated.");
						
						PostUpdate(entity);
						
						store.Commit();
					}
					else
						throw new InvalidOperationException("Cannot update an entity that doesn't already exist in the data store. Save the entity first.");
				}
				
				
			}
		}
		
	}
}
