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

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the mirror of the property provided.", NLog.LogLevel.Debug))
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

	        static public BaseEntity[] AddReferences(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
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
	
		
		                if (attribute.MirrorName != null && attribute.MirrorName != String.Empty)
		                {
		                    PropertyInfo mirrorProperty = GetMirrorProperty(property, attribute);

					if (mirrorProperty != null)
					{
			                    object mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

						AppLogger.Debug("Mirror value: " + mirrorValue.ToString());
			
			                    object propertyValue = property.GetValue(entity, null);
			                    if (mirrorProperty.PropertyType.Equals(typeof(Guid[])))
			                    {
						AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(Guid[]))");
			                        // If the referenced entity is found in the latest reference
			                        // if (Array.IndexOf((Guid[])propertyValue, referenceEntity.ID) > -1)
			                        // {
			                        ArrayList mirrorList = mirrorValue == null ? new ArrayList() : new ArrayList((Guid[])mirrorValue);
						using (LogGroup logGroup3 = AppLogger.StartGroup("Outputting the current mirror values", NLog.LogLevel.Debug))
						{
							foreach (Guid id in mirrorList)
								AppLogger.Debug(id.ToString());
						}
			                        ArrayList list = propertyValue == null ? new ArrayList() : new ArrayList((Guid[])propertyValue);
			                        if (mirrorList.Contains(entity.ID))
			                        {
							AppLogger.Debug("Add entity ID to the list: " + entity.ID);
			                            list.Add(referenceEntity.ID);
			
		
							AppLogger.Debug("Setting the array of IDs to the '" + mirrorProperty.Name + "' property on an object of type '" + entity.GetType().ToString() + "'.");
			                            property.SetValue(entity, list.ToArray(typeof(Guid)), null);
			
							AppLogger.Debug("Adding the referenced entity '" + entity.GetType().ToString() + "' to the list of entities pending update (toUpdate).");
			                            
		
			                            toUpdate.Add(entity);
			                        }
						else
						{
							AppLogger.Debug("The referenced entity ID is not found in the specified property of the source entity so it was skipped.");
						}
			                           
			                        // }
			                        /* else
			                         {
			                             ArrayList list = mirrorValue == null ? new ArrayList() : new ArrayList((Guid[])mirrorValue);
			                             if (list.Contains(entity.ID))
			                             {
			                                 list.Remove(entity.ID);
			
			                                 mirrorProperty.SetValue(referenceEntity, list.ToArray(typeof(Guid)), null);
			
			                                 toUpdate.Add(referenceEntity);
			                             }
			
			                         }*/
			                    }
			                    else if (mirrorProperty.PropertyType.Equals(typeof(BaseEntity[])))
			                    {
		
						AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(BaseEntity[]))");
			                        // TODO: Remove if not needed
			                        //if (mirrorValue.GetType().GetElementType() == entity.GetType())
			                        //{
			                        if (mirrorProperty.PropertyType.BaseType == typeof(BaseEntity))
			                        {
			                            mirrorProperty.SetValue(entity, entity, null);
			
			                            toUpdate.Add(entity);
			                        }
			                        else
			                        {
			                            ArrayList list = mirrorValue != null ? new ArrayList((BaseEntity[])mirrorValue) : new ArrayList();
			                            if (!list.Contains(referenceEntity))
			                            {
			                                list.Add(referenceEntity);
			                                mirrorProperty.SetValue(entity, list.ToArray(entity.GetType()), null);
			
			                                toUpdate.Add(entity);
			                            }
			                        }
			                        //}
			                        //else
			                        //throw new InvalidOperationException("The values types are incompatible: '" + mirrorValue.GetType().ToString() + "' and '" + entity.GetType().ToString() + "'");
			                    }
			                    else if (mirrorProperty.PropertyType.Equals(typeof(Guid)))
			                    {
						AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(Guid))");
			                        if (attribute.MirrorName != String.Empty)// && mirrorValue != null)
			                        {
			                            //   if (mirrorValue.GetType().GetElementType() == referenceEntity.GetType())
			                            //       {
			                            if (mirrorValue == null || (Guid)mirrorValue != entity.ID)
			                            {
			                                mirrorProperty.SetValue(entity, entity.ID, null);
			
			                                toUpdate.Add(entity);
			                            }
			                            //   }
			                            //  else
			                            //      throw new InvalidOperationException("The values types are incompatible: '" + mirrorValue.GetType().ToString() + "' and '" + entity.GetType().ToString() + "'");
			                        }
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
	
	        static public BaseEntity[] RemoveMirrors(BaseEntity entity, BaseEntity referenceEntity, PropertyInfo property, BaseEntityReferenceAttribute attrib)
	        {
	            List<BaseEntity> toUpdate = new List<BaseEntity>();
	
	            if (attrib.MirrorName != null && attrib.MirrorName != String.Empty)
	            {
	                PropertyInfo mirrorProperty = referenceEntity.GetType().GetProperty(attrib.MirrorName);
	
	                object mirrorValue = mirrorProperty.GetValue(referenceEntity, null);
	
	                object propertyValue = property.GetValue(entity, null);
	                if (mirrorValue is Guid[])
	                {
	                    ArrayList list = mirrorValue == null ? new ArrayList() : new ArrayList((Guid[])mirrorValue);
	                    if (list.Contains(entity.ID))
	                    {
	                        list.Remove(entity.ID);
	
	                        mirrorProperty.SetValue(referenceEntity, list.ToArray(typeof(Guid)), null);
	
	                        toUpdate.Add(referenceEntity);
	                    }
	                }
	                else if (mirrorValue is Guid)
	                {
	                    if (mirrorValue == null || (Guid)mirrorValue != entity.ID)
	                    {
	                        mirrorProperty.SetValue(referenceEntity, entity.ID, null);
	
	                        toUpdate.Add(referenceEntity);
	                    }
	                }
	                else if (mirrorValue is BaseEntity)
	                {
	
	
	                    if (mirrorValue != null)
	                    {
	                        if (mirrorValue.GetType().GetElementType() == entity.GetType())
	                        {
	                            Collection<BaseEntity> list = new Collection<BaseEntity>((BaseEntity[])mirrorValue);
	                            if (list.Contains(entity.ID))
	                            {
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
	
	                    foreach (BaseEntity e in (BaseEntity[])mirrorValue)
	                    {
	                        if (mirrorValue != null)
	                        {
	                            if (mirrorValue.GetType().GetElementType() == entity.GetType())
	                            {
	                                Collection<BaseEntity> list = new Collection<BaseEntity>((BaseEntity[])mirrorValue);
	                                if (list.Contains(entity.ID))
	                                {
	                                    list.Remove(entity);
	                                    mirrorProperty.SetValue(referenceEntity, list.ToArray(entity.GetType()), null);
	
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
	
	
	            return toUpdate.ToArray();
	        }

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
                                    object idsValue = GetIDsProperty(property).GetValue(entity, null);

                                    if (value == null)
                                    {
                                        AppLogger.Debug("The entities reference property value is null. Transfer skipped.");
                                    }
                                    else if (idsValue.Equals(value))
                                    {
                                        AppLogger.Debug("The reference property is equivalent to the IDs property. Transfer skipped.");
                                    }
                                    else
                                    {
                                        AppLogger.Debug("Transferring IDs from '" + attribute.EntitiesPropertyName + "' property to '" + attribute.IDsPropertyName + "' property.");
                                        PropertyInfo idsProperty = DataUtilities.GetIDsProperty(property);
                                        idsProperty.SetValue(entity, Collection<BaseEntity>.GetIDs((BaseEntity[])value), null);
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
	}

}
