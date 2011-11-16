using System;
using System.Reflection;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Entities
{

	/// <summary>
	/// Provides a way to dynamically create comparers for any property of any object.
	/// </summary>
	public class DynamicComparer<E> : IComparer<E>
	{
		private Type objectType;
		/// <summary>
		/// Gets/sets the type of object being compared.
		/// </summary>
		public Type ObjectType
		{
			get { return objectType; }
			set { objectType = value; }
		}

		private string propertyName;
		/// <summary>
		/// Gets/sets the name of the property to compare.
		/// </summary>
		public string PropertyName
		{
			get { return propertyName; }
			set { propertyName = value; }
		}

		private SortDirection sortDirection;
		/// <summary>
		/// Gets/sets the sort direction.
		/// </summary>
		public SortDirection SortDirection
		{
			get { return sortDirection; }
			set { sortDirection = value; }
		}

		/// <summary>
		/// Initializes settings of the comparer.
		/// </summary>
		/// <param name="objectType">The type of object being compared.</param>
		/// <param name="propertyName">The name of the property being compared.</param>
		/// <param name="sortDirection">The sort direction.</param>
		public DynamicComparer(Type objectType, string propertyName, SortDirection sortDirection)
		{
			ObjectType = objectType;
			PropertyName = propertyName;
			SortDirection = sortDirection;
		}

		/// <summary>
		/// Initializes settings of the comparer.
		/// </summary>
		/// <param name="sortExpression">The sort expression being applied.</param>
		/// <param name="sortDirection">The sort direction.</param>
		public DynamicComparer(Type objectType, string sortExpression)
		{
			ObjectType = objectType;
			
			PropertyName = GetPropertyNameFromSortExpression(sortExpression);
			SortDirection = GetSortDirectionFromSortExpression(sortExpression);
		}
		
		/// <summary>
		/// Retrieves the name of the property in the provided sort expression. Example: Passing the expression "PriorityAscending" would return the property name "Priority".
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <returns></returns>
		static public string GetPropertyNameFromSortExpression(string sortExpression)
		{
			return sortExpression.Replace("Ascending", "")
				.Replace("Descending", "");
		}
		
		static public SortDirection GetSortDirectionFromSortExpression(string sortExpression)
		{
			string direction = sortExpression.Replace(GetPropertyNameFromSortExpression(sortExpression), "");
			
			return (SortDirection)Enum.Parse(typeof(SortDirection), direction);
		}
		
		/// <summary>
		/// Compares the specified property of the specified object type.
		/// </summary>
		/// <param name="a">The first object to compare.</param>
		/// <param name="b">The second object to compare.</param>
		/// <returns>The comparison value.</returns>
		public int Compare(E a, E b)
		{
			PropertyInfo property = ObjectType.GetProperty(PropertyName);

			if (property == null)
				throw new MissingMemberException(objectType.ToString(), PropertyName);
	
			IComparable c1 = (IComparable)GetPropertyValue(a, property);
			IComparable c2 = (IComparable)GetPropertyValue(b, property);

			if (SortDirection == SortDirection.Ascending)
			{
				if (c1 is Enum && c2 is Enum)
					return ((int)c1).CompareTo((int)c2);

				return c1.CompareTo(c2);
			}
			else
			{
				if (c1 is Enum && c2 is Enum)
					return ((int)c2).CompareTo((int)c1);

				return c2.CompareTo(c1);
			}
		}
		
		public object GetPropertyValue(E obj, PropertyInfo property)
		{
			object value = property.GetValue(obj, null);
			
			// If they are string values then convert to lowercase otherwise comparisons put lowercase "a" below uppercase "Z" in an ascending list
			if (property.PropertyType == typeof(String))
				value = ((string)value).ToLower();
			
			return value;
			
		}
	}

}
