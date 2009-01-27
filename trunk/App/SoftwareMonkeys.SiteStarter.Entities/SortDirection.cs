using System;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Enumerates the directions available for sorting.
	/// </summary>
	public enum SortDirection
	{
		/// <summary>
		/// Forwards, from A-Z or 1-9.
		/// </summary>
		Ascending = 1,
		/// <summary>
		/// Backwards, from Z-A or 9-1
		/// </summary>
		Descending = 2
	}
}
