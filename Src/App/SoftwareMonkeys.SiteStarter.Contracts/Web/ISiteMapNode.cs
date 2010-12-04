using System;

namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Defines the interface of all site map items.
	/// </summary>
	public interface ISiteMapNode
	{
		string Category { get;set; }
		string Title { get;set; }
		string Action { get;set; }
		string TypeName { get;set; }
	}
}
