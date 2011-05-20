using System;
using SoftwareMonkeys.SiteStarter.State;
using SoftwareMonkeys.SiteStarter.Web.Controllers;

namespace SoftwareMonkeys.SiteStarter.Web.Controllers
{
	/// <summary>
	/// Holds the current state of the available controllers.
	/// </summary>
	public class ControllerState
	{
		/// <summary>
		/// Gets a value indicating whether the controller state has been initialized.
		/// </summary>
		static public bool IsInitialized
		{
			get { return controllers != null && controllers.Count > 0; }
		}
		
		static private ControllerStateCollection controllers;
		/// <summary>
		/// Gets/sets a collection of all the available strategies which are held in state.
		/// </summary>
		static public ControllerStateCollection Controllers
		{
			get {
				if (!IsInitialized)
					throw new InvalidOperationException("The controller state has not been initialized.");
				
				if (controllers == null)
					controllers = new ControllerStateCollection();
				return controllers;  }
			set { controllers = value; }
		}
	}
}
