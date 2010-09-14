using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Defines the interface of all business strategy components.
	/// </summary>
	public interface IStrategy
	{
		string TypeName {get;set;}
	}
}
