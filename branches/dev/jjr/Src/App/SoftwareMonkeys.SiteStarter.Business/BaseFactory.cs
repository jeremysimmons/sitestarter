using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Data;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Used as a base for all factory objects.
	/// </summary>
    [DataObject(true)]
    public class BaseFactory : IFactory
    {
       	private Dictionary<string, Type> defaultTypes;
    	public virtual Dictionary<string, Type> DefaultTypes
    	{
    		get { return defaultTypes; }
    		set { defaultTypes = value; }
    	}
	}
}
