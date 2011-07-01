using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Used to manage the items on a site map.
	/// </summary>
	public abstract class SiteMapManager : ISiteMapManager
	{
		public SiteMapManager()
		{
		}
		
		/// <summary>
		/// Adds items to the provided site map.
		/// Note: The items are retrieved by the GetSiteMapItems() function.
		/// </summary>
		/// <param name="siteMap"></param>
		public virtual void Add(ISiteMap siteMap)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Adding items to the provided site map."))
			{
				foreach (ISiteMapNode item in GetSiteMapItems())
				{
					LogWriter.Debug("Action: " + item.Action + " | Type name: " + item.TypeName + " | Category: " + item.Category + " | Title: " + item.Title);
					
					siteMap.Add(item);
				}
			}
		}
		
		/// <summary>
		/// Removes items from the provided site map.
		/// Note: The items are retrieved by the GetSiteMapItems() function.
		/// </summary>
		/// <param name="siteMap"></param>
		public virtual void Remove(ISiteMap siteMap)
		{
			using (LogGroup logGroup = LogGroup.StartDebug("Removing items from the provided site map."))
			{
				foreach (ISiteMapNode item in GetSiteMapItems())
				{
					LogWriter.Debug("Action: " + item.Action + " | Type name: " + item.TypeName + " | Category: " + item.Category + " | Title: " + item.Title);
					
					siteMap.Remove(item);
				}
			}
		}
		
		/// <summary>
		/// Retrieves the site map items that this manager is responsible for adding/removing.
		/// </summary>
		/// <returns></returns>
		public abstract ISiteMapNode[] GetSiteMapItems();
	}
}
