using System;
using System.Web.UI;

namespace SoftwareMonkeys.SiteStarter.Web.Parts
{
	/// <summary>
	/// The base of all part scanners. Derivatives can be created to scan different locations or different approaches.
	/// </summary>
	public abstract class BasePartScanner
	{
		private Page page;
		public Page Page
		{
			get { return page; }
			set { page = value; }
		}
		
		public BasePartScanner()
		{
		}
		
		public BasePartScanner(Page page)
		{
			Page = page;
		}		
		
		/// <summary>
		/// Finds all the parts in the available assemblies.
		/// </summary>
		/// <returns>An array of info about the parts found.</returns>
		public abstract PartInfo[] FindParts();
	}
}
