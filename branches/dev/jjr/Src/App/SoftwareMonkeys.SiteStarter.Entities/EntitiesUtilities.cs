
using System;
using SoftwareMonkeys.SiteStarter.Configuration;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Description of EntitiesUtilities.
	/// </summary>
	public class EntitiesUtilities
	{
		static public void RegisterEntityType(Type type)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Registering entity type: "+ type.ToString(), NLog.LogLevel.Debug))
			{
				// Get the RegisterType method from the IEntity based class
				MethodInfo method = type.GetMethod("RegisterType");
				
				// Invoke the RegisterType method
				if (method != null)
					method.Invoke(null, null);
				
				// Save the mappings config
				string path = Config.Application.PhysicalPath.TrimEnd('\\') + @"\App_Data\";
				ConfigFactory<MappingConfig>.SaveConfig(path, (MappingConfig)Config.Mappings);
				
				//AddMappings(type, dataStoreName);
			}
		}
		
		static public void DeregisterEntityType(Type type)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Deregistering entity type: "+ type.ToString(), NLog.LogLevel.Debug))
			{
				
				// Get the RegisterType method from the IEntity based class
				MethodInfo method = type.GetMethod("DeregisterType");
				
				// Invoke the RegisterType method
				if (method != null)
					method.Invoke(null, null);
				
				// Save the mappings config
				string path = Config.Application.PhysicalPath.TrimEnd('\\') + @"\App_Data\";
				ConfigFactory<MappingConfig>.SaveConfig(path, (MappingConfig)Config.Mappings);
				
				//RemoveMappings(type);
			}
		}
		
		/*static public void AddMappings(Type type, string dataStoreName)
		{
			
			using (LogGroup logGroup = AppLogger.StartGroup("Adding mappings.", NLog.LogLevel.Debug))
			{
				AddDataStoreMapping(type, dataStoreName);
				
				string path = Config.Application.PhysicalPath.TrimEnd('\\') + @"\App_Data\";
				
				ConfigFactory<MappingConfig>.SaveConfig(path, (MappingConfig)Config.Mappings);
			}
		}
		
		static public void AddDataStoreMapping(Type type, string dataStoreName)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Adding data store mapping information.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Type: " + type.ToString());
				AppLogger.Debug("Data store name: " + dataStoreName);
				
				IMappingConfig config = Config.Mappings;
				if (config == null)
					throw new InvalidOperationException("The mappings have not been initialized.");
				
				MappingItem item = new MappingItem(type.Name);
				item.Settings.Add("DataStoreName", dataStoreName);
				
				config.AddItem(item);
			}
		}*/
		
		static public void RemoveMappings(Type type)
		{
			throw new NotImplementedException();
			/*IMappingConfig config = Config.Mappings;
			if (config == null)
				config = new MappingConfig();
			
			config.RemoveItem(config.I*/
		}
		
		static public bool MatchAlias(string typeName, string aliasTypeName)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Matching the type '" + typeName + "' with alias type '" + aliasTypeName + "'.", NLog.LogLevel.Debug))
			{
				IMappingConfig config = Config.Mappings;
				
				if (config != null)
				{
					IMappingItem item = config.GetItem<IMappingItem>(typeName, true);
					
					if (item != null)
					{
						AppLogger.Debug("Mapping item found");
						//  If the settings for the type define an alias
						if (item.Settings.ContainsKey("Alias"))
						{
							AppLogger.Debug("Alias setting found in mappings: " + item.Settings["Alias"]);
							// If the alias specified in settings matches the specified alias
							if (item.Settings["Alias"].Equals(aliasTypeName))
							{
								AppLogger.Debug("Alias matches.");
								return true;
							}
							else
								AppLogger.Debug("The alias specified in mappings '" + item.Settings["Alias"] + "' doesn't match.");
						}
						else
							AppLogger.Debug("No alias setting found for the type name.");
					}
					else
						AppLogger.Debug("No item found for the type name.");
				}
				else
					AppLogger.Debug("The mappings haven't been initialized.");
				
				// No match, return false
				return false;
			}
		}
		
		
		
		static public EntityReferenceCollection GetReferences(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			// Loop through all the properties on the entity class
			foreach (PropertyInfo property in entity.GetType().GetProperties())
			{
				AppLogger.Debug("Checking property: " + property.Name);
				
				if (IsReference(entity.GetType(), property.Name, property.PropertyType))
				{
					AppLogger.Debug("Property is a reference.");
					
					foreach (EntityIDReference reference in GetReferences(entity, property.Name, property.PropertyType))
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
		
		
		static public EntityIDReferenceCollection GetRemovedReferenceIDs(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			EntityIDReferenceCollection references = new EntityIDReferenceCollection();
			
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
		
		static public EntityReferenceCollection GetReferences(IEntity entity, string propertyName, Type returnType)
		{
			EntityReferenceCollection collection = new EntityReferenceCollection();
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the reference entities from the specified property on the provided entity.", NLog.LogLevel.Debug))
			{
				Type entityType = entity.GetType();
				
				PropertyInfo property = GetProperty(entityType, propertyName, returnType);
				
				if (property == null)
					AppLogger.Debug("Property: [null]");
				else
					AppLogger.Debug("Property name: " + property.Name);
				
				if (IsReference(entityType, propertyName, returnType))
				{
					EntityReferenceCollection references = null;
					
					Type referencedEntityType = GetReferenceType(entityType, propertyName, returnType);
					
					string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity.GetType(), property);
					
					if (IsMultipleReference(entity.GetType(), property))
					{
						AppLogger.Debug("Multiple reference property.");
						
						object propertyValue = property.GetValue(entity, null);
						
						AppLogger.Debug("Property value: " + (propertyValue == null ? "[null]" : propertyValue.ToString()));
						
						Collection<IEntity> referencedEntities = new Collection<IEntity>();
						
						//if (propertyValue is Array)
						//{
						referencedEntities.AddRange(GetReferencedEntities(entity, property));
						//}
						//else
						//{
						//	foreach (IEntity entity in (Collection<IEntity>)propertyValue))
						//	{
						//		referencedEntities.Add(entity);
						///	}
						//}
						
						AppLogger.Debug("# of referenced entities found: " + referencedEntities.Count);
						
						references = new EntityReferenceCollection(entity, propertyName, referencedEntities.ToArray(), mirrorPropertyName);
						
						AppLogger.Debug("Reference objects created.");
						
						foreach (EntityIDReference reference in references)
						{
							AppLogger.Debug("Adding reference with ID: " + reference.ID.ToString());

							AppLogger.Debug("Source entity ID: " + reference.Entity1ID.ToString());
							AppLogger.Debug("Referenced entity ID: " + reference.Entity2ID.ToString());
							
							AppLogger.Debug("Source entity name: " + reference.TypeName1);
							AppLogger.Debug("Referenced entity name: " + reference.TypeName2);
							
							AppLogger.Debug("Source property name: " + reference.Property1Name);
							AppLogger.Debug("Mirror property name: " + reference.Property2Name);
							
							
							collection.Add(reference.ToData());
						}
					}
					else if (IsSingleReference(entityType, property))
					{
						AppLogger.Debug("Single reference property.");
						
						IEntity[] referencedEntities = GetReferencedEntities(entity, property);
						
						if (referencedEntities != null && referencedEntities.Length > 0)
						{
							IEntity referencedEntity = referencedEntities[0];
							
							references = new EntityReferenceCollection(entity, propertyName, new IEntity[] {referencedEntity}, mirrorPropertyName);
							
							foreach (EntityIDReference reference in references)
							{
								AppLogger.Debug("Adding reference with ID: " + reference.ID.ToString());
								
								AppLogger.Debug("Source entity ID: " + reference.Entity1ID.ToString());
								AppLogger.Debug("Referenced entity ID: " + reference.Entity2ID.ToString());
								
								AppLogger.Debug("Source entity name: " + reference.TypeName1);
								AppLogger.Debug("Referenced entity name: " + reference.TypeName2);
								
								collection.Add(reference.ToData());
							}
						}
						else
							AppLogger.Debug("referencedEntities == null || referencedEntities.Length = 0");
					}
					else
						throw new NotSupportedException("The property type '" + property.PropertyType.ToString() + "' is not supported.");
				}
				
				AppLogger.Debug("References found: " + collection.Count.ToString());
			}
			
			return collection;
		}
		
		
		static public bool IsReference(Type entityType, string propertyName, Type returnType)
		{
			bool isReference = false;
			
			PropertyInfo property = GetProperty(entityType, propertyName, returnType);
			
			isReference = IsReference(entityType, property);
			
			return isReference;
		}
		
		static public bool IsReference(Type sourceType, PropertyInfo property)
		{
			bool isReference = false;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking if the specified property is an entity reference.", NLog.LogLevel.Debug))
			//{
			//	AppLogger.Debug("Entity: " + sourceType.ToString());
			//	AppLogger.Debug("Property name: " + property.Name);
			
			ReferenceAttribute reference = GetReferenceAttribute(sourceType, property);
			
			isReference = reference != null;
			
			/*foreach (Attribute attribute in property.GetCustomAttributes(true))
			{
				if (attribute is ReferenceAttribute)
				{
					isReference = true;
				}
			}*/
			
			//	AppLogger.Debug("Is reference? " + isReference.ToString());
			//}
			
			return isReference;
		}
		
		/*static public bool IsReference(Type type)
		{
			bool isReference = false;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking if the specified property is an entity reference.", NLog.LogLevel.Debug))
			//{
			
			isReference = type.Name.IndexOf("EntityReferenceCollection") > -1
				|| type.Name.IndexOf("EntityIDReferenceCollection") > -1
				|| type.Name.IndexOf("EntityReference") > -1
				|| type.Name.IndexOf("EntityIDReference") > -1;
			
			//	AppLogger.Debug("Is reference? " + isReference.ToString());
			//}
			
			return isReference;
		}*/
		
		static public bool IsReference(Type type)
		{
			bool isReference = false;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking if the specified property is an entity reference.", NLog.LogLevel.Debug))
			//{
			
			isReference = type.FullName.IndexOf("EntityReference") > -1
				|| type.FullName.IndexOf("EntityIDReference") > -1;
			//	AppLogger.Debug("Is reference? " + isReference.ToString());
			//}
			
			return isReference;
		}
		
		static public ReferenceAttribute GetReferenceAttribute(IEntity entity, string propertyName, Type returnType)
		{
			ReferenceAttribute attribute = null;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = AppLogger.StartGroup("...", NLog.LogLevel.Debug))
			//{
			PropertyInfo property = entity.GetType().GetProperty(propertyName, returnType);
			
			//	AppLogger.Debug("Entity: " + entity.GetType().ToString());
			//	AppLogger.Debug("Property name: " + propertyName);
			
			attribute = GetReferenceAttribute(entity.GetType(), property);
			
			//	AppLogger.Debug("Is reference? " + isReference.ToString());
			//}
			
			return attribute;
		}
		
		static public ReferenceAttribute GetReferenceAttribute(Type sourceType, PropertyInfo property)
		{
			ReferenceAttribute attribute = null;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the reference attribute for the specified property.", NLog.LogLevel.Debug))
			//{
			
			//	AppLogger.Debug("Entity: " + sourceType.ToString());
			//	AppLogger.Debug("Property name: " + property.Name);
			//	AppLogger.Debug("Property type: " + property.PropertyType.ToString());
			
			foreach (Attribute a in property.GetCustomAttributes(true))
			{
				if (a is ReferenceAttribute)
				{
					attribute = (ReferenceAttribute)a;
				}
			}
			
			//	AppLogger.Debug("Attribute found: " + (attribute != null).ToString());
			//}
			
			return attribute;
		}
		
		static public Type GetReferenceType(Type entityType, string propertyName, Type returnType)
		{
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (propertyName == null || propertyName == String.Empty)
				throw new ArgumentException("propertyName", "Property name cannot be null or String.Empty.");
			
			
			PropertyInfo property = GetProperty(entityType, propertyName, returnType);
			
			return GetReferenceType(entityType, property);
		}
		
		static public Type GetReferenceType(Type sourceType, PropertyInfo property)
		{
			Type type = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the referenced entity type.", NLog.LogLevel.Debug))
			{
				if (sourceType == null)
					throw new ArgumentNullException("sourceType");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				AppLogger.Debug("Entity type: " + sourceType.ToString());
				AppLogger.Debug("Property name: " + property.Name);
				AppLogger.Debug("Property type: " + property.PropertyType.ToString());
				
				ReferenceAttribute attribute = GetReferenceAttribute(sourceType, property);
				
				if (attribute == null)
					throw new Exception("The reference attribute was not found on the '" + property.Name + "' property of the type '" + sourceType.ToString() + "'.");
				
				if (attribute.TypeName != String.Empty)
				{
					AppLogger.Debug("attribute.TypeName != String.Empty");
					AppLogger.Debug("attribute.TypeName == " + attribute.TypeName);
					
					type = EntitiesUtilities.GetType(attribute.TypeName);
				}
				else
				{
					AppLogger.Debug("attribute.TypeName == String.Empty");
					
					// If the property is a single entity instance
					if (IsSingleReference(sourceType, property))
					{
						AppLogger.Debug("Is a single entity");
						
						type = property.PropertyType;
					} // Otherwise the property is a collection/array
					else
					{
						AppLogger.Debug("Is a collection/array");
						
						// If it's an array this should work
						type = property.PropertyType.GetElementType();
						
						// If it's not an array (ie. it's a collection) then this should work
						if (type == null)
							type = property.PropertyType.GetGenericArguments()[0];
					}
				}
				
				if (type != null)
					AppLogger.Debug("Type: " + type.ToString());
			}
			
			return type;
		}
		
		static public Type GetType(string typeName)
		{
			Type returnType = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the actual type based on the specified type/alias.", NLog.LogLevel.Debug))
			{
				if (typeName == null || typeName == String.Empty)
					throw new ArgumentNullException("typeName");
				
				AppLogger.Debug("Type name: " + typeName);
				
				// If a short type name was provided (eg. User)
				if (typeName.IndexOf('.') == -1
				    && typeName.IndexOf(',') == -1)
				{
					
					IMappingItem mappingItem = Config.Mappings.GetItem<IMappingItem>(typeName, true);
					if (mappingItem == null)
						throw new InvalidOperationException("No mapping item found for type " + typeName);
					
					string alias = String.Empty;
					if (mappingItem.Settings.ContainsKey("Alias"))
						alias = (string)mappingItem.Settings["Alias"];
					
					if (mappingItem.Settings == null || mappingItem.Settings.Count == 0)
						AppLogger.Debug("No settings defined for this mapping item.");
					else
						AppLogger.Debug(mappingItem.Settings.Count.ToString() + " settings found for this mapping item.");
					
					// If no alias is specified then use the FullName specified
					if (alias == null || alias == String.Empty)
					{
						AppLogger.Debug("No alias. This is the actual type.");
						
						string fullTypeName = String.Empty;
						if (mappingItem.Settings.ContainsKey("FullName"))
							fullTypeName = (string)mappingItem.Settings["FullName"];
						else
							throw new InvalidOperationException("No 'FullName' specified in mappings for type " + typeName);
						
						
						string assemblyName = String.Empty;
						if (mappingItem.Settings.ContainsKey("AssemblyName"))
							assemblyName = (string)mappingItem.Settings["AssemblyName"];
						else
							throw new InvalidOperationException("No 'AssemblyName' specified in mappings for type " + typeName);
						
						returnType = Type.GetType(fullTypeName + ", " + assemblyName);
						
						AppLogger.Debug("Returning type: " + returnType.ToString());
						
					}
					else
					{
						AppLogger.Debug("Alias: " + alias);
						// Use the alias if specified
						returnType = GetType(alias);
					}
				} // Otherwise a long type name was provided
				else
				{
					returnType = Type.GetType(typeName);
				}
			}
			
			return returnType;
			
		}
		
		static public PropertyInfo GetProperty(Type entityType, string propertyName, Type returnType)
		{
			PropertyInfo property = entityType.GetProperty(propertyName, returnType);
			
			//if (property == null)
			//	property = entityType.GetProperty(propertyName);
			
			return property;
		}
		
		static public bool IsMultipleReference(Type entityType, PropertyInfo property)
		{
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (property == null)
				throw new ArgumentNullException("property");
			
			Type propertyType = property.PropertyType;
			
			return IsReference(entityType, property) &&
				(propertyType.GetInterface("IEnumerable") != null
				 || typeof(Array).IsAssignableFrom(propertyType));
		}
		
		static public bool IsSingleReference(Type entityType, PropertyInfo property)
		{
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (property == null)
				throw new ArgumentNullException("property");
			
			return  IsReference(entityType, property) && !IsMultipleReference(entityType, property);
			//return property.PropertyType.GetInterface("IEntity") != null
			//	|| typeof(IEntity).IsAssignableFrom(property.PropertyType);
		}
		
		/// <summary>
		/// Determines whether the specified reference is an array (rather than a collection).
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		static public bool IsArrayReference(Type entityType, PropertyInfo property)
		{
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (property == null)
				throw new ArgumentNullException("property");
			
			return IsMultipleReference(entityType, property) && typeof(Array).IsAssignableFrom(property.PropertyType);
		}
		
		/// <summary>
		/// Determines whether the specified reference is a collection (rather than an array).
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		static public bool IsCollectionReference(Type entityType, PropertyInfo property)
		{
			if (entityType == null)
				throw new ArgumentNullException("entityType");
			
			if (property == null)
				throw new ArgumentNullException("property");
			
			return IsMultipleReference(entityType, property) && !IsArrayReference(entityType, property);
		}
		
		
		static public IEntity[] GetReferencedEntities(IEntity entity, PropertyInfo property)
		{
			List<IEntity> list = new List<IEntity>();
			
			object propertyValue = property.GetValue(entity, null);
			
			if (IsMultipleReference(entity.GetType(), property))
			{
				foreach (IEntity o in Collection<IEntity>.ConvertAll(propertyValue))
				{
					list.Add((IEntity)o);
				}
			}
			else
			{
				if (propertyValue != null)
					list.Add((IEntity)propertyValue);
			}
			
			return list.ToArray();
		}
		
		static public string GetMirrorPropertyName(Type sourceType, PropertyInfo property)
		{
			string mirrorPropertyName = String.Empty;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the mirror property name.", NLog.LogLevel.Debug))
			{
				if (sourceType == null)
					throw new ArgumentNullException("sourceType");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				ReferenceAttribute attribute = GetReferenceAttribute(sourceType, property);
				
				// Is the mirror property name specified on the attribute?
				if (attribute.MirrorPropertyName != String.Empty)
					mirrorPropertyName = attribute.MirrorPropertyName;
				else
				{
					// Should the mirror property name be automatically discovered based on the property type matching the source entity?
					if (attribute.AutoDiscoverMirror)
					{
						Type referencedEntityType = GetReferenceType(sourceType, property);
						
						// Loop through each property and check the types
						foreach (PropertyInfo p in referencedEntityType.GetProperties())
						{
							// Is the property a reference?
							if (IsReference(referencedEntityType, p))
							{
								// Retrieve the entity type being stored on the property
								Type reciprocalType = GetReferenceType(referencedEntityType, p);
								
								// If the entity types match then return the property name
								if (reciprocalType.FullName == sourceType.FullName)
									mirrorPropertyName = p.Name;
							}
						}
					}
				}
				
				AppLogger.Debug("Mirror property name: " + mirrorPropertyName);
			}
			
			return mirrorPropertyName;
		}
	}
}
