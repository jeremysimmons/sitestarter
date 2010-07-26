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
			using (LogGroup logGroup = AppLogger.StartGroup("Preparing entity for saving: " + entity.GetType().ToString(), NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				Type entityType = entity.GetType();
				
				AppLogger.Debug("Entity type: " + entityType.ToString());
				
				Provider.Referencer.MaintainReferences(entity);
				
				AppLogger.Debug("Presave complete.");
			}
		}
			
		/// <summary>
		/// Saves the provided entity into the provided data store.
		/// </summary>
		/// <param name="entity">The entity to save to the data store.</param>
		public override void Save(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Saving entity.", NLog.LogLevel.Debug))
			{
				Db4oDataStore store = (Db4oDataStore)GetDataStore(entity);
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (entity.ID == Guid.Empty)
					throw new ArgumentException("entity.ID must be set.");
				
				//ReferenceValidator validator = new ReferenceValidator();
				//validator.CheckForCircularReference(entity);
				
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
						
						PreSave(entity);
						
						if (entity != null)
						{
							DataUtilities.StripReferences(entity);
							
							// Save the entity
							store.ObjectContainer.Store(entity);
							store.Commit();
							
							AppLogger.Debug("Entity stored in '" + store.Name + "' store.");
						}
						else
						{
							AppLogger.Debug("Cloned entity == null. Not stored.");
							
						}
					}
				}
			}
		}
		
		

	}
}
