using System;
using SoftwareMonkeys.SiteStarter.Business;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Defines the interface of all web projection controllers.
	/// </summary>
	public interface IController
	{
		ICommandInfo Command { get; }
		IControllable Container { get;set; }
	}
}
