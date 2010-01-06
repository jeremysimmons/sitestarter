/*
 * Created by SharpDevelop.
 * User: John
 * Date: 11/06/2009
 * Time: 9:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Description of FactoryManager.
	/// </summary>
	public class FactoryManager
	{	
		/// <summary>
		/// Retrieves the default type for the specified interface. For example Entities.IUser maps to Entities.User.
		/// </summary>
		/// <param name="factory">The factory containing the default type mappings.</param>
		/// <param name="interfaceName">The name of the interface to get the default type for.</param>
		/// <returns>The default entity type for the specified interface.</returns>
		static public Type GetDefaultType(IFactory factory, string interfaceName)
		{
			if (factory.DefaultTypes == null)
				throw new InvalidOperationException("No default types specified on the " + factory.GetType().ToString() + " factory type.");
			if (!factory.DefaultTypes.ContainsKey(interfaceName))
				throw new ArgumentException("The interface name '" + interfaceName + "' wasn't found in the default types list of the " + factory.GetType().ToString() + " factory.");
			
			return factory.DefaultTypes[interfaceName];
		}
	}
}
