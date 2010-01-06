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
	/// Defines the interface for any data filter.
	/// </summary>
    public interface IDataFilterGroup
    {
        FilterOperator Operator {get;set;}

        IDataFilter[] Filters { get;set;}

        void Add(IDataFilter filter);

    }

}
