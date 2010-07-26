
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
		static public bool IsEntity(Type type)
		{
			return typeof(IEntity).IsAssignableFrom(type);
		}

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
				ConfigFactory<MappingConfig>.SaveConfig(path, (MappingConfig)Config.Mappings, Config.Mappings.PathVariation);
				
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
				ConfigFactory<MappingConfig>.SaveConfig(path, (MappingConfig)Config.Mappings, Config.Mappings.PathVariation);
				
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
				MappingConfig config = Config.Mappings;
				
				if (config != null)
				{
					MappingItem item = config.GetItem(typeName, true);
					
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
		
		
		/// <summary>
		/// Gets the references that have been removed from the entity.
		/// </summary>
		/// <param name="entity">The entity that references have been removed from.</param>
		/// <returns>A collection of the removed references.</returns>
		static public EntityReferenceCollection GetRemovedReferences(IEntity entity)
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
				
				if (property != null)
				{
					if (IsReference(entityType, propertyName, returnType))
					{
						Type referencedEntityType = GetReferenceType(entityType, propertyName, returnType);
						
						string mirrorPropertyName = EntitiesUtilities.GetMirrorPropertyName(entity.GetType(), property);
						
						if (IsMultipleReference(entity.GetType(), property))
						{
							foreach (EntityIDReference r in GetReferencesFromMultipleReferenceProperty(entity, property, mirrorPropertyName))
							{
								if (r != null)
									collection.Add(r);
							}
						}
						else if (IsSingleReference(entityType, property))
						{
							EntityIDReference r = GetReferenceFromSingleReferenceProperty(entity, property, mirrorPropertyName);
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
		
		static private EntityIDReference GetReferenceFromSingleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
		{
			EntityIDReference reference = null;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the reference from a single reference property.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Single reference property.");
				
				IEntity[] referencedEntities = GetReferencedEntities(entity, property);
				
				if (referencedEntities != null && referencedEntities.Length > 0)
				{
					IEntity referencedEntity = referencedEntities[0];
					
					// TODO: Check if this can be simplified by skipping the collection part.
					// It's only a single reference so the collection is unnecessary
					EntityReferenceCollection references = new EntityReferenceCollection(entity, property.Name, new IEntity[] {referencedEntity}, mirrorPropertyName);
					
					foreach (EntityIDReference r in references)
					{
						AppLogger.Debug("Adding reference with ID: " + r.ID.ToString());
						
						AppLogger.Debug("Source entity ID: " + r.Entity1ID.ToString());
						AppLogger.Debug("Referenced entity ID: " + r.Entity2ID.ToString());
						
						AppLogger.Debug("Source entity name: " + r.Type1Name);
						AppLogger.Debug("Referenced entity name: " + r.Type2Name);
						
						reference = r.ToData();
					}
				}
				else
					AppLogger.Debug("referencedEntities == null || referencedEntities.Length = 0");
				
			}
			return reference;
		}
		
		static private EntityIDReference[] GetReferencesFromMultipleReferenceProperty(IEntity entity, PropertyInfo property, string mirrorPropertyName)
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
			}
			
			return collection.ToArray();
		}
		
		static public bool IsReference(Type entityType, string propertyName, Type returnType)
		{
			
			bool isReference = false;
			
			PropertyInfo property = GetProperty(entityType, propertyName, returnType);
			
			if (property == null)
			{
				if (returnType == null)
					throw new ArgumentException("Cannot find property '" + propertyName + "' on type '" + entityType.ToString() + "'.");
				else
					throw new ArgumentException("Cannot find property '" + propertyName + "' on type '" + entityType.ToString() + "' with value type '" + returnType.ToString() + "'.");
			}
			
			isReference = IsReference(entityType, property);
			
			return isReference;
		}
		
		static public bool IsReference(Type sourceType, PropertyInfo property)
		{
			bool isReference = false;
			
			if (property == null)
				throw new ArgumentNullException("property");
			
			if (sourceType == null)
				throw new ArgumentNullException("sourceType");
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking if the specified property is an entity reference.", NLog.LogLevel.Debug))
			//{
			//	AppLogger.Debug("Entity: " + sourceType.ToString());
			//	AppLogger.Debug("Property name: " + property.Name);
			
			ReferenceAttribute reference = GetReferenceAttribute(property);
			
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
			
			attribute = GetReferenceAttribute(property);
			
			//	AppLogger.Debug("Is reference? " + isReference.ToString());
			//}
			
			return attribute;
		}
		
		static public ReferenceAttribute GetReferenceAttribute(PropertyInfo property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
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
		
		
		
		static public Type GetReferenceType(Type entityType, string propertyName)
		{
			return GetReferenceType(entityType, propertyName, null);
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
				
				ReferenceAttribute attribute = GetReferenceAttribute(property);
				
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
		
		static public string GetShortType(string longType)
		{
			string tmpType = longType;
			
			if (tmpType.IndexOf(".") > -1)
			{
				string[] typeParts = tmpType.Split('.');
				
				tmpType = typeParts[typeParts.Length-1];
			}
			
			return tmpType;
		}
		
		static public Type GetType(string typeName)
		{
			Type returnType = null;
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the actual type based on the specified type/alias.", NLog.LogLevel.Debug))
			//{
			if (typeName == null || typeName == String.Empty)
				throw new ArgumentNullException("typeName");
			
			//AppLogger.Debug("Type name: " + typeName);
			
			// If a short type name was provided (eg. User)
			if (typeName.IndexOf('.') == -1
			    && typeName.IndexOf(',') == -1)
			{
				
				MappingItem mappingItem = Config.Mappings.GetItem(typeName, true);
				if (mappingItem == null)
					throw new InvalidOperationException("No mapping item found for type " + typeName);
				
				string alias = String.Empty;
				if (mappingItem.Settings.ContainsKey("Alias"))
					alias = (string)mappingItem.Settings["Alias"];
				
				//if (mappingItem.Settings == null || mappingItem.Settings.Count == 0)
				//AppLogger.Debug("No settings defined for this mapping item.");
				//else
				//AppLogger.Debug(mappingItem.Settings.Count.ToString() + " settings found for this mapping item.");
				
				// If no alias is specified then use the FullName specified
				if (alias == null || alias == String.Empty)
				{
					//AppLogger.Debug("No alias. This is the actual type.");
					
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
					
					//AppLogger.Debug("Returning type: " + returnType.ToString());
					
				}
				else
				{
					//AppLogger.Debug("Alias: " + alias);
					// Use the alias if specified
					returnType = GetType(alias);
				}
			} // Otherwise a long type name was provided
			else
			{
				returnType = Type.GetType(typeName);
			}
			//}
			
			return returnType;
			
		}
		
		static public bool PropertyExists(IEntity entity, string propertyName)
		{
			PropertyInfo property = GetProperty(entity.GetType(), propertyName, null);
			return property != null;
		}
		
		static public object GetPropertyValue(IEntity entity, string propertyName)
		{
			PropertyInfo property = GetProperty(entity.GetType(), propertyName, null);
			
			if (property == null)
				throw new ArgumentException("Cannot find property '" + propertyName + "' on type '" + entity.GetType().ToString() + "'.");
			
			return property.GetValue(entity, null);
		}
		
		static public PropertyInfo GetProperty(Type entityType, string propertyName)
		{
			return GetProperty(entityType, propertyName, null);
		}
		
		static public PropertyInfo GetProperty(Type entityType, string propertyName, Type returnType)
		{
			PropertyInfo property = null;
			if (returnType != null)
				property = entityType.GetProperty(propertyName, returnType);
			else
				property = entityType.GetProperty(propertyName);
			
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
		
		static public string GetMirrorPropertyNameReverse(Type entityType, string propertyName, Type referencedEntityType)
		{
			string mirrorPropertyName = String.Empty;
			
			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving mirror property name using reverse method.", NLog.LogLevel.Debug))
			{
				foreach (PropertyInfo property in referencedEntityType.GetProperties())
				{
					using (LogGroup logGroup2 = AppLogger.StartGroup("Checking property: " + property.Name, NLog.LogLevel.Debug))
					{
						
						if (IsReference(entityType, property))
						{
							AppLogger.Debug("Is reference == true");
							
							//string m = GetMirrorPropertyName(referencedEntityType, property);
							
							//AppLogger.Debug("Possible mirror property name: " + m);
							
							Type reverseReferenceType = GetReferenceType(referencedEntityType, property);
							
							AppLogger.Debug("reverseReferenceType: " + reverseReferenceType.ToString());
							
							ReferenceAttribute attribute = GetReferenceAttribute(property);
							
							if ((entityType.FullName.ToString() == reverseReferenceType.FullName.ToString()
							     && attribute.AutoDiscoverMirror)
							    || attribute.MirrorPropertyName == propertyName)
							{
								AppLogger.Debug("Mirror property name decided: " + property.Name);
								mirrorPropertyName = property.Name;
							}
						}
					}
				}
				
				AppLogger.Debug("Mirror property name: " + mirrorPropertyName);
			}
			
			return mirrorPropertyName;
		}
		
		static public string GetMirrorPropertyName(Type sourceType, string propertyName)
		{
			PropertyInfo property = sourceType.GetProperty(propertyName);
			
			return GetMirrorPropertyName(sourceType, property);
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
				
				ReferenceAttribute attribute = GetReferenceAttribute(property);
				
				// Is the mirror property name specified on the attribute?
				if (attribute.MirrorPropertyName != String.Empty
				    || attribute.MirrorPropertyName != null)
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
		
		/*public IEntity[] GetEntitiesFromArray(IEntity[] source, Guid[] ids)
		{
			if (source == null || source.Length == 0)
				return new IEntity[]{};
			
			Collection<IEntity> entities = new Collection<IEntity>(source);
		 */
		/*foreach (Guid id in ids)
			{
				AppLogger.Debug("Post contains ID: " + id.ToString());
								
				if (id != Guid.Empty)
				{
					
					IEntity entity = null;
					
					// TODO: Check if needed. Was throwing errors so it's been removed to simplify code.
					// ////!May incur performance hit though by always reloading entities from DB instead of DataSource property
					//if (DataSource != null)
					//{
					
					//AppLogger.Debug("DataSource != null");
					
					AppLogger.Debug("Getting entity from DataSource");
					
					entity = Collection<IEntity>.GetByID(source, id);
					
					if (entity == null)
						throw new Exception("Entity could not be retrieved from DataSource property.");
					
					AppLogger.Debug("Found entity from post ID: " + entity.GetType().ToString());
					//}
					//else
					//{
					//	AppLogger.Debug("DataSource == null");
					
					//	AppLogger.Debug("Getting entity from EntityFactory");
					
					//	entity = (E)EntityFactory.GetEntity<E>(id);
					
					//	AppLogger.Debug("Found entity from post ID: " + typeof(E).ToString());
					//}
					
					AppLogger.Debug("Adding entity to list.");
					entities.Add(entity);
				}
			}*/
		
		/*	return entities.GetByIDs(ids).ToArray();
		
		}*/

		static public string FormatUniqueKey(string originalData)
		{
			originalData = originalData.Trim()
				.Replace(" ", "-")
				.Replace("'", "")
				.Replace("\"", "-")
				.Replace("\\", "-")
				.Replace("/", "-")
				.Replace("?", "-")
				.Replace(":", "-")
				.Replace(".", "_")
			 	.Replace("<", "-")
				.Replace(">", "-");
				
			if (originalData.Length > 100)
				return originalData.Substring(0, 100);
			else
				return originalData;
		}
		
		/// <summary>
		/// Retrieves the name of the private field that corresponds with the specified property.
		/// </summary>
		/// <param name="type">The type of entity that the field is located on.</param>
		/// <param name="propertyName">The name of the property that corresponds with the desired field.</param>
		/// <returns>The name of the field that corresponds with the provided property.</returns>
		static public string GetFieldName(Type type, string propertyName)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			// The ID property needs to be handled differently because it's converted to lowercase rather than camel case
			if (propertyName == "ID")
				return "id";
			
			// Create a camel case field name
			string fieldName1 = ToCamelCase(propertyName);
			
			// Create a field name with an underscore _ prefix
			string fieldName2 = "_" + propertyName;
			
			// Try to find a camel case field
			FieldInfo field = type.GetField(fieldName1, BindingFlags.NonPublic | BindingFlags.Instance);
			
			// If the camel case field can't be found then
			if (field == null)
			{
				// Find the underscore _ prefixed field name
				field = type.GetField(fieldName2, BindingFlags.NonPublic | BindingFlags.Instance);
			}
			
			// TODO: Finish implementing and testing
			/*// If both attempts fail then look for a "CorrespondingField" attribute
			if (field == null)
			{
				CorrespondingFieldAttribute attribute = GetCorrespondingFieldAttribute(type,  propertyName);
				if (attribute != null)
					field = type.GetField(attribute.Name, BindingFlags.NonPublic | BindingFlags.Instance);
			}*/
			
			// If neither field can be found then throw an exception.
			// If the field doesn't follow a compatible naming convention then the developer should know as early as possible
			if (field == null)
				throw new InvalidOperationException("Cannot find field on type '" + type.ToString() + "' with name '" + fieldName1 + "' or '" + fieldName2 + "' corresponding with property '" + propertyName + "'.");
			
			// Return the name of the field that was found
			return field.Name;
		}
		
		static private CorrespondingFieldAttribute GetCorrespondingFieldAttribute(Type type, string propertyName)
		{
			CorrespondingFieldAttribute attribute = null;
			
			PropertyInfo property = EntitiesUtilities.GetProperty(type, propertyName);
			
			foreach (Attribute a in property.GetCustomAttributes(true))
			{
				if (a is CorrespondingFieldAttribute)
					attribute = (CorrespondingFieldAttribute)a;
			}
			
			return attribute;
		}
		
		static public string ToCamelCase(string propertyName)
		{
			string firstLetter = propertyName.Substring(0, 1);
			string withoutFirstLetter = propertyName.Substring(1, propertyName.Length-1);
			
			string output = firstLetter.ToLower() + withoutFirstLetter;
			
			return output;
		}
		
		
		/// <summary>
		/// Checks whether the specified counter is within the specified page.
		/// </summary>
		/// <param name="i">The counter of the items being paged.</param>
		/// <param name="pageIndex">The index of the page to retrieve.</param>
		/// <param name="pageSize">The number of items on each page.</param>
		/// <returns>A flag indicating whether the specified counter is within the specified page.</returns>
		[Obsolete("Use PagingLocation.IsInPage function instead.")]
		static public bool IsInPage(int i, int pageIndex, int pageSize)
		{
			return new PagingLocation(pageIndex, pageSize).IsInPage(i);
		}
		
	}
}
