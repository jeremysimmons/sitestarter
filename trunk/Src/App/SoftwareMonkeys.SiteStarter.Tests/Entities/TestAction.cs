using System;
using SoftwareMonkeys.SiteStarter.Entities;
using SoftwareMonkeys.SiteStarter.Configuration;

namespace SoftwareMonkeys.SiteStarter.Tests.Entities
{
	/// <summary>
	/// Description of TestAction.
	/// </summary>
	[Serializable]
	public class TestAction : BaseTestEntity
	{
		private string title;
		public string Title
		{
			get { return title; }
			set { title = value; }
		}
		
		private TestCategory[] categories;
		[Reference]
		public TestCategory[] Categories
		{
			get { return categories; }
			set { categories = value; }
		}
		
		public TestAction()
		{
		}
		
		/// <summary>
        /// Registers the entity in the system.
        /// </summary>
        static public void RegisterType()
        {
				
			MappingItem item2 = new MappingItem("TestAction");
			item2.Settings.Add("DataStoreName", "Testing_Actions");
			item2.Settings.Add("IsEntity", true);
			item2.Settings.Add("FullName", typeof(TestAction).FullName);
			item2.Settings.Add("AssemblyName", typeof(TestAction).Assembly.FullName);
			
			Config.Mappings.AddItem(item2);
        }
        
        /// <summary>
        /// Deregisters the entity from the system.
        /// </summary>
        static public void DeregisterType()
        {
        	throw new NotImplementedException();
        }
	}
}
