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
		public override void PreSave(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Preparing entity for saving: " + entity.GetType().ToString(), NLog.LogLevel.Debug))
			{				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				entity.PreStore();
				
				Type entityType = entity.GetType();
				
				LogWriter.Debug("Entity type: " + entityType.ToString());
				
				Provider.Referencer.MaintainReferences(entity);
				
				LogWriter.Debug("Presave complete.");
			}
		}
			
		/// <summary>
		/// Saves the provided entity into the provided data store.
		/// </summary>
		/// <param name="entity">The entity to save to the data store.</param>
		public override void Save(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Saving entity.", NLog.LogLevel.Debug))
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
						
						PreSave(clonedEntity);
						
						if (clonedEntity != null)
						{
							DataUtilities.StripReferences(clonedEntity);
							
							// Save the entity
							store.ObjectContainer.Store(clonedEntity);
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
