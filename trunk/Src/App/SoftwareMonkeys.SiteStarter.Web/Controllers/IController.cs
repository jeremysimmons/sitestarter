using System;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Defines the interface of all web projection controllers.
	/// </summary>
	public interface IController
	{
		string TypeName {get;set;}
		string Action {get;}
		IControllable Container {get;}
	}
}
