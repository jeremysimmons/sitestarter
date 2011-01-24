using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;

namespace SoftwareMonkeys.SiteStarter.Data
{
	/// <summary>
	/// Defines all possible operators for a data filter group.
	/// </summary>
	public enum FilterGroupOperator
	{
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
