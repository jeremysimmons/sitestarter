using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Description of Db4oDataSaver.
	/// </summary>
	public class Db4oDataSaver : DataSaver
	{
		
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataSaver(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}	
		
		/// <summary>
		/// Prepares the provided entity for saving.
		/// </summary>
		/// <param name="entity">The entity to prepare for saving.</param>
		/// <param name="handleReferences">A value indicating whether to delete old references and save new references.</param>
		public override void PreSave(IEntity entity, bool handleReferences)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Preparing entity for saving: " + entity.GetType().ToString()))
			{				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				entity.PreStore();
				
				// If the entity is NOT a reference object (ie. it's a standard entity)
				// then prepare the save.
				// If is IS a reference object the preparation is not needed and should be skipped
				if (!(entity is EntityReference))
				{
				Type entityType = entity.GetType();
				
				LogWriter.Debug("Entity type: " + entityType.ToString());
				
					if (handleReferences)
					{
					// Maintain the entity references
				Provider.Referencer.MaintainReferences(entity);
					}
				
				LogWriter.Debug("Presave complete.");
			}
		}
		}
			
		public void PostSave(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Executing post-save for entity type '" + entity.ShortTypeName + "'."))
			{
				Provider.Referencer.SetMirrorCountProperties(entity);
			}
		}
		
		/// <summary>
		/// Saves the provided entity into the provided data store.
		/// </summary>
		/// <param name="entity">The entity to save to the data store.</param>
		/// <param name="handleReferences">A value indicating whether to delete old references and save new references.</param>
		public override void Save(IEntity entity, bool handleReferences)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Saving entity of type '" + entity.ShortTypeName + "'."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity.ID == Guid.Empty)
					throw new ArgumentException("entity.ID must be set.");
				
				// Clone the entity so that it doesn't get bound to the store
				IEntity clonedEntity = entity.Clone();
				
				Db4oDataStore store = (Db4oDataStore)GetDataStore(clonedEntity);
				
				if (store.ObjectContainer == null)
					throw new InvalidOperationException("The ObjectContainer has not been initialized on the '" + store.Name + "' data store.");
				
				using (Batch batch = BatchState.StartBatch())
				{
					if (EntitiesUtilities.IsReference(clonedEntity.GetType()) && DataAccess.Data.IsStored(clonedEntity))
					{
						LogWriter.Debug("Existing reference found. Skipping save.");
						// Just skip the saving altogether, if the reference already exists
					}
					else
					{
						if (clonedEntity == null)
							throw new ArgumentNullException("entity");
						
						LogWriter.Debug("Entity type: " + clonedEntity.GetType().ToString());
						LogWriter.Debug("Entity ID: " + clonedEntity.ID.ToString());
						
						PreSave(clonedEntity, handleReferences);
						
						if (clonedEntity != null)
						{
							DataUtilities.StripReferences(clonedEntity);
							
							// Save the entity
							store.ObjectContainer.Store(clonedEntity);
							
							// Post save the original entity NOT the cloned entity
							PostSave(entity);
							
							store.Commit();
							
							LogWriter.Debug("Entity stored in '" + store.Name + "' store.");
						}
						else
						{
							LogWriter.Debug("Cloned entity == null. Not stored.");
							
						}
					}
				}
			}
		}
		
		

	}
}
