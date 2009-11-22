
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
				
				if (IsReference(entity, property.Name, property.PropertyType))
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
				PropertyInfo property = entity.GetType().GetProperty(propertyName, returnType);
				
				if (property == null)
					AppLogger.Debug("Property: [null]");
				else
					AppLogger.Debug("Property name: " + property.Name);
				
				if (IsReference(entity, propertyName, returnType))
				{
					EntityReferenceCollection references = null;
					
					if (property.PropertyType.GetInterface("IEnumerable") != null)
					{
						AppLogger.Debug("Multiple reference property.");
						
						object propertyValue = property.GetValue(entity, null);
						
						AppLogger.Debug("Property value: " + (propertyValue == null ? "[null]" : propertyValue.ToString()));
						
						Collection<IEntity> referencedEntities = new Collection<IEntity>();
						
						//if (propertyValue is Array)
						//{
						foreach (IEntity o in Collection<IEntity>.ConvertAll(propertyValue))
						{
							referencedEntities.Add((IEntity)o);
						}
						//}
						//else
						//{
						//	foreach (IEntity entity in (Collection<IEntity>)propertyValue))
						//	{
						//		referencedEntities.Add(entity);
						///	}
						//}
						
						AppLogger.Debug("# of referenced entities found: " + referencedEntities.Count);
						
						references = new EntityReferenceCollection(entity, referencedEntities.ToArray());
						
						AppLogger.Debug("Reference objects created.");
						
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
					else if (property.PropertyType.GetInterface("IEntity") != null)
					{
						AppLogger.Debug("Single reference property.");
						
						IEntity referencedEntity = (IEntity)property.GetValue(entity, null);
						
						references = new EntityReferenceCollection(entity, (IEntity)referencedEntity);
						
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
						throw new NotSupportedException("The property type '" + property.PropertyType.ToString() + "' is not supported.");
				}
				
				AppLogger.Debug("References found: " + collection.Count.ToString());
			}
			
			return collection;
		}
		
		
		static public bool IsReference(IEntity entity, string propertyName, Type returnType)
		{
			bool isReference = false;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking if the specified property is an entity reference.", NLog.LogLevel.Debug))
			//{
			PropertyInfo property = entity.GetType().GetProperty(propertyName, returnType);
			
			//	AppLogger.Debug("Entity: " + entity.GetType().ToString());
			//	AppLogger.Debug("Property name: " + propertyName);
			
			foreach (Attribute attribute in property.GetCustomAttributes(true))
			{
				if (attribute is ReferenceAttribute)
				{
					isReference = true;
				}
			}
			
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
	}
}
