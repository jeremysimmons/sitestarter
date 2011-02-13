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
	public class PropertyFilter : BaseFilter
	{
		private FilterOperator _operator = FilterOperator.Equal;
		/// <summary>
		/// Gets/sets the base log level for this group.
		/// </summary>
		public FilterOperator Operator
		{
			get { return _operator; }
			set { _operator = value; }
		}

		private string propertyName = String.Empty;
		/// <summary>
		/// Gets/sets the name of the property being matched.
		/// </summary>
		public string PropertyName
		{
			get { return propertyName; }
			set { propertyName = value; }
		}

		private object propertyValue;
		/// <summary>
		/// Gets/sets the value of the property being matched.
		/// </summary>
		public object PropertyValue
		{
			get { return propertyValue; }
			set { propertyValue = value; }
		}

		/// <summary>
		/// Sets the provided property name and property value to the filter.
		/// </summary>
		public PropertyFilter(Type type, string propertyName, object propertyValue)
		{
			Types = new Type[] {type};
			PropertyName = propertyName;
			PropertyValue= propertyValue;
		}
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public PropertyFilter()
		{
		}
		
		public override bool IsMatch(IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (Types == null || Types.Length == 0)
				throw new InvalidOperationException("No types have been added to the filter.");
			
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
			
			PropertyInfo property = entityType.GetProperty(PropertyName);
			
			if (property == null)
				throw new InvalidOperationException("Can't find property '" + PropertyName + "' on type '" + entityType.ToString() + "'.");
			
			object value = property.GetValue(entity, null);
			
			bool valueMatches = value == PropertyValue
				|| (value != null && value.Equals(PropertyValue));
			
			return typeMatches && valueMatches;
		}
		
	}

}
