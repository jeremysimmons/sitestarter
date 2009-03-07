using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines all possible modes for a trace group.
	/// </summary>
	public enum FilterOperator
    	{
		NotSet,
		/// <summary>
		/// The values are equal.
		/// </summary>
		Equal,
		/// <summary>
		/// The values are not equal.
		/// </summary>
		NotEqual,
		/// <summary>
		/// The stored value is greater than the provided value.
		/// </summary>
		Greater,	
		/// <summary>
		/// The stored value is less than the provided value.
		/// </summary>
		Lesser,
		/// <summary>
		/// Indicates that any filter needs to match (not all).
		/// </summary>
		Or,
		/// <summary>
		/// Indicates that all filters need to match (not just any).
		/// </summary>
		And
	}

}
