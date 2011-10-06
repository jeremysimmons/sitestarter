using System;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// The base of all elemenet scanners. Derivatives can be created to scan different locations or different approaches.
	/// </summary>
	public abstract class BaseElementScanner
	{
		public BaseElementScanner()
		{
		}
		
		
		/// <summary>
		/// Finds all the elements in the available assemblies.
		/// </summary>
		/// <param name="includeTestElements"></param>
		/// <returns>An array of info about the elements found.</returns>
		public abstract ElementInfo[] FindElements(bool includeTestElements);
	}
}
