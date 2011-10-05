using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Reflection;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the base field filter class that all field filter objects inherit.
	/// </summary>
	public class ReferenceFilter : BaseFilter
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
		
		private Type referenceType;
		/// <summary>
		/// Gets/sets the type of entity being referenced.
		/// </summary>
		public Type ReferenceType
		{
			get { return referenceType; }
			set { referenceType = value; }
		}

		/// <summary>
		/// Sets the provided values.
		/// </summary>
		public ReferenceFilter(Type type, string propertyName, Type referenceType, Guid referencedEntityID)
		{
			Types = new Type[] {type};
			ReferenceType = referenceType;
			PropertyName = propertyName;
			ReferencedEntityID = referencedEntityID;
		}
		
		/// <summary>
		/// Sets the provided values.
		/// </summary>
		public ReferenceFilter(Type type, string propertyName, string referenceType, Guid referencedEntityID)
		{
			Types = new Type[] {type};
			ReferenceType = Entities.EntityState.GetType(referenceType);
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
			bool referenceMatches = false;
			
			
			using (LogGroup logGroup = LogGroup.Start("Checking whether provided entity matches this filter.", LogLevel.Debug))
			{
				if (referenceType == null)
					throw new InvalidOperationException("ReferenceType property has not been set.");
				
				if (entity == null)
					throw new ArgumentNullException("entity");
			
				if (Types == null || Types.Length == 0)
					throw new InvalidOperationException("No types have been added to the filter.");
				
				if (this.ReferencedEntityID == Guid.Empty)
					throw new InvalidOperationException("ReferencedEntityID is not set.");
				
				LogWriter.Debug("Property name: " + propertyName);
				LogWriter.Debug("Referenced entity ID: " + referencedEntityID.ToString());
				
				
				
				LogWriter.Debug("Referenced type: " + referenceType.ToString());
				
				Type entityType = entity.GetType();
				
				LogWriter.Debug("Checking entity type: " + entityType.ToString());
				LogWriter.Debug("Checking entity with ID: " + entity.ID);
				
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
				
				
				referenceMatches = DataAccess.Data.Referencer.MatchReference(entity.GetType(), entity.ID, propertyName, referenceType, referencedEntityID);
				//bool referenceMatches = DataAccess.Data.Referencer.MatchReference(entity.GetType(), entity.ID, propertyName, property.Type, referencedEntityType, referencedEntityID);
				
				
				LogWriter.Debug("Type matches: " + typeMatches.ToString());
				LogWriter.Debug("Reference matches: " + referenceMatches.ToString());
				
			}
			return typeMatches && referenceMatches;
		}
		
	}

}
