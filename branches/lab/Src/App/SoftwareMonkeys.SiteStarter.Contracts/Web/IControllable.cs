using System;
using System.Collections.Generic;
using SoftwareMonkeys.SiteStarter.Business;


namespace SoftwareMonkeys.SiteStarter.Web
{
	/// <summary>
	/// Description of IControllable.
	/// </summary>
	public interface IControllable
	{
		string WindowTitle { get;set; }
		
		string Action { get;set; }
		string InternalAction { get;set; }
		Type Type { get;set; }
		
		void CheckType();
		void CheckAction();
		
		bool RequireAuthorisation {get;set;}
	}
}
