/*
 * Created by SharpDevelop.
 * User: John
 * Date: 11/06/2009
 * Time: 11:33 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SoftwareMonkeys.SiteStarter.Configuration
{
	/// <summary>
	/// Description of IMappingConfig.
	/// </summary>
	public interface IMappingConfig : IConfig
	{
		IMappingItem[] Items {get;set;}
		void AddItem(IMappingItem item);
	}
}
