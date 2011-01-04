using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using Db4objects.Db4o.Query;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data.Db4o
{
    /// <summary>
    /// Represents a data filter based on a specific entity field.
    /// </summary>
    public class Db4oPropertyFilter : PropertyFilter
    {
    	// TODO: Check if needed
    	// The base implementation should take care of it
        /*public override bool IsMatch(IEntity entity)
        {
            bool isMatch = false;

            //using (LogGroup logGroup2 = AppLogger.StartGroup("Checking whether the filter matches the provided entity.", NLog.LogLevel.Debug))
            //{
            //    AppLogger.Debug("Field Name: " + PropertyName);
            //    AppLogger.Debug("Field Value: " + PropertyValue);

                Type type = entity.GetType();

            //    AppLogger.Debug("Entity type: " + type.ToString());

                if (Types == null)
                {
            //        AppLogger.Debug("No filter types have been specified. Match failed.");
                }
                else if (Array.IndexOf(Types, type) == -1)
                {
            //        AppLogger.Debug("The provided entity type isn't allowed by the filter. Match failed.");
                }
                else
                {
                    PropertyInfo property = type.GetProperty(PropertyName);

            //        AppLogger.Debug("Property name: " + property.Name);

                    isMatch = property.GetValue(entity, null).Equals(PropertyValue);
                }


            //    AppLogger.Debug("Is match? " + isMatch.ToString());
            //}

            return isMatch;
        }*/
    }

}
