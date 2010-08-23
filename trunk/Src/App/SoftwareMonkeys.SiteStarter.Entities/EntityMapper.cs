using System;
using SoftwareMonkeys.SiteStarter.Diagnostics;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Entities
{
	/// <summary>
	/// Takes care of entity mappings.
	/// </summary>
	public static class EntityMapper
	{
		/// <summary>
		/// Registers the specified entity type.
		/// </summary>
		/// <param name="shortTypeName">The short name of the entity type.</param>
		/// <param name="dataStoreName">The name of the data store that the entity is stored in.</param>
		/// <param name="fullName">The full name of the entity type.</param>
		/// <param name="assemblyName">The name of the assembly containing the entity (without the *.dll)</param>
		static public void RegisterType(string shortTypeName, string  dataStoreName, string fullName, string assemblyName)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Registering the entity type: " + shortTypeName, NLog.LogLevel.Debug))
			{
				AppLogger.Debug("Data store name: " + dataStoreName);
				AppLogger.Debug("Full name: " + fullName);
				AppLogger.Debug("Assembly name: " + assemblyName);
				
				MappingItem item = new MappingItem(shortTypeName);
				item.Settings.Add("DataStoreName", dataStoreName);
				item.Settings.Add("FullName", fullName);
				item.Settings.Add("AssemblyName", assemblyName);
				item.Settings.Add("IsEntity", true);
				
				Config.Mappings.AddItem(item);
				
				Config.Mappings.Save();
				
			}
		}
		
		/// <summary>
		/// Deregisters the specified entity type.
		/// </summary>
		/// <param name="shortTypeName">The short name of the type to deregister.</param>
		static public void DeregisterType(string shortTypeName)
		{
			using (LogGroup logGroup = AppLogger.StartGroup("Deregistering the entity type: " + shortTypeName,NLog.LogLevel.Debug))
			{
				Config.Mappings.RemoveItem(Config.Mappings[shortTypeName]);
				
				Config.Mappings.Save();
			}
		}
	}
}
