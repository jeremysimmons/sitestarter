using System;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// 
	/// </summary>
	public class Widget : BaseTestEntity
	{
		public Widget()
		{
		}
		
		/// <summary>
		/// Registers the entity in the system.
		/// </summary>
		/// <param name="config">The mapping configuration object to add the settings to.</param>
		static public void RegisterType()
		{
			MappingItem item = new MappingItem("Widget");
			item.Settings.Add("DataStoreName", "Widgets");
			item.Settings.Add("IsEntity", true);
			item.Settings.Add("FullName", typeof(Widget).FullName);
			item.Settings.Add("AssemblyName", typeof(Widget).Assembly.FullName);
			
			
			MappingItem item2 = new MappingItem("IWidget");
			item2.Settings.Add("Alias", "Widget");
			
			Config.Mappings.AddItem(item);
			Config.Mappings.AddItem(item2);
		}
		
		/// <summary>
		/// Deregisters the entity from the system.
		/// </summary>
		/// <param name="config">The mapping configuration object to remove the settings from.</param>
		static public void DeregisterType()
		{
			throw new NotImplementedException();
		}
	}
}
