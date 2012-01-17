using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using SoftwareMonkeys.SiteStarter.Entities;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines the interface for any data filter.
	/// </summary>
    public interface IDataFilterGroup
    {
        FilterGroupOperator Operator {get;set;}

        IDataFilter[] Filters { get;set;}
        
        IDataFilterGroup[] ChildGroups { get;set; }

        void Add(IDataFilter filter);
        
        bool IsMatch(IEntity entity);
        
        Type[] AllTypes { get; }
    }

}
