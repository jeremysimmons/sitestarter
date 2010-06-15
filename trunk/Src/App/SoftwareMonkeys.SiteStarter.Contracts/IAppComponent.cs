using System;
using System.Collections.Generic;
using System.Reflection;

namespace SoftwareMonkeys.SiteStarter
{
	/// <summary>
    /// Description of IComponent.
	/// </summary>
	public interface IAppComponent
	{
		string Name { get; }
		//static IFactory Current {get;set;}
		//Dictionary<string,Type> DefaultTypes {get;set;}
		//Dictionary<string,PropertyInfo> Properties {get;set;}
		
	}
}
