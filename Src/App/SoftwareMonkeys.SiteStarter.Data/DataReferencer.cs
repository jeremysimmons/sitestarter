using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataReferencer.
	/// </summary>
	public abstract class DataReferencer : DataAdapter, IDataReferencer
	{
		
		/// <summary>
		/// Retrieves all the references between the entities of the specified types.
		/// </summary>
		/// <param name="type1Name">The first type of one of the entities involved in the reference.</param>
		/// <param name="type2Name">The type of the other entity involved in the reference.</param>
		/// <returns>A collection of the entities found in the data store between the two specified types.</returns>
		public virtual EntityReferenceCollection GetReferences(string type1Name, string type2Name)
		{
			string dataStoreName = DataUtilities.GetDataStoreName(type1Name, type2Name);
			
			EntityIDReference[] entities =
				Provider.Stores[dataStoreName].Indexer.GetEntities<EntityIDReference>();
			
			EntityReferenceCollection references = new EntityReferenceCollection();
			
			references.AddRange(entities);
			
			return references;
		}
		
		/// <summary>
		/// Checks whether there is a reference between the specified entities.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entityID"></param>
		/// <param name="propertyName"></param>
		/// <param name="referencedEntityType"></param>
		/// <param name="referencedEntityID"></param>
		/// <param name="mirrorPropertyName"></param>
		/// <returns>A value indicating whether a reference exists between the specified entities.</returns>
		public virtual bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID, string mirrorPropertyName)
		{
			return MatchReference(entityType, entityID, propertyName, referencedEntityType, referencedEntityID);
		}
		
		/// <summary>
		/// Checks whether there is a reference between the specified entities.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entityID"></param>
		/// <param name="propertyName"></param>
		/// <param name="referencedEntityType"></param>
		/// <param name="referencedEntityID"></param>
		/// <returns>A value indicating whether a reference exists between the specified entities.</returns>
		public virtual bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID)
		{
			bool matches = false;
			
			// Logging disabled to boost performance
			
			// TODO: Comment out logging to improve performance
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the provided entity and specified property matches the specified referenced entity ID.", NLog.LogLevel.Debug))
			//{
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (entityID == Guid.Empty)
				throw new ArgumentException("entityID");
			
			if (referencedEntityType == null)
				throw new ArgumentNullException("referencedEntityType");
			
			/*if (referencedEntityID == Guid.Empty)
					throw new ArgumentException("referencedEntityID");*/

			
			/*AppLogger.Debug("Entity type: " + entityType);
				AppLogger.Debug("Entity ID: " + entityID);
				AppLogger.Debug("Property name: " + propertyName);
				AppLogger.Debug("Referenced entity type: " + referencedEntityType.ToString());
				AppLogger.Debug("Referenced entity ID: " + referencedEntityID.ToString());
			 */
			string mirrorPropertyName = String.Empty;
			if (propertyName == String.Empty)
				mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(entityType, propertyName, referencedEntityType);
			else
				mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entityType, propertyName);
			
			EntityReference reference = DataAccess.Data.Referencer.GetReference(entityType, entityID, propertyName, referencedEntityType, referencedEntityID, mirrorPropertyName, false);
			
			//if (reference == null)
			//	AppLogger.Debug("reference == null");
			//else
			//	AppLogger.Debug("reference != null");
			
			matches = (reference != null);
			
			//AppLogger.Debug("Matches: " + matches.ToString());
			//}
			return matches;
		}
		
		/// <summary>
		/// Retrieves all the references to the specified entity. The specified entity can be either the source or reference entity as references work both ways.
		/// </summary>
		/// <param name="entityType">The type of the entity to retrieve the corresponding references for.</param>
		/// <param name="entityID">The ID of the entity to retrieve the corresponding references for.</param>
		/// <param name="propertyName">The name of the property on the specified entity that the reference corresponds with.</param>
		/// <param name="referenceType">The type of entity at the other side of the reference to the one specified.</param>
		/// <param name="activateAll">A value indicating whether to activate the references by loading the corresponding entities and setting them to the SourceEntity and ReferenceEntity properties.</param>
		/// <returns>A collection of references that match the provided parameters.</returns>
		public abstract EntityReferenceCollection GetReferences(Type entityType, Guid entityID, string propertyName, Type referenceType, bool activateAll);
		
		/// <summary>
		/// Retrieves the reference between the specified entities.
		/// </summary>
		/// <returns>The reference matching the parameters.</returns>
		public abstract EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll);
		
		/// <summary>
		/// Retrieves the references that are no longer active on the provided entity and haven't yet been removed from the data store.
		/// </summary>
		/// <param name="entity">The entity containing the active references to compare with.</param>
		/// <param name="idsOfEntitiesToKeep">An array of IDs of the entities that are still active and are not obsolete.</param>
		/// <returns>A collection of the obsolete references that correspond with the provided entity.</returns>
		public virtual EntityReferenceCollection GetObsoleteReferences(IEntity entity, Guid[] idsOfEntitiesToKeep)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection(entity);
			
			Type entityType = entity.GetType();
			
			foreach (PropertyInfo property in entityType.GetProperties())
			{
				if (EntitiesUtilities.IsReference(entityType, property.Name, property.PropertyType))
				{
					collection.AddRange(
						GetObsoleteReferences(entity,
						                      property.Name,
						                      DataUtilities.GetEntityType(entity, property),
						                      idsOfEntitiesToKeep
						                     )
					);
				}
			}
			return collection;
		}
		
		/// <summary>
		/// Retrieves the references that are no longer active on the provided entity and haven't yet been removed from the data store.
		/// </summary>
		/// <param name="entity">The entity containing the active references to compare with.</param>
		/// <param name="propertyName">The name of the property that the references correspond with.</param>
		/// <param name="referenceType">The type of the referenced entity.</param>
		/// <param name="idsOfEntitiesToKeep">An array of IDs of the entities that are still active and are not obsolete.</param>
		/// <returns>A collection of the obsolete references that correspond with the provided entity.</returns>
		public virtual EntityReferenceCollection GetObsoleteReferences(IEntity entity, string propertyName, Type referenceType, Guid[] idsOfEntitiesToKeep)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection(entity);
			
			foreach (EntityReference r in GetReferences(entity.GetType(), entity.ID, propertyName, referenceType, false))
			{
				EntityReference reference = (EntityReference)r;
				
				// If the ID is NOT found in the idsOfEntitiesToKeep array then it should be added to the list.
				// The list is references intended for deletion
				if (Array.IndexOf(idsOfEntitiesToKeep, reference.Entity2ID) == -1)
					collection.Add(reference);
			}
			
			return collection;
		}
		
		/// <summary>
		/// Retrieves all references from the data store that involve with the provided entity.
		/// </summary>
		/// <param name="entity">One of the two entities indicated on the retrieved references. Either the source or reference entity as references work both ways.</param>
		/// <returns>A collection of the references that involve the provided entity.</returns>
		public virtual EntityReferenceCollection GetReferences(IEntity entity)
		{
			return GetReferences(entity, false);
		}
		
		/// <summary>
		/// Persists/saves the provided references into the data store.
		/// </summary>
		/// <param name="references">The collection of entity references to persist.</param>
		public virtual void PersistReferences(EntityReferenceCollection references)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Persisting the provided references.", NLog.LogLevel.Debug))
			{
				if (references != null)
				{
					
					AppLogger.Debug("Reference count: " + references.Count.ToString());
					
					foreach (EntityReference reference in references)
					{
						if (!DataAccess.Data.IsStored(reference))
						{
							//AppLogger.Debug("Reference is new. Saving.");
							
							DataAccess.Data.Saver.Save(reference);
						}
						else
						{
							//AppLogger.Debug("Reference already exists. Updating.");
							
							DataAccess.Data.Updater.Update(reference);
						}
					}
				}
			}
		}
		
		
		/// <summary>
		/// Deletes the provided obsolete references.
		/// </summary>
		/// <param name="references">The array of references to delete.</param>
		public virtual void DeleteObsoleteReferences(EntityReferenceCollection references)
		{
			using (LogGroup logGroup2 = AppLogger.StartGroup("Deleting all references that need to be deleted.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Reference #: " + references.Count);
				
				foreach (IEntity reference in references)
				{
					if (DataAccess.Data.IsStored(reference))
					{
						DataAccess.Data.Stores[reference].Deleter.Delete((IEntity)reference);
					}
				}
			}
		}
		
		/// <summary>
		/// Maintains the references of the provided entity by deleting the obsolete references (those in the data store but no longer active on one of the properties)
		/// and persisting the new references into the data store.
		/// </summary>
		/// <param name="entity">The entity to maintain the references for.</param>
		public virtual void MaintainReferences(IEntity entity)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Maintaining the references for the provided entity.", NLog.LogLevel.Debug))
			{
				EntityReferenceCollection updateList = new EntityReferenceCollection();
				EntityReferenceCollection deleteList = new EntityReferenceCollection();
				
				// Get the removed references
				foreach (EntityIDReference reference in GetRemovedReferences(entity))
				{
					deleteList.Add(reference);
				}
				
				// Get the current/actives references
				foreach (EntityIDReference reference in GetActiveReferences(entity))
				{
					updateList.Add(reference);
				}
				
				// Delete the removed references
				AppLogger.Debug("References to delete: " + deleteList.Count);
				
				this.DeleteObsoleteReferences(deleteList);
				
				
				// Update/save the current references
				AppLogger.Debug("References to update: " + updateList.Count);
				
				this.PersistReferences(updateList);
			}
		}
		
		/// <summary>
		/// Retrieves the referenced entities specified by the provided references and corresponding entity.
		/// </summary>
		/// <param name="references">The collection of references specifying the entities to retrieve.</param>
		/// <param name="entity">The entity that the retrieved references are associated with.</param>
		/// <returns>The referenced entities found.</returns>
		public virtual IEntity[] GetReferencedEntities(EntityReferenceCollection references, IEntity entity)
		{
			Collection<IEntity> collection = new Collection<IEntity>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the referenced entities indicated by the provided reference collection, and from the perspective of the provided entity.",NLog.LogLevel.Debug))
			{
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				AppLogger.Debug("# of references provided: " + references.Count.ToString());
				
				foreach (EntityReference reference in references)
				{
					AppLogger.Debug("Reference type #1: " + reference.Type1Name);
					AppLogger.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
					AppLogger.Debug("Reference type #2: " + reference.Type2Name);
					AppLogger.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
					
					DataAccess.Data.Activator.ActivateReference(reference);
					
					// Check that the entities are set. If they're not set they were likely deleted.
					if (reference.SourceEntity != null && reference.ReferenceEntity != null)
					{
						IEntity otherEntity = reference.GetOtherEntity(entity);
						
						if (otherEntity != null)
						{
							AppLogger.Debug("Other entity type: " + otherEntity.GetType().ToString());
							AppLogger.Debug("Other entity ID: " + otherEntity.ID.ToString());
							
							collection.Add(otherEntity);
						}
						else
						{
							throw new Exception("otherEntity == null");
							AppLogger.Debug("Other entity == null");
						}
					}
				}
				
			}
			
			return collection.ToArray();
		}
		
		/// <summary>
		/// Retrieves the references from the data store that involve the provided entity.
		/// </summary>
		/// <param name="entity">The entity to retrieve the references for.</param>
		/// <param name="activateAll">A value indicating whether to activate the reference by loading the corresponding entities and setting them to the SourceEntity and ReferenceEntity properties.</param>
		/// <returns>A collection of all the references in the store that involve the provided entity.</returns>
		public virtual EntityReferenceCollection GetReferences(IEntity entity, bool activateAll)
		{
			EntityReferenceCollection references = new EntityReferenceCollection();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the references associated with the provided entity."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				foreach (PropertyInfo property in entity.GetType().GetProperties())
				{
					if (EntitiesUtilities.IsReference(entity.GetType(), property))
					{
						Type referenceType = EntitiesUtilities.GetReferenceType(entity.GetType(),
						                                                        property.Name,
						                                                        property.PropertyType);
						
						// TODO: See if performance can be improved by not loading the source entity, as it has been provided as a parameter.
						// It should be possible to use the parameter instead of having to load it again
						
						references.AddRange(
							GetReferences(entity.GetType(),
							              entity.ID,
							              property.Name,
							              referenceType,
							              activateAll));
					}
				}
			}
			
			return references;
		}
		
		
		#region Active references functions
		
		/// <summary>
		/// Retrieves the active references from the provided entity. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing that the references are assigned to.</param>
		/// <returns>A collection of the active entity references.</returns>
		public virtual EntityReferenceCollection GetActiveReferences(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			// Loop through all the properties on the entity class
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				AppLogger.Debug("Checking property: " + property.Name);
				
				if (EntitiesUtilities.IsReference(entity.GetType(), property.Name, property.PropertyType))
				{
					AppLogger.Debug("Property is a reference.");
					
					foreach (EntityIDReference reference in GetActiveReferences(entity, property.Name, property.PropertyType))
					{
						
						AppLogger.Debug("Saving reference.");
						
						collection.Add(reference);
					}
				}
				else
					AppLogger.Debug("Property is NOT a reference.");
				
			}
			
			return collection;
		}
		
		
		/// <summary>
		/// Gets the references that have been removed from the entity.
		/// </summary>
		/// <param name="entity">The entity that references have been removed from.</param>
		/// <returns>A collection of the removed references.</returns>
		public virtual EntityReferenceCollection GetRemovedReferences(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			EntityReferenceCollection references = new EntityReferenceCollection();
			
			// Loop through all the properties on the entity class
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				AppLogger.Debug("Checking property: " + property.Name);
				
				
				if (property.PropertyType.IsSubclassOf(typeof(EntityIDReferenceCollection)))
				{
					EntityIDReferenceCollection collection = (EntityIDReferenceCollection)property.GetValue(entity, null);
					if (collection != null && collection.RemovedReferences != null)
						references.AddRange(collection.RemovedReferences);
				}
				
			}
			
			return references;
		}
		
		
		/// <summary>
		/// Retrieves the active references from the provided property. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing the property that the references are assigned to.</param>
		/// <param name="propertyName">The name of the property that the references are assigned to.</param>
		/// <param name="returnType">The type of the property that the references are assigned to.</param>
		/// <returns>A collection of the entity references.</returns>
		public virtual EntityReferenceCollection GetActiveReferences(IEntity entity, string propertyName, Type returnType)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the reference entities from the specified property on the provided entity.", NLog.LogLevel.Debug))
			{
				Type entityType = entity.GetType();
				
				PropertyInfo property = EntitiesUtilities.GetProperty(entityType, propertyName, returnType);
				
				if (property == null)
					AppLogger.Debug("Property: [null]");
				else
					AppLogger.Debug("Property name: " + property.Name);
				
				if (property != null)
				{
					if (EntitiesUtilities.IsReference(entityType, propertyName, returnType))
					{
						Type referencedEntityType = EntitiesUtilities.GetReferenceType(entityType, propertyName, returnType);
						
						string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity.GetType(), property);
						
						if (EntitiesUtilities.IsMultipleReference(entity.GetType(), property))
						{
							foreach (EntityIDReference r in GetActiveReferencesFromMultipleReferenceProperty(entity, property, mirrorPropertyName))
							{
								if (r != null)
									collection.Add(r);
							}
						}
						else if (EntitiesUtilities.IsSingleReference(entityType, property))
						{
							EntityIDReference r = GetActiveReferenceFromSingleReferenceProperty(entity, property, mirrorPropertyName);
							if (r != null)
								collection.Add(r);
						}
						else
							throw new NotSupportedException("The property type '" + property.PropertyType.ToString() + "' is not supported.");
					}
					else
						throw new ArgumentException("The specified property is not a reference.");
					
					AppLogger.Debug("References found: " + collection.Count.ToString());
				}
				else
				{
					throw new Exception("Cannot find property '" + propertyName + "' on type '" + entity.GetType().ToString() + "'.");
				}
			}
			
			return collection;
		}
		
		/// <summary>
		/// Retrieves the active references from the provided property. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing the property that the references are assigned to.</param>
		/// <param name="property">The property that the references are assigned to.</param>
		/// <param name="mirrorPropertyName">The name of the mirror property.</param>
		/// <returns>The active entity reference if it exists.</returns>
		protected virtual EntityIDReference GetActiveReferenceFromSingleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the reference from a single reference property.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Single reference property.");
				
				IEntity[] referencedEntities = EntitiesUtilities.GetReferencedEntities(entity, property);
				
				if (referencedEntities != null && referencedEntities.Length > 0)
				{
					IEntity referencedEntity = referencedEntities[0];
					
					// TODO: Check if this can be simplified by skipping the collection part.
					// It's only a single reference so the collection is unnecessary
					collection = new EntityReferenceCollection(entity, property.Name, new IEntity[] {referencedEntity}, mirrorPropertyName);
					
					//foreach (EntityIDReference r in references)
					//{
					//AppLogger.Debug("Adding reference with ID: " + r.ID.ToString());
					
					//AppLogger.Debug("Source entity ID: " + r.Entity1ID.ToString());
					//AppLogger.Debug("Referenced entity ID: " + r.Entity2ID.ToString());
					
					//AppLogger.Debug("Source entity name: " + r.Type1Name);
					//AppLogger.Debug("Referenced entity name: " + r.Type2Name);
					
					//}
				}
				else
					AppLogger.Debug("referencedEntities == null || referencedEntities.Length = 0");
				
				// Ensure the references are bound with those stored
				BindReferences(collection);
			}
			
			if (collection.Count > 0)
				return collection[0].ToData();
			else
				return null;
		}
		
		/// <summary>
		/// Retrieves the active references from the provided entity property. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing the property that the references are assigned to.</param>
		/// <param name="property">The property that the references are assigned to.</param>
		/// <param name="mirrorPropertyName">The name of the mirror property.</param>
		/// <returns>An array of the entity references.</returns>
		protected virtual EntityIDReference[] GetActiveReferencesFromMultipleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the references from a multiple reference property.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Multiple reference property.");
				
				object propertyValue = property.GetValue(entity, null);
				
				AppLogger.Debug("Property value: " + (propertyValue == null ? "[null]" : propertyValue.ToString()));
				
				Collection<IEntity> referencedEntities = new Collection<IEntity>();
				
				//if (propertyValue is Array)
				//{
				referencedEntities.AddRange(EntitiesUtilities.GetReferencedEntities(entity, property));
				//}
				//else
				//{
				//	foreach (IEntity entity in (Collection<IEntity>)propertyValue))
				//	{
				//		referencedEntities.Add(entity);
				///	}
				//}
				
				AppLogger.Debug("# of referenced entities found: " + referencedEntities.Count);
				
				EntityReferenceCollection references = new EntityReferenceCollection(entity, property.Name, referencedEntities.ToArray(), mirrorPropertyName);

				
				AppLogger.Debug("Reference objects created.");
				
				foreach (EntityIDReference reference in references)
				{
					AppLogger.Debug("Adding reference with ID: " + reference.ID.ToString());

					AppLogger.Debug("Source entity ID: " + reference.Entity1ID.ToString());
					AppLogger.Debug("Referenced entity ID: " + reference.Entity2ID.ToString());
					
					AppLogger.Debug("Source entity name: " + reference.Type1Name);
					AppLogger.Debug("Referenced entity name: " + reference.Type2Name);
					
					AppLogger.Debug("Source property name: " + reference.Property1Name);
					AppLogger.Debug("Mirror property name: " + reference.Property2Name);
					
					
					collection.Add(reference.ToData());
				}
				
				// Ensure the references are bound with those stored
				BindReferences(collection);
			}
			
			return collection.ToArray();
		}
		
		/// <summary>
		/// Binds the references to those already in the data store (or skips those which are new).
		/// </summary>
		/// <param name="references">The collection of references to be bound.</param>
		public virtual void BindReferences(EntityReferenceCollection references)
		{
			// Logging commented out to boost performance
			//using (LogGroup logGroup = AppLogger.StartGroup("Binding the provided references with those in the data storer.", NLog.LogLevel.Debug))
			//{
			if (references == null)
				throw new ArgumentNullException("references");
			
			for (int i = 0; i < references.Count; i++)
			{
				EntityReference reference = references[i];
				
				bool exists = DataAccess.Data.IsStored(reference);
				
				if (exists)
				{
					//AppLogger.Debug("Already exists: " + exists.ToString());
					
					Type type1 = EntitiesUtilities.GetType(reference.Type1Name);
					Type type2 = EntitiesUtilities.GetType(reference.Type2Name);
					
					EntityReference foundReference = DataAccess.Data.Referencer
						.GetReference(type1,
						              reference.Entity1ID,
						              reference.Property1Name,
						              type2,
						              reference.Entity2ID,
						              reference.Property2Name,
						              false);
					
					if (foundReference != null)
						references[i] = foundReference;
					else
						throw new Exception("Reference matched IsStored check but failed to load.");
				}
			}
			//}
		}
		
		#endregion
	}
}
