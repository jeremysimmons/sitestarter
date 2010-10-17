using System;

namespace SoftwareMonkeys.SiteStarter.Web.Projections
{
	/// <summary>
	/// The base of all projection scanners. Derivatives can be created to scan different locations or different approaches.
	/// </summary>
	public abstract class BaseProjectionScanner
	{
		public BaseProjectionScanner()
		{
		}
		
		
		/// <summary>
		/// Finds all the projections in the available assemblies.
		/// </summary>
		/// <returns>An array of info about the projections found.</returns>
		public abstract ProjectionInfo[] FindProjections();
	}
}
