using System;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// The base of all controller scanners. Derivatives can be created to scan different locations or different approaches.
	/// </summary>
	public abstract class BaseControllerScanner
	{
		public BaseControllerScanner()
		{
		}
		
		
		/// <summary>
		/// Finds all the controllers in the available assemblies.
		/// </summary>
		/// <param name="includeTestControllers"></param>
		/// <returns>An array of info about the controllers found.</returns>
		public abstract ControllerInfo[] FindControllers(bool includeTestControllers);
	}
}
