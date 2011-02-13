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
	public class FilterGroup : IDataFilterGroup
	{
		
		private FilterGroupOperator _operator = FilterGroupOperator.And;
		/// <summary>
		/// Gets/sets the base log level for this group.
		/// </summary>
		public FilterGroupOperator Operator
		{
			get { return _operator; }
			set { _operator = value; }
		}

		private IDataFilter[] filters;
		/// <summary>
		/// Gets/sets the filters in the group.
		/// </summary>
		public IDataFilter[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}
		
		private FilterGroup[] childGroups;
		/// <summary>
		/// Gets/sets the child groups.
		/// </summary>
		public FilterGroup[] ChildGroups
		{
			get { return childGroups; }
			set { childGroups = value; }
		}
		
		
		public FilterGroup()
		{
			
		}
		
		/// <summary>
		/// Adds the provided filter values to the filter group.
		/// </summary>
		/// <param name="filterValues"></param>
		public FilterGroup(Type type, IDictionary<string, object> filterValues)
		{
			Operator = FilterGroupOperator.And;
			
			foreach (string key in filterValues.Keys)
			{
				PropertyFilter filter = new PropertyFilter(type, key, filterValues[key]);
				if (filter != null)
					Add(filter);
			}
		}

		public void Add(IDataFilter filter)
		{
			if (filter == null)
				throw new ArgumentNullException("filter");
			
			List<IDataFilter> list = (filters == null ? new List<IDataFilter>() : new List<IDataFilter>(filters));
			
			list.Add(filter);
			filters = (IDataFilter[])list.ToArray();
			
		}
		
		public void Add(FilterGroup group)
		{
			if (group == null)
				throw new ArgumentNullException("group");
			
			List<FilterGroup> list = (childGroups == null ? new List<FilterGroup>() : new List<FilterGroup>(childGroups));
			
			list.Add(group);
			childGroups = (FilterGroup[])list.ToArray();
			
		}
		
		
		public virtual bool IsMatch(IEntity entity)
		{
			if ((Filters == null || Filters.Length == 0)
			    && (ChildGroups == null || ChildGroups.Length == 0))
				throw new InvalidOperationException("No filters or child groups have been added so IsMatch cannot execute.");
			
			bool isMatch = false;
			
			//using (LogGroup logGroup = AppLogger.StartGroup("Checking whether the filter group matches the provided entity.", NLog.LogLevel.Debug))
			//{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			bool allMatch = true;
			bool anyMatch = false;
			
			//	AppLogger.Debug("Entity ID: " + entity.ID);
			//	AppLogger.Debug("Entity type: " + entity.GetType().ToString());
			//	AppLogger.Debug("Filter operator: " + Operator.ToString());

			
			//	AppLogger.Debug("# of filters: " + Filters.Length);
			// If there are no filters then match all
			if ((Filters == null || Filters.Length == 0)
			    && (ChildGroups == null || ChildGroups.Length == 0))
			{
				isMatch = true;
			}
			else
			{
				if (Filters != null)
				{
					foreach (IDataFilter filter in this.Filters)
					{
						//		using (LogGroup logGroup2 = AppLogger.StartGroup("Checking individual filter.", NLog.LogLevel.Debug))
						//		{
						if (filter.IsMatch(entity))
						{
							//				AppLogger.Debug("Filter matches. At least one of the filters match. anyMatch = true");
							// At least one entity matches to anyMatch gets set to true
							anyMatch = true;
						}
						else
						{
							//				AppLogger.Debug("Filter failed. At least one of the filters fail. allMatch = false");
							// At least one entity FAILED to match to allMatch gets set to false
							allMatch = false;
						}
						//}
						
					}
				}
				
				if (ChildGroups != null)
				{
					foreach (FilterGroup group in this.ChildGroups)
					{
						//		using (LogGroup logGroup2 = AppLogger.StartGroup("Checking individual filter.", NLog.LogLevel.Debug))
						//		{
						if (group.IsMatch(entity))
						{
							//				AppLogger.Debug("Filter matches. At least one of the filters match. anyMatch = true");
							// At least one entity matches to anyMatch gets set to true
							anyMatch = true;
						}
						else
						{
							//				AppLogger.Debug("Filter failed. At least one of the filters fail. allMatch = false");
							// At least one entity FAILED to match to allMatch gets set to false
							allMatch = false;
						}
						//}
						
					}
				}
				
				if (Operator == FilterGroupOperator.Or)
				{
					//		AppLogger.Debug("Filter operator is OR. Using value of anyMatch variable.");
					
					isMatch = anyMatch;
				}
				else
				{
					//		AppLogger.Debug("Filter operator is AND. Using value of allMatch variable.");
					isMatch = allMatch;
				}
				
				//	AppLogger.Debug("Is match: " + isMatch.ToString());
				//}
			}
			
			return isMatch;

		}
		
		static public FilterGroup New(Type type, string referencePropertyName, IEntity referencedEntity, string propertyName, object propertyValue)
		{
			return New(type, referencePropertyName, referencedEntity.ShortTypeName, referencedEntity.ID, propertyName, propertyValue);
		}
		
		static public FilterGroup New(Type type, string referencePropertyName, string referenceType, Guid referencedEntityID, string propertyName, object propertyValue)
		{
			FilterGroup group = DataAccess.Data.CreateFilterGroup();
			
			PropertyFilter propertyFilter = DataAccess.Data.CreatePropertyFilter();
			propertyFilter.AddType(type);
			propertyFilter.PropertyName = propertyName;
			propertyFilter.PropertyValue = propertyValue;
			
			group.Add(propertyFilter);
			
			ReferenceFilter referenceFilter = DataAccess.Data.CreateReferenceFilter();
			referenceFilter.AddType(type);
			referenceFilter.ReferencedEntityID = referencedEntityID;
			referenceFilter.PropertyName = referencePropertyName;
			referenceFilter.ReferenceType = EntityState.GetType(referenceType);
			
			group.Add(referenceFilter);
			
			return group;
		}
	}


}
