using System;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;
using System.IO;

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
			using (LogGroup logGroup = LogGroup.Start("Retrieving the data store name for a particulary entity property reference.", LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				LogWriter.Debug("Entity type: " + entity.GetType());
				LogWriter.Debug("Entity ID: " + entity.ID.ToString());
				LogWriter.Debug("Property name: " + property.Name);
				LogWriter.Debug("Property type: " + property.GetType().ToString());
				
				object value = property.GetValue(entity, null);
				
				
				Type referenceType = DataUtilities.GetReferenceType(entity, property);
				BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);
				
				
				LogWriter.Debug("Property value: " + (value == null ? "[null]" :value.ToString()));
				LogWriter.Debug("Reference type: " + referenceType != null ? referenceType.ToString() : "[null]");
				
				if (attribute == null)
					throw new InvalidOperationException("No reference attribute found for this property on this entity.");
				
				if (attribute.EntitiesPropertyName == String.Empty || attribute.EntitiesPropertyName.Length == 0)
					throw new InvalidOperationException("The specified property '" + property.Name + "' doesn't have an entities property specified in the reference attribute. Cannot retrieve the type.");
				
				if (property.Name != attribute.EntitiesPropertyName)
				{
					string name = String.Empty;
					
					using (LogGroup logGroup2 = LogGroup.Start("The specified property is not an entities reference. Retrieving the corresponding entities property now.", LogLevel.Debug))
					{
						PropertyInfo entitiesProperty = GetEntitiesProperty(property);
						
						if (entitiesProperty == null)
							LogWriter.Debug("The entities property is null.");
						else
							LogWriter.Debug("Entities property type: " + entitiesProperty.GetType().ToString());
						
						name = GetDataStoreNameForReference(entity, entitiesProperty);
					}
					
					return name;
				}
				else
				{
					LogWriter.Debug("The property is an entities reference.");
					
					if (value == null || (value is Array && ((Array)value).Length == 0))
					{
						LogWriter.Debug("The property value is an array. Using reference type to determine data store name.");
						
						return GetDataStoreName(referenceType);
					}
					else
					{
						LogWriter.Debug("The value is not null.");
						LogWriter.Debug("The value is not a 0 length array");
						
						Type type = value.GetType();
						if (type.IsSubclassOf(typeof(Array)))
						{
							LogWriter.Debug("The value type is an array.");
							return GetDataStoreName(type.GetElementType(), true);
						}
						else
						{
							LogWriter.Debug("The value type is not an array.");
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
			return type.Name;
			/*string dataStoreName = String.Empty;
			//using (LogGroup logGroup = LogGroup.Start("Retrieving the name of the data store.", LogLevel.Debug))
			// {
			if (type == null)
				throw new ArgumentNullException("type");

			if (Config.Mappings == null)
				throw new InvalidOperationException("No mappings have been initialized.");

			Type actualType = EntitiesUtilities.GetType(type.Name);


			if (actualType == null)
				actualType = type;

			// LogWriter.Debug("Actual type: " + actualType.ToString());

			MappingItem item = Config.Mappings.GetItem(actualType, false);
			if (item == null)
			{
				throw new InvalidOperationException("No mappings found for the type " + actualType.ToString() + ".");
			}
			//else
			//LogWriter.Debug("Item found: " + item.TypeName);

			if (!item.Settings.ContainsKey("DataStoreName"))
				throw new InvalidOperationException("No data store name has been declared in the mappings for the '" + actualType.ToString() + "' type.");
			//else
			//LogWriter.Debug("Data store: " + item.Settings["DataStoreName"]);

			dataStoreName = (string)item.Settings["DataStoreName"];

			//}
			return dataStoreName;*/
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
			
			string dataStoreName = String.Empty;
			Type type = entity.GetType();
			
			using (LogGroup logGroup = LogGroup.Start("Retrieving data store name for entity of type '" + type.ToString() + "'.", LogLevel.Debug))
			{
				if (EntitiesUtilities.IsReference(entity.GetType()))
				{
					EntityReference reference = (EntityReference)entity;
					
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
			
			//using (LogGroup logGroup = LogGroup.Start("Retrieving the data store name for provided types.", LogLevel.Debug))
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
				
				//LogWriter.Debug("Type name 0: " + typeNames[0]);
				//LogWriter.Debug("Type name 1: " + typeNames[1]);
			}
			
			Array.Sort(typeNames);
			
			//LogWriter.Debug("Sorted names.");
			
			//LogWriter.Debug("Type name 0: " + typeNames[0]);
			//LogWriter.Debug("Type name 1: " + typeNames[1]);
			
			Type type0 = EntityState.GetType(typeNames[0]);
			Type type1 = EntityState.GetType(typeNames[1]);
			
			//LogWriter.Debug("Type 0: " + type0.ToString());
			//LogWriter.Debug("Type 1: " + type1.ToString());
			
			string[] dataStoreNames = new String[]{
				GetDataStoreName(type0),
				GetDataStoreName(type1)
			};
			
			returnName = dataStoreNames[0] + "-" + dataStoreNames[1];
			
			//LogWriter.Debug("Data store name: " + returnName);
			//}
			
			return returnName;
		}
		
		static public Type GetEntityType(IEntity entity, PropertyInfo property)
		{
			Type type = null;

			using (LogGroup group = LogGroup.Start("Retrieving the type of entity being referenced by the provided property.", LogLevel.Debug))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				LogWriter.Debug("Entity type: " + entity.GetType().ToString());
				LogWriter.Debug("Property name: " + property.Name);
				LogWriter.Debug("Property type: " + property.PropertyType.Name);
				
				type = EntitiesUtilities.GetReferenceType(entity.GetType(), property);
				
				/*if (typeof(IEntity).IsAssignableFrom(property.PropertyType))
				{
					LogWriter.Debug("typeof(IEntity).IsAssignableFrom(property.PropertyType)");
					type = property.PropertyType;
				}
				else
				{
					LogWriter.Debug("!typeof(IEntity).IsAssignableFrom(property.PropertyType)");
					
					EntityReference reference = EntitiesUtilities.GetReferen
					
					//type = property.PropertyType.GetGenericArguments()[0];
				}*/
				
				/*if (property.PropertyType.FullName == typeof(EntityReference).FullName
				    || property.PropertyType.FullName == typeof(EntityReference).FullName)
				{
					EntityReference reference = (EntityReference)property.GetValue(entity, null);
					
					LogWriter.Debug("Reference - Source entity ID: " + reference.EntityIDs[0]);
					LogWriter.Debug("Reference - Reference entity ID: " + reference.EntityIDs[1]);
					LogWriter.Debug("Reference - Source entity type: " + reference.TypeNames[0]);
					LogWriter.Debug("Reference - Reference entity type: " + reference.TypeNames[1]);
					
					if (reference.TypeNames == null || reference.TypeNames.Length < 2)
						throw new InvalidOperationException("The reference doesn't have the correct number of type names specified.");
					
					if (reference != null)
						type = GetType(reference.TypeNames[1]);
				}
				else
				{
					EntityReferenceCollection collection = (EntityReferenceCollection)property.GetValue(entity, null);
					//if (collection.Count == 0)
					//	throw new InvalidOperationException("No references have been added.");
					
					//EntityReference reference = collection[0];
					
	//				LogWriter.Debug("Reference - Source entity ID: " + reference.EntityIDs[0]);
	//				LogWriter.Debug("Reference - Reference entity ID: " + reference.EntityIDs[1]);
	//				LogWriter.Debug("Reference - Source entity type: " + reference.TypeNames[0]);
	//				LogWriter.Debug("Reference - Reference entity type: " + reference.TypeNames[1]);
					
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

				LogWriter.Debug("Property name: " + property.Name);

				type = property.PropertyType;

				LogWriter.Debug("Property type: " + property.PropertyType.ToString());

				if (type.IsSubclassOf(typeof(Array)))
				{
					LogWriter.Debug("type.IsSubclassOf(typeof(Array))");
					type = type.GetElementType();
				}
				else if (type.IsSubclassOf(typeof(IEntity)))
				{
					LogWriter.Debug("type.IsSubclassOf(typeof(IEntity))");
					type = property.PropertyType;
				}*/

			}

			if (type == null)
				LogWriter.Debug("return type == null");
			else
				LogWriter.Debug("return type == " + type.ToString());
			
			return type;
		}
		
		static public void StripReferences(IEntity entity)
		{
			using (LogGroup logGroup2 = LogGroup.Start("Clearing all the object references so that they don't cascade automatically.", LogLevel.Debug))
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
						LogWriter.Debug("Property name: " + property.Name);
						LogWriter.Debug("Property type: " + property.PropertyType.ToString());
						
						// If the property is a reference
						// OR the actual provided entity is a reference AND the property holds an IEntity instance
						if (EntitiesUtilities.IsReference(entity.GetType(), property.Name, property.PropertyType))
						{
							LogWriter.Debug("Cleared property. (Set to null)");
							Reflector.SetPropertyValue(entity, property.Name, null);
						}
					}
				}
			}
		}
	}
}
