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
		object GetViewState(string key);
		void SetViewState(string key, object value);
		//System.Web.UI.Page Page {get;}
		Dictionary<string, object> DataItems {get;set;}
		string GetTextItem(string key);
		bool ContainsTextItem(string key);
	}
}
