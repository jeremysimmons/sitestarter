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
				foreach (EntityIDReference reference in EntitiesUtilities.GetRemovedReferences(entity))
				{
					deleteList.Add(reference);
				}
				
				// Get the current references
				foreach (EntityIDReference reference in EntitiesUtilities.GetReferences(entity))
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
	}
}
