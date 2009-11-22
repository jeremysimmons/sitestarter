/*
 * Created by SharpDevelop.
 * User: John
 * Date: 11/06/2009
 * Time: 9:39 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
