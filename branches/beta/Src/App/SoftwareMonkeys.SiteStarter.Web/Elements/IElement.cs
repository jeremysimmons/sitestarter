using System;

namespace SoftwareMonkeys.SiteStarter.Web.Elements
{
	/// <summary>
	/// Defines the interface of all web elements.
	/// </summary>
	public interface IElement
	{
		string TypeName {get;set;}
		string Action {get;}
		string ElementName { get; }
	}
}
