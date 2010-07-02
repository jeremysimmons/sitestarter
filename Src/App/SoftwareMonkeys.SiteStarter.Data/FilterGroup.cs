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
	public class FilterGroup
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

		private IDataFilter[] filters;
		/// <summary>
		/// Gets/sets the filters in the group.
		/// </summary>
		public IDataFilter[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		public void Add(IDataFilter filter)
		{
			List<IDataFilter> list = (filters == null ? new List<IDataFilter>() : new List<IDataFilter>(filters));
			list.Add(filter);
			filters = (IDataFilter[])list.ToArray();
		}
		
		
		public bool IsMatch(IEntity entity)
		{
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
				
				foreach (IDataFilter filter in this.Filters)
				{
			//		using (LogGroup logGroup2 = AppLogger.StartGroup("Checking individual filter.", NLog.LogLevel.Debug))
			//		{
						if (filter.IsMatch(entity))
						{
			//				AppLogger.Debug("Filter matches. At least one of the filters match. anyMatch = true");
							anyMatch = true;
						}
						else
						{
			//				AppLogger.Debug("Filter failed. At least one of the filters fail. allMatch = false");
							allMatch = false;
						}
					//}
					
				}
				
				if (Operator == FilterOperator.Or)
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
			
			
			return isMatch;

		}
	}


}
