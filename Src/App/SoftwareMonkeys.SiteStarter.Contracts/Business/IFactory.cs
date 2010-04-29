
using System;
using System.Collections.Generic;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of IFactory.
	/// </summary>
	public interface IFactory
	{
		//static IFactory Current {get;set;}
		Dictionary<string,Type> DefaultTypes {get;set;}
	}
}
