using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Data;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
	/// <summary>
	/// Description of Db4oDataReferencer.
	/// </summary>
	public class Db4oDataReferencer : DataReferencer
	{
		/// <summary>
		/// Sets the data provider and data store of the adapter.
		/// </summary>
		/// <param name="provider">The data provider of adapter.</param>
		/// <param name="store">The data store to tie the adapter to, or [null] to automatically select store.</param>
		public Db4oDataReferencer(Db4oDataProvider provider, Db4oDataStore store)
		{
			Initialize(provider, store);
		}
		
		public override EntityReferenceCollection GetReferences(string type1Name, string type2Name)
		{
			string dataStoreName = DataUtilities.GetDataStoreName(type1Name, type2Name);
			
			EntityIDReference[] entities =
				Provider.Stores[dataStoreName].Indexer.GetEntities<EntityIDReference>();
			
			return new EntityReferenceCollection(entities);
		}
		
		public override bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID, string mirrorPropertyName)
		{
			return MatchReference(entityType, entityID, propertyName, referencedEntityType, referencedEntityID);
		}
		
		public override bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referencedEntityType, Guid referencedEntityID)
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
		/// Retrieves all references that include the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="activateAll"></param>
		/// <returns></returns>
		public override EntityReferenceCollection GetReferences(Type entityType, Guid entityID, string propertyName, Type referenceType, bool activateAll)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving references.", NLog.LogLevel.Debug))
			{
				
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (entityID == Guid.Empty)
					throw new ArgumentNullException("entityID");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
				AppLogger.Debug("Entity type: " + entityType.ToString());
				AppLogger.Debug("Reference type: " + referenceType.ToString());
				
				Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);
				
				if(dataStore.DoesExist)
				{
					// Load the references from the data store
					IList<EntityIDReference> list = dataStore.ObjectContainer.Query<EntityIDReference>(
						delegate(EntityIDReference reference)
						{
							return reference.Includes(entityID, propertyName);
						});
					
					if (list.Count == 0){
						AppLogger.Debug("No references loaded from the data store.");
					}
					else
					{
						AppLogger.Debug("Count: " + list.Count);
						
						foreach (EntityIDReference r in list)
						{
							using (LogGroup logGroup2 = AppLogger.StartGroup("Processing ID reference.", NLog.LogLevel.Debug))
							{
								
								EntityReference reference = (EntityReference)r.SwitchFor(entityType, entityID);
								
								AppLogger.Debug("Loaded reference - Entity ID 1: " + reference.Entity1ID);
								AppLogger.Debug("Loaded reference - Entity ID 2: " + reference.Entity2ID);
								
								AppLogger.Debug("Loaded reference - Property 1 name: " + reference.Property1Name);
								AppLogger.Debug("Loaded reference - Property 2 name: " + reference.Property2Name);
								
								AppLogger.Debug("Loaded reference - Type name 1: " + reference.Type1Name);
								AppLogger.Debug("Loaded reference - Type name 2: " + reference.Type2Name);
								
								if (reference.Entity1ID != Guid.Empty
								    && reference.Entity2ID != Guid.Empty)
								{
									AppLogger.Debug("Adding to the collection.");
									collection.Add(reference);
								}
								else
								{
									AppLogger.Error("Reference not added to the collection. IDs are empty. This shouldn't happen but the system can ignore it and continue. Invalid references like these should probably be deleted.");
								}
							}
						}
					}
				}
				
				AppLogger.Debug("References #: " + collection.Count.ToString());
				
				if (activateAll)
				{
					AppLogger.Debug("Activating references.");
					
					foreach (EntityReference reference in collection)
					{
						Provider.Activator.ActivateReference(reference);
					}
				}
				
				AppLogger.Debug("References #: " + collection.Count.ToString());
			}
			
			return collection;
		}
		
		/// <summary>
		/// Retrieves all references that include the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="activateAll"></param>
		/// <param name="referenceEntityID"></param>
		/// <returns></returns>
		public override EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();//(EntityReferenceCollection)Reflector.CreateGenericObject(typeof(EntityReferenceCollection<,>),
			//                        new Type[] {entity.GetType(),
			//              	referenceType},
			//              new object[] { entity });
			
			// TODO: Comment out logging to improve performance
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving references.", NLog.LogLevel.Debug))
			//{
			
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (referenceType == null)
				throw new ArgumentNullException("referenceType");
			
			//	AppLogger.Debug("Entity type: " + entityType.ToString());
			//	AppLogger.Debug("Reference type: " + referenceType.ToString());
			
			string storeName = DataUtilities.GetDataStoreName(
				entityType.Name,
				referenceType.Name);
			
			//	AppLogger.Debug("Data store name: " + storeName);
			
			Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);

			EntityIDReferenceCollection list = new EntityIDReferenceCollection();
			
			IQuery query1 = dataStore.ObjectContainer.Query();
			query1.Constrain(typeof(EntityIDReference));
			//query1.Constrain(query1.Descend("entity1ID").Constrain(entityID).Equal());
			
			query1.Descend("entity1ID").Constrain(entityID).Equal().And(
				query1.Descend("property1Name").Constrain(propertyName).Equal().And(
					query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query1.Descend("entity2ID").Constrain(referenceEntityID).Equal().And(
							query1.Descend("property2Name").Constrain(mirrorPropertyName).Equal().And(
								query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
			
			
			//query1.Descend("entity1ID").Constrain(entityID).Equal().And(
			//	query1.Descend("entity2ID").Constrain(referenceEntityID).Equal());
			
			IQuery query2 = dataStore.ObjectContainer.Query();
			query2.Constrain(typeof(EntityIDReference));
			//query2.Constrain(query2.Descend("entity2ID").Constrain(entityID).Equal());
			
			query2.Descend("entity2ID").Constrain(entityID).Equal().And(
				query2.Descend("property2Name").Constrain(propertyName).Equal().And(
					query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query2.Descend("entity1ID").Constrain(referenceEntityID).Equal().And(
							query2.Descend("property1Name").Constrain(mirrorPropertyName).Equal().And(
								query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
			
			//IConstraint c2 = query2.Descend("entity2ID").Constrain(entityID).Equal().And(
			//	query2.Descend("entity1ID").Constrain(referenceEntityID).Equal());
			
			//ssquery2.Constrain(c2);
			
			
			
			
			
			IObjectSet os1 = query1.Execute();
			
			while (os1.HasNext())
			{
				EntityIDReference reference = (EntityIDReference)os1.Next();
				if (reference != null)
				{
					if (reference.Includes(entityID, propertyName) &&
					    reference.Includes(referenceEntityID, mirrorPropertyName))
					{
						//				AppLogger.Debug("1 Reference matches expected. Adding to the list.");
						list.Add(reference);
					}
					else
					{
						//				AppLogger.Debug("1 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
					}
				}
			}
			
			IObjectSet os2 = query2.Execute();
			
			while (os2.HasNext())
			{
				EntityIDReference reference = (EntityIDReference)os2.Next();
				if (reference != null)
				{
					if (reference.Includes(entityID, propertyName) &&
					    reference.Includes(referenceEntityID, mirrorPropertyName))
					{
						//				AppLogger.Debug("2 Reference matches expected. Adding to the list.");
						list.Add(reference);
					}
					else
					{
						//				AppLogger.Debug("2 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
					}
				}
			}
			
			foreach (EntityIDReference reference in list)
			{
				if (!reference.Includes(entityID, propertyName))
				{
					throw new Exception("Retrieved invalid reference. Doesn't match expected entity with type '" + entityType.Name + "', ID '" + entityID.ToString() + "' and property name '" + propertyName + "'. Instead types are '" + reference.Type1Name + "' and '" + reference.Type2Name + "', IDs are '" + reference.Entity1ID + "' and '" + reference.Entity2ID + "' and property names are '" + reference.Property1Name + "' and '" + reference.Property2Name + "'.");
				}
				if (!reference.Includes(referenceEntityID, mirrorPropertyName))
				{
					throw new Exception("Retrieved invalid reference. Doesn't match expected reference with type '" + referenceType.Name + "', ID '" + referenceEntityID.ToString() + "' and property name '" + propertyName + "'. Instead types are '" + reference.Type1Name + "' and '" + reference.Type2Name + "', IDs are '" + reference.Entity1ID + "' and '" + reference.Entity2ID + "' and property names are '" + reference.Property1Name + "' and '" + reference.Property2Name + "'.");
				}
			}
			
			
			// Load the references from the data store
			/*List<EntityIDReference> list = new List<EntityIDReference>(dataStore.ObjectContainer.Query<EntityIDReference>(delegate(EntityIDReference reference)
				                                                                                                              {
				                                                                                                              	//return ((entityID.Equals(reference.Entity1ID) && propertyName.Equals(reference.Property1Name))
				                                                                                                              	//        || (entityID.Equals(reference.Entity2ID) && propertyName.Equals(reference.Property2Name)));
				                                                                                                              	return reference.Includes(entityID, propertyName) && reference.Includes(referenceEntityID, mirrorPropertyName);
				                                                                                                              }));*/
			
			if (list.Count == 0)
			{
				//		AppLogger.Debug("No references loaded from the data store.");
			}
			else
			{
				
				
				//		AppLogger.Debug("# references loaded: " + list.Count.ToString());
				
				if (list.Count > 1)
					throw new Exception("Multiple (" + list.Count.ToString() + ") references found when there should only be one.");
				
				int i = 0;
				
				foreach (EntityIDReference idReference in list)
				{
					i++;
					
					//			using (LogGroup logGroup2 = AppLogger.StartGroup("Processing ID reference.", NLog.LogLevel.Debug))
					//			{
					//				AppLogger.Debug("Data store name: " + storeName);
					
					EntityReference reference = null;
					
					if (!(idReference is EntityReference))
					{
						reference = new EntityReference(idReference);
					}
					
					reference = (EntityReference)idReference.SwitchFor(entityType, entityID);
					
					/*if (idReference.EntityIDs == null)
						AppLogger.Debug("Loaded reference - Entity IDs: [null]");
					else
					{
						AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Entity ID 1: " + reference.Entity1ID);
						AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Entity ID 2: " + reference.Entity2ID);
					}*/
					
					
					//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Property name 1: " + reference.Property1Name);
//					//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Property name 2: " + reference.Property2Name);
					
					
					//if (idReference.TypeNames == null)
					//{
					//	AppLogger.Debug("Loaded reference - Type names: [null]");
					
					//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Type name 1: " + reference.Type1Name);
					//	AppLogger.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Type name 2: " + reference.Type2Name);
					//}
					
					if (activateAll)
					{
						//					AppLogger.Debug("Activating reference.");
						Provider.Activator.ActivateReference(reference);
					}
					if (reference.Entity1ID != Guid.Empty
					    && reference.Entity2ID != Guid.Empty)
					{
						//					AppLogger.Debug("Adding to the collection.");
						collection.Add(reference);
					}
					else
					{
						//					AppLogger.Debug("Reference not added to the collection. IDs are empty.");
					}
					//			//}
				}
			}
			//}
			if (collection != null && collection.Count > 0)
				return collection[0];
			else
				return null;
		}
		
		public override EntityReferenceCollection GetObsoleteReferences(IEntity entity, Guid[] idsOfEntitiesToKeep)
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
		
		public override EntityReferenceCollection GetObsoleteReferences(IEntity entity, string propertyName, Type referenceType, Guid[] idsOfEntitiesToKeep)
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
		/*public void ActivateReferences<E1, E2>(EntityReferenceCollection<E1, E2> references)
			where E1 : IEntity
			where E2 : IEntity
		{
			foreach (EntityReference<E1, E2> reference in references)
			{
				ActivateReference(reference);
			}
		}*/
		
		
		public override void PersistReferences(EntityReferenceCollection references)
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
		public override void DeleteObsoleteReferences(EntityReferenceCollection references)
		{
			using (LogGroup logGroup2 = AppLogger.StartGroup("Deleting all references that need to be deleted.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Reference #: " + references.Count);
				
				foreach (IEntity reference in references)
				{
					DataAccess.Data.Stores[reference].Deleter.Delete((IEntity)reference);
				}
			}
		}
		
		public override void MaintainReferences(IEntity entity)
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
		
		
		public override IEntity[] GetReferencedEntities(EntityReferenceCollection references, IEntity entity)
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
		
		public override EntityReferenceCollection GetReferences(IEntity entity)
		{
			return GetReferences(entity, false);
		}
		
		public override EntityReferenceCollection GetReferences(IEntity entity, bool activateAll)
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
		
		#region Latest references functions
		
		/// <summary>
		/// Retrieves the active references from the provided entity. This only includes those references currently active and not those in the data store.
		/// </summary>
		/// <param name="entity">The entity containing that the references are assigned to.</param>
		/// <returns>A collection of the active entity references.</returns>
		public override EntityReferenceCollection GetActiveReferences(IEntity entity)
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
		public override EntityReferenceCollection GetRemovedReferences(IEntity entity)
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
		public override EntityReferenceCollection GetActiveReferences(IEntity entity, string propertyName, Type returnType)
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
		/// <returns>The active entity reference.</returns>
		private EntityIDReference GetActiveReferenceFromSingleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
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
		private EntityIDReference[] GetActiveReferencesFromMultipleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
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
		public void BindReferences(EntityReferenceCollection references)
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
