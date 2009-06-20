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
using SoftwareMonkeys.SiteStarter.Configuration;

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

        static public string GetMirrorPropertyName(PropertyInfo property)
        {
            return GetMirrorPropertyName(property, null);
        }

        static public string GetMirrorPropertyName(PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            string mirrorPropertyName = String.Empty;

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the mirror of the property provided.", NLog.LogLevel.Debug))
            {
                if (attribute == null)
                    attribute = GetReferenceAttribute(property);

                mirrorPropertyName = attribute.MirrorName;

                AppLogger.Debug("Mirror property name: " + mirrorPropertyName);
                
            }
            
            return mirrorPropertyName;
        }

        static public PropertyInfo GetMirrorProperty(IEntity entity, PropertyInfo property)
        {
            return GetMirrorProperty(entity, property, null);
        }

        static public PropertyInfo GetMirrorProperty(IEntity entity, PropertyInfo property, BaseEntityReferenceAttribute attribute)
        {
            string mirrorPropertyName = String.Empty;

            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the mirror of the property provided.", NLog.LogLevel.Debug))
            {
                if (attribute == null)
                    attribute = GetReferenceAttribute(property);

                mirrorPropertyName = attribute.MirrorName;

                AppLogger.Debug("Mirror property name: " + mirrorPropertyName);

            }

            if (mirrorPropertyName == String.Empty)
                return null;
                
            Type referenceType = DataUtilities.GetReferenceType(entity, property);

            PropertyInfo mirrorProperty = null;
            if (attribute.MirrorType != null)
            	mirrorProperty = referenceType.GetProperty(mirrorPropertyName, attribute.MirrorType);
            else
            	mirrorProperty = referenceType.GetProperty(mirrorPropertyName);

            return mirrorProperty;
        }

        static public PropertyInfo GetIDsProperty(PropertyInfo property)
        {
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

        static public PropertyInfo GetProperty(Type entityType, string propertyName, Type propertyType)
        {
            if (propertyType != null)
                return entityType.GetProperty(propertyName, propertyType);
            else
                return entityType.GetProperty(propertyName);
        }

        static public PropertyInfo GetEntitiesProperty(PropertyInfo property)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the entities property related to the property provided.", NLog.LogLevel.Debug))
            {
                BaseEntityReferenceAttribute attribute = GetReferenceAttribute(property);

                if (attribute.EntitiesPropertyName == property.Name)
                    return property;
                else
                {
                    PropertyInfo entitiesProperty = GetProperty(property.DeclaringType, attribute.EntitiesPropertyName, attribute.EntitiesPropertyType);
                    return entitiesProperty;
                }
            }

            return null;
        }

        static public Type GetReferenceType(IEntity entity, PropertyInfo property)
        {
            Type type = null;

            using (LogGroup group = AppLogger.StartGroup("Retrieving the type of object being referenced by the provided property.", NLog.LogLevel.Debug))
            {
                if (property == null)
                    throw new ArgumentNullException("property");

                if (entity == null)
                    throw new ArgumentNullException("entity");

                BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);

                //object value = property.GetValue(entity, null);

                AppLogger.Debug("Property name: " + property.Name);

                //if (value == null || (value is IEnumerable && ((Array)value).Length == 0))
                //{
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
                    }
                //}
                //else
                //{
                //    type = value.GetType();

                //    if (type.IsSubclassOf(typeof(Array)))
                //   {
                //        AppLogger.Debug("type.IsSubclassOf(typeof(Array))");
                //        type = type.GetElementType();
                //    }
                //}

                // If the type is Guid then get the type of the entities reference property
                if (type == typeof(Guid))
                {
                    AppLogger.Debug("type == typeof(Guid)");

                    if (attribute.EntitiesPropertyName.Length > 0)
                    {
                        AppLogger.Debug("attribute.EntitiesPropertyName.Length > 0");

                        PropertyInfo entitiesProperty = GetProperty(entity.GetType(), attribute.EntitiesPropertyName, attribute.EntitiesPropertyType);

                        AppLogger.Debug("Entities property name: " + entitiesProperty.Name);

                        type = GetReferenceType(entity, entitiesProperty);

                        AppLogger.Debug("Reference type: " + type.ToString());

                    }
                }

            }

            return type;
        }

        static public IEntity[] AddReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)//, BaseEntityReferenceAttribute attribute)
        {
            List<IEntity> toUpdate = new List<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Adding mirror references.", NLog.LogLevel.Debug))
            {
            	if (entity == null)
            		throw new ArgumentNullException("entity");
            	
            	if (referenceEntity == null)
            		throw new ArgumentNullException("referenceEntity");
            	
            	if (property == null)
            		throw new ArgumentNullException("property");
            	
                if (referenceEntity != null)
                {
                    AppLogger.Debug("referenceEntity != null");

                    AppLogger.Debug("Entity type: " + entity.GetType().ToString());
                    AppLogger.Debug("Referenced entity type: " + referenceEntity.GetType().ToString());
                    AppLogger.Debug("Property name: " + property.Name);

                    //PropertyInfo property = GetProperty(entity.GetType(), property.Name, property.PropertyType);

                    BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);


                    if (property != null && attribute != null && attribute.MirrorName != null && attribute.MirrorName != String.Empty)
                    {
                        PropertyInfo mirrorProperty = GetMirrorProperty(entity, property, attribute);

                        if (mirrorProperty != null)
                        {
                            object mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

                            if (mirrorValue == null)
                            	AppLogger.Debug("Mirror value: null");
                            else
                            	AppLogger.Debug("Mirror value: " + mirrorValue.ToString());

                            //object propertyValue = property.GetValue(entity, null);
                            if (property.PropertyType.Equals(typeof(Guid[])))
                            {
                                AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(Guid[]))");
                                toUpdate.AddRange(AddIDsReferences(entity, referenceEntity, property));
                            }
                            else if (property.PropertyType.IsSubclassOf(typeof(IEntity[])))
                            {
                                // This can be skipped. It's taken care of by the ID references
                                //AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(IEntity[]))");
                                //toUpdate.AddRange(AddEntitiesReferences(entity, referenceEntity, property));
                            }
                            else if (property.PropertyType.Equals(typeof(Guid)))
                            {
                                AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(Guid))");
                                toUpdate.AddRange(AddIDReferences(entity, referenceEntity, property));
                            }
                            else if (property.PropertyType.IsSubclassOf(typeof(IEntity)))
                            {
                                // This can be skipped. It's taken care of by the ID references
                                //AppLogger.Debug("mirrorProperty.PropertyType.Equals(typeof(IEntity))");
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

        static public IEntity[] AddIDsReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            List<IEntity> toUpdate = new List<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Adding IDs references.", NLog.LogLevel.Debug))
            {
                AppLogger.Debug("Entity type: " + entity.GetType().ToString());
                AppLogger.Debug("Entity ID: " + entity.ID);
                AppLogger.Debug("Reference entity type: " + referenceEntity.GetType().ToString());
                AppLogger.Debug("Reference entity ID: " + referenceEntity.ID);
                AppLogger.Debug("Property name: " + property.Name);


                //PropertyInfo mirrorProperty = DataUtilities.GetMirrorPropertyName(property);

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

                if (property.PropertyType.Equals(typeof(Guid)))
                {
                    AppLogger.Debug("Property value is Guid");

                    Guid propertyValue = Guid.Empty;
                    if (property != null)
                        propertyValue = (Guid)property.GetValue(entity, null);

                    if ((Guid)propertyValue != entity.ID)
                    {
                        //AppLogger.Debug("Property value: " + propertyValue.ToString());

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
                    AppLogger.Debug("Property value is not Guid (must be Guid[])");


                    Guid[] propertyValue = null;
                    if (property != null)
                        propertyValue = (Guid[])property.GetValue(entity, null);

                    // Get a list of the existing references
                    List<Guid> list = new List<Guid>();
                    if (propertyValue != null)
                        list.AddRange((Guid[])propertyValue);

                    if (!list.Contains(referenceEntity.ID))
                    {

                        AppLogger.Debug("Add the ID " + referenceEntity.ID + " of type " + referenceEntity.GetType().ToString() + " to the '" + property.Name + "' property on an object of type '" + entity.GetType().ToString() + "'.");
                        //if (property.PropertyType.Equals(typeof(Guid)))
                        //{
                        //	property.SetValue(entity, referenceEntity.ID, null);
                        //}	
                        //else
                        //{
                        list.Add(referenceEntity.ID);
                        property.SetValue(entity, list.ToArray(), null);
                        //}

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

        static public IEntity[] AddEntitiesReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            List<IEntity> toUpdate = new List<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Adding entities references.", NLog.LogLevel.Debug))
            {
                BaseEntityReferenceAttribute attribute = GetReferenceAttribute(property);

                PropertyInfo mirrorProperty = GetProperty(referenceEntity.GetType(), DataUtilities.GetMirrorPropertyName(property), attribute.MirrorType);

                object mirrorValue = null;
                if (mirrorProperty != null)
                    mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

                if (mirrorProperty.PropertyType.IsSubclassOf(typeof(IEntity[])))
                {

                    ArrayList list = mirrorValue != null ? new ArrayList((IEntity[])mirrorValue) : new ArrayList();
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

        static public IEntity[] AddIDReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {

            List<IEntity> toUpdate = new List<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Adding ID reference.", NLog.LogLevel.Debug))
            {

                BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);

                if (attribute.MirrorName != String.Empty)// && mirrorValue != null)
                {
                    //PropertyInfo mirrorProperty = DataUtilities.GetMirrorPropertyName(property);

                    //object mirrorValue = null;
                    //if (mirrorProperty != null)
                    //	mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

                    object propertyValue = null;
                    if (property != null)
                        propertyValue = property.GetValue(entity, null);

                    AppLogger.Debug("Property value: " + propertyValue.ToString());

                    if (property.PropertyType.IsSubclassOf(typeof(Array)))
                    {

                        AppLogger.Debug("Property type is Array (must be Guid[])");

                        // ArrayList mirrorList = null;
                        //	if (mirrorValue != null)
                        //	{
                        //		if (mirrorValue is Guid[])
                        //			mirrorList = new ArrayList((Guid[])mirrorValue);
                        //		else
                        //		{
                        //			mirrorList = new ArrayList();
                        //			mirrorList.Add((Guid)mirrorValue);
                        //		}
                        //	}
                        //	else
                        //		mirrorList = new ArrayList();

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
                        AppLogger.Debug("Property type is not an Array (must be Guid)");

                        if ((Guid)propertyValue != referenceEntity.ID)
                        {
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

        static public IEntity[] AddEntityReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            List<IEntity> toUpdate = new List<IEntity>();

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

                        Collection<IEntity> list = (propertyValue != null ? new Collection<IEntity>((IEntity[])propertyValue) : new Collection<IEntity>());
                        if (!list.Contains(referenceEntity.ID))
                        {
                            AppLogger.Debug("Adding referenced entity ID " + referenceEntity.ID + " to the list.");
                            list.Add(referenceEntity);

                            AppLogger.Debug("Setting list of IDs to " + property.Name + " property of " + entity.GetType().ToString());
                            property.SetValue(entity, list.ToArray(typeof(IEntity)), null);

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

        static public IEntity[] RemoveReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            List<IEntity> toUpdate = new List<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Removing mirror references.", NLog.LogLevel.Debug))
            {
                Type entityType = entity.GetType();

                BaseEntityReferenceAttribute attribute = GetReferenceAttribute(property);

                //PropertyInfo property = GetProperty(entityType, propertyName, attribute.EntitiesPropertyType);


                if (property == null)
                    throw new ArgumentException("The property '" + property.Name + "' was not found on the type '" + entityType.ToString() + "'.");

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
                    else if (property.PropertyType.IsSubclassOf(typeof(IEntity)))
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

        static public IEntity[] RemoveIDsReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (referenceEntity == null)
                throw new ArgumentNullException("referenceEntity");
            if (property == null)
                throw new ArgumentNullException("property");

            List<IEntity> toUpdate = new List<IEntity>();

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

                //PropertyInfo mirrorProperty = DataUtilities.GetMirrorPropertyName(property);
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
                        AppLogger.Debug("Entity reference to " + referenceEntity.GetType() + " being removed from " + property.Name + " property on type " + entity.GetType());

                        list.Remove(referenceEntity.ID);

                        property.SetValue(entity, list.ToArray(), null);
                    }
                    else // If the property is not an array (must be Guid)
                    {
                        AppLogger.Debug("Entity ID " + Guid.Empty + " + being set to " + property.Name + " property of " + entity.GetType() + ".");
                        property.SetValue(entity, Guid.Empty, null);
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

        static public IEntity[] RemoveIDReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            List<IEntity> toUpdate = new List<IEntity>();

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

                PropertyInfo mirrorProperty = referenceEntity.GetType().GetProperty(DataUtilities.GetMirrorPropertyName(property));
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


                        AppLogger.Debug("Entity ID " + Guid.Empty + " being set to " + property.Name + " property of " + entity.GetType() + ".");
                        property.SetValue(entity, Guid.Empty, null);


                        // Adding entity to update list
                        toUpdate.Add(entity);
                    }
                }
                else
                {
                    AppLogger.Debug("Reference not found. Removal skipped.");
                }
            }

            return toUpdate.ToArray();

            //throw new NotImplementedException();
            /*List<IEntity> toUpdate = new List<IEntity>();

            PropertyInfo mirrorProperty = DataUtilities.GetMirrorPropertyName(property);
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

        static public IEntity[] RemoveEntityReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            List<IEntity> toUpdate = new List<IEntity>();
            //AppLogger.Debug("mirrorValue is IEntity");

            PropertyInfo mirrorProperty = referenceEntity.GetType().GetProperty(DataUtilities.GetMirrorPropertyName(property));
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
                    //Collection<IEntity> list = new Collection<IEntity>((IEntity)propertyValue);

                    // If the reference is still between the provided entities
                    if (((IEntity)propertyValue).ID == referenceEntity.ID)
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


        static public IEntity[] RemoveEntitiesReferences(IEntity entity, IEntity referenceEntity, PropertyInfo property)
        {
            throw new NotImplementedException();
            List<IEntity> toUpdate = new List<IEntity>();

            AppLogger.Debug("mirrorValue is IEntity[]");


            object propertyValue = property.GetValue(entity, null);

            List<IEntity> list = new List<IEntity>();
            if (propertyValue.GetType().IsSubclassOf(typeof(Array)))
                list.AddRange((IEntity[])propertyValue);
            else
                list.Add((IEntity)propertyValue);

            object mirrorValue = null;
            PropertyInfo mirrorProperty = referenceEntity.GetType().GetProperty(DataUtilities.GetMirrorPropertyName(property));
            if (mirrorProperty != null)
                mirrorValue = mirrorProperty.GetValue(referenceEntity, null);

            foreach (IEntity e in (IEntity[])mirrorValue)
            {
                if (mirrorValue != null)
                {
                    if (mirrorValue.GetType().GetElementType() == entity.GetType())
                    {
                        //List<IEntity> list = new Collection<IEntity>((IEntity[])mirrorValue);
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


        /* static public IEntity[] RemoveMirrors(IEntity entity, IEntity referenceEntity, PropertyInfo property, BaseEntityReferenceAttribute attrib)
         {
             List<IEntity> toUpdate = new List<IEntity>();

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
                     else if (mirrorValue is IEntity)
                     {
                         AppLogger.Debug("mirrorValue is IEntity");


                         if (mirrorValue != null)
                         {
                             if (mirrorValue.GetType().GetElementType() == entity.GetType())
                             {
                                 Collection<IEntity> list = new Collection<IEntity>((IEntity[])mirrorValue);
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
                     else if (mirrorValue is IEntity[])
                     {

                         AppLogger.Debug("mirrorValue is IEntity[]");


             List<IEntity> list = new List<IEntity>();
             if (propertyValue.GetType().IsSubclassOf(typeof(Array)))
                 list.AddRange((IEntity[])propertyValue);
             else
                 list.Add((IEntity)propertyValue);

                         foreach (IEntity e in (IEntity[])mirrorValue)
                         {
                             if (mirrorValue != null)
                             {
                                 if (mirrorValue.GetType().GetElementType() == entity.GetType())
                                 {
                                     //List<IEntity> list = new Collection<IEntity>((IEntity[])mirrorValue);
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

        static public void TransferReferenceIDs(IEntity entity)
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
                                                idsProperty.SetValue(entity, new Collection<IEntity>(value).GetIDs(), null);
                                            else
                                                idsProperty.SetValue(entity, new Collection<IEntity>().GetIDs(), null);
                                        }
                                        else
                                        {

                                            AppLogger.Debug("attribute is EntityReferenceAttribute");
                                            AppLogger.Debug("Transferring ID from '" + attribute.EntitiesPropertyName + "' property to '" + attribute.IDsPropertyName + "' property.");
                                            if (value != null)
                                                idsProperty.SetValue(entity, ((IEntity)value).ID, null);
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

        static public void ApplyExclusions(IEntity entity)
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
        static public IEntity[] SynchroniseReverseReferences(IEntity entity, PropertyInfo property, IEntity[] newReferencedEntities, IEntity[] obsoleteReferencedEntities)
        {
            Collection<IEntity> toUpdate = new Collection<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Synchronising the reverse references for the specified property on the provided entity."))
            {
                if (property == null)
                    throw new ArgumentNullException("property");

                AppLogger.Debug("Entity type: " + entity.GetType().ToString());
                AppLogger.Debug("Entity ID: " + entity.ID);
                AppLogger.Debug("Entity string: " + entity.ToString());
                AppLogger.Debug("Property name: " + property.Name);
                AppLogger.Debug("Property type: " + property.PropertyType.ToString());

                BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);

                if (attribute != null && !attribute.ExcludeFromDataStore)
                {

                    Type referenceEntityType = DataUtilities.GetReferenceType(entity, property);

                    Guid[] referenceIDs = GetReferenceIDs(entity, property);

                    AppLogger.Debug("Total new referenced entities: " + newReferencedEntities.Length.ToString());
                    AppLogger.Debug("Total existing referenced entities: " + obsoleteReferencedEntities.Length.ToString());


                    toUpdate.Add(RemoveObsoleteReverseReferences(entity, property, newReferencedEntities, obsoleteReferencedEntities));

                    toUpdate.Add(AddNewReverseReferences(entity, property, newReferencedEntities));

                }


            }

            return (IEntity[])toUpdate.ToArray();

        }

        static public IEntity[] RemoveObsoleteReverseReferences(IEntity entity, PropertyInfo property, IEntity[] newReferences, IEntity[] reverseReferencedEntities)
        {
            Collection<IEntity> toUpdate = new Collection<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Removing obsolete reverse references.", NLog.LogLevel.Debug))
            {
                // If a reference exists in both old and new copy then keep it in the new
                if (reverseReferencedEntities != null && reverseReferencedEntities.Length > 0)
                {
                    AppLogger.Debug("# of existing references: " + reverseReferencedEntities.Length.ToString());

                    for (int i = 0; i < reverseReferencedEntities.Length; i++)
                    {
                        using (LogGroup logGroup2 = AppLogger.StartGroup("Checking old reference.", NLog.LogLevel.Debug))
                        {
                            AppLogger.Debug("Reference ID: " + reverseReferencedEntities[i].ID);
                            AppLogger.Debug("Reference type: " + reverseReferencedEntities[i].GetType().ToString());

                            // If the reference is not still being kept then allow it to be removed
                            if (!new Collection<IEntity>(newReferences).Contains(reverseReferencedEntities[i].ID))
                            {
                                string mirrorPropertyName = DataUtilities.GetMirrorPropertyName(property);

                                // TODO: Remove. Redundant
                                //using (LogGroup logGroup3 = AppLogger.StartGroup("Removing reference", NLog.LogLevel.Debug))
                                //{
                                //	AppLogger.Debug("Entity type: " + reverseReferencedEntities[i].GetType().ToString());
                                //	AppLogger.Debug("Entity ID: " + reverseReferencedEntities[i].ID);
                                //	AppLogger.Debug("Reference ID: " + entity.ID);
                                //	AppLogger.Debug("Reference type: " + entity.GetType().ToString());
                                //	AppLogger.Debug("Property: " + mirrorPropertyName);

                                // Update the mirror references
                                toUpdate.Add(DataUtilities.RemoveReferences(reverseReferencedEntities[i], entity, GetMirrorProperty(entity, property)));
                                //}

                                // Update the referenced entity
                                // TODO: Shouldn't be needed
                                //toUpdate.Add(reverseReferencedEntities[i]);
                            }
                            else
                            {

                                AppLogger.Debug("Keeping reference to type " + reverseReferencedEntities[i] + " with ID " + reverseReferencedEntities[i].ID + ".");
                            }
                        }
                    }
                }
                else
                {
                    AppLogger.Debug("No previous references found.");
                }

            }

            return toUpdate.ToArray();
        }

        /// <summary>
        /// Adds all necessary reverse references from the provided entity and provided property, to the array of provided entities.
        /// </summary>
        /// <param name="entity">The entity that the reverse references point to.</param>
        /// <param name="property">The property that the reverse reference mirrors.</param>
        /// <param name="newReferences">The new entities that the reverse references will be added to.</param>
        /// <returns></returns>
        static public IEntity[] AddNewReverseReferences(IEntity entity, PropertyInfo property, IEntity[] newReferences)
        {
            Collection<IEntity> toUpdate = new Collection<IEntity>();

            using (LogGroup logGroup = AppLogger.StartGroup("Adding new reverse references.", NLog.LogLevel.Debug))
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (property == null)
                    throw new ArgumentNullException("property");

                if (newReferences == null)
                    throw new ArgumentNullException("newReferences");

                // If references were specified
                if (newReferences.Length > 0)
                {
                    // Loop through all the referenced entities
                    foreach (IEntity r in newReferences)
                    {
                        using (LogGroup logGroup2 = AppLogger.StartGroup("Checking new reference.", NLog.LogLevel.Debug))
                        {
                            AppLogger.Debug("Reference ID: " + r.ID);
                            AppLogger.Debug("Reference type: " + r.GetType().ToString());

                            string mirrorPropertyName = DataUtilities.GetMirrorPropertyName(property);

                            PropertyInfo mirrorProperty = r.GetType().GetProperty(mirrorPropertyName);

                            if (mirrorProperty == null)
                            {
                                throw new InvalidOperationException("The mirror property '" + mirrorPropertyName + "' wasn't found on the type '" + r.GetType().ToString() + "'.");
                            }

                            object mirrorValue = mirrorProperty.GetValue(r, null);

                            if (mirrorProperty.PropertyType.Equals(typeof(Guid[])))
                            {
                                // If the reference is already there then don't bother creating it
                                if (mirrorValue == null || Array.IndexOf((Guid[])mirrorValue, entity.ID) == -1)
                                {
                                    AppLogger.Debug("Adding reference to type " + r + " with ID " + r.ID + ".");

                                    // Update the mirror references
                                    toUpdate.Add(DataUtilities.AddReferences(r, entity, DataUtilities.GetMirrorProperty(entity, property)));

                                    // Update the referenced entity
                                    // TODO: Shouldn't be needed. This is added elsewhere if necessary
                                    //toUpdate.Add(r);
                                }
                            }
                            else
                            {
                                // If the reference is already there then don't bother creating it
                                if (mirrorValue == null || !entity.ID.Equals((Guid)mirrorValue))
                                {
                                    AppLogger.Debug("Adding reference to type " + r + " with ID " + r.ID + ".");

                                    // Update the mirror references
                                    toUpdate.Add(DataUtilities.AddReferences(r, entity, DataUtilities.GetMirrorProperty(entity, property)));

                                    // Update the referenced entity
                                    // TODO: Shouldn't be needed. This is added elsewhere if necessary
                                    //toUpdate.Add(r);
                                }
                            }
                        }
                    }
                }
                else
                {
                    AppLogger.Debug("No new referenced entities.");
                }
            }

            return toUpdate.ToArray();
        }

        static public Guid[] GetReferenceIDs(IEntity entity, PropertyInfo property)
        {
            using (LogGroup logGroup = AppLogger.StartGroup("Retrieving the entity IDs on the provided property.", NLog.LogLevel.Debug))
            {

                if (property.PropertyType.Equals(typeof(Guid[])))
                {
                    AppLogger.Debug("Property type is Guid[]");

                    Guid[] referencedIDs = (Guid[])property.GetValue(entity, null);
                    AppLogger.Debug("# of referenced IDs found: " + referencedIDs.Length);
                    foreach (Guid id in referencedIDs)
                    {
                        AppLogger.Debug("Contains ID: " + id.ToString());
                    }

                    return referencedIDs;
                }
                else if (property.PropertyType.Equals(typeof(Guid)))
                {
                    AppLogger.Debug("Property type is Guid");

                    Guid referencedID = (Guid)property.GetValue(entity, null);

                    AppLogger.Debug("Referenced ID: " + referencedID.ToString());

                    if (referencedID != Guid.Empty)
                        return new Guid[] { referencedID };
                }

            }


            return new Guid[] { };
        
    	}

        /// <summary>
        /// Gets the name of the data store that the provided entity is stored in.
        /// </summary>
        /// <param name="type">The type of entity to get the data store name of.</param>
        /// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
        /// <returns>The data store that the provided entity is stored in.</returns>
        static public string GetDataStoreNameForReference(IEntity entity, PropertyInfo property)
        {
			object value = property.GetValue(entity, null);
			Type referenceType = DataUtilities.GetReferenceType(entity, property);
			BaseEntityReferenceAttribute attribute = DataUtilities.GetReferenceAttribute(property);
	
			if (attribute.EntitiesPropertyName.Length == 0)
				throw new InvalidOperationException("The specified property '" + property.Name + "' doesn't have an entities property specified in the reference attribute. Cannot retrieve the type.");
	
			if (property.Name != attribute.EntitiesPropertyName)
			{
				return GetDataStoreNameForReference(entity, GetEntitiesProperty(property));
			}
			else
			{
				if (value == null || (value is Array && ((Array)value).Length == 0))
				{
					return GetDataStoreName(referenceType);
				}
				else
				{
					Type type = value.GetType();
					if (type.IsSubclassOf(typeof(Array)))
						return GetDataStoreName(type.GetElementType(), true);
					else
						return GetDataStoreName(value.GetType(), true);
				}
			}
        }

        /// <summary>
        /// Gets the name of the data store that the provided entity is stored in.
        /// </summary>
        /// <param name="type">The type of entity to get the data store name of.</param>
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
        /// <param name="type">The type of entity to get the data store name of.</param>
        /// <param name="throwErrorIfNotFound">A flag indicating whether an error should be thrown when no data store attribute is found.</param>
        /// <returns>The data store that the provided entity is stored in.</returns>
        static public string GetDataStoreName(Type type, bool throwErrorIfNotFound)
        {
		/*if (type == null)
			throw new ArgumentNullException("type");

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
		
			MappingItem item = Config.Mappings.GetItem(type);
			if (item == null)
				throw new InvalidOperationException("No mappings found for the type " + type.ToString());
			
			if (!item.Settings.ContainsKey("DataStoreName"))
				throw new InvalidOperationException("No data store name has been declared in the mappings for the '" + type.ToString() + "' type.");
			return (string)item.Settings["DataStoreName"];
        }
	}

}
