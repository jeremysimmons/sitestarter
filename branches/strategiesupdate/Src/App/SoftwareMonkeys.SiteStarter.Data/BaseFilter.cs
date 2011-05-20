using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the base data filter class that all other filter objects inherit.
	/// </summary>
    public abstract class BaseFilter : IDataFilter
    {
        private Type[] types;
        /// <summary>
        /// Gets/sets the types that the filter includes.
        /// </summary>
        public Type[] Types
        {
            get { return types; }
            set { types = value; }
        }

        /*public virtual BaseEntity[] GetEntities(IDataStore dataStore)
        {
            //... add logic

            return null;
        }*/

        public void AddType(Type type)
        {
		LogWriter.Debug("Adding type " + type.ToString() + " to filter.");

            if (type != null)
            {
                List<Type> list = (types == null ? new List<Type>() : new List<Type>(types));
                list.Add(type);
                types = (Type[])list.ToArray();
            }
        }

		public virtual bool IsMatch(IEntity entity)
		{
			throw new NotSupportedException("The BaseFilter.IsMatch property cannot be called directly. A derived filter class must override it.");
		}
    }

}
