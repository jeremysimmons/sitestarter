using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Description of DataUtilities.
	/// </summary>
	public static class DataUtilities
	{
		
		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="type">The type of entity to get the data store name of.</param>
		/// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		/*static public string GetDataStoreNameForReference(IEntity entity, PropertyInfo property)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the data store name for a particulary entity property reference.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				AppLogger.Debug("Entity type: " + entity.GetType());
				AppLogger.Debug("Entity ID: " + entity.ID.ToString());
				AppLogger.Debug("Property name: " + property.Name);
				AppLogger.Debug("Property type: " + property.GetType().ToString());
				
				object value = property.GetValue(entity, null);
				
				
				Type referenceType = DataUtilities.GetReferenceType(entity, property);
				BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);
				
				
				AppLogger.Debug("Property value: " + (value == null ? "[null]" :value.ToString()));
				AppLogger.Debug("Reference type: " + referenceType != null ? referenceType.ToString() : "[null]");
				
				if (attribute == null)
					throw new InvalidOperationException("No reference attribute found for this property on this entity.");
				
				if (attribute.EntitiesPropertyName == String.Empty || attribute.EntitiesPropertyName.Length == 0)
					throw new InvalidOperationException("The specified property '" + property.Name + "' doesn't have an entities property specified in the reference attribute. Cannot retrieve the type.");
				
				if (property.Name != attribute.EntitiesPropertyName)
				{
					string name = String.Empty;
					
					using (LogGroup logGroup2 = AppLogger.StartGroup("The specified property is not an entities reference. Retrieving the corresponding entities property now.", NLog.LogLevel.Debug))
					{
						PropertyInfo entitiesProperty = GetEntitiesProperty(property);
						
						if (entitiesProperty == null)
							AppLogger.Debug("The entities property is null.");
						else
							AppLogger.Debug("Entities property type: " + entitiesProperty.GetType().ToString());
						
						name = GetDataStoreNameForReference(entity, entitiesProperty);
					}
					
					return name;
				}
				else
				{
					AppLogger.Debug("The property is an entities reference.");
					
					if (value == null || (value is Array && ((Array)value).Length == 0))
					{
						AppLogger.Debug("The property value is an array. Using reference type to determine data store name.");
						
						return GetDataStoreName(referenceType);
					}
					else
					{
						AppLogger.Debug("The value is not null.");
						AppLogger.Debug("The value is not a 0 length array");
						
						Type type = value.GetType();
						if (type.IsSubclassOf(typeof(Array)))
						{
							AppLogger.Debug("The value type is an array.");
							return GetDataStoreName(type.GetElementType(), true);
						}
						else
						{
							AppLogger.Debug("The value type is not an array.");
							return GetDataStoreName(value.GetType(), true);
						}
					}
				}
			}
		}*/
		
		
		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="type">The type of entity to get the data store name for.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return GetDataStoreName(type, true);
		}
		

		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="entity">The entity to get the data store name for.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			return GetDataStoreName(entity, true);
		}
		
		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="type">The type of entity to get the data store name for.</param>
		/// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(Type type, bool throwErrorIfNotFound)
		{
			string dataStoreName = String.Empty;
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the name of the data store.", NLog.LogLevel.Debug))
			// {
			if (type == null)
				throw new ArgumentNullException("type");

			if (Config.Mappings == null)
				throw new InvalidOperationException("No mappings have been initialized.");


			Type actualType = EntitiesUtilities.GetType(type.Name);


			if (actualType == null)
				actualType = type;

			// AppLogger.Debug("Actual type: " + actualType.ToString());

			MappingItem item = Config.Mappings.GetItem(actualType, false);
			if (item == null)
			{
				throw new InvalidOperationException("No mappings found for the type " + actualType.ToString() + ".");
			}
			//else
			//AppLogger.Debug("Item found: " + item.TypeName);

			if (!item.Settings.ContainsKey("DataStoreName"))
				throw new InvalidOperationException("No data store name has been declared in the mappings for the '" + actualType.ToString() + "' type.");
			//else
			//AppLogger.Debug("Data store: " + item.Settings["DataStoreName"]);

			dataStoreName = (string)item.Settings["DataStoreName"];

			//}
			return dataStoreName;
		}

		/// <summary>
		/// Gets the name of the data store that the provided entity is stored in.
		/// </summary>
		/// <param name="entity">The entity to get the data store name for.</param>
		/// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
		/// <returns>The data store that the provided entity is stored in.</returns>
		static public string GetDataStoreName(IEntity entity, bool throwErrorIfNotFound)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			/*
            object[] attributes = (object[])type.GetCustomAttributes(true);
		if (attributes != null)
		{
	            foreach (object attribute in attributes)
	            {
	                if (attribute is DataStoreAttribute)
	                    return ((DataStoreAttribute)attribute).DataStoreName;
	            }
	            if (throwErrorIfNotFound)
	            {
	                throw new Exception("No data store name was found for the entity '" + type.ToString() + "'");
	            }
		}
            return String.Empty;*/
			string dataStoreName = String.Empty;
			Type type = entity.GetType();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving data store name for entity of type '" + type.ToString() + "'.", NLog.LogLevel.Debug))
			{
				if (EntitiesUtilities.IsReference(entity.GetType()))
				{
					EntityIDReference reference = (EntityIDReference)entity;
					
					dataStoreName = GetDataStoreName(new string[] {reference.Type1Name, reference.Type2Name});//GetType(names[0]), GetType(names[1]));//dataStoreNames[0] + "-" + dataStoreNames[1];
				}
				else
				{
					dataStoreName = GetDataStoreName(entity.GetType());
				}
			}
			
			return dataStoreName;
			
		}
		
		static public string GetDataStoreName(params string[] typeNames)
		{
			string returnName;
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the data store name for provided types.", NLog.LogLevel.Debug))
			//{
			if (typeNames == null)
				throw new ArgumentNullException("typeNames");
			
			if (typeNames.Length != 2)
				throw new ArgumentException("Incorrect number of type names provided. Expected 2 but was " + typeNames.Length + ".");
			else
			{
				if (typeNames[0].Trim().Length == 0)
					throw new ArgumentException("The type name at position 0 is empty.");
				if (typeNames[1].Trim().Length == 0)
					throw new ArgumentException("The type name at position 1 is empty.");
				
				//AppLogger.Debug("Type name 0: " + typeNames[0]);
				//AppLogger.Debug("Type name 1: " + typeNames[1]);
			}
			
			Array.Sort(typeNames);
			
			//AppLogger.Debug("Sorted names.");
			
			//AppLogger.Debug("Type name 0: " + typeNames[0]);
			//AppLogger.Debug("Type name 1: " + typeNames[1]);
			
			Type type0 = EntitiesUtilities.GetType(typeNames[0]);
			Type type1 = EntitiesUtilities.GetType(typeNames[1]);
			
			//AppLogger.Debug("Type 0: " + type0.ToString());
			//AppLogger.Debug("Type 1: " + type1.ToString());
			
			string[] dataStoreNames = new String[]{
				GetDataStoreName(type0),
				GetDataStoreName(type1)
			};
			
			returnName = dataStoreNames[0] + "-" + dataStoreNames[1];
			
			//AppLogger.Debug("Data store name: " + returnName);
			//}
			
			return returnName;
		}
		
		static public Type GetEntityType(IEntity entity, PropertyInfo property)
		{
			Type type = null;

			using (LogGroup group = AppLogger.StartGroup("Retrieving the type of entity being referenced by the provided property.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Property name: " + property.Name);
				AppLogger.Debug("Property type: " + property.PropertyType.Name);
				
				type = EntitiesUtilities.GetReferenceType(entity.GetType(), property);
				
				/*if (typeof(IEntity).IsAssignableFrom(property.PropertyType))
				{
					AppLogger.Debug("typeof(IEntity).IsAssignableFrom(property.PropertyType)");
					type = property.PropertyType;
				}
				else
				{
					AppLogger.Debug("!typeof(IEntity).IsAssignableFrom(property.PropertyType)");
					
					EntityReference reference = EntitiesUtilities.GetReferen
					
					//type = property.PropertyType.GetGenericArguments()[0];
				}*/
				
				/*if (property.PropertyType.FullName == typeof(EntityIDReference).FullName
				    || property.PropertyType.FullName == typeof(EntityReference).FullName)
				{
					EntityIDReference reference = (EntityIDReference)property.GetValue(entity, null);
					
					AppLogger.Debug("Reference - Source entity ID: " + reference.EntityIDs[0]);
					AppLogger.Debug("Reference - Reference entity ID: " + reference.EntityIDs[1]);
					AppLogger.Debug("Reference - Source entity type: " + reference.TypeNames[0]);
					AppLogger.Debug("Reference - Reference entity type: " + reference.TypeNames[1]);
					
					if (reference.TypeNames == null || reference.TypeNames.Length < 2)
						throw new InvalidOperationException("The reference doesn't have the correct number of type names specified.");
					
					if (reference != null)
						type = GetType(reference.TypeNames[1]);
				}
				else
				{
					EntityIDReferenceCollection collection = (EntityIDReferenceCollection)property.GetValue(entity, null);
					//if (collection.Count == 0)
					//	throw new InvalidOperationException("No references have been added.");
					
					//EntityIDReference reference = collection[0];
					
	//				AppLogger.Debug("Reference - Source entity ID: " + reference.EntityIDs[0]);
	//				AppLogger.Debug("Reference - Reference entity ID: " + reference.EntityIDs[1]);
	//				AppLogger.Debug("Reference - Source entity type: " + reference.TypeNames[0]);
	//				AppLogger.Debug("Reference - Reference entity type: " + reference.TypeNames[1]);
					
					//if (reference.TypeNames == null || reference.TypeNames.Length < 2)
					//	throw new InvalidOperationException("The reference doesn't have the correct number of type names specified.");
					
					//if (reference != null)
						type = GetType(collection.ReferenceTypeName);//reference.TypeNames[1]);
				}
				 */
				/*if (property == null)
					throw new ArgumentNullException("property");

				if (entity == null)
					throw new ArgumentNullException("entity");
				
				

				//BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);

				AppLogger.Debug("Property name: " + property.Name);

				type = property.PropertyType;

				AppLogger.Debug("Property type: " + property.PropertyType.ToString());

				if (type.IsSubclassOf(typeof(Array)))
				{
					AppLogger.Debug("type.IsSubclassOf(typeof(Array))");
					type = type.GetElementType();
				}
				else if (type.IsSubclassOf(typeof(IEntity)))
				{
					AppLogger.Debug("type.IsSubclassOf(typeof(IEntity))");
					type = property.PropertyType;
				}*/

			}

			if (type == null)
				AppLogger.Debug("return type == null");
			else
				AppLogger.Debug("return type == " + type.ToString());
			
			return type;
		}
		
		/// <summary>
		/// Checks whether the specified counter is within the specified page.
		/// </summary>
		/// <param name="i">The counter of the items being paged.</param>
		/// <param name="pageIndex">The index of the page to retrieve.</param>
		/// <param name="pageSize">The number of items on each page.</param>
		/// <returns>A flag indicating whether the specified counter is within the specified page.</returns>
		static public bool IsInPage(int i, int pageIndex, int pageSize)
		{
			bool match = false;
			using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the specified position is within the specified page.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Position (i): " + i.ToString());
				AppLogger.Debug("Page index: " + pageIndex);
				AppLogger.Debug("Page size: " + pageSize);
				
				int first = (pageIndex * pageSize);
				int last = ((pageIndex * pageSize) + pageSize) -1; // -1 to make it the last of the page, instead of first item on next page
				
				AppLogger.Debug("First position: " + first.ToString());
				AppLogger.Debug("Last position: " + last.ToString());
				
				match = i >= first
					&& i <= last;
				
				AppLogger.Debug("Match: " + match.ToString());
			}
			return match;
		}
		
		
		static public IEntity[] GetReferencedEntities(EntityReferenceCollection references, IEntity entity)
		{
			Collection<IEntity> collection = new Collection<IEntity>();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Getting the referenced entities from the provided entity.", NLog.LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				foreach (EntityReference reference in references)
				{
					AppLogger.Debug("Reference type #1: " + reference.Type1Name);
					AppLogger.Debug("Reference ID #1: " + reference.Entity1ID.ToString());
					AppLogger.Debug("Reference type #2: " + reference.Type2Name);
					AppLogger.Debug("Reference ID #2: " + reference.Entity2ID.ToString());
					
					DataAccess.Data.ActivateReference(reference);
					
					IEntity otherEntity = reference.GetOtherEntity(entity);
					
					if (otherEntity != null)
					{
						AppLogger.Debug("Other entity type: " + otherEntity.GetType().ToString());
						AppLogger.Debug("Other entity ID: " + otherEntity.ID.ToString());
						
						collection.Add(otherEntity);
					}
					else
					{
						AppLogger.Debug("Other entity == null");
					}
				}
			}
			
			return collection.ToArray();
		}
		
		static public void StripReferences(IEntity entity)
		{
			using (LogGroup logGroup2 = AppLogger.StartGroup("Clearing all the object references so that they don't cascade automatically.", NLog.LogLevel.Debug))
			{
				if (entity is EntityReference)
				{
					((EntityReference)entity).Deactivate();
				}
				else
				{
					//  Clear all the references from the entity once they're ready to be saved separately
					foreach (PropertyInfo property in entity.GetType().GetProperties())
					{
						AppLogger.Debug("Property name: " + property.Name);
						AppLogger.Debug("Property type: " + property.PropertyType.ToString());
						
						// If the property is a reference
						// OR the actual provided entity is a reference AND the property holds an IEntity instance
						if (EntitiesUtilities.IsReference(entity.GetType(), property.Name, property.PropertyType))
						{
							AppLogger.Debug("Cleared property. (Set to null)");
							Reflector.SetPropertyValue(entity, property.Name, null);
						}
					}
				}
			}
		}
		
	}
}
