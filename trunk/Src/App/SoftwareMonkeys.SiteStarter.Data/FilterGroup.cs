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
            List<IDataFilter> list = (filters == null ? new List<IDataFilter>() : new List<IDataFilter>());
			list.Add(filter);
            filters = (IDataFilter[])list.ToArray();
		}
	}

}
