using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of TestTask.
	/// </summary>
	public class TestTask : BaseTestEntity
	{
		private string title;
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		
		private TestTask[] prerequisites;
		[Reference]
		public TestTask[] Prerequisites
		{
			get { return prerequisites; }
			set { prerequisites = value; }
		}
		
		public TestTask()
		{
		}
		
		/// <summary>
		/// Registers the entity in the system.
		/// </summary>
		/// <param name="config">The mapping configuration object to add the settings to.</param>
		static public void RegisterType()
		{
			MappingItem item = new MappingItem("TestTask");
			item.Settings.Add("DataStoreName", "Testing_Tasks");
			item.Settings.Add("IsEntity", true);
			item.Settings.Add("FullName", typeof(TestTask).FullName);
			item.Settings.Add("AssemblyName", typeof(TestTask).Assembly.FullName);
			
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
