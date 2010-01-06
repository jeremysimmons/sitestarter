using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the base field filter class that all field filter objects inherit.
	/// </summary>
    public class ReferenceFilter : BaseFilter
    {
        private FilterOperator _operator;
        /// <summary>
        /// Gets/sets the base log level for this group.
        /// </summary>
        public FilterOperator Operator
        {
            get { return _operator; }
            set { _operator = value; }
        }

        private string propertyName;
        /// <summary>
        /// Gets/sets the name of the property being matched.
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        private Guid referencedEntityID;
        /// <summary>
        /// Gets/sets the ID of the referenced entity.
        /// </summary>
        public Guid ReferencedEntityID
        {
            get { return referencedEntityID; }
            set { referencedEntityID = value; }
        }

		/// <summary>
		/// Sets the provided property name and property value to the filter.
		/// </summary>
		public ReferenceFilter(Type type, string propertyName, Guid referencedEntityID)
		{
			Types = new Type[] {type};
			PropertyName = propertyName;
			ReferencedEntityID = referencedEntityID;
		}
	
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ReferenceFilter()
		{
		}
	
		public override bool IsMatch(IEntity entity)
		{
			bool typeMatches = false;
			Type entityType = entity.GetType();
			foreach (Type type in Types)
			{
				if (type.Equals(entityType)
				    || entityType.IsSubclassOf(type)
				    || type.ToString() == entityType.ToString())
				{
					typeMatches = true;
				}
			}
			
			/*PropertyInfo property = entityType.GetProperty(PropertyName);
			object value = property.GetValue(entity, null);*/
			
			
			//bool valueMatches = value.Equals(PropertyValue);
			
			bool referenceMatches = DataAccess.Data.MatchReference(entity, propertyName, null, referencedEntityID);
			
			return typeMatches && referenceMatches;
		}
	
    }

}
