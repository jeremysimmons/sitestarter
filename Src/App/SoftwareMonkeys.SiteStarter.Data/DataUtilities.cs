using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using NLog;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Represents a group of data filters.
	/// </summary>
	static public class DataUtilities
    	{
		static public BaseEntityReferenceAttribute GetReferenceAttribute(PropertyInfo property)
		{
			if (property == null)
				return null;
                        foreach (object attribute in property.GetCustomAttributes(true))
                        {
	                        if (attribute is BaseEntityReferenceAttribute)
	                        {
					return (BaseEntityReferenceAttribute)attribute;
				}
			}
			return null;
		}	  

		static public PropertyInfo GetMirrorProperty(PropertyInfo property)
		{
			return GetMirrorProperty(property, null);
		}     

		static public PropertyInfo GetMirrorProperty(PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{
			PropertyInfo mirrorProperty = null;

			using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the mirror of the property provided.", NLog.LogLevel.Debug))
			{
				if (attribute == null)
					attribute = GetReferenceAttribute(property);
				
				string mirrorPropertyName = attribute.MirrorName;

				AppLogger.Debug("Mirror property name: " + mirrorPropertyName);
	
				if (mirrorPropertyName == String.Empty)
					throw new Exception("No mirror property name has been specified.");
	
				Type mirrorType = GetReferenceType(property, attribute);

				AppLogger.Debug("Mirror type: " + mirrorType.ToString());

				mirrorProperty = mirrorType.GetProperty(mirrorPropertyName);
	
				if (mirrorProperty == null)
				{
					AppLogger.Debug("No property found.");
					throw new Exception("The mirror property cannot be found or the name specified is invalid.");
				}
			}

			return mirrorProperty;
		}

        static public PropertyInfo GetIDsProperty(PropertyInfo property)
        {
            PropertyInfo mirrorProperty = null;

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the IDs property related to the property provided.", NLog.LogLevel.Debug))
            {
                BaseEntityReferenceAttribute attribute = GetReferenceAttribute(property);

                if (attribute.IDsPropertyName == property.Name)
                    return property;
                else
                {
                    PropertyInfo idsProperty = property.DeclaringType.GetProperty(attribute.IDsPropertyName);
                    return idsProperty;
                }
            }

            return null;
        }

		static public Type GetReferenceType(PropertyInfo property, BaseEntityReferenceAttribute attribute)
		{

			if (property == null)
				throw new ArgumentNullException("property");

			if (attribute == null)
				throw new ArgumentNullException("attribute");
			Type type = property.PropertyType;

			if (type.IsSubclassOf(typeof(Array)))
			{
				type = type.GetElementType();
			}
			else if (type.IsSubclassOf(typeof(BaseEntity)))
				type = property.PropertyType;

			// If the type is Guid then get the type of the entities reference property
			if (type == typeof(Guid))
			{
				if (attribute.EntitiesPropertyName.Length > 0)
				{
					PropertyInfo entitiesProperty = property.DeclaringType.GetProperty(attribute.EntitiesPropertyName);

					type = GetReferenceType(entitiesProperty, GetReferenceAttribute(entitiesProperty));

				}
			}

			return type;
		}

	        static public BaseEntity[] AddReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)//, BaseEntityReferenceAttribute attribute)
	        {
	                List<BaseEntity> toUpdate = new List<BaseEntity>();
	
			using (LogGroup logGroup = AppLogger.StartGroup("Adding mirror references.", NLog.LogLevel.Debug))
			{
		            if (referenceEntity != null)
		            {
				AppLogger.Debug("referenceEntity != null");
	
				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Referenced entity type: " + referenceEntity.GetType().ToString());
				AppLogger.Debug("Property name: " + property.Name);

				BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);
	
		
		                if (attribute.MirrorName != null && attribute.MirrorName != String.Empty)
		                {
		                    PropertyInfo mirrorProperty = GetMirrorProperty(property, attribute);

					if (mirrorProperty != null)
					{
			                    object mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

						AppLogger.Debug("Mirror value: " + mirrorValue.ToString());
			
			                    //object propertyValue = property.GetValue(entity, null);
			                    if (property.PropertyType.Equals(typeof(Guid[])))
			                    {
						AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(Guid[]))");
						toUpdate.AddRange(AddIDsReferences(entity, referenceEntity, property));
			                    }
			                    else if (property.PropertyType.IsSubclassOf(typeof(BaseEntity[])))
			                    {
						// This can be skipped. It's taken care of by the ID references
						//AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(BaseEntity[]))");
						//toUpdate.AddRange(AddEntitiesReferences(entity, referenceEntity, property));
			                    }
			                    else if (property.PropertyType.Equals(typeof(Guid)))
			                    {
						AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(Guid))");
						toUpdate.AddRange(AddIDReferences(entity, referenceEntity, property));
			                    }
			                    else if (property.PropertyType.IsSubclassOf(typeof(BaseEntity)))
			                    {
						// This can be skipped. It's taken care of by the ID references
						//AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(BaseEntity))");
						//toUpdate.AddRange(AddEntityReferences(entity, referenceEntity, property));
			                    }
			                }
					else
						AppLogger.Debug("Mirror property not found.");
				}
		
		            }
		            else
				{
					AppLogger.Debug("referenceEntity != null");
		
				}
			}
	
		                return toUpdate.ToArray();
	        }

		static public BaseEntity[] AddIDsReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{
			List<BaseEntity> toUpdate = new List<BaseEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Adding IDs references.", NLog.LogLevel.Debug))
			{
				object propertyValue = null;
				if (property != null)
					propertyValue = property.GetValue(entity, null);
	
	
				PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
	
				//object mirrorValue = null;
				//if (mirrorProperty != null)
				//	mirrorValue = mirrorProperty.GetValue(referenceEntity, null);
	
	                        // If the referenced entity is found in the latest reference
	                        // if (Array.IndexOf((Guid[])propertyValue, referenceEntity.ID) > -1)
	                        // {
	                        //ArrayList mirrorList = mirrorValue == null ? new ArrayList() : new ArrayList((Guid[])mirrorValue);
				//using (LogGroup logGroup3 = AppLogger.StartGroup("Outputting the current mirror values", NLog.LogLevel.Debug))
				//{
				//	foreach (Guid id in mirrorList)
				//		AppLogger.Debug(id.ToString());
				//}

				if (propertyValue is Guid)
				{
					AppLogger.Debug("propertyValue is Guid");
		                        if ((Guid)propertyValue != entity.ID)
		                        {
	
						AppLogger.Debug("Setting the ID '" + entity.ID + "' to the property '" + property.Name + "' on an object of type '" + entity.GetType().ToString() + "'.");
		                            property.SetValue(entity, (Guid)propertyValue, null);
		
						AppLogger.Debug("Adding the referenced entity '" + entity.GetType().ToString() + "' to the list of entities pending update (toUpdate).");
		                            
	
		                            toUpdate.Add(entity);
		                        }
					else
					{
						AppLogger.Debug("Reference already up to date. Skipped.");
						//AppLogger.Debug("The referenced entity ID is not found in the specified property of the source entity so it was skipped.");
					}
				}
				else
				{
					AppLogger.Debug("propertyValue is not Guid (must be Guid[])");

					List<Guid> list = new List<Guid>();
					if (propertyValue != null)
					{
						if (property.PropertyType.Equals(typeof(Guid)))
						{
							
							list.Add((Guid)propertyValue);
						}
						else
						{
							
							list.AddRange((Guid[])propertyValue);
						}
					}

		                        if (!list.Contains(referenceEntity.ID))
		                        {
	
						AppLogger.Debug("Setting the ID to the '" + property.Name + "' property on an object of type '" + entity.GetType().ToString() + "'.");
						if (property.PropertyType.Equals(typeof(Guid)))
						{
		                            		property.SetValue(entity, referenceEntity.ID, null);
						}	
						else
						{
							AppLogger.Debug("Add entity ID to the list: " + entity.ID);
				                            list.Add(referenceEntity.ID);
		                            		property.SetValue(entity, list.ToArray(), null);
						}
		
						AppLogger.Debug("Adding the entity '" + entity.GetType().ToString() + "' to the list of entities pending update (toUpdate).");
		                            
	
		                            	toUpdate.Add(entity);
		                        }
					else
					{
						AppLogger.Debug("Reference already exists. Skipped.");
						//AppLogger.Debug("The referenced entity ID is not found in the specified property of the source entity so it was skipped.");
					}
				}
			}

	                return toUpdate.ToArray();
		}

		static public BaseEntity[] AddEntitiesReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{
			List<BaseEntity> toUpdate = new List<BaseEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Adding entities references.", NLog.LogLevel.Debug))
			{
	
					PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
	
					object mirrorValue = null;
					if (mirrorProperty != null)
						mirrorValue = mirrorProperty.GetValue(referenceEntity, null);
	
	                        if (mirrorProperty.PropertyType.IsSubclassOf(typeof(BaseEntity[])))
	                        {
	
	                            ArrayList list = mirrorValue != null ? new ArrayList((BaseEntity[])mirrorValue) : new ArrayList();
	                            if (!list.Contains(referenceEntity))
	                            {
	                                list.Add(referenceEntity);
	
	                                property.SetValue(entity, list.ToArray(referenceEntity.GetType()), null);
	
	                                toUpdate.Add(entity);
	                            }
				}
				else
				{
	                            property.SetValue(entity, referenceEntity, null);
	
	                            toUpdate.Add(entity);
	                        }
			}
	
		        return toUpdate.ToArray();
		}

		static public BaseEntity[] AddIDReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{

			List<BaseEntity> toUpdate = new List<BaseEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Adding ID reference.", NLog.LogLevel.Debug))
			{
	
				BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);
	
	                        if (attribute.MirrorName != String.Empty)// && mirrorValue != null)
	                        {
					PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
	
					object mirrorValue = null;
					if (mirrorProperty != null)
						mirrorValue = mirrorProperty.GetValue(referenceEntity, null);
	
					object propertyValue = null;
					if (property != null)
						propertyValue = property.GetValue(entity, null);

					AppLogger.Debug("Property value: " + propertyValue.ToString());
	
					if (property.PropertyType.IsSubclassOf(typeof(Array)))
					{
	
						AppLogger.Debug("Property type is Array (must be Guid[])");
	
		                            ArrayList mirrorList = null;
						if (mirrorValue != null)
						{
							if (mirrorValue is Guid[])
								mirrorList = new ArrayList((Guid[])mirrorValue);
							else
							{
								mirrorList = new ArrayList();
								mirrorList.Add((Guid)mirrorValue);
							}
						}
						else
							mirrorList = new ArrayList();
	
						ArrayList list = (propertyValue != null ? new ArrayList((Guid[])propertyValue) : new ArrayList());
	
	
		                            if (!list.Contains(referenceEntity.ID))
		                            {
						AppLogger.Debug("Adding referenced entity ID " + referenceEntity.ID + " to the list.");
		                                list.Add(referenceEntity.ID);
	
						AppLogger.Debug("Setting list of IDs to " + property.Name + " property of " + entity.GetType().ToString());
		                                property.SetValue(entity, list.ToArray(typeof(Guid)), null);
		
						AppLogger.Debug("Adding entity " + entity.GetType().ToString() + " to the to-update list.");
		                                toUpdate.Add(entity);
		                            }
					}
					else
					{
						if ((Guid)propertyValue != referenceEntity.ID)
						{
						AppLogger.Debug("Property type is not an Array (must be Guid)");
		
		
							AppLogger.Debug("Referenced entity ID is:" + referenceEntity.ID);
		
							AppLogger.Debug("Setting referenced entity ID to the " + property.Name + " property of " + entity.GetType().ToString());
			                                property.SetValue(entity, referenceEntity.ID, null);
		
							AppLogger.Debug("Adding entity " + entity.GetType().ToString() + " to the to-update list.");
			                                toUpdate.Add(entity);
						}
						else
						{
							AppLogger.Debug("Reference already up to date. Skipped.");
						}
					}
	                        }
			}

		       	return toUpdate.ToArray();
		}

		static public BaseEntity[] AddEntityReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{
			List<BaseEntity> toUpdate = new List<BaseEntity>();

	                using (LogGroup logGroup = AppLogger.StartGroup("Adding entity references.", NLog.LogLevel.Debug))
			{
				BaseEntityReferenceAttribute attribute = GetReferenceAttribute(property);

	                        if (attribute.MirrorName != String.Empty)// && mirrorValue != null)
	                        {
					object propertyValue = property.GetValue(entity, null);
					AppLogger.Debug("Property value: " + propertyValue.ToString());


					if (property.PropertyType.IsSubclassOf(typeof(Array)))
					{
						AppLogger.Debug("Property type is an Array (must be Guid[])");
	
		                            Collection<BaseEntity> list = (propertyValue != null ? new Collection<BaseEntity>((BaseEntity[])propertyValue) : new Collection<BaseEntity>());
		                            if (!list.Contains(referenceEntity.ID))
		                            {
						AppLogger.Debug("Adding referenced entity ID " + referenceEntity.ID + " to the list.");
		                                list.Add(referenceEntity);
	
						AppLogger.Debug("Setting list of IDs to " + property.Name + " property of " + entity.GetType().ToString());
		                                property.SetValue(entity, list.ToArray(typeof(BaseEntity)), null);
		
						AppLogger.Debug("Adding entity " + entity.GetType().ToString() + " to the to-update list.");
		                                toUpdate.Add(entity);
		                            }
					}
					else
					{
						AppLogger.Debug("Property type is not an Array (must be Guid)");
	
	
						AppLogger.Debug("Referenced entity ID is:" + referenceEntity.ID);
	
						AppLogger.Debug("Setting referenced entity ID to the " + property.Name + " property of " + entity.GetType().ToString());
		                                property.SetValue(entity, referenceEntity, null);
	
						AppLogger.Debug("Adding entity " + entity.GetType().ToString() + " to the to-update list.");
		                                toUpdate.Add(entity);
					}
	                        }
			}

			return toUpdate.ToArray();
		}

	       static public BaseEntity[] RemoveReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
	       {
	                List<BaseEntity> toUpdate = new List<BaseEntity>();
	
	                using (LogGroup logGroup = AppLogger.StartGroup("Removing mirror references.", NLog.LogLevel.Debug))
	                {
	                    AppLogger.Debug("Entity type: " + entity.GetType().ToString());
	                    AppLogger.Debug("Entity ID: " + entity.ID);
	                    AppLogger.Debug("Property name: " + property.Name);
	                    AppLogger.Debug("Property type: " + property.PropertyType);
	                    AppLogger.Debug("Reference entity type: " + referenceEntity.GetType().ToString());
	                    AppLogger.Debug("Reference entity ID: " + referenceEntity.ID.ToString());
	
			//	BaseEntityReferenceAttribute attrib = DataUtilities.GetReferenceAttribute(property);
	
	                  //  AppLogger.Debug("Mirror name: " + attrib.MirrorName);
	
	
	                    if (property != null)
	                    {
	                        //PropertyInfo mirrorProperty = referenceEntity.GetType().GetProperty(attrib.MirrorName);
	
	                        //object mirrorValue = mirrorProperty.GetValue(referenceEntity, null);
	
	                        //object propertyValue = property.GetValue(entity, null);
	
	                        if (property.PropertyType.Equals(typeof(Guid[])))
	                        {
	                           toUpdate.AddRange(RemoveIDsReferences(entity, referenceEntity, property));
	                        }
	                        else if (property.PropertyType.Equals(typeof(Guid)))
	                        {
	                            toUpdate.AddRange(RemoveIDReferences(entity, referenceEntity, property));
	                        }
	                        else if (property.PropertyType.IsSubclassOf(typeof(BaseEntity)))
	                        {
	                            toUpdate.AddRange(RemoveEntityReferences(entity, referenceEntity, property));
	                        }
	                        else if (property.PropertyType.IsSubclassOf(typeof(Array)))
	                        {
	                            toUpdate.AddRange(RemoveEntitiesReferences(entity, referenceEntity, property));
	                        }
	                        else
	                            throw new InvalidOperationException("The values types are incompatible: '" + property.PropertyType.GetElementType().ToString() + "' and '" + referenceEntity.GetType().ToString() + "'");
	                    }
	
	                }
	            return toUpdate.ToArray();
	        }

		static public BaseEntity[] RemoveIDsReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (referenceEntity == null)
				throw new ArgumentNullException("referenceEntity");
			if (property == null)
				throw new ArgumentNullException("property");

			List<BaseEntity> toUpdate = new List<BaseEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Removing IDs references from entity.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Property name: " + property.Name);
				AppLogger.Debug("Property type: " + property.GetType().ToString());

				object propertyValue = property.GetValue(entity, null);

				AppLogger.Debug("Property value: " + propertyValue.ToString());
	
				List<Guid> list = new List<Guid>();
				if (propertyValue.GetType().IsSubclassOf(typeof(Array)))
					list.AddRange((Guid[])propertyValue);
				else
					list.Add((Guid)propertyValue);

				AppLogger.Debug("# of references: " + list.Count.ToString());
	
				//PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
				//object mirrorValue = null;
				//if (mirrorProperty != null)
				//{
				//	mirrorValue = mirrorProperty.GetValue(referenceEntity, null);
				//	AppLogger.Debug("Mirror value: " + mirrorValue.ToString());
				//}
				//else
				//	AppLogger.Debug("Mirror value: [null]");
	
	
	                        //    ArrayList mirrorList = new ArrayList();
				//	if (mirrorValue != null)
				//	{
				//		if (mirrorValue.GetType().IsSubclassOf(typeof(Array)))
				//			mirrorList.AddRange((Guid[])mirrorValue);
				//		else
				//		{
				//			mirrorList.Add((Guid)mirrorValue);
				//		}
				//	}

				//AppLogger.Debug("# of mirror references: " + mirrorList.Count.ToString());
	
					// If the mirror reference exists
	                            if (list.Contains(referenceEntity.ID))
	                            {
					
					// If the property type is an array (must be Guid[])
					if (property.PropertyType.IsSubclassOf(typeof(Array)))
					{
		                                AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + DataUtilities.GetReferenceAttribute(property).MirrorName + " property on type " + referenceEntity.GetType());
		
		                                list.Remove(entity.ID);

	                                	property.SetValue(entity, list.ToArray(), null);
					}
					else // If the property is not an array (must be Guid)
					{
						AppLogger.Debug("Entity ID " + Guid.NewGuid() + " + being set to " + property.Name + " property of " + entity.GetType() + ".");
						property.SetValue(entity, Guid.NewGuid(), null);
					}
						
	
					// Adding entity to update list
	                                toUpdate.Add(entity);
	                            }
				    else
				    {
					AppLogger.Debug("Reference not found. Removal skipped.");
				    }
			}

			return toUpdate.ToArray();
		}

		static public BaseEntity[] RemoveIDReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{
			List<BaseEntity> toUpdate = new List<BaseEntity>();

			using (LogGroup logGroup = AppLogger.StartGroup("Removing IDs references from entity.", NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Entity type: " + entity.GetType().ToString());
				AppLogger.Debug("Property name: " + property.Name);
				AppLogger.Debug("Property type: " + property.GetType().ToString());

				object propertyValue = property.GetValue(entity, null);

				AppLogger.Debug("Property value: " + propertyValue.ToString());
	
				List<Guid> list = new List<Guid>();
				if (propertyValue.GetType().IsSubclassOf(typeof(Array)))
					list.AddRange((Guid[])propertyValue);
				else
					list.Add((Guid)propertyValue);

				AppLogger.Debug("# of references: " + list.Count.ToString());
	
				PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
				object mirrorValue = null;
				if (mirrorProperty != null)
					mirrorValue = mirrorProperty.GetValue(referenceEntity, null);
	
	
	                            ArrayList mirrorList = null;
					if (mirrorValue != null)
					{
						if (mirrorValue.GetType().IsSubclassOf(typeof(Array)))
							mirrorList = new ArrayList((Guid[])mirrorValue);
						else
						{
							mirrorList = new ArrayList();
							mirrorList.Add((Guid)mirrorValue);
						}
					}

				AppLogger.Debug("# of mirror references: " + mirrorList.Count.ToString());
	
					// If the reference exists
	                            if (list.Contains(referenceEntity.ID))
	                            {
					// If the new reference doesn't yet exist
					//if (!list.Contains(referenceEntity.ID))
					//{
						// If the property type is an array (must be Guid[])
						if (property.PropertyType.IsSubclassOf(typeof(Array)))
						{
			                                AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + mirrorProperty.Name + " property on type " + referenceEntity.GetType());
			
			                                list.Remove(referenceEntity.ID);
	
		                                	property.SetValue(entity, list.ToArray(), null);
		
							// Adding entity to update list
			                                toUpdate.Add(entity);
						}
						else // If the property is not an array (must be Guid)
						{
							
						
							AppLogger.Debug("Entity ID " + Guid.NewGuid() + " + being set to " + property.Name + " property of " + entity.GetType() + ".");
							property.SetValue(entity, Guid.NewGuid(), null);

			
							// Adding entity to update list
			                                toUpdate.Add(entity);
						}
							
					//}
					//else
					//{
					//	AppLogger.Debug("Valid reference already exists. Kept.");
					//}
	                            }
				    else
				    {
					AppLogger.Debug("Reference not found. Removal skipped.");
				    }
			}

			return toUpdate.ToArray();

			//throw new NotImplementedException();
			/*List<BaseEntity> toUpdate = new List<BaseEntity>();

			PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
			object mirrorValue = null;
			if (mirrorProperty != null)
				mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

			 AppLogger.Debug("mirrorValue is Guid");
                            if (mirrorValue == null || (Guid)mirrorValue != entity.ID)
                            {
                                //if ((Guid)mirrorValue != Guid.Empty)
                                //{
                                    AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + mirrorProperty.Name + " property on type " + referenceEntity.GetType());

                                    mirrorProperty.SetValue(referenceEntity, entity.ID, null);

                                    toUpdate.Add(referenceEntity);
                                //}
                            }

			return toUpdate.ToArray();*/
		}

		static public BaseEntity[] RemoveEntityReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{
			List<BaseEntity> toUpdate = new List<BaseEntity>();
			//AppLogger.Debug("mirrorValue is BaseEntity");

				PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
				object mirrorValue = null;
				object propertyValue = null;
				Guid referenceID = Guid.Empty;

				if (mirrorProperty != null)
					mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

				if (property != null && entity != null)
					propertyValue = property.GetValue(entity, null);

				
				//if (propertyValue is Guid)
				//	referenceID = (Guid)propertyValue;
				//else
				//	throw new NotSupportedException("Invalid property type: " + propertyValue.GetType());

                            //if (mirrorValue != null)
                            //{
                                if (property.PropertyType == referenceEntity.GetType())
                                {
					if (propertyValue != null)
					{
	                                    //Collection<BaseEntity> list = new Collection<BaseEntity>((BaseEntity)propertyValue);
	
						// If the reference is still between the provided entities
	                                    if (((BaseEntity)propertyValue).ID == referenceEntity.ID)
	                                    {
	                                        AppLogger.Debug("Entity reference to " + referenceEntity.GetType() + " being removed from " + property.Name + " property on type " + entity.GetType());
	
	                                        //list.Remove(referenceEntity);
	                                        property.SetValue(entity, Guid.Empty, null);
	
	                                        toUpdate.Add(entity);
	                                    }
					}
                                }
                                else
                                    throw new InvalidOperationException("The values types are incompatible: '" + property.PropertyType.ToString() + "' and '" + referenceEntity.GetType().ToString() + "'");
                           // }

			return toUpdate.ToArray();
		}


		static public BaseEntity[] RemoveEntitiesReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property)
		{
			throw new NotImplementedException();
			List<BaseEntity> toUpdate = new List<BaseEntity>();

			AppLogger.Debug("mirrorValue is BaseEntity[]");


				object propertyValue = property.GetValue(entity, null);

				List<BaseEntity> list = new List<BaseEntity>();
				if (propertyValue.GetType().IsSubclassOf(typeof(Array)))
					list.AddRange((BaseEntity[])propertyValue);
				else
					list.Add((BaseEntity)propertyValue);

				object mirrorValue = null;
				PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);
				if (mirrorProperty != null)
					mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

                            foreach (BaseEntity e in (BaseEntity[])mirrorValue)
                            {
                                if (mirrorValue != null)
                                {
                                    if (mirrorValue.GetType().GetElementType() == entity.GetType())
                                    {
                                        //List<BaseEntity> list = new Collection<BaseEntity>((BaseEntity[])mirrorValue);
                                        if (list.Contains(entity))
                                        {
                                            AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + mirrorProperty.Name + " property on type " + referenceEntity.GetType());

                                            list.Remove(entity);
                                            mirrorProperty.SetValue(referenceEntity, list.ToArray(), null);

                                            toUpdate.Add(referenceEntity);
                                        }
                                    }
                                    else
                                        throw new InvalidOperationException("The values types are incompatible: '" + mirrorValue.GetType().ToString() + "' and '" + entity.GetType().ToString() + "'");
                                }
                            }

			return toUpdate.ToArray();
		}

	
	       /* static public BaseEntity[] RemoveMirrors(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property, BaseEntityReferenceAttribute attrib)
            {
                List<BaseEntity> toUpdate = new List<BaseEntity>();

                using (LogGroup logGroup = AppLogger.StartGroup("Removing mirror references.", NLog.LogLevel.Debug))
                {
                    AppLogger.Debug("Entity type: " + entity.GetType().ToString());
                    AppLogger.Debug("Entity ID: " + entity.ID);
                    AppLogger.Debug("Property name: " + property.Name);
                    AppLogger.Debug("Property type: " + property.PropertyType);
                    AppLogger.Debug("Reference entity type: " + referenceEntity.GetType().ToString());
                    AppLogger.Debug("Reference entity ID: " + referenceEntity.ID.ToString());
                    AppLogger.Debug("Mirror name: " + attrib.MirrorName);


                    if (attrib.MirrorName != null && attrib.MirrorName != String.Empty)
                    {
                        PropertyInfo mirrorProperty = referenceEntity.GetType().GetProperty(attrib.MirrorName);

                        object mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

                        object propertyValue = property.GetValue(entity, null);

                        if (mirrorValue is Guid[])
                        {
                            AppLogger.Debug("mirrorValue is Guid[]");

				List<Guid> list = new List<Guid>();
				if (propertyValue.GetType().IsSubclassOf(typeof(Array)))
					list.AddRange((Guid[])propertyValue);
				else
					list.Add((Guid)propertyValue);

                            ArrayList mirrorList = mirrorValue == null ? new ArrayList() : new ArrayList((Guid[])mirrorValue);
                            if (mirrorList.Contains(entity.ID))
                            {
				if (!list.Contains(referenceEntity.ID))
				{
	                                AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + mirrorProperty.Name + " property on type " + referenceEntity.GetType());
	
	                                mirrorList.Remove(entity.ID);
	
	                                mirrorProperty.SetValue(referenceEntity, mirrorList.ToArray(typeof(Guid)), null);
	
	                                toUpdate.Add(referenceEntity);
				}
				else
				{
					AppLogger.Debug("Reference is still valid. Kept.");
				}
                            }
                        }
                        else if (mirrorValue is Guid)
                        {
                            AppLogger.Debug("mirrorValue is Guid");
                            if (mirrorValue == null || (Guid)mirrorValue != entity.ID)
                            {
                                //if ((Guid)mirrorValue != Guid.Empty)
                                //{
                                    AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + mirrorProperty.Name + " property on type " + referenceEntity.GetType());

                                    mirrorProperty.SetValue(referenceEntity, entity.ID, null);

                                    toUpdate.Add(referenceEntity);
                                //}
                            }
                        }
                        else if (mirrorValue is BaseEntity)
                        {
                            AppLogger.Debug("mirrorValue is BaseEntity");


                            if (mirrorValue != null)
                            {
                                if (mirrorValue.GetType().GetElementType() == entity.GetType())
                                {
                                    Collection<BaseEntity> list = new Collection<BaseEntity>((BaseEntity[])mirrorValue);
                                    if (list.Contains(entity.ID))
                                    {
                                        AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + mirrorProperty.Name + " property on type " + referenceEntity.GetType());

                                        list.Remove(entity);
                                        mirrorProperty.SetValue(referenceEntity, list.ToArray(entity.GetType()), null);

                                        toUpdate.Add(referenceEntity);
                                    }
                                }
                                else
                                    throw new InvalidOperationException("The values types are incompatible: '" + mirrorValue.GetType().ToString() + "' and '" + entity.GetType().ToString() + "'");
                            }
                        }
                        else if (mirrorValue is BaseEntity[])
                        {

                            AppLogger.Debug("mirrorValue is BaseEntity[]");


				List<BaseEntity> list = new List<BaseEntity>();
				if (propertyValue.GetType().IsSubclassOf(typeof(Array)))
					list.AddRange((BaseEntity[])propertyValue);
				else
					list.Add((BaseEntity)propertyValue);

                            foreach (BaseEntity e in (BaseEntity[])mirrorValue)
                            {
                                if (mirrorValue != null)
                                {
                                    if (mirrorValue.GetType().GetElementType() == entity.GetType())
                                    {
                                        //List<BaseEntity> list = new Collection<BaseEntity>((BaseEntity[])mirrorValue);
                                        if (list.Contains(entity))
                                        {
                                            AppLogger.Debug("Entity reference to " + entity.GetType() + " being removed from " + mirrorProperty.Name + " property on type " + referenceEntity.GetType());

                                            list.Remove(entity);
                                            mirrorProperty.SetValue(referenceEntity, list.ToArray(), null);

                                            toUpdate.Add(referenceEntity);
                                        }
                                    }
                                    else
                                        throw new InvalidOperationException("The values types are incompatible: '" + mirrorValue.GetType().ToString() + "' and '" + entity.GetType().ToString() + "'");
                                }
                            }
                        }
                        else
                            throw new InvalidOperationException("The values types are incompatible: '" + mirrorValue.GetType().ToString() + "' and '" + entity.GetType().ToString() + "'");
                    }

                }
	            return toUpdate.ToArray();
	        }*/

		static public void TransferReferenceIDs(BaseEntity entity)
		{
            using (LogGroup logGroup = AppLogger.StartGroup("Transferring entity IDs from entity reference properties to entity ID properties.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Entity type: " + entity.GetType().ToString());

                foreach (PropertyInfo property in entity.GetType().GetProperties())
                {
                    BaseEntityReferenceAttribute attribute = GetReferenceAttribute(property);

                    if (attribute != null)
                    {
                        if (attribute.IDsPropertyName != String.Empty
                            && attribute.EntitiesPropertyName != String.Empty)
                        {
                            using (LogGroup logGroup2 = AppLogger.StartGroup("Transferrable property identified.", NLog.LogLevel.Debug))
                            {
                                AppLogger.Debug("Property name: " + property.Name);

                                // If this is the entities reference property then do the transfer
                                if (attribute.EntitiesPropertyName == property.Name)
                                {
                                    AppLogger.Debug("Entities reference property found.");

                                    object value = property.GetValue(entity, null);

					PropertyInfo idsProperty = GetIDsProperty(property);
				object idsValue = null;

				if (idsProperty != null)
                                    idsValue = idsProperty.GetValue(entity, null);

                                    if (value == null)
                                    {
                                        AppLogger.Debug("The entities reference property value is null. Transfer skipped.");
                                    }
                                    else if (idsValue != null && idsValue.Equals(value))
                                    {
                                        AppLogger.Debug("The reference property is equivalent to the IDs property. Transfer skipped.");
                                    }
                                    else if (idsProperty != null)
                                    {
					if (property.PropertyType.IsSubclassOf(typeof(Array)))
					{
						AppLogger.Debug("attribute is EntityReferencesAttribute");
	                                        AppLogger.Debug("Transferring IDs from '" + attribute.EntitiesPropertyName + "' property to '" + attribute.IDsPropertyName + "' property.");
						if (value != null)
		                                        idsProperty.SetValue(entity, new Collection<BaseEntity>(value).GetIDs(), null);
						else
		                                        idsProperty.SetValue(entity, new Collection<BaseEntity>().GetIDs(), null);
					}
					else
					{
						
						AppLogger.Debug("attribute is EntityReferenceAttribute");
	                                        AppLogger.Debug("Transferring ID from '" + attribute.EntitiesPropertyName + "' property to '" + attribute.IDsPropertyName + "' property.");
						if (value != null)
			                                idsProperty.SetValue(entity, ((BaseEntity)value).ID, null);
						else
			                                idsProperty.SetValue(entity, Guid.Empty, null);
					}
                                    }

                                }
                            }
                        }
                    }
                }
            }
		}

		static public void ApplyExclusions(BaseEntity entity)
		{
            using (LogGroup logGroup = AppLogger.StartGroup("Applying property exclusions to the provided entity.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Entity type: " + entity.ToString());

                foreach (PropertyInfo property in entity.GetType().GetProperties())
                {
                    BaseEntityReferenceAttribute attribute = GetReferenceAttribute(property);

                    if (attribute != null)
                    {
                        if (attribute.ExcludeFromDataStore)
                        {
                            AppLogger.Debug("Applying exclusion to property '" + property.Name + "'.");

                            property.SetValue(entity, null, null);
                        }
                    }
                }
            }
		}

        /// <summary>
        /// Clears the obsolete reverse references then applies the new reverse references.
        /// </summary>
        static public BaseEntity[] SynchroniseReverseReferences(BaseEntity entity, PropertyInfo property, BaseEntity[] reverseReferencedEntities)
        {
            Collection<BaseEntity> references = new Collection<BaseEntity>();
            Collection<BaseEntity> toUpdate = new Collection<BaseEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Synchronising the reverse references for the specified property on the provided entity."))
            {
                AppLogger.Debug("Entity type: " + entity.GetType().ToString());
                AppLogger.Debug("Entity ID: " + entity.ID);
                AppLogger.Debug("Entity string: " + entity.ToString());
                AppLogger.Debug("Property name: " + property.Name);
                AppLogger.Debug("Property type: " + property.PropertyType.ToString());

                Type referenceEntityType = DataUtilities.GetReferenceType(property, DataUtilities.GetReferenceAttribute(property));
                PropertyInfo mirrorProperty = DataUtilities.GetMirrorProperty(property);

                AppLogger.Debug("Mirror property: " + mirrorProperty.Name);
                AppLogger.Debug("Mirror property type: " + mirrorProperty.PropertyType);

                BaseEntity[] entities = reverseReferencedEntities;

                BaseEntity[] originalReferences = (BaseEntity[])new List<BaseEntity>(entities).ToArray();
                //Entity[] originalReferences = GetEntities(referenceEntityType, query.Execute());

                if (property.PropertyType.Equals(typeof(Guid[])))
                {
                    AppLogger.Debug("Property type is Guid[]");

                    Guid[] referencedIDs = (Guid[])property.GetValue(entity, null);
                    foreach (Guid id in referencedIDs)
                    {
                        AppLogger.Debug("Contains ID: " + id.ToString());
                    }

                    references.Add(DataAccess.Data.Stores[entity.GetType()].GetEntities(referencedIDs));
                }
                else if (property.PropertyType.Equals(typeof(Guid)))
                {
                    AppLogger.Debug("Property type is Guid");

                    Guid referencedID = (Guid)property.GetValue(entity, null);

                    AppLogger.Debug("Referenced ID: " + referencedID.ToString());

                    if (referencedID != Guid.Empty)
                        references.Add(DataAccess.Data.Stores[entity.GetType()].GetEntity(typeof(BaseEntity), "id", referencedID));
                }

                // TODO: Check if needed
                /*  if (references != null && references.Count > 0)
                      property.SetValue(entity, references.GetIDs(), null);
                  else
                      property.SetValue(entity, null, null);*/

                // If a reference exists in both old and new copy then keep it in the new
                if (originalReferences != null)
                {
                    for (int i = 0; i < originalReferences.Length; i++)
                    {
                        // If the reference is not still being kept then allow it to be removed
                        if (!references.Contains(originalReferences[i]))
                        {
                            AppLogger.Debug("Removing reference from type " + originalReferences[i] + " with ID " + originalReferences[i].ID + ".");

                            // Update the mirror references
                            toUpdate.Add(DataUtilities.RemoveReferences(originalReferences[i], entity, DataUtilities.GetMirrorProperty(property)));

                            // Update the referenced entity
                            // TODO: Shouldn't be needed
                            //toUpdate.Add(originalReferences[i]);
                        }
                        else
                        {

                            AppLogger.Debug("Keeping reference to type " + originalReferences[i] + " with ID " + originalReferences[i].ID + ".");
                        }
                    }
                }
                else
                {
                    AppLogger.Debug("No previous references found.");
                }

                // If references were specified
                if (references.Count > 0)
                {
                    // Loop through all the referenced entities
                    foreach (BaseEntity r in references)
                    {
                        object mirrorValue = mirrorProperty.GetValue(r, null);

                        // If the reference is already there then don't bother creating it
                        if (mirrorValue == null || Array.IndexOf((Guid[])mirrorValue, entity.ID) == -1)
                        {
                            AppLogger.Debug("Adding reference to type " + r + " with ID " + r.ID + ".");

                            // Update the mirror references
                            toUpdate.Add(DataUtilities.AddReferences(r, entity, DataUtilities.GetMirrorProperty(property)));

                            // Update the referenced entity
                            // TODO: Shouldn't be needed
                            //toUpdate.Add(r);
                        }
                    }
                }
                else
                {
                    AppLogger.Debug("No references found.");
                }
            }

            return (BaseEntity[])toUpdate.ToArray();

        }
	}

}
