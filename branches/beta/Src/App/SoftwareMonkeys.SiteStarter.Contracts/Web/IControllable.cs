using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Business;


namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Defines the interface of all controllable objects (such as projections).
	/// </summary>
	public interface IControllable
	{
		string WindowTitle { get;set; }
		
		ICommandInfo Command { get;set; }
		
		void CheckCommand();
		
		bool RequireAuthorisation {get;set;}
	}
}
