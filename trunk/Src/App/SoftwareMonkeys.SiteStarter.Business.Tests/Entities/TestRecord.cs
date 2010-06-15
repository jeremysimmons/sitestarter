using System;
using SoftwareMonkeys.SiteStarter.Entities;
using System.Configuration;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Business.Tests.Entities
{
	/// <summary>
	/// Description of TestRecord.
	/// </summary>
	[Serializable]
	public class TestRecord : BaseEntity
	{
		private string name;
		public string Name
		{
			get { return name;  }
			set { name = value; }
		}
		
		private string text;
		public string Text
		{
			get { return text; }
			set { text = value; }
		}
		
		public TestRecord()
		{
		}
		
		 
        /// <summary>
        /// Registers the entity in the system.
        /// </summary>
        /// <param name="config">The mapping configuration object to add the settings to.</param>
        static public void RegisterType()
        {
			MappingItem item = new MappingItem("TestRecord");
			item.Settings.Add("DataStoreName", "Testing_Records");
			item.Settings.Add("IsEntity", true);
			item.Settings.Add("FullName", typeof(TestRecord).FullName);
			item.Settings.Add("AssemblyName", typeof(TestRecord).Assembly.FullName);
			
		
			
			Config.Mappings.AddItem(item);
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
