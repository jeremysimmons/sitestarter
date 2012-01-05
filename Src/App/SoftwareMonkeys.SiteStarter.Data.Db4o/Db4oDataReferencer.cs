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
	/// 
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
		
		/// <summary>
		/// Retrieves the reference between the specified entities.
		/// </summary>
		/// <returns>The reference matching the parameters.</returns>
		public override EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll)
		{
			// TODO: Clean up this function
			
			EntityReferenceCollection output = new EntityReferenceCollection();
			
			// TODO: Check if logging should be commented out to boost performance
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving reference."))
			{
				
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
				if (referenceEntityID == Guid.Empty)
					throw new ArgumentException("A reference entity ID must be provided.", "referenceEntityID");
				
				LogWriter.Debug("Entity ID: " + entityID.ToString());
				LogWriter.Debug("Entity type: " + entityType.ToString());
				LogWriter.Debug("Reference entity ID: " + referenceEntityID.ToString());
				LogWriter.Debug("Reference type: " + referenceType.ToString());
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
				
				string storeName = DataUtilities.GetDataStoreName(
					entityType.Name,
					referenceType.Name);
				
				LogWriter.Debug("Data store name: " + storeName);
				
				Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);
				
				IQuery query1 = dataStore.ObjectContainer.Query();
				query1.Constrain(typeof(EntityReference));
				
				query1.Descend("entity1ID").Constrain(entityID).Equal().And(
					query1.Descend("property1Name").Constrain(propertyName).Equal().And(
						query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query1.Descend("entity2ID").Constrain(referenceEntityID).Equal().And(
								query1.Descend("property2Name").Constrain(mirrorPropertyName).Equal().And(
									query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
				
				IQuery query2 = dataStore.ObjectContainer.Query();
				query2.Constrain(typeof(EntityReference));
				
				query2.Descend("entity2ID").Constrain(entityID).Equal().And(
					query2.Descend("property2Name").Constrain(propertyName).Equal().And(
						query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query2.Descend("entity1ID").Constrain(referenceEntityID).Equal().And(
								query2.Descend("property1Name").Constrain(mirrorPropertyName).Equal().And(
									query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
				
				
				
				IObjectSet os1 = query1.Execute();
				
				while (os1.HasNext())
				{
					EntityReference reference = (EntityReference)os1.Next();
					if (reference != null && reference.Entity1ID != Guid.Empty && reference.Entity2ID != Guid.Empty)
					{
						if (reference.Includes(entityID, propertyName) &&
						    reference.Includes(referenceEntityID, mirrorPropertyName))
						{
							output.Add(reference);
						}
					}
				}
				
				IObjectSet os2 = query2.Execute();
				
				while (os2.HasNext())
				{
					EntityReference reference = (EntityReference)os2.Next();
					if (reference != null && reference.Entity1ID != Guid.Empty && reference.Entity2ID != Guid.Empty)
					{
						if (reference.Includes(entityID, propertyName) &&
						    reference.Includes(referenceEntityID, mirrorPropertyName))
						{
							output.Add(reference);
						}
					}
				}
				
				if (output.Count == 0)
				{
					LogWriter.Debug("No references loaded from the data store.");
				}
				else
				{
					
					
					LogWriter.Debug("# references loaded: " + output.Count.ToString());
					
					if (output.Count > 1)
						LogWriter.Error("Multiple (" + output.Count.ToString() + ") references found when there should only be one.");
					
					int i = 0;
					
					foreach (EntityReference r in output)
					{
						i++;
						
						using (LogGroup logGroup2 = LogGroup.StartDebug("Processing ID reference."))
						{
							LogWriter.Debug("Data store name: " + storeName);
							
							
							EntityReference reference = (EntityReference)r.SwitchFor(entityType, entityID);
							
							
							LogWriter.Debug("Loaded reference " + i + "/" + output.Count.ToString() + " - Property name 1: " + reference.Property1Name);
							LogWriter.Debug("Loaded reference " + i + "/" + output.Count.ToString() + " - Property name 2: " + reference.Property2Name);
							
							
							
							if (activateAll)
							{
								LogWriter.Debug("Activating reference.");
								Provider.Activator.ActivateReference(reference);
							}
						}
					}
				}
			}
			if (output != null && output.Count > 0)
				return output[0];
			else
				return null;
		}
		
		/// <summary>
		/// Checks whether there is a reference between the specified entity and the specified type.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entityID"></param>
		/// <param name="propertyName"></param>
		/// <param name="referenceType"></param>
		/// <param name="referenceEntityID"></param>
		/// <param name="mirrorPropertyName"></param>
		/// <returns>A value indicating whether a match was found.</returns>
		public override bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referenceType, string mirrorPropertyName)
		{
			bool isMatch = false;
			
			//using (LogGroup logGroup = LogGroup.StartDebug("Retrieving reference."))
			//{
				
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
				bool referenceFound = false;
				
				LogWriter.Debug("Entity ID: " + entityID.ToString());
				LogWriter.Debug("Entity type: " + entityType.ToString());
				LogWriter.Debug("Reference type: " + referenceType.ToString());
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
				
				string storeName = DataUtilities.GetDataStoreName(
					entityType.Name,
					referenceType.Name);
				
			//	LogWriter.Debug("Data store name: " + storeName);
				
				Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);
				
				IQuery query1 = dataStore.ObjectContainer.Query();
				query1.Constrain(typeof(EntityReference));
				
				query1.Descend("entity1ID").Constrain(entityID).Equal().And(
					query1.Descend("property1Name").Constrain(propertyName).Equal().And(
						query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
								query1.Descend("property2Name").Constrain(mirrorPropertyName).Equal().And(
									query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal()))));
				
				IQuery query2 = dataStore.ObjectContainer.Query();
				query2.Constrain(typeof(EntityReference));
				
				query2.Descend("entity2ID").Constrain(entityID).Equal().And(
					query2.Descend("property2Name").Constrain(propertyName).Equal().And(
						query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
								query2.Descend("property1Name").Constrain(mirrorPropertyName).Equal().And(
									query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal()))));
				
				
				
				IObjectSet os1 = query1.Execute();
				
				if (os1.Count > 0)
					referenceFound = true;
				
				IObjectSet os2 = query2.Execute();
				
				if (os2.Count > 0)
					referenceFound = true;
				
				isMatch = referenceFound;
				
			//	LogWriter.Debug("Is match: " + isMatch.ToString());
				
			//}
			
			return isMatch;
		}
		
		/// <summary>
		/// Checks whether there is a reference between the specified entities (in either order).
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entityID"></param>
		/// <param name="propertyName"></param>
		/// <param name="referenceType"></param>
		/// <param name="referenceEntityID"></param>
		/// <returns>A value indicating whether a matching entity was found.</returns>
		public override bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID)
		{
			return MatchReference(entityType, entityID, propertyName, referenceType, referenceEntityID, String.Empty);
		}
		
		/// <summary>
		/// Checks whether there is a reference between the specified entities (in either order).
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entityID"></param>
		/// <param name="propertyName"></param>
		/// <param name="referenceType"></param>
		/// <param name="referenceEntityID"></param>
		/// <param name="mirrorPropertyName"></param>
		/// <returns>A value indicating whether a matching entity was found.</returns>
		public override bool MatchReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referencedEntityID, string mirrorPropertyName)
		{
			bool isMatch = false;
			
			//using (LogGroup logGroup = LogGroup.StartDebug("Retrieving reference."))
			//{
				
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
				bool referenceFound = false;
				
			//	LogWriter.Debug("Entity ID: " + entityID.ToString());
			//	LogWriter.Debug("Entity type: " + entityType.ToString());
			//	LogWriter.Debug("Reference entity ID: " + referencedEntityID.ToString());
			//	LogWriter.Debug("Reference type: " + referenceType.ToString());
			//	LogWriter.Debug("Property name: " + propertyName);
			//	LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
				
				string storeName = DataUtilities.GetDataStoreName(
					entityType.Name,
					referenceType.Name);
				
			//	LogWriter.Debug("Data store name: " + storeName);
				
				Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);
				
				IQuery query1 = dataStore.ObjectContainer.Query();
				query1.Constrain(typeof(EntityReference));
				
				query1.Descend("entity1ID").Constrain(entityID).Equal().And(
					query1.Descend("property1Name").Constrain(propertyName).Equal().And(
						query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query1.Descend("entity2ID").Constrain(referencedEntityID).Equal().And(
								query1.Descend("property2Name").Constrain(mirrorPropertyName).Equal().And(
									query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
				
				IQuery query2 = dataStore.ObjectContainer.Query();
				query2.Constrain(typeof(EntityReference));
				
				query2.Descend("entity2ID").Constrain(entityID).Equal().And(
					query2.Descend("property2Name").Constrain(propertyName).Equal().And(
						query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query2.Descend("entity1ID").Constrain(referencedEntityID).Equal().And(
								query2.Descend("property1Name").Constrain(mirrorPropertyName).Equal().And(
									query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
				
				
				
				IObjectSet os1 = query1.Execute();
				
				if (os1.Count > 0)
					referenceFound = true;
				
				IObjectSet os2 = query2.Execute();
				
				if (os2.Count > 0)
					referenceFound = true;
				
				isMatch = referenceFound;
				
			//	LogWriter.Debug("Is match: " + isMatch.ToString());
				
			//}
			
			return isMatch;
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
		public override EntityReferenceCollection GetReferences(Type entityType, Guid entityID, string propertyName, Type referenceType, bool activateAll)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving references."))
			{
				
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
				LogWriter.Debug("Entity type: " + entityType.ToString());
				LogWriter.Debug("Reference type: " + referenceType.ToString());
				
				Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);
				
				if(dataStore.DoesExist)
				{
					EntityReferenceCollection list = new EntityReferenceCollection();
					
					IQuery query1 = dataStore.ObjectContainer.Query();
					query1.Constrain(typeof(EntityReference));
					
					IConstraint constraint1 = query1.Descend("property1Name").Constrain(propertyName).Equal().And(
						query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal()));
					
					if (entityID != Guid.Empty)
						constraint1.And(query1.Descend("entity1ID").Constrain(entityID).Equal());
					
					IQuery query2 = dataStore.ObjectContainer.Query();
					query2.Constrain(typeof(EntityReference));
					
					IConstraint constraint2 = query2.Descend("property2Name").Constrain(propertyName).Equal().And(
						query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
							query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal()));
					
					if (entityID != Guid.Empty)
						constraint2.And(query2.Descend("entity2ID").Constrain(entityID).Equal());
					
					IObjectSet os1 = query1.Execute();
					
					while (os1.HasNext())
					{
						EntityReference reference = (EntityReference)os1[i];
						
						list.Add(reference);
					}
					
					IObjectSet os2 = query2.Execute();
					
					for (int i = 0; i < os2.Count; i++)
					{
						EntityReference reference = (EntityReference)os2[i];
						
						list.Add(reference);
					}
					
					if (list.Count == 0)
					{
						LogWriter.Debug("No references loaded from the data store.");
					}
					else
					{
						LogWriter.Debug("Count: " + list.Count);
						
						foreach (EntityReference r in list)
						{
							using (LogGroup logGroup2 = LogGroup.StartDebug("Processing ID reference."))
							{
								
								EntityReference reference = (EntityReference)r.SwitchFor(entityType, entityID);
								
								LogWriter.Debug("Loaded reference - Entity ID 1: " + reference.Entity1ID);
								LogWriter.Debug("Loaded reference - Entity ID 2: " + reference.Entity2ID);
								
								LogWriter.Debug("Loaded reference - Property 1 name: " + reference.Property1Name);
								LogWriter.Debug("Loaded reference - Property 2 name: " + reference.Property2Name);
								
								LogWriter.Debug("Loaded reference - Type name 1: " + reference.Type1Name);
								LogWriter.Debug("Loaded reference - Type name 2: " + reference.Type2Name);
								
								if (reference.Entity1ID != Guid.Empty
								    && reference.Entity2ID != Guid.Empty)
								{
									//	LogWriter.Debug("Adding to the collection.");
									collection.Add(reference);
								}
								else
								{
									LogWriter.Error("Reference not added to the collection. IDs are empty. This shouldn't happen but the system can ignore it and continue. Invalid references like these should probably be deleted.");
								}
							}
						}
					}
				}
				
				LogWriter.Debug("References #: " + collection.Count.ToString());
				
				
				if (activateAll)
				{
					LogWriter.Debug("Activating references.");
					
					foreach (EntityReference reference in collection)
					{
						Provider.Activator.ActivateReference(reference);
					}
				}
				
				LogWriter.Debug("References #: " + collection.Count.ToString());
			}
			
			return collection;
		}

		/// <summary>
		/// Retrieves all the references to the specified entity. The specified entity can be either the source or reference entity as references work both ways.
		/// </summary>
		/// <param name="entityType">The type of the entity to retrieve the corresponding references for.</param>
		/// <param name="entityID">The ID of the entity to retrieve the corresponding references for.</param>
		/// <param name="referenceType">The type of entity at the other side of the reference to the one specified.</param>
		/// <param name="activateAll">A value indicating whether to activate the references by loading the corresponding entities and setting them to the SourceEntity and ReferenceEntity properties.</param>
		/// <returns>A collection of references that match the provided parameters.</returns>
		public override EntityReferenceCollection GetReferences(Type entityType, Guid entityID, Type referenceType, bool activateAll)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving references."))
			{
				
				if (entityType == null)
					throw new ArgumentNullException("entityType");
				
				if (referenceType == null)
					throw new ArgumentNullException("referenceType");
				
				LogWriter.Debug("Entity type: " + entityType.ToString());
				LogWriter.Debug("Reference type: " + referenceType.ToString());
				
				Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);
				
				if(dataStore.DoesExist)
				{
					EntityReferenceCollection list = new EntityReferenceCollection();
					
					IQuery query1 = dataStore.ObjectContainer.Query();
					query1.Constrain(typeof(EntityReference));
					
					IConstraint constraint1 = query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal());
					
					if (entityID != Guid.Empty)
						constraint1.And(query1.Descend("entity1ID").Constrain(entityID).Equal());
					
					IQuery query2 = dataStore.ObjectContainer.Query();
					query2.Constrain(typeof(EntityReference));
					
					IConstraint constraint2 = query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal());
					
					if (entityID != Guid.Empty)
						constraint2.And(query2.Descend("entity2ID").Constrain(entityID).Equal());
					
					IObjectSet os1 = query1.Execute();
					
					while (os1.HasNext())
					{
						EntityReference reference = (EntityReference)os1.Next();
						
						list.Add(reference);
					}
					
					IObjectSet os2 = query2.Execute();
					
					while (os2.HasNext())
					{
						EntityReference reference = (EntityReference)os2.Next();
						
						list.Add(reference);
					}
					
					if (list.Count == 0)
					{
						LogWriter.Debug("No references loaded from the data store.");
					}
					else
					{
						LogWriter.Debug("Count: " + list.Count);
						
						foreach (EntityReference r in list)
						{
							using (LogGroup logGroup2 = LogGroup.StartDebug("Processing reference."))
							{
								
								EntityReference reference = (EntityReference)r.SwitchFor(entityType, entityID);
								
								LogWriter.Debug("Loaded reference - Entity ID 1: " + reference.Entity1ID);
								LogWriter.Debug("Loaded reference - Entity ID 2: " + reference.Entity2ID);
								
								LogWriter.Debug("Loaded reference - Property 1 name: " + reference.Property1Name);
								LogWriter.Debug("Loaded reference - Property 2 name: " + reference.Property2Name);
								
								LogWriter.Debug("Loaded reference - Type name 1: " + reference.Type1Name);
								LogWriter.Debug("Loaded reference - Type name 2: " + reference.Type2Name);
								
								if (reference.Entity1ID != Guid.Empty
								    && reference.Entity2ID != Guid.Empty)
								{
									//	LogWriter.Debug("Adding to the collection.");
									collection.Add(reference);
								}
								else
								{
									LogWriter.Error("Reference not added to the collection. IDs are empty. This shouldn't happen but the system can ignore it and continue. Invalid references like these should probably be deleted.");
								}
							}
						}
					}
				}
				
				LogWriter.Debug("References #: " + collection.Count.ToString());
				
				
				if (activateAll)
				{
					LogWriter.Debug("Activating references.");
					
					foreach (EntityReference reference in collection)
					{
						Provider.Activator.ActivateReference(reference);
					}
				}
				
				LogWriter.Debug("References #: " + collection.Count.ToString());
			}
			
			return collection;
		}
		
	}
}
