using System;
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
		
		static public bool MatchAlias(string typeName, string aliasTypeName)
		{
			// If the Alias property on the specified alias type is the specified type, then it means the alias is referencing back to the original
			// and therefore the match is true
			return Entities.EntityState.Entities[aliasTypeName].Alias == typeName;
		}
		
		/// <summary>
		/// Checks whether the specified property is a reference property.
		/// </summary>
		/// <param name="entityType">The type of entity containing the property.</param>
		/// <param name="propertyName">The name of the property being checked.</param>
		/// <param name="returnType">The return type of the property being checked.</param>
		/// <returns>A boolean value indicating whether the specified property is a reference property.</returns>
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
			bool validType = false;
			
			if (property == null)
				throw new ArgumentNullException("property");
			
			if (sourceType == null)
				throw new ArgumentNullException("sourceType");
			
			//using (LogGroup logGroup = LogGroup.Start("Checking if the specified property is an entity reference.", NLog.LogLevel.Debug))
			//{
			//	LogWriter.Debug("Entity: " + sourceType.ToString());
			//	LogWriter.Debug("Property name: " + property.Name);
			
			ReferenceAttribute reference = GetReferenceAttribute(property);
			
			isReference = reference != null;
			
			if (isReference)
			{
				Type referenceType = property.PropertyType;
				
				if (typeof(Array).IsAssignableFrom(referenceType))
				{
					// If it's an array this should work
					referenceType = property.PropertyType.GetElementType();
					
					// If it's not an array (ie. it's a collection) then this should work
					if (referenceType == null)
					{
						//LogWriter.Debug("Property is a collection.");
						referenceType = property.PropertyType.GetGenericArguments()[0];
					}
					//else
					//	LogWriter.Debug("Property is an array.");
				}
				
				validType = EntityState.IsType(referenceType);
				
				// If it's not a valid type then try using the type sepcified on the attribute
				if (!validType && reference.TypeName != String.Empty)
				{
					referenceType = EntityState.GetType(reference.TypeName);
					
					validType = EntityState.IsType(referenceType);
				}
				
				if (!validType)
					throw new Exception("Referenced type '" + referenceType.Name + "' on property '" + property.Name + "' of source entity type '" + sourceType.FullName + "' is not registered as a valid entity type. Make sure that the referenced type has an EntityAttribute.");
			}
			
			//	LogWriter.Debug("Is reference? " + isReference.ToString());
			//	LogWriter.Debug("Valid type? " + validType.ToString());
			//}
			
			return isReference && validType;
		}
		
		static public bool IsReference(IEntity entity)
		{
			return entity is EntityReference
				|| entity.GetType().Name == typeof(EntityReference).Name;
		}
		
		static public bool IsReference(Type type)
		{
			bool isReference = false;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = LogGroup.Start("Checking if the specified property is an entity reference.", NLog.LogLevel.Debug))
			//{
			isReference = type.Name == typeof(EntityReference).Name;
			//	LogWriter.Debug("Is reference? " + isReference.ToString());
			//}
			
			return isReference;
		}
		
		static public ReferenceAttribute GetReferenceAttribute(IEntity entity, string propertyName, Type returnType)
		{
			ReferenceAttribute attribute = null;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = LogGroup.Start("...", NLog.LogLevel.Debug))
			//{
			PropertyInfo property = entity.GetType().GetProperty(propertyName, returnType);
			
			//	LogWriter.Debug("Entity: " + entity.GetType().ToString());
			//	LogWriter.Debug("Property name: " + propertyName);
			
			attribute = GetReferenceAttribute(property);
			
			//	LogWriter.Debug("Is reference? " + isReference.ToString());
			//}
			
			return attribute;
		}
		
		static public ReferenceAttribute GetReferenceAttribute(IEntity entity, string propertyName)
		{
			ReferenceAttribute attribute = null;
			
			PropertyInfo property = entity.GetType().GetProperty(propertyName);
			
			if (property == null)
				throw new ArgumentException("Can't find '" + propertyName + "' property on '" + entity.GetType().FullName + "' entity type.");
			
			attribute = GetReferenceAttribute(property);
			
			return attribute;
		}
		
		static public ReferenceAttribute GetReferenceAttribute(PropertyInfo property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			
			ReferenceAttribute attribute = null;
			
			// Logging disabled simply to reduce the size of the logs
			//using (LogGroup logGroup = LogGroup.Start("Retrieving the reference attribute for the specified property.", NLog.LogLevel.Debug))
			//{
			
			//	LogWriter.Debug("Entity: " + sourceType.ToString());
			//	LogWriter.Debug("Property name: " + property.Name);
			//	LogWriter.Debug("Property type: " + property.PropertyType.ToString());
			
			foreach (Attribute a in property.GetCustomAttributes(true))
			{
				if (a is ReferenceAttribute)
				{
					attribute = (ReferenceAttribute)a;
				}
			}
			
			//	LogWriter.Debug("Attribute found: " + (attribute != null).ToString());
			//}
			
			return attribute;
		}
		
		
		
		static public Type GetReferenceType(IEntity sourceEntity, string propertyName)
		{
			return GetReferenceType(sourceEntity, propertyName, null);
		}
		
		static public Type GetReferenceType(IEntity sourceEntity, string propertyName, Type returnType)
		{
			if (sourceEntity == null)
				throw new ArgumentNullException("sourceEntity");
			
			if (propertyName == null || propertyName == String.Empty)
				throw new ArgumentException("propertyName", "Property name cannot be null or String.Empty.");
			
			
			PropertyInfo property = GetProperty(sourceEntity.GetType(), propertyName, returnType);
			
			return GetReferenceType(sourceEntity, property);
		}
		
		static public Type GetReferenceType(IEntity entity, PropertyInfo property)
		{
			Type type = null;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the referenced entity type."))
			{
				if (entity == null)
					throw new ArgumentNullException("entity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				LogWriter.Debug("Entity type: " + entity.GetType().ToString());
				LogWriter.Debug("Property name: " + property.Name);
				LogWriter.Debug("Property type: " + property.PropertyType.ToString());
				
				ReferenceAttribute attribute = GetReferenceAttribute(property);
				
				type = GetReferenceType(attribute, entity, property);
				
				if (type != null)
					LogWriter.Debug("Type: " + type.ToString());
			}
			
			return type;
		}
		
		static public Type GetReferenceType(ReferenceAttribute attribute, IEntity sourceEntity, PropertyInfo property)
		{
			Type type = null;
			
			Type sourceType = sourceEntity.GetType();
			
			if (attribute == null)
				throw new Exception("The reference attribute was not found on the '" + property.Name + "' property of the type '" + sourceType.ToString() + "'.");
			
			if (IsReference(sourceType, property))
			{
				if (attribute.TypeName != null && attribute.TypeName != String.Empty)
				{
					LogWriter.Debug("attribute.TypeName != String.Empty");
					LogWriter.Debug("attribute.TypeName == " + attribute.TypeName);
					
					type = EntityState.GetType(attribute.TypeName);
				}
				else if (attribute.TypePropertyName != String.Empty)
				{
					PropertyInfo typeProperty = sourceType.GetProperty(attribute.TypePropertyName);
					
					// TODO: Use a custom exception instead of a general one
					if (typeProperty == null)
						throw new Exception("'" + attribute.TypePropertyName + "' property not found on '" + sourceType.Name + "' type as specified by reference attribute on '" + property.Name + "' property.");
					
					string typeName = (string)typeProperty.GetValue(sourceEntity, null);
					
					// If a type name is specified then get the type otherwise leave it null
					if (typeName != null && typeName != String.Empty)
					{	
						type = EntityState.GetType(typeName);
					}
				}
				else
				{
					LogWriter.Debug("attribute.TypeName == String.Empty");
					
					// If the property is a single entity instance
					if (IsSingleReference(sourceType, property))
					{
						LogWriter.Debug("Is a single entity");
						
						type = property.PropertyType;
					} // Otherwise the property is a collection/array
					else
					{
						LogWriter.Debug("Is a collection/array");
						
						// If it's an array this should work
						type = property.PropertyType.GetElementType();
						
						// If it's not an array (ie. it's a collection) then this should work
						if (type == null)
							type = property.PropertyType.GetGenericArguments()[0];
					}
				}
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
		
		[Obsolete("Use EntityState.GetType(string typeName) instead.")]
		// TODO: Remove when not in use
		static public Type GetType(string typeName)
		{
			return EntityState.GetType(typeName);
			
		}
		
		static public bool PropertyExists(IEntity entity, string propertyName)
		{
			PropertyInfo property = GetProperty(entity.GetType(), propertyName, null);
			return property != null;
		}
		
		static public object GetPropertyValue(IEntity entity, string propertyName)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
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
		
		static public string GetMirrorPropertyNameReverse(IEntity sourceEntity, string propertyName, Type referencedEntityType)
		{
			string mirrorPropertyName = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving mirror property name using reverse method."))
			{
				foreach (PropertyInfo property in referencedEntityType.GetProperties())
				{
					using (LogGroup logGroup2 = LogGroup.StartDebug("Checking property: " + property.Name))
					{
						
						if (IsReference(sourceEntity.GetType(), property))
						{
							LogWriter.Debug("Is reference == true");
							
							Type reverseReferenceType = GetReferenceType(sourceEntity, property);
							
							LogWriter.Debug("reverseReferenceType: " + reverseReferenceType.ToString());
							
							ReferenceAttribute attribute = GetReferenceAttribute(property);
							
							LogWriter.Debug("Mirror property name on attribute: " + attribute.MirrorPropertyName);
							
							LogWriter.Debug("Allow auto discovery (as specified by attribute): " + attribute.AutoDiscoverMirror);
							
							bool attributeMirrorPropertyMatches = attribute.MirrorPropertyName == propertyName;
							
							bool canAutoDiscover = attribute.AutoDiscoverMirror;
							
							bool typeMatches = sourceEntity.GetType().FullName.ToString() == reverseReferenceType.FullName.ToString();
							
							LogWriter.Debug("Attribute mirror property matches: " + attributeMirrorPropertyMatches.ToString());
							LogWriter.Debug("Can auto discover: " + canAutoDiscover);
							LogWriter.Debug("Type matches: " + typeMatches.ToString());
							
							LogWriter.Debug("Equation: " + typeMatches.ToString() + " && (" + canAutoDiscover + " || " + attributeMirrorPropertyMatches + ")");
							
							if (typeMatches
							    && (canAutoDiscover
							        || attributeMirrorPropertyMatches))
							{
								LogWriter.Debug("Mirror property name decided: " + property.Name);
								mirrorPropertyName = property.Name;
							}
						}
					}
				}
				
				LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
			}
			
			return mirrorPropertyName;
		}
		
		static public string GetMirrorPropertyName(IEntity sourceEntity, string propertyName)
		{
			string mirrorPropertyName = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the name of the mirror property that corresponds with the specified property."))
			{
				if (sourceEntity == null)
					throw new ArgumentNullException("sourceEntity");
				
				PropertyInfo property = sourceEntity.GetType().GetProperty(propertyName);
				
				if (property == null)
					throw new Exception("Cannot find property '" + propertyName + "' on type '" + sourceEntity.GetType().ToString() + "'.");
				
				mirrorPropertyName = GetMirrorPropertyName(sourceEntity, property);
				
				LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
			}
			return mirrorPropertyName;
		}
		
		static public string GetMirrorPropertyName(IEntity sourceEntity, PropertyInfo property)
		{
			string mirrorPropertyName = String.Empty;
			
			using (LogGroup logGroup = LogGroup.StartDebug("Retrieving the name of the mirror property that corresponds with the specified property."))
			{
				if (sourceEntity == null)
					throw new ArgumentNullException("sourceEntity");
				
				if (property == null)
					throw new ArgumentNullException("property");
				
				Type sourceType = sourceEntity.GetType();
				
				ReferenceAttribute attribute = GetReferenceAttribute(property);
				
				if (attribute == null)
					throw new Exception("No ReferenceAttribute found on the '" + property.Name + "' property of the type '" + sourceType.ToString() + "'.");
				
				Type referencedEntityType = GetReferenceType(sourceEntity, property);
				
				// Is the mirror property name specified on the attribute?
				if (attribute.MirrorPropertyName != String.Empty
				    || attribute.MirrorPropertyName != null)
					mirrorPropertyName = attribute.MirrorPropertyName;
				else
				{
					// Should the mirror property name be automatically discovered based on the property type matching the source entity?
					if (attribute.AutoDiscoverMirror)
					{						
						// Loop through each property and check the types
						foreach (PropertyInfo p in referencedEntityType.GetProperties())
						{
							// Is the property a reference?
							if (IsReference(referencedEntityType, p))
							{
								IEntity[] referencedEntities = GetReferencedEntities(sourceEntity, property);
								
								// Retrieve the entity type being stored on the property
								Type reciprocalType = GetReferenceType(referencedEntities[0], p);
								
								// If the entity types match then return the property name
								if (reciprocalType.FullName == sourceType.FullName)
									mirrorPropertyName = p.Name;
							}
						}
					}
				}
				
				LogWriter.Debug("Mirror property name: " + mirrorPropertyName);
			}
			
			return mirrorPropertyName;
		}

		static public string FormatUniqueKey(string originalData)
		{
			if (originalData == null || originalData == String.Empty)
				return String.Empty;
			
			originalData = originalData.Trim()
				.Replace(" ", "-")
				.Replace("'", "")
				.Replace("\"", "")
				.Replace("\\", "")
				.Replace("/", "")
				.Replace("?", "")
				.Replace(":", "")
				.Replace(".", "")
				.Replace("<", "")
				.Replace(">", "")
				.Replace("#", "")
				.Replace("*", "");
			
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
			
			if (propertyName == "UniqueKey")
				return "uniqueKey";
			
			// Create a camel case field name
			string fieldName1 = ToCamelCase(propertyName);
			
			// Create a field name with an underscore _ prefix
			string fieldName2 = "_" + propertyName;
			
			// Try to find a camel case field
			FieldInfo field = type.GetField(fieldName1, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			
			// If the camel case field can't be found then
			if (field == null)
			{
				// Find the underscore _ prefixed field name
				field = type.GetField(fieldName2, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			}
				
			// If neither field can be found then throw an exception.
			// If the field doesn't follow a compatible naming convention then the developer should know as early as possible
			if (field == null)
				throw new InvalidOperationException("Cannot find field on type '" + type.ToString() + "' with name '" + fieldName1 + "' or '" + fieldName2 + "' corresponding with property '" + propertyName + "'.");
			
			// Return the name of the field that was found
			return field.Name;
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
