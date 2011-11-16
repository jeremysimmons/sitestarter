using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Collections.Generic;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Description of Db4oDataDeleter.
	/// </summary>
	public class Db4oDataDeleter : DataDeleter
	{
		
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataDeleter(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		/// <summary>
		/// Prepares the provided entity for deletion by removing dependent referenced, etc.
		/// </summary>
		/// <param name="entity">The entity to prepare for deletion.</param>
		public override void PreDelete(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.Start("Preparing to delete the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				Db4oDataStore store = (Db4oDataStore)GetDataStore(entity);
				
				//if (entity.ID == Guid.Empty)
				//	throw new ArgumentException("entity.ID must be set.");

				EntityReferenceCollection toDelete = new EntityReferenceCollection();

				if (entity != null && entity.ID != Guid.Empty)
				{
					//Provider.Activator.Activate(entity);
					
					EntityReferenceCollection latestReferences = Provider.Referencer.GetReferences(entity);
					
					// Delete all references
					foreach (PropertyInfo property in entity.GetType().GetProperties())
					{
						bool isReference = EntitiesUtilities.IsReference(entity.GetType(), property.Name, property.PropertyType);
						
						if (isReference)
						{
							using (LogGroup logGroup2 = LogGroup.StartDebug("Checking reference property '" + property.Name + "' for obsolete references."))
							{
							Type referenceType = EntitiesUtilities.GetReferenceType(entity.GetType(), property.Name);
							
								EntityReferenceCollection references = DataAccess.Data.Referencer.GetReferences(entity.GetType(),
							                                                                                 entity.ID,
							                                                                                 property.Name,
							                                                                                 referenceType,
								                                                                                false);
								if (references.Count > 0)
							{
									LogWriter.Debug("Found references: " + references.Count.ToString());
									
									foreach (EntityReference reference in references)
									{
										LogWriter.Debug("Found reference between '" + reference.Type1Name + "' and '" + reference.Type2Name + "'.");
										
								toDelete.Add(reference);
							}
						}
								else
									LogWriter.Debug("No references found associated with this property.");
					}
				}
					}
				}
				
				LogWriter.Debug("References to delete: " + toDelete.Count);
				
				//entitiesToUpdate = toUpdate.ToArray();
				Provider.Referencer.DeleteObsoleteReferences(toDelete);
				
			}
		}

		
		
		/// <summary>
		/// Deletes the provided entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		public override void Delete(IEntity entity)
		{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
			using (LogGroup logGroup = LogGroup.StartDebug("Deleting provided '" + entity.ShortTypeName + "' entity."))
			{
				Db4oDataStore store = (Db4oDataStore)GetDataStore(entity);
			
				if (DataAccess.Data.IsStored(entity))
				{
					using (Batch batch = BatchState.StartBatch())
					{
						IDataReader reader = Provider.InitializeDataReader();
						reader.AutoRelease = false;
					
						entity = reader.GetEntity(entity);
					
						if (entity == null)
							throw new Exception("The entity wasn't found.");
					
						// PreDelete must be called to ensure all references are correctly managed
						PreDelete(entity);

						// Delete the entity
						if (entity != null)
						{
							store.ObjectContainer.Delete(entity);
							
							store.Commit();
						}
					}
				}
			}
		}

	}
}
