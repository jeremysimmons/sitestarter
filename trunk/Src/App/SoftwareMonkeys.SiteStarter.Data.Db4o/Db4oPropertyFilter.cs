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

            //using (LogGroup logGroup2 = LogGroup.Start("Checking whether the filter matches the provided entity.", NLog.LogLevel.Debug))
            //{
            //    LogWriter.Debug("Field Name: " + PropertyName);
            //    LogWriter.Debug("Field Value: " + PropertyValue);

                Type type = entity.GetType();

            //    LogWriter.Debug("Entity type: " + type.ToString());

                if (Types == null)
                {
            //        LogWriter.Debug("No filter types have been specified. Match failed.");
                }
                else if (Array.IndexOf(Types, type) == -1)
                {
            //        LogWriter.Debug("The provided entity type isn't allowed by the filter. Match failed.");
                }
                else
                {
                    PropertyInfo property = type.GetProperty(PropertyName);

            //        LogWriter.Debug("Property name: " + property.Name);

                    isMatch = property.GetValue(entity, null).Equals(PropertyValue);
                }


            //    LogWriter.Debug("Is match? " + isMatch.ToString());
            //}

            return isMatch;
        }*/
    }

}
