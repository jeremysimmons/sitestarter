﻿using System;
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
		
		/// <summary>
		/// Retrieves the reference between the specified entities.
		/// </summary>
		/// <returns>The reference matching the parameters.</returns>
		public override EntityReference GetReference(Type entityType, Guid entityID, string propertyName, Type referenceType, Guid referenceEntityID, string mirrorPropertyName, bool activateAll)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			// Commented out logging to improve performance
			//using (LogGroup logGroup = LogGroup.Start("Retrieving references.", NLog.LogLevel.Debug))
			//{
			
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (referenceType == null)
				throw new ArgumentNullException("referenceType");
			
			if (referenceEntityID == Guid.Empty)
				throw new ArgumentException("A reference entity ID must be provided.", "referenceEntityID");
			
			//	LogWriter.Debug("Entity type: " + entityType.ToString());
			//	LogWriter.Debug("Reference type: " + referenceType.ToString());
			
			string storeName = DataUtilities.GetDataStoreName(
				entityType.Name,
				referenceType.Name);
			
			//	LogWriter.Debug("Data store name: " + storeName);
			
			Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);

			EntityIDReferenceCollection list = new EntityIDReferenceCollection();
			
			IQuery query1 = dataStore.ObjectContainer.Query();
			query1.Constrain(typeof(EntityIDReference));
			
			query1.Descend("entity1ID").Constrain(entityID).Equal().And(
				query1.Descend("property1Name").Constrain(propertyName).Equal().And(
					query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query1.Descend("entity2ID").Constrain(referenceEntityID).Equal().And(
							query1.Descend("property2Name").Constrain(mirrorPropertyName).Equal().And(
								query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
			
			IQuery query2 = dataStore.ObjectContainer.Query();
			query2.Constrain(typeof(EntityIDReference));
			
			query2.Descend("entity2ID").Constrain(entityID).Equal().And(
				query2.Descend("property2Name").Constrain(propertyName).Equal().And(
					query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query2.Descend("entity1ID").Constrain(referenceEntityID).Equal().And(
							query2.Descend("property1Name").Constrain(mirrorPropertyName).Equal().And(
								query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal())))));
			
			
			
			IObjectSet os1 = query1.Execute();
			
			while (os1.HasNext())
			{
				EntityIDReference reference = (EntityIDReference)os1.Next();
				if (reference != null)
				{
					if (reference.Includes(entityID, propertyName) &&
					    reference.Includes(referenceEntityID, mirrorPropertyName))
					{
						//				LogWriter.Debug("1 Reference matches expected. Adding to the list.");
						list.Add(reference);
					}
					else
					{
						//				LogWriter.Debug("1 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
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
						//				LogWriter.Debug("2 Reference matches expected. Adding to the list.");
						list.Add(reference);
					}
					else
					{
						//				LogWriter.Debug("2 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
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
			
			if (list.Count == 0)
			{
				//		LogWriter.Debug("No references loaded from the data store.");
			}
			else
			{
				
				
				//		LogWriter.Debug("# references loaded: " + list.Count.ToString());
				
				if (list.Count > 1)
					throw new Exception("Multiple (" + list.Count.ToString() + ") references found when there should only be one.");
				
				int i = 0;
				
				foreach (EntityIDReference idReference in list)
				{
					i++;
					
					//			using (LogGroup logGroup2 = LogGroup.Start("Processing ID reference.", NLog.LogLevel.Debug))
					//			{
					//				LogWriter.Debug("Data store name: " + storeName);
					
					EntityReference reference = null;
					
					if (!(idReference is EntityReference))
					{
						reference = new EntityReference(idReference);
					}
					
					reference = (EntityReference)idReference.SwitchFor(entityType, entityID);
					
					/*if (idReference.EntityIDs == null)
						LogWriter.Debug("Loaded reference - Entity IDs: [null]");
					else
					{
						LogWriter.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Entity ID 1: " + reference.Entity1ID);
						LogWriter.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Entity ID 2: " + reference.Entity2ID);
					}*/
					
					
					//	LogWriter.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Property name 1: " + reference.Property1Name);
//					//	LogWriter.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Property name 2: " + reference.Property2Name);
					
					
					//if (idReference.TypeNames == null)
					//{
					//	LogWriter.Debug("Loaded reference - Type names: [null]");
					
					//	LogWriter.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Type name 1: " + reference.Type1Name);
					//	LogWriter.Debug("Loaded reference " + i + "/" + list.Count.ToString() + " - Type name 2: " + reference.Type2Name);
					//}
					
					if (activateAll)
					{
						//					LogWriter.Debug("Activating reference.");
						Provider.Activator.ActivateReference(reference);
					}
					if (reference.Entity1ID != Guid.Empty
					    && reference.Entity2ID != Guid.Empty)
					{
						//					LogWriter.Debug("Adding to the collection.");
						collection.Add(reference);
					}
					else
					{
						//					LogWriter.Debug("Reference not added to the collection. IDs are empty.");
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
			
			//using (LogGroup logGroup = LogGroup.Start("Retrieving references.", NLog.LogLevel.Debug))
			//{
			
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (referenceType == null)
				throw new ArgumentNullException("referenceType");
			
			//LogWriter.Debug("Entity type: " + entityType.ToString());
			//LogWriter.Debug("Reference type: " + referenceType.ToString());
			
			Db4oDataStore dataStore = (Db4oDataStore)GetDataStore(entityType.Name, referenceType.Name);
			
			if(dataStore.DoesExist)
			{
				EntityIDReferenceCollection list = new EntityIDReferenceCollection();
				
				IQuery query1 = dataStore.ObjectContainer.Query();
				query1.Constrain(typeof(EntityIDReference));
				
				IConstraint constraint1 = query1.Descend("property1Name").Constrain(propertyName).Equal().And(
					query1.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query1.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal()));
				
				if (entityID != Guid.Empty)
					constraint1.And(query1.Descend("entity1ID").Constrain(entityID).Equal());
				
				IQuery query2 = dataStore.ObjectContainer.Query();
				query2.Constrain(typeof(EntityIDReference));
				
				IConstraint constraint2 = query2.Descend("property2Name").Constrain(propertyName).Equal().And(
					query2.Descend("type2Name").Constrain(EntitiesUtilities.GetShortType(entityType.Name)).Equal().And(
						query2.Descend("type1Name").Constrain(EntitiesUtilities.GetShortType(referenceType.Name)).Equal()));
				
				if (entityID != Guid.Empty)
					constraint2.And(query2.Descend("entity2ID").Constrain(entityID).Equal());
								
				IObjectSet os1 = query1.Execute();
				
				while (os1.HasNext())
				{
					EntityIDReference reference = (EntityIDReference)os1.Next();
					// TODO: Check if commented out code is needed
				//	if (reference != null)
				//	{
				//		if (reference.Includes(entityID, propertyName))
				//		{
							//				LogWriter.Debug("1 Reference matches expected. Adding to the list.");
							list.Add(reference);
				//		}
				//		else
				//		{
							//				LogWriter.Debug("1 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
				//		}
				//	}
				}
				
				IObjectSet os2 = query2.Execute();
				
				while (os2.HasNext())
				{
					EntityIDReference reference = (EntityIDReference)os2.Next();
					// TODO: Check if commented out code is needed
				//	if (reference != null)
				//	{
				//		if (reference.Includes(entityID, propertyName))
				//		{
				//			//				LogWriter.Debug("2 Reference matches expected. Adding to the list.");
							list.Add(reference);
				//		}
				//		else
				//		{
							//				LogWriter.Debug("2 Reference failed match. Skipping. IMPORTANT!!! IT SHOULD NOT HAVE FAILED!!!");
				//		}
				//	}
				}
				
				if (list.Count == 0)
				{
					//LogWriter.Debug("No references loaded from the data store.");
				}
				else
				{
					//LogWriter.Debug("Count: " + list.Count);
					
					foreach (EntityIDReference r in list)
					{
						//using (LogGroup logGroup2 = LogGroup.Start("Processing ID reference.", NLog.LogLevel.Debug))
						//{
						
						EntityReference reference = (EntityReference)r.SwitchFor(entityType, entityID);
						
						//LogWriter.Debug("Loaded reference - Entity ID 1: " + reference.Entity1ID);
						//LogWriter.Debug("Loaded reference - Entity ID 2: " + reference.Entity2ID);
						
						//LogWriter.Debug("Loaded reference - Property 1 name: " + reference.Property1Name);
						//LogWriter.Debug("Loaded reference - Property 2 name: " + reference.Property2Name);
						
						//LogWriter.Debug("Loaded reference - Type name 1: " + reference.Type1Name);
						//LogWriter.Debug("Loaded reference - Type name 2: " + reference.Type2Name);
						
						if (reference.Entity1ID != Guid.Empty
						    && reference.Entity2ID != Guid.Empty)
						{
							//	LogWriter.Debug("Adding to the collection.");
							collection.Add(reference);
						}
						//else
						//{
						//	LogWriter.Error("Reference not added to the collection. IDs are empty. This shouldn't happen but the system can ignore it and continue. Invalid references like these should probably be deleted.");
						//}
						//}
					}
				}
			}
			
			//LogWriter.Debug("References #: " + collection.Count.ToString());
			
			if (activateAll)
			{
				//LogWriter.Debug("Activating references.");
				
				foreach (EntityReference reference in collection)
				{
					Provider.Activator.ActivateReference(reference);
				}
			}
			
			//LogWriter.Debug("References #: " + collection.Count.ToString());
			//}
			
			return collection;
		}
		
	}
}