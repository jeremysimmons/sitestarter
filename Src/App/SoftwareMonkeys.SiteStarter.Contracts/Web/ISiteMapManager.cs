using System;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Defines the interface of all site map manager components.
	/// </summary>
	public interface ISiteMapManager
	{
		/// <summary>
		/// Adds items to the provided site map.
		/// Note: The items are retrieved by the GetSiteMapItems() function.
		/// </summary>
		/// <param name="siteMap"></param>
		void Add(ISiteMap siteMap);
		
		/// <summary>
		/// Removes items from the provided site map.
		/// Note: The items are retrieved by the GetSiteMapItems() function.
		/// </summary>
		/// <param name="siteMap"></param>
		void Remove(ISiteMap siteMap);
		
		/// <summary>
		/// Retrieves the site map items that this manager is responsible for adding/removing.
		/// </summary>
		/// <returns></returns>
		ISiteMapNode[] GetSiteMapItems();
	}
}
