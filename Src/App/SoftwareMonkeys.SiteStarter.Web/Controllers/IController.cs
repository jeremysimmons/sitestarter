using System;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Defines the interface of all web projection controllers.
	/// </summary>
	public interface IController
	{
		Type Type { get; set; }
	}
}
