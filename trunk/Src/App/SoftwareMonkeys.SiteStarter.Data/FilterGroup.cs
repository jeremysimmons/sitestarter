using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using NLog;
using System.Reflection;

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
      
		private BaseFilter[] filters;
		/// <summary>
		/// Gets/sets the filters in the group.
		/// </summary>
		public BaseFilter[] Filters
		{
			get { return filters; }
			set { filters = value; }
		}

		public void Add(BaseFilter filter)
		{
			List<BaseFilter> list = (filters == null ? new List<BaseFilter>() : new List<BaseFilter>());
			list.Add(filter);
			filters = (BaseFilter[])list.ToArray();
		}
	}

}
