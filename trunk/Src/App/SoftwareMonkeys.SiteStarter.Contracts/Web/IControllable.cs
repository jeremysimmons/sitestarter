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
		string Action { get;set; }
		string InternalAction { get;set; }
		Type Type { get;set; }
		
		void EnsureTypeInitialized();
		
		bool RequireAuthorisation {get;set;}
		string UnauthorisedUrl {get;set;}
		
		bool EnsureAuthorised();
		void FailAuthorisation();
	}
}
