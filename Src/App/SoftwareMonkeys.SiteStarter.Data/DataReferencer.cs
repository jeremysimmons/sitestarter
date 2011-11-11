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
		/// Retrieves all the data references between all types.
		/// </summary>
		/// <returns>A collection of the entities found in all data stores.</returns>
		public virtual EntityReferenceCollection GetReferences()
		{
			EntityReferenceCollection references = new EntityReferenceCollection();
			
			foreach (string dataStoreName in DataAccess.Data.GetDataStoreNames())
			{
				// If the name contains a dash then the store contains references
				if (dataStoreName.IndexOf('-') > -1)
				{
					// Constrain the query to the IEntity type to ensure both EntityIDReference and EntityReference objects are returned
					EntityIDReference[] entities = Collection<EntityIDReference>.ConvertAll(
						Provider.Stores[dataStoreName].Indexer.GetEntities());
					
					references.AddRange(entities);
				}
			}
			
			return references;
		}
		
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
			//using (LogGroup logGroup = LogGroup.StartDebug("Checking whether a reference is found matching the details provided between '" + entityType.Name + "' and '" + referencedEntityType.Name + "'."))
			//{
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (entityID == Guid.Empty)
				throw new ArgumentException("entityID");
			
			if (referencedEntityType == null)
				throw new ArgumentNullException("referencedEntityType");
			
			/*if (referencedEntityID == Guid.Empty)
					throw new ArgumentException("referencedEntityID");*/

			
			/*LogWriter.Debug("Entity type: " + entityType);
				LogWriter.Debug("Entity ID: " + entityID);
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Referenced entity type: " + referencedEntityType.ToString());
				LogWriter.Debug("Referenced entity ID: " + referencedEntityID.ToString());
			 */
			string mirrorPropertyName = String.Empty;
			if (propertyName == String.Empty)
				mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyNameReverse(entityType, propertyName, referencedEntityType);
			else
				mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entityType, propertyName);
			
			EntityReference reference = Provider.Referencer.GetReference(entityType, entityID, propertyName, referencedEntityType, referencedEntityID, mirrorPropertyName, false);
			
			//if (reference == null)
			//	LogWriter.Debug("reference == null");
			//else
			//	LogWriter.Debug("reference != null");
			
			matches = (reference != null);
			
			//LogWriter.Debug("Matches: " + matches.ToString());
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
			EntityReferenceCollection collection = null;
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the references that involve the provided entity."))
			{
				collection = GetReferences(entity, false);
			}
			return collection;
		}
		
		/// <summary>
		/// Persists/saves the provided references into the data store.
		/// </summary>
		/// <param name="references">The collection of entity references to persist.</param>
		public virtual void PersistReferences(EntityReferenceCollection references)
		{
			using (LogGroup logGroup = LogGroup.Start("Persisting the provided references.", LogLevel.Debug))
			{
				if (references != null)
				{
					
					LogWriter.Debug("Reference count: " + references.Count.ToString());
					
					foreach (EntityReference reference in references)
					{
						if (!DataAccess.Data.IsStored(reference))
						{
							//LogWriter.Debug("Reference is new. Saving.");
							
							Provider.Saver.Save(reference);
						}
						else
						{
							//LogWriter.Debug("Reference already exists. Updating.");
							
							Provider.Updater.Update(reference);
						}
						
						if (reference.Property1Name != String.Empty)
							SetCountProperty(reference.SourceEntity, reference.Property1Name, reference.Entity2ID);
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
			using (LogGroup logGroup2 = LogGroup.Start("Deleting provided obsolete references.", LogLevel.Debug))
			{
				LogWriter.Debug("References: " + references.Count);
				
				foreach (EntityReference reference in references)
				{
					if (DataAccess.Data.IsStored(reference))
					{
						LogWriter.Debug("Deleting reference.");
						DataAccess.Data.Stores[reference].Deleter.Delete((IEntity)reference);
					}
					else
						LogWriter.Debug("Reference not stored. Skipping.");
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
			using (LogGroup logGroup = LogGroup.Start("Maintaining the references for the provided entity.", LogLevel.Debug))
			{
				EntityReferenceCollection updateList = new EntityReferenceCollection();
				EntityReferenceCollection deleteList = new EntityReferenceCollection();
				
				// Get the current/actives references
				foreach (EntityIDReference reference in GetActiveReferences(entity))
				{
					updateList.Add(reference);
				}
				
				// Get the obsolete references
				foreach (EntityIDReference reference in GetObsoleteReferences(entity, updateList.GetEntityIDs(entity.ID)))
				{
					deleteList.Add(reference);
				}
				
				// Delete the obsolete references
				LogWriter.Debug("References to delete: " + deleteList.Count);
				
				this.DeleteObsoleteReferences(deleteList);
				
				
				// Update/save the current references
				LogWriter.Debug("References to update: " + updateList.Count);
				
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
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving the referenced entities indicated by the provided reference collection, and from the perspective of the provided entity.",LogLevel.Debug))
			{
				
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("# of references provided: " + references.Count.ToString());
				
				foreach (EntityReference reference in references)
				{
					LogWriter.Debug("Reference type #1: " + reference.Type1Name);
					LogWriter.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
					LogWriter.Debug("Reference type #2: " + reference.Type2Name);
					LogWriter.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
					
					DataAccess.Data.Activator.ActivateReference(reference);
					
					// Check that the entities are set. If they're not set they were likely deleted.
					if (reference.SourceEntity != null && reference.ReferenceEntity != null)
					{
						IEntity otherEntity = reference.GetOtherEntity(entity);
						
						if (otherEntity != null)
						{
							LogWriter.Debug("Other entity type: " + otherEntity.GetType().ToString());
							LogWriter.Debug("Other entity ID: " + otherEntity.ID.ToString());
							
							collection.Add(otherEntity);
						}
						else
						{
							throw new Exception("otherEntity == null");
							LogWriter.Debug("Other entity == null");
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
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving the references associated with the provided entity."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				foreach (IDataStore store in DataAccess.Data.Stores)
				{
					if (IsReferenceStore(store))
					{
						if (StoresReferencesToType(store, entity.ShortTypeName))
						{
							references.AddRange(store.Indexer.GetEntities<EntityIDReference>("Entity1ID", entity.ID));
							references.AddRange(store.Indexer.GetEntities<EntityIDReference>("Entity2ID", entity.ID));
						}
					}
				}
			}
			
			return references;
		}

		public bool IsReferenceStore(IDataStore store)
		{
			return store.Name.IndexOf("-") > -1;
		}
		
		public bool StoresReferencesToType(IDataStore store, string entityType)
		{
			string[] parts = store.Name.Split('-');
			
			if (parts.Length > 0)
			{
				return Array.IndexOf(parts, entityType) > -1;
			}
			
			return false;
		}
		
		public bool SetCountProperty(IEntity entity, string referencePropertyName, Guid referencedEntityID)
		{
			bool wasChanged = false;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Setting the specified reference count property."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				LogWriter.Debug("Entity type: " + entity.GetType().FullName);
				LogWriter.Debug("Reference property name: " + referencePropertyName);
				LogWriter.Debug("Referenced entity ID: " + referencedEntityID);
				
				ReferenceAttribute attribute = EntitiesUtilities.GetReferenceAttribute(entity, referencePropertyName);
				
				Type referencedType = EntitiesUtilities.GetReferenceType(entity.GetType(), referencePropertyName);
				
				LogWriter.Debug("Referenced type: " + referencedType.FullName);
				
				if (attribute.CountPropertyName != String.Empty)
				{
					LogWriter.Debug("Count property name: " + attribute.CountPropertyName);
					
					string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity.GetType(), referencePropertyName);
					
					PropertyInfo property = entity.GetType().GetProperty(attribute.CountPropertyName);
					
					// Get the original reference count
					int originalCount = (int)property.GetValue(entity, null);
					
					LogWriter.Debug("Original count: " + originalCount);
					
					if (property == null)
						throw new Exception("'" + attribute.CountPropertyName + "' count property not found on type '" + entity.ShortTypeName + "'.");
					
					// Get the latest reference count
					int count = Provider.Counter.CountEntitiesWithReference(
						entity.GetType(),
						entity.ID,
						referencePropertyName,
						referencedType,
						mirrorPropertyName
					);
					
					LogWriter.Debug("Count: " + count.ToString());
					
					// If the new count is not the same as the old one then update the property
					// otherwise skip
					if (count != originalCount)
					{
						wasChanged = true;
						
						property.SetValue(entity, count, null);
					}
				}
				else
					LogWriter.Debug("No count property specified on the reference attribute.");
			}
			return wasChanged;
		}
		
		/// <summary>
		/// Sets the mirror count property on entities referenced by the one provided.
		/// </summary>
		/// <param name="entity"></param>
		public void SetMirrorCountProperties(IEntity entity)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Setting the mirror count property on entities referenced by the '" + entity.ShortTypeName + "' provided."))
			{
				EntityReferenceCollection references = Provider.Referencer.GetActiveReferences(entity);
				
				foreach (EntityReference reference in references)
				{
					// Activate the reference if the ReferenceProperty and SourceProperty are null
					if (reference.ReferenceEntity == null || reference.SourceEntity == null)
						Provider.Activator.ActivateReference(reference);
					
					// Get the referenced entity
					IEntity referencedEntity = reference.GetOtherEntity(entity);
					
					// Switch the reference to be in the context of the reference entity (ie. the source entity becomes the referenced entity and the referenced entity becomes the source entity.)
					EntityReference switched = reference.SwitchFor(referencedEntity);
					
					string mirrorPropertyName = switched.Property1Name;
					
					if (mirrorPropertyName != String.Empty)
					{
						// Set the new count property on the referenced entity
						bool wasChanged = Provider.Referencer.SetCountProperty(referencedEntity, mirrorPropertyName, entity.ID);
						
						// If the count property was changed then activate and update the referenced entity
						if (wasChanged)
						{
							Provider.Activator.Activate(referencedEntity);
							Provider.Updater.Update(referencedEntity);
						}
					}
				}
			}
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
			
			EntityReferenceCollection collection = new EntityReferenceCollection(entity);
			
			// Loop through all the properties on the entity class
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				if (EntitiesUtilities.IsReference(entity.GetType(), property.Name, property.PropertyType))
				{
					LogWriter.Debug("Property is a reference: " + property.Name);
					
					foreach (EntityReference reference in GetActiveReferences(entity, property.Name, property.PropertyType))
					{
						LogWriter.Debug("Found reference.");
						
						collection.Add(reference);
					}
				}
			}
			
			// TODO: Check if references actually need to be switched (below). In theory they shouldn't and the switch will incur a very
			// minor performance hit (likely negligable) but is here as an added measure to avoid issues
			
			// Ensure that all references have the correct perspective
			//for (int i = 0; i < collection.Count; i++)
			//{
			//	if (collection[i].SourceEntity == null || collection[i].ReferenceEntity == null)
			//		Provider.Activator.ActivateReference(collection[i]);
			//	collection[i] = collection[i].SwitchFor(entity);
			//}
			
			return collection;
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
			EntityReferenceCollection collection = new EntityReferenceCollection(entity);
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving the reference entities from the specified property on the provided entity.", LogLevel.Debug))
			{
				Type entityType = entity.GetType();
				
				PropertyInfo property = EntitiesUtilities.GetProperty(entityType, propertyName, returnType);
				
				if (property == null)
					LogWriter.Debug("Property: [null]");
				else
					LogWriter.Debug("Property name: " + property.Name);
				
				if (property != null)
				{
					if (EntitiesUtilities.IsReference(entityType, propertyName, returnType))
					{
						Type referencedEntityType = EntitiesUtilities.GetReferenceType(entityType, propertyName, returnType);
						
						string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity.GetType(), property);
						
						if (EntitiesUtilities.IsMultipleReference(entity.GetType(), property))
						{
							foreach (EntityReference r in GetActiveReferencesFromMultipleReferenceProperty(entity, property, mirrorPropertyName))
							{
								if (r != null)
									collection.Add(r);
							}
						}
						else if (EntitiesUtilities.IsSingleReference(entityType, property))
						{
							EntityReference r = GetActiveReferenceFromSingleReferenceProperty(entity, property, mirrorPropertyName);
							if (r != null)
								collection.Add(r);
						}
						else
							throw new NotSupportedException("The property type '" + property.PropertyType.ToString() + "' is not supported.");
					}
					else
						throw new ArgumentException("The specified property is not a reference.");
					
					LogWriter.Debug("References found: " + collection.Count.ToString());
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
		protected virtual EntityReference GetActiveReferenceFromSingleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving the reference from a single reference property.", LogLevel.Debug))
			{
				LogWriter.Debug("Single reference property.");
				
				IEntity[] referencedEntities = EntitiesUtilities.GetReferencedEntities(entity, property);
				
				if (referencedEntities != null && referencedEntities.Length > 0)
				{
					IEntity referencedEntity = referencedEntities[0];
					
					// TODO: Check if this can be simplified by skipping the collection part.
					// It's only a single reference so the collection is unnecessary
					collection = new EntityReferenceCollection(entity, property.Name, new IEntity[] {referencedEntity}, mirrorPropertyName);
					
					//foreach (EntityIDReference r in references)
					//{
					//LogWriter.Debug("Adding reference with ID: " + r.ID.ToString());
					
					//LogWriter.Debug("Source entity ID: " + r.Entity1ID.ToString());
					//LogWriter.Debug("Referenced entity ID: " + r.Entity2ID.ToString());
					
					//LogWriter.Debug("Source entity name: " + r.Type1Name);
					//LogWriter.Debug("Referenced entity name: " + r.Type2Name);
					
					//}
				}
				else
					LogWriter.Debug("referencedEntities == null || referencedEntities.Length = 0");
				
				// Ensure the references are bound with those stored
				BindReferences(collection);
			}
			
			if (collection.Count > 0)
				return (EntityReference)collection[0];
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
		protected virtual EntityReference[] GetActiveReferencesFromMultipleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection(entity);
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving the references from a multiple reference property.", LogLevel.Debug))
			{
				LogWriter.Debug("Multiple reference property.");
				
				object propertyValue = property.GetValue(entity, null);
				
				LogWriter.Debug("Property value: " + (propertyValue == null ? "[null]" : propertyValue.ToString()));
				
				Collection<IEntity> referencedEntities = new Collection<IEntity>();
				
				referencedEntities.AddRange(EntitiesUtilities.GetReferencedEntities(entity, property));
				
				LogWriter.Debug("# of referenced entities found: " + referencedEntities.Count);
				
				EntityReferenceCollection references = new EntityReferenceCollection(entity, property.Name, referencedEntities.ToArray(), mirrorPropertyName);

				
				LogWriter.Debug("Reference objects created.");
				
				foreach (EntityReference reference in references)
				{
					LogWriter.Debug("Adding reference with ID: " + reference.ID.ToString());

					LogWriter.Debug("Source entity ID: " + reference.Entity1ID.ToString());
					LogWriter.Debug("Referenced entity ID: " + reference.Entity2ID.ToString());
					
					LogWriter.Debug("Source entity name: " + reference.Type1Name);
					LogWriter.Debug("Referenced entity name: " + reference.Type2Name);
					
					LogWriter.Debug("Source property name: " + reference.Property1Name);
					LogWriter.Debug("Mirror property name: " + reference.Property2Name);
					
					
					collection.Add((EntityReference)reference);
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
			//using (LogGroup logGroup = LogGroup.Start("Binding the provided references with those in the data storer.", LogLevel.Debug))
			//{
			if (references == null)
				throw new ArgumentNullException("references");
			
			for (int i = 0; i < references.Count; i++)
			{
				EntityReference reference = references[i];
				
				bool exists = DataAccess.Data.IsStored(reference);
				
				if (exists)
				{
					//LogWriter.Debug("Already exists: " + exists.ToString());
					
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
