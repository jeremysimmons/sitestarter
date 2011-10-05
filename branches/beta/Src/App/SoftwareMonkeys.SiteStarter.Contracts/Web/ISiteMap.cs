using System;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Defines the interface of all site map components.
	/// </summary>
	public interface ISiteMap
	{
		void Add(ISiteMapNode item);
		
		void Remove(ISiteMapNode item);
	}
}
