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
			EntityReferenceCollection toUpdate = new EntityReferenceCollection();
			EntityReferenceCollection toDelete = new EntityReferenceCollection();
			
			using (LogGroup logGroup = LogGroup.Start("Preparing entity to be updated.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Entity type: " + entity.GetType().ToString());
				LogWriter.Debug("Entity ID : " + entity.ID);
			
				entity.PreStore();
				
				EntityReferenceCollection latestReferences = Provider.Referencer.GetActiveReferences(entity);
				
				using (LogGroup logGroup2 = LogGroup.Start("Creating list of deletable obsolete references.", NLog.LogLevel.Debug))
				{
					// Delete the old references
					//foreach (EntityIDReference reference in DataAccess.Data.GetObsoleteReferences(entity, new Collection<IEntity>(DataUtilities.GetReferencedEntities(latestReferences, entity)).IDs))
					foreach (EntityIDReference reference in DataAccess.Data.Referencer.GetObsoleteReferences(entity, new Guid[]{}))
					{
						DataAccess.Data.Activator.ActivateReference((EntityReference)reference);
						
						LogWriter.Debug("Reference type #1: " + reference.Type1Name);
						LogWriter.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
						LogWriter.Debug("Reference type #2: " + reference.Type2Name);
						LogWriter.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
						
						toDelete.Add(reference);
					}
				}
				
				Provider.Referencer.DeleteObsoleteReferences(toDelete);
				
				LogWriter.Debug("# to delete: " + toDelete.Count);
				
				using (LogGroup logGroup2 = LogGroup.Start("Creating list of references to be updated.", NLog.LogLevel.Debug))
				{
					foreach (EntityIDReference reference in latestReferences)
					{
						LogWriter.Debug("Reference type #1: " + reference.Type1Name);
						LogWriter.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
						LogWriter.Debug("Reference type #2: " + reference.Type2Name);
						LogWriter.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
						
						Data.DataAccess.Data.Activator.ActivateReference((EntityReference)reference);
						toUpdate.Add((EntityIDReference)reference);
					}
				}
				
				LogWriter.Debug("# to update: " + toUpdate.Count);
				
				Provider.Referencer.PersistReferences(toUpdate);
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
				Db4oDataStore store = (Db4oDataStore)GetDataStore(entity);
				
				if (!store.IsStored(entity))
					throw new ArgumentException("entity", "The provided entity of type '" + entity.GetType() + "' is not found in the store with name '" + store.Name + "'.");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity.ID == Guid.Empty)
					throw new ArgumentException("entity.ID must be set.");
				
				// TODO: Check if needed. Circular references are sometimes wanted.
				//ReferenceValidator validator = new ReferenceValidator();
				//validator.CheckForCircularReference(entity);
				
				// TODO: Check if needed - Deactivate function should be sufficient, which is called anyway
				// If the entity is an EntityReference then cast it back to the simpler EntityIDReference
				//if (EntitiesUtilities.IsReference(entity.GetType()))
				//	entity = ((EntityReference)entity).ToData();
				
				using (Batch batch = BatchState.StartBatch())
				{
					
					LogWriter.Debug("Entity type: " + entity.GetType().ToString());
					LogWriter.Debug("Entity ID: " + entity.ID);
					
					// Preupdate must be called to ensure all references are correctly stored
					PreUpdate(entity);
					
					// Get a bound copy of the entity
					IDataReader reader = DataAccess.Data.InitializeDataReader();
					reader.AutoRelease = false; // Tell the reader not to unbind the entity from the store because it's still within the data access system
					
					IEntity existingEntity = reader.GetEntity(entity.GetType(), "ID", entity.ID);
					
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
						
						store.Commit();
					}
					else
						throw new InvalidOperationException("Cannot update an entity that doesn't not already exist in the data store. Save the entity first.");
				}
				
				
			}
		}
		
	}
}